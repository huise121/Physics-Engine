// CollisionBody 表示一个可参与碰撞检测和碰撞响应的物体。
// 它是物理引擎中的一个关键概念，用于描述具有碰撞形状的物理实体。
//
// CollisionBody 通常包含以下重要成员：
//
// 碰撞形状（Collision Shape）：
// 描述了物体的几何形状。提供了多种类型的碰撞形状，如球体、盒体、胶囊体、凸多边形等，以及复合形状，如凸包、凸缺形状等。
// 每个 CollisionBody 可以附加一个或多个碰撞形状。
//
// 变换信息（Transform）：
// 描述了物体的位置和旋转状态。这些信息用于将物体的局部坐标系转换为世界坐标系，以便进行碰撞检测和碰撞响应。
//
// 碰撞过滤数据（Collision Filtering Data）：
// 包含了用于控制物体碰撞的组和层信息。通过设置碰撞组和碰撞层，可以指定物体之间的碰撞关系，以及哪些碰撞应该被忽略。
//
// 用户数据（User Data）：
// 一个可选的用户自定义数据指针，可以附加到 CollisionBody 上，以便在需要时与外部应用程序交互。
//
// 碰撞回调（Collision Callbacks）：
// 一组回调函数，用于处理碰撞事件。提供了一些回调函数，如碰撞开始、碰撞持续和碰撞结束等，可以用于监听和处理碰撞事件。
//
// 通过 CollisionBody，提供了一种简单而强大的方式来创建和管理具有碰撞形状的物体，并在物理仿真中模拟它们的运动和碰撞行为。


using System.Runtime.CompilerServices;

namespace RP3D
{
    public class CollisionBody
    {
        public PhysicsWorld mWorld;
        protected readonly Entity mEntity;

        public CollisionBody(PhysicsWorld world, Entity entity)
        {
            mWorld = world;
            mEntity = entity;
        }

        // 要求广义相位再次测试物体的碰撞形状是否发生碰撞
        // （就好像物体已经移动一样）。
        protected void askForBroadPhaseCollisionCheck()
        {
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            var nbColliderEntities = colliderEntities.Count;
            for (var i = 0; i < nbColliderEntities; i++)
            {
                var collider = mWorld.mCollidersComponents.getCollider(colliderEntities[i]);
                mWorld.mCollisionDetection.askForBroadPhaseCollisionCheck(collider);
            }
        }

        // 创建一个新的碰撞体并将其添加到物体中
        /**
         * 该方法将返回指向新碰撞体的指针。碰撞体是
         * 一个带有碰撞形状的对象，附加到一个物体上。可以
         * 将多个碰撞体附加到给定的物体上。您可以使用
         * 返回的碰撞体来获取和设置关于相应碰撞形状的信息
         * 为该物体。
         * @param collisionShape 指向新碰撞体的碰撞形状的指针
         * @param transform 用于将碰撞体的局部坐标系变换为物体的局部坐标系的变换
         * @return 指向已创建的碰撞体的指针
         */
        public Collider addCollider(CollisionShape collisionShape, PTransform pTransform)
        {
            var colliderEntity = mWorld.mEntityManager.CreateEntity();
            var collider = new Collider(colliderEntity, this);
            collisionShape.GetLocalBounds(out var localBoundsMin, out var localBoundsMax);

            var localToWorldTransform = mWorld.mTransformComponents.getTransform(mEntity) * pTransform;
            var material = new Material(mWorld.mConfig.defaultFrictionCoefficient, mWorld.mConfig.defaultBounciness);

            var colliderComponent = new ColliderComponent
            {
                bodyEntity = mEntity,
                collider = collider,
                localBounds = new AABB(localBoundsMin, localBoundsMax),
                LocalToBodyPTransform = pTransform,
                collisionShape = collisionShape,
                collisionCategoryBits = 1,
                collideWithMaskBits = short.MaxValue,
                LocalToWorldPTransform = localToWorldTransform,
                material = material
            };

            var isActive = mWorld.mCollisionBodyComponents.getIsActive(mEntity);
            mWorld.mCollidersComponents.addComponent(colliderEntity, !isActive, colliderComponent);
            mWorld.mCollisionBodyComponents.addColliderToBody(mEntity, colliderEntity);
            collisionShape.addCollider(collider);

            var localTransform = mWorld.mTransformComponents.getTransform(mEntity);
            collisionShape.ComputeAABB(out var aabb, localTransform * pTransform);
            mWorld.mCollisionDetection.addCollider(collider, aabb);


            return collider;
        }

        // 返回与该物体关联的碰撞体数量
        /**
         * @return 与该物体关联的碰撞体数量
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int getNbColliders()
        {
            return mWorld.mCollisionBodyComponents.getColliders(mEntity).Count;
        }

        // 返回物体的给定碰撞体的常量指针
        /**
         * @param index 物体碰撞体的索引
         * @return 物体给定碰撞体的常量指针
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Collider getCollider(int colliderIndex)
        {
            var colliderEntity = mWorld.mCollisionBodyComponents.getColliders(mEntity)[colliderIndex];
            return mWorld.mCollidersComponents.getCollider(colliderEntity);
        }

        // 设置物体是否活动
        /**
         * @param isActive 如果要激活物体，则为 true
         */
        public virtual void setIsActive(bool isActive)
        {
            // If the state does not change
            if (mWorld.mCollisionBodyComponents.getIsActive(mEntity) == isActive) return;

            mWorld.mCollisionBodyComponents.setIsActive(mEntity, isActive);

            // If we have to activate the body
            if (isActive)
            {
                var transform = mWorld.mTransformComponents.getTransform(mEntity);

                // For each collider of the body
                var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
                for (var i = 0; i < colliderEntities.Count; i++)
                {
                    var collider = mWorld.mCollidersComponents.getCollider(colliderEntities[i]);

                    // Compute the world-space AABB of the new collision shape
                    AABB aabb;
                    collider.getCollisionShape().ComputeAABB(out aabb,
                        transform * mWorld.mCollidersComponents.getLocalToBodyTransform(collider.GetEntity()));

                    // Add the collider to the collision detection
                    mWorld.mCollisionDetection.addCollider(collider, aabb);
                }
            }
            else
            {
                // If we have to deactivate the body

                // For each collider of the body
                var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
                for (var i = 0; i < colliderEntities.Count; i++)
                {
                    var collider = mWorld.mCollidersComponents.getCollider(colliderEntities[i]);

                    if (collider.getBroadPhaseId() != -1)
                        // Remove the collider from the collision detection
                        mWorld.mCollisionDetection.removeCollider(collider);
                }
            }
        }


        // 从物体中移除一个碰撞体
        /**
         * 要移除碰撞体，需要指定其指针
         * @param collider 要移除的碰撞体的指针
         */
        public virtual void removeCollider(Collider collider)
        {
            // Remove the collider from the broad-phase
            if (collider.getBroadPhaseId() != -1) mWorld.mCollisionDetection.removeCollider(collider);

            mWorld.mCollisionBodyComponents.removeColliderFromBody(mEntity, collider.GetEntity());

            // Unassign the collider from the collision shape
            collider.getCollisionShape().removeCollider(collider);

            // Remove the collider component
            mWorld.mCollidersComponents.removeComponent(collider.GetEntity());

            // Destroy the entity
            mWorld.mEntityManager.destroyEntity(collider.GetEntity());
        }

        // 返回当前位置和方向
        /**
         * @return 物体的当前变换，将物体的局部坐标系变换为世界坐标系
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PTransform getTransform()
        {
            return mWorld.mTransformComponents.getTransform(mEntity);
        }

        // Remove all the colliders
        public void removeAllColliders()
        {
            var collidersEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            for (var i = 0; i < collidersEntities.Count; i++)
                removeCollider(mWorld.mCollidersComponents.getCollider(collidersEntities[i]));
        }


        // 如果一个点在碰撞体内部则返回 true
        /**
         * 该方法在点在物体的任何碰撞形状内时返回 true
         * @param worldPoint 要测试的点（世界坐标系中的点）
         * @return 如果点在物体内部则返回 true
         */
        public bool testPointInside(PVector3 worldPoint)
        {
            // For each collider of the body
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            for (var i = 0; i < colliderEntities.Count; i++)
            {
                var collider = mWorld.mCollidersComponents.getCollider(colliderEntities[i]);
                // Test if the point is inside the collider
                if (collider.testPointInside(worldPoint)) return true;
            }

            return false;
        }

        /**
         * 具有反馈信息的射线投射方法
         * 该方法返回与物体的所有碰撞形状中最近的命中结果
         * @param ray 用于对物体进行射线投射的射线
         * @param[out] raycastInfo 包含射线投射结果的结构体（仅在方法返回 true 时有效）
         * @return 如果射线命中物体则返回 true，否则返回 false
         */
        public bool raycast(PRay pRay, out RaycastInfo raycastInfo)
        {
            raycastInfo = new RaycastInfo();
            // 如果物体不是活动的，则无法被射线命中
            if (!mWorld.mCollisionBodyComponents.getIsActive(mEntity)) return false;

            var isHit = false;
            var rayTemp = pRay;

            // 对于物体的每个碰撞体
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            var nbColliderEntities = colliderEntities.Count;
            for (var i = 0; i < nbColliderEntities; i++)
            {
                var collider = mWorld.mCollidersComponents.getCollider(colliderEntities[i]);
                // 测试射线是否击中碰撞体
                if (collider.Raycast(rayTemp, out raycastInfo))
                {
                    rayTemp.maxFraction = raycastInfo.hitFraction;
                    isHit = true;
                }
            }

            return isHit;
        }


        // 计算并返回物体的轴对齐边界框（AABB），通过合并所有碰撞体的AABB
        /**
         * @return 物体在世界坐标系中的轴对齐边界框（AABB）
         */
        public AABB getAABB()
        {
            // 创建物体的AABB
            var bodyAABB = new AABB();

            // 获取物体的所有碰撞体
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            if (colliderEntities.Count == 0) return bodyAABB;

            // 获取物体的变换信息
            var transform = mWorld.mTransformComponents.getTransform(mEntity);

            // 获取第一个碰撞体
            var collider = mWorld.mCollidersComponents.getCollider(colliderEntities[0]);

            // 计算碰撞体的AABB，并转换为世界坐标系
            collider.getCollisionShape().ComputeAABB(out bodyAABB, transform * collider.getLocalToBodyTransform());

            // 遍历物体的每个碰撞体
            var nbColliderEntities = colliderEntities.Count;
            for (var i = 1; i < nbColliderEntities; i++)
            {
                collider = mWorld.mCollidersComponents.getCollider(colliderEntities[i]);

                // 计算碰撞体的AABB，并转换为世界坐标系
                AABB aabb;
                collider.getCollisionShape().ComputeAABB(out aabb, transform * collider.getLocalToBodyTransform());

                // 将碰撞体的AABB与当前物体的AABB合并
                bodyAABB.MergeWithAABB(aabb);
            }

            return bodyAABB;
        }


        // 设置当前位置和方向
        /**
         * @param transform 将物体的局部坐标系变换为世界坐标系的物体变换信息
         */
        public virtual void setTransform(PTransform pTransform)
        {
            // Update the transform of the body
            mWorld.mTransformComponents.setTransform(mEntity, pTransform);

            // Update the broad-phase state of the body
            updateBroadPhaseState();
        }


        // 更新此物体的广义相位状态（例如，因为它已经移动）
        public void updateBroadPhaseState()
        {
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            var nbColliderEntities = colliderEntities.Count;
            for (var i = 0; i < nbColliderEntities; i++)
            {
                // Update the local-to-world transform of the collider
                mWorld.mCollidersComponents.setLocalToWorldTransform(colliderEntities[i],
                    mWorld.mTransformComponents.getTransform(mEntity) *
                    mWorld.mCollidersComponents.getLocalToBodyTransform(colliderEntities[i]));

                // Update the collider
                mWorld.mCollisionDetection.updateCollider(colliderEntities[i]);
            }
        }

        // 设置物体是否活动
        /**
         * @param isActive 如果要激活物体，则为 true
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool isActive()
        {
            return mWorld.mCollisionBodyComponents.getIsActive(mEntity);
        }

        // 根据物体的局部坐标系中的点返回世界坐标系中的坐标
        /**
         * @param localPoint 物体局部坐标系中的点
         * @return 世界坐标系中的点
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 getWorldPoint(PVector3 localPoint)
        {
            // 使用物体的变换信息将局部坐标系中的点转换为世界坐标系中的点
            return mWorld.mTransformComponents.getTransform(mEntity) * localPoint;
        }

        // 根据物体的局部坐标系中的向量返回世界坐标系中的向量
        /**
         * @param localVector 物体局部坐标系中的向量
         * @return 世界坐标系中的向量
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 getWorldVector(PVector3 localVector)
        {
            // 使用物体的变换信息将局部坐标系中的向量转换为世界坐标系中的向量
            return mWorld.mTransformComponents.getTransform(mEntity).GetOrientation() * localVector;
        }

        // 根据世界坐标系中的点返回物体局部坐标系中的点
        /**
         * @param worldPoint 世界坐标系中的点
         * @return 物体局部坐标系中的点
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 getLocalPoint(PVector3 worldPoint)
        {
            // 使用物体的变换信息将世界坐标系中的点转换为物体局部坐标系中的点
            return mWorld.mTransformComponents.getTransform(mEntity).GetInverse() * worldPoint;
        }

        // 根据世界坐标系中的向量返回物体局部坐标系中的向量
        /**
         * @param worldVector 世界坐标系中的向量
         * @return 物体局部坐标系中的向量
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 getLocalVector(PVector3 worldVector)
        {
            // 使用物体的变换信息将世界坐标系中的向量转换为物体局部坐标系中的向量
            return mWorld.mTransformComponents.getTransform(mEntity).GetOrientation().getInverse() * worldVector;
        }


        // 测试碰撞体是否与给定的AABB重叠
        /**
         * @param worldAABB 将用于测试重叠的AABB（世界坐标系中的）
         * @return 如果给定的AABB与碰撞体的AABB重叠则返回 true
         */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool testAABBOverlap(AABB worldAABB)
        {
            return worldAABB.TestCollision(getAABB());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity getEntity()
        {
            return mEntity;
        }


        public override string ToString()
        {
            return this.mEntity.ToString();
        }
    }
}