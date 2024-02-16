using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    //碰撞检测系统
    public partial class CollisionDetectionSystem
    {
        // 碰撞器组件的引用
        public  ColliderComponents mCollidersComponents;
        // 刚体组件的引用
        public  RigidBodyComponents mRigidBodyComponents;
        // 碰撞检测分派配置
        public  CollisionDispatch mCollisionDispatch;
        // 指向物理世界的指针
        public  PhysicsWorld mWorld;
        // 不能相互碰撞的一对物体
        public  List<Pair<Entity, Entity>> mNoCollisionPairs = new();
        // 广义相位重叠对
        public  OverlappingPairs mOverlappingPairs;
        // 广义相位系统
        public  BroadPhaseSystem mBroadPhaseSystem;
        // 将广义相位ID映射到碰撞器相应实体的字典
        public  Dictionary<int, Entity> mMapBroadPhaseIdToColliderEntity = new();
        // 狭义相位碰撞检测输入
        public  NarrowPhaseInput mNarrowPhaseInput;
        
        // 潜在联系点信息结构体数组
        public List<ContactPointInfo> mPotentialContactPoints = new();
        // 潜在碰撞检测过程中发现的接触点数组
        public  List<ContactManifoldInfo> mPotentialContactManifolds = new();
        
        // 指向前一帧接触对数组的指针（指向mContactPairs1或mContactPairs2）
        public List<ContactPair> mPreviousContactPairs = new();
        // 指向当前帧接触对数组的指针（指向mContactPairs1或mContactPairs2）
        public List<ContactPair> mCurrentContactPairs = new();
        
        // 失去接触的接触对数组（上一帧中接触但当前帧中未接触的接触对）
        public  List<ContactPair> mLostContactPairs = new();
        
        // 指向重叠PairId到上一帧接触对索引映射的指针（指向mMapPairIdToContactPairIndex1或mMapPairIdToContactPairIndex2）
        public Dictionary<int, int> mPreviousMapPairIdToContactPairIndex = new();
        // 指向前一帧接触曲面数组的指针（指向mContactManifolds1或mContactManifolds2）
        public List<ContactManifold> mPreviousContactManifolds = new();
        
        // 指向当前帧接触曲面数组的指针（指向mContactManifolds1或mContactManifolds2）
        public List<ContactManifold> mCurrentContactManifolds = new();
        // 指向前一帧接触点数组的指针（指向mContactPoints1或mContactPoints2）
        public List<ContactPoint> mPreviousContactPoints = new();
        // 指向当前帧接触点数组的指针（指向mContactPoints1或mContactPoints2）
        public List<ContactPoint> mCurrentContactPoints = new();
        



        public CollisionDetectionSystem(PhysicsWorld world, ColliderComponents collidersComponents,
            TransformComponents transformComponents,RigidBodyComponents rigidBodyComponents)
        {
            mWorld = world;
            mCollidersComponents = collidersComponents;
            mRigidBodyComponents = rigidBodyComponents;
            mCollisionDispatch = new CollisionDispatch();
            mOverlappingPairs = new OverlappingPairs(mCollidersComponents, mCollisionDispatch);
            mBroadPhaseSystem = new BroadPhaseSystem(this, mCollidersComponents, transformComponents);
            mNarrowPhaseInput = new NarrowPhaseInput(/*mOverlappingPairs*/);
        }

        // 计算碰撞检测。
        public void computeCollisionDetection()
        {
            // 执行广义相位碰撞检测。在这一阶段，通常会使用一些快速的算法来识别可能发生碰撞的物体，从而减少后续检测的计算量。
            computeBroadPhase();

            // 执行中间相位碰撞检测。这一阶段可能会执行一些更复杂的几何计算，以确定物体之间的接触情况，
            // 但仍然不涉及具体的碰撞点或碰撞力的计算。
            computeMiddlePhase(mNarrowPhaseInput, true);

            // 执行窄相位碰撞检测。在这一阶段，会计算碰撞点、法线、深度等具体的碰撞信息，并进行相应的碰撞响应处理。
            computeNarrowPhase();
        }

        // 通知需要测试重叠的涉及特定碰撞体的重叠对
        public void notifyOverlappingPairsToTestOverlap(Collider collider)
        {
            // 获取与此碰撞体相关的重叠对
            var overlappingPairs = mCollidersComponents.getOverlappingPairs(collider.GetEntity());
            var nbPairs = overlappingPairs.Count;
            for (var i = 0; i < nbPairs; i++)
                // 通知需要测试重叠的重叠对
                mOverlappingPairs.setNeedToTestOverlap(overlappingPairs[i], true);
        }



        // 创建实际的接触曲面和接触点
        public void createContacts()
        {

            // 我们遍历所有的接触对，并将至少一个 CollisionBody 的对添加到 mProcessContactPairsOrderIslands 数组的末尾，
            // 因为这些对在岛创建过程中没有被添加（只有两个 RigidBody 的对在岛创建过程中被添加）。
            mWorld.mProcessContactPairsOrderIslands.AddRange(mCollisionBodyContactPairsIndices);

            //处理接触对的顺序由岛定义，以便给定岛的接触流形和接触点在流形和接触点的数组中紧密排列在一起。
            var nbContactPairsToProcess = mWorld.mProcessContactPairsOrderIslands.Count;
            for (var p = 0; p < nbContactPairsToProcess; p++)
            {
                var contactPairIndex = mWorld.mProcessContactPairsOrderIslands[p];

                var contactPair = mCurrentContactPairs[contactPairIndex];

                contactPair.ContactManifoldsIndex = mCurrentContactManifolds.Count;
                contactPair.NbContactManifolds = contactPair.NbPotentialContactManifolds;
                contactPair.ContactPointsIndex = mCurrentContactPoints.Count;

                // 对于该对的每个潜在接触流形
                for (var m = 0; m < contactPair.NbPotentialContactManifolds; m++)
                {
                    var potentialManifold = mPotentialContactManifolds[contactPair.PotentialContactManifoldsIndices[m]];

                    // 该流形的起始索引和接触点数量
                    var contactPointsIndex = mCurrentContactPoints.Count;
                    var nbContactPoints = potentialManifold.nbPotentialContactPoints;
                    contactPair.NbTotalContactPoints += nbContactPoints;


                    // 创建并添加接触流形
                    mCurrentContactManifolds.Add(new ContactManifold(contactPair.Body1Entity, contactPair.Body2Entity,
                        contactPair.Collider1Entity,
                        contactPair.Collider2Entity, contactPointsIndex, nbContactPoints));


                    // 对于流形中的每个接触点
                    for (var c = 0; c < potentialManifold.nbPotentialContactPoints; c++)
                    {
                        var potentialContactPoint =
                            mPotentialContactPoints[potentialManifold.potentialContactPointsIndices[c]];

                        // Create and add the contact point
                        mCurrentContactPoints.Add(new ContactPoint(potentialContactPoint,
                            mWorld.mConfig.persistentContactDistanceThreshold));
                    }
                }
            }

            // 使用上一帧的接触点信息初始化当前接触点（用于预热）
            initContactsWithPreviousOnes();

            // 计算丢失的接触点（在上一帧中发生碰撞但在当前帧中没有）
            computeLostContactPairs();

            mPreviousContactPoints.Clear();
            mPreviousContactManifolds.Clear();
            mPreviousContactPairs.Clear();

            // mNbPreviousPotentialContactManifolds = mPotentialContactManifolds.Count;
            // mNbPreviousPotentialContactPoints = mPotentialContactPoints.Count;

            // Reset the potential contacts
            mPotentialContactPoints.Clear();
            mPotentialContactManifolds.Clear();

            //"计算接触对ID到接触对的映射，用于下一帧"
            computeMapPreviousContactPairs();

            mCollisionBodyContactPairsIndices.Clear();
            mNarrowPhaseInput.clear();
        }

        // 计算失去的接触对（前一帧中存在接触但当前帧中不存在的接触对）。
        // 计算丢失的接触对
        public void computeLostContactPairs()
        {
            // 对于每个凸对
            var nbConvexPairs = mOverlappingPairs.mConvexPairs.Count;
            for (var i = 0; i < nbConvexPairs; i++)
                // 如果对的两个碰撞体在上一帧发生碰撞，但在当前帧未发生碰撞
                if (mOverlappingPairs.mConvexPairs[i].CollidingInPreviousFrame &&
                    !mOverlappingPairs.mConvexPairs[i].CollidingInCurrentFrame)
                    // 如果两个物体仍然存在
                    if (mCollidersComponents.hasComponent(mOverlappingPairs.mConvexPairs[i].Collider1) &&
                        mCollidersComponents.hasComponent(mOverlappingPairs.mConvexPairs[i].Collider2))
                        // 创建一个丢失的接触对
                        addLostContactPair(mOverlappingPairs.mConvexPairs[i]);

            // 对于每个凹对
            /*var nbConcavePairs = mOverlappingPairs.mConcavePairs.Count;
            for (var i = 0; i < nbConcavePairs; i++)
                // 如果对的两个碰撞体在上一帧发生碰撞，但在当前帧未发生碰撞
                if (mOverlappingPairs.mConcavePairs[i].CollidingInPreviousFrame &&
                    !mOverlappingPairs.mConcavePairs[i].CollidingInCurrentFrame)
                    // 如果两个物体仍然存在
                    if (mCollidersComponents.hasComponent(mOverlappingPairs.mConcavePairs[i].Collider1) &&
                        mCollidersComponents.hasComponent(mOverlappingPairs.mConcavePairs[i].Collider2))
                        // 创建一个丢失的接触对
                        addLostContactPair(mOverlappingPairs.mConcavePairs[i]);*/
        }


        // 使用上一帧的联系初始化当前的联系（用于预热）
        public void initContactsWithPreviousOnes()
        {
            var persistentContactDistThresholdSqr = mWorld.mConfig.persistentContactDistanceThreshold *
                                                    mWorld.mConfig.persistentContactDistanceThreshold;

            // 遍历当前帧的每对接触点
            var nbCurrentContactPairs = mCurrentContactPairs.Count;
            for (var i = 0; i < nbCurrentContactPairs; i++)
            {
                var currentContactPair = mCurrentContactPairs[i];

                // 如果在上一帧中找到了对应的接触点对（如果有）
                if (mPreviousMapPairIdToContactPairIndex.TryGetValue(currentContactPair.PairId,
                        out var previousContactPairIndex))
                {
                    var previousContactPair = mPreviousContactPairs[previousContactPairIndex];

                    // --------------------- 接触凸体 --------------------- //

                    var contactManifoldsIndex = currentContactPair.ContactManifoldsIndex;
                    var nbContactManifolds = currentContactPair.NbContactManifolds;

                    // 遍历当前接触点对的每个接触凸体
                    for (var m = contactManifoldsIndex; m < contactManifoldsIndex + nbContactManifolds; m++)
                    {
                        var currentContactManifold = mCurrentContactManifolds[m];
                        var currentContactPoint = mCurrentContactPoints[currentContactManifold.contactPointsIndex];
                        var currentContactPointNormal = currentContactPoint.GetNormal();

                        // 在上一帧的接触凸体中找到一个类似的接触凸体（用于预热）
                        var previousContactManifoldIndex = previousContactPair.ContactManifoldsIndex;
                        var previousNbContactManifolds = previousContactPair.NbContactManifolds;
                        for (var p = previousContactManifoldIndex;
                             p < previousContactManifoldIndex + previousNbContactManifolds;
                             p++)
                        {
                            var previousContactManifold = mPreviousContactManifolds[p];
                            var previousContactPoint =
                                mPreviousContactPoints[previousContactManifold.contactPointsIndex];

                            // 如果上一帧的接触凸体与当前接触凸体具有相似的接触法线
                            if (previousContactPoint.GetNormal().dot(currentContactPointNormal) >=
                                mWorld.mConfig.cosAngleSimilarContactManifold)
                            {
                                // 将数据从上一帧的接触凸体传输到当前接触凸体
                                currentContactManifold.frictionVector1 = previousContactManifold.frictionVector1;
                                currentContactManifold.frictionVector2 = previousContactManifold.frictionVector2;
                                currentContactManifold.frictionImpulse1 = previousContactManifold.frictionImpulse1;
                                currentContactManifold.frictionImpulse2 = previousContactManifold.frictionImpulse2;
                                currentContactManifold.frictionTwistImpulse = previousContactManifold.frictionTwistImpulse;
                                break;
                            }
                        }
                    }

                    // --------------------- 接触点 --------------------- //

                    var contactPointsIndex = currentContactPair.ContactPointsIndex;
                    var nbTotalContactPoints = currentContactPair.NbTotalContactPoints;

                    // 遍历当前接触点对的每个接触点
                    for (var c = contactPointsIndex; c < contactPointsIndex + nbTotalContactPoints; c++)
                    {
                        var currentContactPoint = mCurrentContactPoints[c];

                        var currentContactPointLocalShape1 = currentContactPoint.GetLocalPointOnShape1();

                        // 在上一帧的接触点中找到一个类似的接触点（用于预热）
                        var previousContactPointsIndex = previousContactPair.ContactPointsIndex;
                        var previousNbContactPoints = previousContactPair.NbTotalContactPoints;
                        for (var p = previousContactPointsIndex;
                             p < previousContactPointsIndex + previousNbContactPoints;
                             p++)
                        {
                            var previousContactPoint = mPreviousContactPoints[p];

                            // 如果上一帧的接触点与当前接触点非常接近
                            var distSquare =
                                (currentContactPointLocalShape1 - previousContactPoint.GetLocalPointOnShape1())
                                .LengthSquare();
                            if (distSquare <= persistentContactDistThresholdSqr)
                            {
                                // 将数据从上一帧的接触点传输到当前接触点
                                currentContactPoint.SetPenetrationImpulse(previousContactPoint.GetPenetrationImpulse());
                                currentContactPoint.SetIsRestingContact(previousContactPoint.GetIsRestingContact());
                                break;
                            }
                        }
                    }
                }
            }
        }

        // 射线投射方法
        public void raycast(RaycastCallback raycastCallback, PRay ray, short raycastWithCategoryMaskBits)
        {
            var rayCastTest = new RaycastTest(raycastCallback);
            // 请求广义相位算法调用testRaycastAgainstShape()回调方法，对射线击中的每个碰撞体执行检测
            mBroadPhaseSystem.raycast(ray, rayCastTest, raycastWithCategoryMaskBits);
        }

        // 返回世界事件监听器
        public EventListener getWorldEventListener()
        {
            return mWorld.mEventListener;
        }

        // 返回给定碰撞体的世界空间AABB
        public AABB getWorldAABB(Collider collider)
        {
            return mBroadPhaseSystem.getFatAABB(collider.getBroadPhaseId());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PhysicsWorld getWorld()
        {
            return mWorld;
        }

        // 更新碰撞体（例如，已移动的碰撞体）
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void updateCollider(Entity colliderEntity)
        {
            mBroadPhaseSystem.updateCollider(colliderEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void updateColliders()
        {
            mBroadPhaseSystem.updateColliders();
        }

        // 请求在宽相位期间重新测试碰撞形状。
        // 我们只需将形状放入上一帧移动的碰撞形状数组中，以便在宽相位中再次进行碰撞测试。
        public void askForBroadPhaseCollisionCheck(Collider collider)
        {
            if (collider.getBroadPhaseId() != -1)
                mBroadPhaseSystem.addMovedCollider(collider.getBroadPhaseId(), collider);
        }

        // 向碰撞检测中添加一个物体
        public void addCollider(Collider collider, AABB aabb)
        {
            // 将物体添加到粗粒度相交检测系统中
            mBroadPhaseSystem.addCollider(collider, aabb);
            // 获取碰撞体的粗粒度相交检测系统ID
            var broadPhaseId = mCollidersComponents.getBroadPhaseId(collider.GetEntity());
            // 添加碰撞体粗粒度相交检测系统ID与其实体之间的映射关系
            mMapBroadPhaseIdToColliderEntity.Add(broadPhaseId, collider.GetEntity());
        }

        // 从碰撞检测中移除一个碰撞体
        public void removeCollider(Collider collider)
        {
            var colliderBroadPhaseId = collider.getBroadPhaseId();

            // 移除涉及此碰撞体的所有重叠对
            var overlappingPairs = mCollidersComponents.getOverlappingPairs(collider.GetEntity());
            while (overlappingPairs.Count > 0)
                // TODO：从涉及的两个体的接触面数组中移除所有重叠对的接触面
                // 移除重叠对
                mOverlappingPairs.removePair(overlappingPairs[0]);

            mMapBroadPhaseIdToColliderEntity.Remove(colliderBroadPhaseId);

            // 从粗略检测中移除碰撞体
            mBroadPhaseSystem.removeCollider(collider);
        }

        // 移除无法相互碰撞的一对物体
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void removeNoCollisionPair(Entity body1Entity, Entity body2Entity)
        {
            mNoCollisionPairs.Remove(OverlappingPairs.computeBodiesIndexPair(body1Entity, body2Entity));
        }
        
        // 添加一对不可相互碰撞的物体
        public void addNoCollisionPair(Entity body1Entity, Entity body2Entity)
        {
            mNoCollisionPairs.Add(OverlappingPairs.computeBodiesIndexPair(body1Entity, body2Entity));

            // 如果已经存在涉及的 OverlappingPairs，它们应该被移除；否则它们将保持在碰撞状态
            var toBeRemoved = new List<int>();
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(body1Entity);
            for (var i = 0; i < colliderEntities.Count; ++i)
            {
                // 获取 body1 的碰撞器当前重叠的对。
                var overlappingPairs = mCollidersComponents.getOverlappingPairs(colliderEntities[i]);

                for (var j = 0; j < overlappingPairs.Count; ++j)
                {
                    var pair = mOverlappingPairs.getOverlappingPair(overlappingPairs[j]);

                    var overlappingBody1 = mOverlappingPairs.mColliderComponents.getBody(pair.Collider1);
                    var overlappingBody2 = mOverlappingPairs.mColliderComponents.getBody(pair.Collider2);
                    if (overlappingBody1 == body2Entity || overlappingBody2 == body2Entity)
                        toBeRemoved.Add(overlappingPairs[j]);
                }
            }

            // 移除需要移除的重叠对。
            for (var i = 0; i < toBeRemoved.Count; ++i)
                mOverlappingPairs.removePair(toBeRemoved[i]);
        }
      
        
    }
}