namespace RP3D
{
    public class RigidBody : CollisionBody
    {
        public RigidBody(PhysicsWorld world, Entity entity) : base(world, entity)
        {
        }

        public BodyType getType()
        {
            return mWorld.mRigidBodyComponents.GetBodyType(mEntity);
        }

        public void setType(BodyType type)
        {
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) == type) return;

            mWorld.mRigidBodyComponents.SetBodyType(mEntity, type);

            // If it is a static body
            if (type == BodyType.STATIC)
            {
                // Reset the velocity to zero
                mWorld.mRigidBodyComponents.SetLinearVelocity(mEntity, PVector3.Zero());
                mWorld.mRigidBodyComponents.SetAngularVelocity(mEntity, PVector3.Zero());
            }

            // If it is a static or a kinematic body
            if (type == BodyType.STATIC || type == BodyType.KINEMATIC)
            {
                // Reset the inverse mass and inverse inertia tensor to zero
                mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 0.0f);
                mWorld.mRigidBodyComponents.SetInverseInertiaTensorLocal(mEntity, PVector3.Zero());
            }
            else
            {
                // If it is a dynamic body

                var mass = mWorld.mRigidBodyComponents.GetMass(mEntity);

                if (mass > 0.0f)
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 1.0f / mass);
                else
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 0.0f);

                // Compute the inverse local inertia tensor
                var inertiaTensorLocal = mWorld.mRigidBodyComponents.GetLocalInertiaTensor(mEntity);
                var inverseInertiaTensorLocal = new PVector3(
                    inertiaTensorLocal.x != 0.0f ? 1.0f / inertiaTensorLocal.x : 0,
                    inertiaTensorLocal.y != 0.0f ? 1.0f / inertiaTensorLocal.y : 0,
                    inertiaTensorLocal.z != 0.0f ? 1.0f / inertiaTensorLocal.z : 0);
                mWorld.mRigidBodyComponents.SetInverseInertiaTensorLocal(mEntity, inverseInertiaTensorLocal);
            }

            // Awake the body
            setIsSleeping(false);

            // Update the active status of currently overlapping pairs
            resetOverlappingPairs();

            // Reset the force and torque on the body
            mWorld.mRigidBodyComponents.SetExternalForce(mEntity, PVector3.Zero());
            mWorld.mRigidBodyComponents.SetExternalTorque(mEntity, PVector3.Zero());
        }

        public float getMass()
        {
            return mWorld.mRigidBodyComponents.GetMass(mEntity);
        }


        // 手动在给定点（局部空间中）对物体施加外部力。
        /**
         * 如果点不在物体的质心上，它还将产生一些扭矩，从而改变物体的角速度。
         * 如果物体处于睡眠状态，则调用此方法将唤醒它。注意，力将被添加到施加力的总和中，
         * 并且该总和将在每次调用 PhysicsWorld::update() 方法结束时重置为零。
         * 您只能将力应用于动态物体，否则，此方法将不起作用。
         * @param force 施加在物体上的力（以牛顿为单位，局部空间中）
         * @param point 施加力的点（物体的局部空间中）
         */
        public void applyLocalForceAtLocalPosition(PVector3 force, PVector3 point)
        {
            // Convert the local-space force to world-space
            var worldForce = mWorld.mTransformComponents.getTransform(mEntity).GetOrientation() * force;
            applyWorldForceAtLocalPosition(worldForce, point);
        }


        // 在给定点（局部空间中）对物体施加外部力（世界空间中）。
        /**
         * 如果点不在物体的质心上，它还将产生一些扭矩，从而改变物体的角速度。
         * 如果物体处于睡眠状态，则调用此方法将唤醒它。注意，力将被添加到施加力的总和中，
         * 并且该总和将在每次调用 PhysicsWorld::update() 方法结束时重置为零。
         * 您只能将力应用于动态物体，否则，此方法将不起作用。
         * @param force 施加在物体上的力（以牛顿为单位，世界空间中）
         * @param point 施加力的点（物体的局部空间中）
         */
        public void applyWorldForceAtLocalPosition(PVector3 force, PVector3 point)
        {
            // If it is not a dynamic body, we do nothing
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) != BodyType.DYNAMIC) return;

            // Awake the body if it was sleeping
            if (mWorld.mRigidBodyComponents.GetIsSleeping(mEntity)) setIsSleeping(false);

            // Add the force
            var externalForce = mWorld.mRigidBodyComponents.GetExternalForce(mEntity);
            mWorld.mRigidBodyComponents.SetExternalForce(mEntity, externalForce + force);

            // Add the torque
            var externalTorque = mWorld.mRigidBodyComponents.GetExternalTorque(mEntity);
            var centerOfMassWorld = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);
            var worldPoint = mWorld.mTransformComponents.getTransform(mEntity) * point;
            mWorld.mRigidBodyComponents.SetExternalTorque(mEntity,
                externalTorque + (worldPoint - centerOfMassWorld).cross(force));
        }

        // 在给定点（世界空间中）对物体施加外部力（局部空间中）。
        /**
         * 如果点不在物体的质心上，它还将产生一些扭矩，从而改变物体的角速度。
         * 如果物体处于睡眠状态，则调用此方法将唤醒它。注意，力将被添加到施加力的总和中，
         * 并且该总和将在每次调用 PhysicsWorld::update() 方法结束时重置为零。
         * 您只能将力应用于动态物体，否则，此方法将不起作用。
         * @param force 施加在物体上的力（以牛顿为单位，物体的局部空间中）
         * @param point 施加力的点（世界空间中）
         */
        public void applyLocalForceAtWorldPosition(PVector3 force, PVector3 point)
        {
            // Convert the local-space force to world-space
            var worldForce = mWorld.mTransformComponents.getTransform(mEntity).GetOrientation() * force;
            applyWorldForceAtWorldPosition(worldForce, point);
        }

        // 在给定点（世界空间中）对物体施加外部力（世界空间中）。
        /**
         * 如果点不在物体的质心上，它还将产生一些扭矩，从而改变物体的角速度。
         * 如果物体处于睡眠状态，则调用此方法将唤醒它。注意，力将被添加到施加力的总和中，
         * 并且该总和将在每次调用 PhysicsWorld::update() 方法结束时重置为零。
         * 您只能将力应用于动态物体，否则，此方法将不起作用。
         * @param force 施加在物体上的力（以牛顿为单位，世界空间中）
         * @param point 施加力的点（世界空间中）
         */
        public void applyWorldForceAtWorldPosition(PVector3 force, PVector3 point)
        {
            // If it is not a dynamic body, we do nothing
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) != BodyType.DYNAMIC) return;

            // Awake the body if it was sleeping
            if (mWorld.mRigidBodyComponents.GetIsSleeping(mEntity)) setIsSleeping(false);

            // Add the force
            var externalForce = mWorld.mRigidBodyComponents.GetExternalForce(mEntity);
            mWorld.mRigidBodyComponents.SetExternalForce(mEntity, externalForce + force);

            // Add the torque
            var externalTorque = mWorld.mRigidBodyComponents.GetExternalTorque(mEntity);
            var centerOfMassWorld = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);
            mWorld.mRigidBodyComponents.SetExternalTorque(mEntity,
                externalTorque + (point - centerOfMassWorld).cross(force));
        }


        // 返回物体的局部惯性张量（在物体坐标系中）
        /**
        @return 一个包含局部空间惯性张量对角线3x3矩阵的三个值的向量
        */
        public PVector3 getLocalInertiaTensor()
        {
            return mWorld.mRigidBodyComponents.GetLocalInertiaTensor(mEntity);
        }

        // 设置身体的局部惯性张量（以局部空间坐标为单位）
        /**
         * 注意，对角线上值为零的惯性张量被解释为无限惯性。
         * @param inertiaTensorLocal 三个值的向量，表示局部空间惯性张量的对角线3x3矩阵
         */
        public void setLocalInertiaTensor(PVector3 inertiaTensorLocal)
        {
            mWorld.mRigidBodyComponents.SetLocalInertiaTensor(mEntity, inertiaTensorLocal);

            // 如果是动态物体
            var type = mWorld.mRigidBodyComponents.GetBodyType(mEntity);
            if (type == BodyType.DYNAMIC)
            {
                // 计算逆局部惯性张量
                var inverseInertiaTensorLocal = new PVector3(
                    inertiaTensorLocal.x != 0.0f ? 1.0f / inertiaTensorLocal.x : 0,
                    inertiaTensorLocal.y != 0.0f ? 1.0f / inertiaTensorLocal.y : 0f,
                    inertiaTensorLocal.z != 0.0f ? 1.0f / inertiaTensorLocal.z : 0f);
                mWorld.mRigidBodyComponents.SetInverseInertiaTensorLocal(mEntity, inverseInertiaTensorLocal);
            }
        }


        // 在质心处手动施加一个外部力（以局部空间为单位）到身体上。
        /**
         * 如果物体正在睡眠，则调用此方法将唤醒它。请注意，力将添加到施加力的总和中，并且此总和将在每次调用 PhyscisWorld::update() 方法结束时重置为零。
         * 您只能将力施加到动态物体上，否则此方法将不起作用。
         * @param force 要施加在物体质心上的外部力（以物体局部空间为单位）（牛顿）
         */
        public void applyLocalForceAtCenterOfMass(PVector3 force)
                {
                    // 将局部空间的力转换为世界空间
                    var worldForce = mWorld.mTransformComponents.getTransform(mEntity).GetOrientation() * force;

                    applyWorldForceAtCenterOfMass(worldForce);
                }


        // 在质心处手动施加一个外部力（以世界空间为单位）到身体上。
        /**
         * 如果物体正在睡眠，则调用此方法将唤醒它。请注意，力将添加到施加力的总和中，并且此总和将在每次调用 PhyscisWorld::update() 方法结束时重置为零。
         * 您只能将力施加到动态物体上，否则此方法将不起作用。
         * @param force 要施加在物体质心上的外部力（以世界空间为单位）（牛顿）
         */
        public void applyWorldForceAtCenterOfMass(PVector3 force)
                {
                    // 如果不是动态物体，则不执行任何操作
                    if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) != BodyType.DYNAMIC) return;

                    // 如果物体正在睡眠，则唤醒它
                    if (mWorld.mRigidBodyComponents.GetIsSleeping(mEntity)) setIsSleeping(false);

                    // 添加力
                    var externalForce = mWorld.mRigidBodyComponents.GetExternalForce(mEntity);
                    mWorld.mRigidBodyComponents.SetExternalForce(mEntity, externalForce + force);
                }

        // 返回线性速度阻尼因子
        /**
         * @return 该物体的线性阻尼因子（范围为 [0; +inf]）。零表示无阻尼。
         */
        public float getLinearDamping()
        {
            return mWorld.mRigidBodyComponents.GetLinearDamping(mEntity);
        }

        // 返回角速度阻尼因子
        /**
         * @return 该物体的角阻尼因子（范围为 [0; +inf]）。零表示无阻尼。
         */
        public float getAngularDamping()
        {
            return mWorld.mRigidBodyComponents.GetAngularDamping(mEntity);
        }


        // 设置物体的质心（以局部空间坐标为单位）
        /**
         * 此方法不会移动刚体在世界中的位置。
         * @param centerOfMass 物体的质心，以局部空间坐标表示
         */
        public void setLocalCenterOfMass(PVector3 centerOfMass)
                {
                    var oldCenterOfMass = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);
                    mWorld.mRigidBodyComponents.SetCenterOfMassLocal(mEntity, centerOfMass);

                    // 计算世界空间坐标中的质心位置
                    mWorld.mRigidBodyComponents.SetCenterOfMassWorld(mEntity,
                        mWorld.mTransformComponents.getTransform(mEntity) * centerOfMass);

                    // 更新质心的线性速度
                    var linearVelocity = mWorld.mRigidBodyComponents.GetLinearVelocity(mEntity);
                    var angularVelocity = mWorld.mRigidBodyComponents.GetAngularVelocity(mEntity);
                    var centerOfMassWorld = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);
                    linearVelocity += angularVelocity.cross(centerOfMassWorld - oldCenterOfMass);
                    mWorld.mRigidBodyComponents.SetLinearVelocity(mEntity, linearVelocity);
                }

        // 返回物体的质心（以局部空间坐标为单位）
        /**
         * @return 物体质心的局部空间位置
         */
        public PVector3 getLocalCenterOfMass()
        {
            return mWorld.mRigidBodyComponents.GetCenterOfMassLocal(mEntity);
        }


        // 使用物体的碰撞器计算并设置质心的局部空间坐标
        /// 此方法使用碰撞器的形状、质量密度和变换来设置物体的质心。请注意，调用此方法将覆盖之前使用 RigidBody::setCenterOfMass() 方法设置的质量。
        /// 此外，此方法不使用用户使用 RigidBody::setMass() 方法设置的质量来计算质心，而仅使用碰撞器的质量密度和体积。
        public void updateLocalCenterOfMassFromColliders()
                {
                    var oldCenterOfMassWorld = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);

                    var centerOfMassLocal = computeCenterOfMass();

                    var centerOfMassWorld = mWorld.mTransformComponents.getTransform(mEntity) * centerOfMassLocal;

                    // 设置质心
                    mWorld.mRigidBodyComponents.SetCenterOfMassLocal(mEntity, centerOfMassLocal);
                    mWorld.mRigidBodyComponents.SetCenterOfMassWorld(mEntity, centerOfMassWorld);

                    // 更新质心的线性速度
                    var linearVelocity = mWorld.mRigidBodyComponents.GetLinearVelocity(mEntity);
                    var angularVelocity = mWorld.mRigidBodyComponents.GetAngularVelocity(mEntity);
                    linearVelocity += angularVelocity.cross(centerOfMassWorld - oldCenterOfMassWorld);
                    mWorld.mRigidBodyComponents.SetLinearVelocity(mEntity, linearVelocity);
                }


        // Compute and return the local-space center of mass of the body using its colliders
        public PVector3 computeCenterOfMass()
        {
            var totalMass = 0.0f;
            var centerOfMassLocal = new PVector3(0, 0, 0);

            // Compute the local center of mass
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            for (var i = 0; i < colliderEntities.Count; i++)
            {
                var colliderIndex = mWorld.mCollidersComponents.getEntityIndex(colliderEntities[i]);

                var colliderVolume = mWorld.mCollidersComponents.mCollisionShapes[colliderIndex].getVolume();
                var colliderMassDensity = mWorld.mCollidersComponents.mMaterials[colliderIndex].getMassDensity();

                var colliderMass = colliderVolume * colliderMassDensity;

                totalMass += colliderMass;
                centerOfMassLocal += colliderMass *
                                     mWorld.mCollidersComponents.mLocalToBodyTransforms[colliderIndex].GetPosition();
            }

            if (totalMass > 0.0f) centerOfMassLocal /= totalMass;

            return centerOfMassLocal;
        }

        // Compute the local-space inertia tensor and total mass of the body using its colliders
        public void computeMassAndInertiaTensorLocal(out PVector3 inertiaTensorLocal, out float totalMass)
        {
            inertiaTensorLocal = PVector3.Zero();
            inertiaTensorLocal.SetToZero();
            totalMass = 0.0f;

            var tempLocalInertiaTensor = Matrix3x3.Zero();

            var centerOfMassLocal = mWorld.mRigidBodyComponents.GetCenterOfMassLocal(mEntity);

            // Compute the inertia tensor using all the colliders
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            for (var i = 0; i < colliderEntities.Count; i++)
            {
                var colliderIndex = mWorld.mCollidersComponents.getEntityIndex(colliderEntities[i]);

                var colliderVolume = mWorld.mCollidersComponents.mCollisionShapes[colliderIndex].getVolume();
                var colliderMassDensity = mWorld.mCollidersComponents.mMaterials[colliderIndex].getMassDensity();
                var colliderMass = colliderVolume * colliderMassDensity;

                totalMass += colliderMass;

                // Get the inertia tensor of the collider in its local-space
                var shapeLocalInertiaTensor = mWorld.mCollidersComponents.mCollisionShapes[colliderIndex]
                    .getLocalInertiaTensor(colliderMass);

                // Convert the collider inertia tensor into the local-space of the body
                var shapeTransform = mWorld.mCollidersComponents.mLocalToBodyTransforms[colliderIndex];
                var rotationMatrix = shapeTransform.GetOrientation().getMatrix();
                var rotationMatrixTranspose = rotationMatrix.GetTranspose();
                rotationMatrixTranspose[0] *= shapeLocalInertiaTensor.x;
                rotationMatrixTranspose[1] *= shapeLocalInertiaTensor.y;
                rotationMatrixTranspose[2] *= shapeLocalInertiaTensor.z;
                var inertiaTensor = rotationMatrix * rotationMatrixTranspose;

                // Use the parallel axis theorem to convert the inertia tensor w.r.t the collider
                // center into a inertia tensor w.r.t to the body origin.
                var offset = shapeTransform.GetPosition() - centerOfMassLocal;
                var offsetSquare = offset.LengthSquare();
                var offsetMatrix = new Matrix3x3();
                offsetMatrix[0].SetAllValues(offsetSquare, 0.0f, 0.0f);
                offsetMatrix[1].SetAllValues(0.0f, offsetSquare, 0.0f);
                offsetMatrix[2].SetAllValues(0.0f, 0.0f, offsetSquare);
                offsetMatrix[0] += offset * -offset.x;
                offsetMatrix[1] += offset * -offset.y;
                offsetMatrix[2] += offset * -offset.z;
                offsetMatrix *= colliderMass;

                tempLocalInertiaTensor += inertiaTensor + offsetMatrix;
            }

            // Get the diagonal value of the computed local inertia tensor
            inertiaTensorLocal.SetAllValues(tempLocalInertiaTensor[0][0], tempLocalInertiaTensor[1][1],
                tempLocalInertiaTensor[2][2]);
        }

        // Compute and set the local-space inertia tensor of the body using its colliders
        /// This method uses the shape, mass density and transforms of the colliders to set
        /// the local-space inertia tensor of the body. Note that calling this method will overwrite the
        /// mass that has been set with the RigidBody::setInertiaTensorLocal() method.
        public void updateLocalInertiaTensorFromColliders()
                {
                    // Compute the local-space inertia tensor
                    var inertiaTensorLocal = PVector3.Zero();
                    var totalMass = 0.0f;
                    computeMassAndInertiaTensorLocal(out inertiaTensorLocal, out totalMass);

                    mWorld.mRigidBodyComponents.SetLocalInertiaTensor(mEntity, inertiaTensorLocal);

                    // If it is a dynamic body
                    var type = mWorld.mRigidBodyComponents.GetBodyType(mEntity);
                    if (type == BodyType.DYNAMIC)
                    {
                        // Compute the inverse local inertia tensor
                        var inverseInertiaTensorLocal = new PVector3(
                            inertiaTensorLocal.x != 0.0f ? 1.0f / inertiaTensorLocal.x : 0,
                            inertiaTensorLocal.y != 0.0f ? 1.0f / inertiaTensorLocal.y : 0,
                            inertiaTensorLocal.z != 0.0f ? 1.0f / inertiaTensorLocal.z : 0);
                        mWorld.mRigidBodyComponents.SetInverseInertiaTensorLocal(mEntity, inverseInertiaTensorLocal);
                    }
                }


        // Compute and set the mass of the body using its colliders
        /// This method uses the shape, mass density and transforms of the colliders to set
        /// the total mass of the body. Note that calling this method will overwrite the
        /// mass that has been set with the RigidBody::setMass() method
        public void updateMassFromColliders()
        {
            var totalMass = 0.0f;

            // Compute the total mass of the body
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            for (var i = 0; i < colliderEntities.Count; i++)
            {
                var colliderIndex = mWorld.mCollidersComponents.getEntityIndex(colliderEntities[i]);

                var colliderVolume = mWorld.mCollidersComponents.mCollisionShapes[colliderIndex].getVolume();
                var colliderMassDensity = mWorld.mCollidersComponents.mMaterials[colliderIndex].getMassDensity();

                var colliderMass = colliderVolume * colliderMassDensity;

                totalMass += colliderMass;
            }

            // Set the mass
            mWorld.mRigidBodyComponents.SetMass(mEntity, totalMass);

            // If it is a dynamic body
            var type = mWorld.mRigidBodyComponents.GetBodyType(mEntity);
            if (type == BodyType.DYNAMIC)
            {
                // Compute the inverse mass
                if (totalMass > 0.0f)
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 1.0f / totalMass);
                else
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 0.0f);
            }
        }


        // Compute and set the center of mass, the mass and the local-space inertia tensor of the body using its colliders
        /// This method uses the shape, mass density and transform of the colliders of the body to set
        /// the total mass, the center of mass and the local inertia tensor of the body.
        /// Note that calling this method will overwrite the
        /// mass that has been set with the RigidBody::setMass(), the center of mass that has been
        /// set with RigidBody::setCenterOfMass() and the local inertia tensor that has been set with
        /// RigidBody::setInertiaTensorLocal().
        public void updateMassPropertiesFromColliders()
        {
            var oldCenterOfMassWorld = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);

            // Compute the local center of mass
            var centerOfMassLocal = computeCenterOfMass();

            var centerOfMassWorld = mWorld.mTransformComponents.getTransform(mEntity) * centerOfMassLocal;

            // Set the center of mass
            mWorld.mRigidBodyComponents.SetCenterOfMassLocal(mEntity, centerOfMassLocal);
            mWorld.mRigidBodyComponents.SetCenterOfMassWorld(mEntity, centerOfMassWorld);

            // If it is a dynamic body
            var type = mWorld.mRigidBodyComponents.GetBodyType(mEntity);
            if (type == BodyType.DYNAMIC)
            {
                // Update the linear velocity of the center of mass
                var linearVelocity = mWorld.mRigidBodyComponents.GetLinearVelocity(mEntity);
                var angularVelocity = mWorld.mRigidBodyComponents.GetAngularVelocity(mEntity);
                linearVelocity += angularVelocity.cross(centerOfMassWorld - oldCenterOfMassWorld);
                mWorld.mRigidBodyComponents.SetLinearVelocity(mEntity, linearVelocity);
            }


            // Compute the mass and local-space inertia tensor
            var inertiaTensorLocal = PVector3.Zero();
            var totalMass = 0.0f;
            computeMassAndInertiaTensorLocal(out inertiaTensorLocal, out totalMass);

            mWorld.mRigidBodyComponents.SetLocalInertiaTensor(mEntity, inertiaTensorLocal);

            // If it is a dynamic body
            if (type == BodyType.DYNAMIC)
            {
                // Compute the inverse local inertia tensor
                var inverseInertiaTensorLocal = new PVector3(
                    inertiaTensorLocal.x != 0.0f ? 1.0f / inertiaTensorLocal.x : 0,
                    inertiaTensorLocal.y != 0.0f ? 1.0f / inertiaTensorLocal.y : 0,
                    inertiaTensorLocal.z != 0.0f ? 1.0f / inertiaTensorLocal.z : 0);
                mWorld.mRigidBodyComponents.SetInverseInertiaTensorLocal(mEntity, inverseInertiaTensorLocal);
            }


            // Set the mass
            mWorld.mRigidBodyComponents.SetMass(mEntity, totalMass);

            // If it is a dynamic body
            if (type == BodyType.DYNAMIC)
            {
                // Compute the inverse mass
                if (totalMass > 0.0f)
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 1.0f / totalMass);
                else
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 0.0f);
            }
        }


        // Set the mass of the rigid body
        /**
         * Note that a mass of zero is interpreted as infinite mass.
         * @param mass The mass (in kilograms) of the body
         */
        public void setMass(float mass)
        {
            if (mass < 0.0f) return;

            mWorld.mRigidBodyComponents.SetMass(mEntity, mass);

            // If it is a dynamic body
            var type = mWorld.mRigidBodyComponents.GetBodyType(mEntity);
            if (type == BodyType.DYNAMIC)
            {
                if (mass > 0.0f)
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 1.0f / mass);
                else
                    mWorld.mRigidBodyComponents.SetMassInverse(mEntity, 0.0f);
            }
        }


        // Create a new collider and add it to the body
        /**
         * This method will return a pointer to a new collider. A collider is
         * an object with a collision shape that is attached to a body. It is possible to
         * attach multiple colliders to a given body. You can use the
         * returned collider to get and set information about the corresponding
         * collision shape for that body.
         * @param collisionShape A pointer to the collision shape of the new collider
         * @param transform The transformation of the collider that transforms the
         * local-space of the collider into the local-space of the body
         * @return A pointer to the collider that has been created
         */
        public Collider addCollider(CollisionShape collisionShape, PTransform transform)
        {
            // Create a new entity for the collider
            var colliderEntity = mWorld.mEntityManager.CreateEntity();

            // Check that the transform is valid
            // if (!transform.IsValid()) {
            //
            // }

            // Create a new collider for the body
            var collider = new Collider(colliderEntity, this);

            // Add the collider component to the entity of the body
            PVector3 localBoundsMin;
            PVector3 localBoundsMax;
            // TODO : Maybe this method can directly returns an AABB
            collisionShape.GetLocalBounds(out localBoundsMin, out localBoundsMax);
            var localToWorldTransform = mWorld.mTransformComponents.getTransform(mEntity) * transform;
            var material = new Material(mWorld.mConfig.defaultFrictionCoefficient, mWorld.mConfig.defaultBounciness);
            // ColliderComponent colliderComponent = new ColliderComponent(mEntity, collider,new AABB(localBoundsMin, localBoundsMax),
            //                                                         transform, collisionShape, 0x0001, 0xFFFF, localToWorldTransform);

            var colliderComponent = new ColliderComponent
            {
                bodyEntity = mEntity,
                collider = collider,
                localBounds = new AABB(localBoundsMin, localBoundsMax),
                LocalToBodyPTransform = transform,
                collisionShape = collisionShape,
                collisionCategoryBits = 1,
                collideWithMaskBits = short.MaxValue,
                material = material,
                LocalToWorldPTransform = localToWorldTransform
            };

            var isSleeping = mWorld.mRigidBodyComponents.GetIsSleeping(mEntity);
            mWorld.mCollidersComponents.addComponent(colliderEntity, isSleeping, colliderComponent);

            mWorld.mCollisionBodyComponents.addColliderToBody(mEntity, colliderEntity);

            // Assign the collider with the collision shape
            collisionShape.addCollider(collider);


            // Compute the world-space AABB of the new collision shape
            AABB aabb;
            collisionShape.ComputeAABB(out aabb, mWorld.mTransformComponents.getTransform(mEntity) * transform);

            // Notify the collision detection about this new collision shape
            mWorld.mCollisionDetection.addCollider(collider, aabb);

            // Return a pointer to the collider
            return collider;
        }




        // Set the variable to know if the gravity is applied to this rigid body
        /**
         * @param isEnabled True if you want the gravity to be applied to this body
         */
        public void enableGravity(bool isEnabled)
        {
            mWorld.mRigidBodyComponents.SetIsGravityEnabled(mEntity, isEnabled);
        }

        // Set the linear damping factor.
        /**
         * @param linearDamping The linear damping factor of this body (in range [0; +inf]). Zero means no damping.
         */
        public void setLinearDamping(float linearDamping)
        {
            if (linearDamping >= 0.0f) mWorld.mRigidBodyComponents.SetLinearDamping(mEntity, linearDamping);
        }

        // Set the angular damping factor.
        /**
         * @param angularDamping The angular damping factor of this body (in range [0; +inf]). Zero means no damping.
         */
        public void setAngularDamping(float angularDamping)
        {
            if (angularDamping >= 0.0f) mWorld.mRigidBodyComponents.SetAngularDamping(mEntity, angularDamping);
        }

        // Set the linear velocity of the rigid body.
        /**
         * @param linearVelocity Linear velocity vector of the body
         */
        public void setLinearVelocity(PVector3 linearVelocity)
        {
            // If it is a static body, we do nothing
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) == BodyType.STATIC) return;

            // Update the linear velocity of the current body state
            mWorld.mRigidBodyComponents.SetLinearVelocity(mEntity, linearVelocity);

            // If the linear velocity is not zero, awake the body
            if (linearVelocity.LengthSquare() > 0.0f) setIsSleeping(false);
        }

        // 设置角速度。
        /**
         * @param angularVelocity 身体的角速度向量
         */
        public void setAngularVelocity(PVector3 angularVelocity)
        {
            // 如果是静态物体，则不执行任何操作
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) == BodyType.STATIC) return;

            // 设置角速度
            mWorld.mRigidBodyComponents.SetAngularVelocity(mEntity, angularVelocity);

            // 如果速度不为零，则唤醒物体
            if (angularVelocity.LengthSquare() > 0.0f) setIsSleeping(false);
        }

        // Set the current position and orientation
        /**
         * @param transform The transformation of the body that transforms the local-space
         * of the body into world-space
         */
        public override void setTransform(PTransform transform)
        {
            var oldCenterOfMass = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);

            // Compute the new center of mass in world-space coordinates
            var centerOfMassLocal = mWorld.mRigidBodyComponents.GetCenterOfMassLocal(mEntity);
            mWorld.mRigidBodyComponents.SetCenterOfMassWorld(mEntity, transform * centerOfMassLocal);

            // Update the linear velocity of the center of mass
            var linearVelocity = mWorld.mRigidBodyComponents.GetLinearVelocity(mEntity);
            var angularVelocity = mWorld.mRigidBodyComponents.GetAngularVelocity(mEntity);
            var centerOfMassWorld = mWorld.mRigidBodyComponents.GetCenterOfMassWorld(mEntity);
            linearVelocity += angularVelocity.cross(centerOfMassWorld - oldCenterOfMass);
            mWorld.mRigidBodyComponents.SetLinearVelocity(mEntity, linearVelocity);

            base.setTransform(transform);

            // Awake the body if it is sleeping
            setIsSleeping(false);
        }

        // Return the linear velocity
        /**
         * @return The linear velocity vector of the body
         */
        public PVector3 getLinearVelocity()
        {
            return mWorld.mRigidBodyComponents.GetLinearVelocity(mEntity);
        }

        // Return the angular velocity of the body
        /**
         * @return The angular velocity vector of the body
         */
        public PVector3 getAngularVelocity()
        {
            return mWorld.mRigidBodyComponents.GetAngularVelocity(mEntity);
        }

        // Return true if the gravity needs to be applied to this rigid body
        /**
         * @return True if the gravity is applied to the body
         */
        public bool isGravityEnabled()
        {
            return mWorld.mRigidBodyComponents.GetIsGravityEnabled(mEntity);
        }

        // Return the linear lock axis factor
        /**
         * The linear lock axis factor specify whether linear motion along world-space axes X,Y,Z is
         * restricted or not.
         * @return A Vector3 with the linear lock axis factor for each X,Y,Z world-space axis
         */
        public PVector3 getLinearLockAxisFactor()
                {
                    return mWorld.mRigidBodyComponents.GetLinearLockAxisFactor(mEntity);
                }

        // Set the linear lock axis factor
        /**
         * This method allows to restrict the linear motion of a rigid body along the world-space
         * axes X,Y and Z. For instance, it's possible to disable the linear motion of a body
         * along a given axis by setting a lock axis factor of zero.
         * @param linearLockAxisFactor A Vector3 with the lock factor for each world-space axis X,Y,Z
         */
        public void setLinearLockAxisFactor(PVector3 linearLockAxisFactor)
                {
                    mWorld.mRigidBodyComponents.SetLinearLockAxisFactor(mEntity, linearLockAxisFactor);
                }

        // Return the angular lock axis factor
        /**
         * The angular lock axis factor specify whether angular motion around world-space axes X,Y,Z is
         * restricted or not.
         * @return A Vector3 with the angular lock axis factor for each X,Y,Z world-space axis
         */
        public PVector3 getAngularLockAxisFactor()
        {
            return mWorld.mRigidBodyComponents.GetAngularLockAxisFactor(mEntity);
        }

        // Set the angular lock axis factor
        /**
         * This method allows to restrict the angular motion of a rigid body around the world-space
         * axes X,Y and Z. For instance, it's possible to disable the angular motion of a body
         * around a given axis by setting a lock axis factor of zero.
         * @param angularLockAxisFactor A Vector3 with the lock factor for each world-space axis X,Y,Z
         */
        public void setAngularLockAxisFactor(PVector3 angularLockAxisFactor)
        {
            mWorld.mRigidBodyComponents.SetAngularLockAxisFactor(mEntity, angularLockAxisFactor);
        }


        // Manually apply an external torque to the body (in world-space).
        /**
         * If the body is sleeping, calling this method will wake it up. Note that the
         * force will we added to the sum of the applied torques and that this sum will be
         * reset to zero at the end of each call of the PhyscisWorld::update() method.
         * You can only apply a force to a dynamic body otherwise, this method will do nothing.
         * @param torque The external torque to apply on the body (in world-space)
         */
        public void applyWorldTorque(PVector3 torque)
        {
            // If it is not a dynamic body, we do nothing
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) != BodyType.DYNAMIC) return;

            // Awake the body if it was sleeping
            if (mWorld.mRigidBodyComponents.GetIsSleeping(mEntity)) setIsSleeping(false);

            // Add the torque
            var externalTorque = mWorld.mRigidBodyComponents.GetExternalTorque(mEntity);
            mWorld.mRigidBodyComponents.SetExternalTorque(mEntity, externalTorque + torque);
        }


        // Manually apply an external torque to the body (in local-space).
        /**
         * If the body is sleeping, calling this method will wake it up. Note that the
         * force will we added to the sum of the applied torques and that this sum will be
         * reset to zero at the end of each call of the PhyscisWorld::update() method.
         * You can only apply a force to a dynamic body otherwise, this method will do nothing.
         * @param torque The external torque to apply on the body (in local-space)
         */
        public void applyLocalTorque(PVector3 torque)
        {
            // Convert the local-space torque to world-space
            var worldTorque = mWorld.mTransformComponents.getTransform(mEntity).GetOrientation() * torque;

            applyWorldTorque(worldTorque);
        }


        // Reset the accumulated force to zero
        public void resetForce()
        {
            // If it is not a dynamic body, we do nothing
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) != BodyType.DYNAMIC) return;

            // Set the external force to zero
            mWorld.mRigidBodyComponents.SetExternalForce(mEntity, new PVector3(0, 0, 0));
        }

        // Reset the accumulated torque to zero
        public void resetTorque()
        {
            // If it is not a dynamic body, we do nothing
            if (mWorld.mRigidBodyComponents.GetBodyType(mEntity) != BodyType.DYNAMIC) return;

            // Set the external torque to zero
            mWorld.mRigidBodyComponents.SetExternalTorque(mEntity, new PVector3(0, 0, 0));
        }


        // 返回施加在刚体上的总手动应用力（在世界空间中）
        /**
         * 返回施加在刚体上的总手动应用力（在世界空间中）
         * @return 刚体上的总手动应用力（在世界空间中）
         */
        public PVector3 RigidBodygetForce()
        {
            return mWorld.mRigidBodyComponents.GetExternalForce(mEntity);
        }

        // 返回作用在物体上的总手动施加的扭矩（在世界空间中）
        /**
        @return 物体上的总手动施加的扭矩（在世界空间中）
        */
        public PVector3 getTorque()
        {
            return mWorld.mRigidBodyComponents.GetExternalTorque(mEntity);
        }


        // 设置变量以确定物体是否处于休眠状态
        public void setIsSleeping(bool isSleeping)
        {
            var isBodySleeping = mWorld.mRigidBodyComponents.GetIsSleeping(mEntity);

            if (isBodySleeping == isSleeping) return;

            // 如果物体不活动，则不执行任何操作（它正在休眠）
            if (!mWorld.mCollisionBodyComponents.getIsActive(mEntity)) return;

            if (isSleeping)
            {
                mWorld.mRigidBodyComponents.SetSleepTime(mEntity, 0.0f);
            }
            else
            {
                if (isBodySleeping) 
                    mWorld.mRigidBodyComponents.SetSleepTime(mEntity, 0.0f);
            }

            mWorld.mRigidBodyComponents.SetIsSleeping(mEntity, isSleeping);

            // 通知所有组件
            mWorld.setBodyDisabled(mEntity, isSleeping);

            // 更新当前重叠的成对项
            resetOverlappingPairs();

            if (isSleeping)
            {
                mWorld.mRigidBodyComponents.SetLinearVelocity(mEntity, PVector3.Zero());
                mWorld.mRigidBodyComponents.SetAngularVelocity(mEntity, PVector3.Zero());
                mWorld.mRigidBodyComponents.SetExternalForce(mEntity, PVector3.Zero());
                mWorld.mRigidBodyComponents.SetExternalTorque(mEntity, PVector3.Zero());
            }
        }


        public void resetOverlappingPairs()
        {
            // 对于每个碰撞体
            var colliderEntities = mWorld.mCollisionBodyComponents.getColliders(mEntity);
            foreach (var enity in colliderEntities)
            {
                // 获取此碰撞体当前重叠的成对项
                var overlappingPairs = mWorld.mCollidersComponents.getOverlappingPairs(enity);

                while (overlappingPairs.Count>0)
                {
                    mWorld.mCollisionDetection.mOverlappingPairs.removePair(overlappingPairs[0]);
                }
                
                // for (var j = 0; j < overlappingPairs.Count; j++)
                //     mWorld.mCollisionDetection.mOverlappingPairs.removePair(overlappingPairs[j]);
                // foreach (var value in overlappingPairs)
                //     mWorld.mCollisionDetection.mOverlappingPairs.removePair(value);
            }

            // 确保我们在下一帧重新计算与此体的重叠对
            askForBroadPhaseCollisionCheck();
        }



        public void setIsAllowedToSleep(bool isAllowedToSleep)
        {
            mWorld.mRigidBodyComponents.SetIsAllowedToSleep(mEntity, isAllowedToSleep);
            if (!isAllowedToSleep) setIsSleeping(false);
        }

        public bool isAllowedToSleep()
        {
            return mWorld.mRigidBodyComponents.GetIsAllowedToSleep(mEntity);
        }

        public bool isSleeping()
        {
            return mWorld.mRigidBodyComponents.GetIsSleeping(mEntity);
        }

        public override void setIsActive(bool isActive)
        {
            // 如果状态没有改变
            if (mWorld.mCollisionBodyComponents.getIsActive(mEntity) == isActive) return;
            setIsSleeping(!isActive);
            base.setIsActive(isActive);
        }

        // 返回施加在物体上的总手动外力（在世界空间中）
        /**
         * @return 施加在物体上的总手动外力（在世界空间中）
         */
        public PVector3 getForce()
        {
            return mWorld.mRigidBodyComponents.GetExternalForce(mEntity);
        }
        


    }
}