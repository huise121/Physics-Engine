namespace RP3D
{
    public partial class PhysicsWorld
    {
        public CollisionBody createCollisionBody(PTransform pTransform)
        {
            var entity = mEntityManager.CreateEntity();
            mTransformComponents.AddComponent(entity, false, new TransformComponent { PTransform = pTransform });
            var collisionBody = new CollisionBody(this, entity);
            var bodyComponent = new CollisionBodyComponent
            {
                body = collisionBody
            };
            mCollisionBodyComponents.addComponent(entity, false, bodyComponent);
            mCollisionBodies.Add(collisionBody);
            return collisionBody;
        }

        public void destroyCollisionBody(CollisionBody collisionBody)
        {
            collisionBody.removeAllColliders();
            mCollisionBodyComponents.removeComponent(collisionBody.getEntity());
            mTransformComponents.removeComponent(collisionBody.getEntity());
            mEntityManager.destroyEntity(collisionBody.getEntity());
            //collisionBody.~CollisionBody();
            mCollisionBodies.Remove(collisionBody);
        }

        public RigidBody createRigidBody(PTransform pTransform)
        {
            var entity = mEntityManager.CreateEntity();
            mTransformComponents.AddComponent(entity, false, new TransformComponent { PTransform = pTransform });
            var rigidBody = new RigidBody(this, entity);
            mCollisionBodyComponents.addComponent(entity, false, new CollisionBodyComponent { body = rigidBody });
            mRigidBodyComponents.addComponent(entity, false,
                new RigidBodyComponent
                    { body = rigidBody, bodyType = BodyType.DYNAMIC, worldPosition = pTransform.GetPosition() });
            mRigidBodyComponents.SetMassInverse(entity, 1.0f / mRigidBodyComponents.GetMass(entity));
            mRigidBodies.Add(rigidBody);
            return rigidBody;
        }

        public void destroyRigidBody(RigidBody rigidBody)
        {
            rigidBody.removeAllColliders();
            mCollisionBodyComponents.removeComponent(rigidBody.getEntity());
            mRigidBodyComponents.removeComponent(rigidBody.getEntity());
            mTransformComponents.removeComponent(rigidBody.getEntity());
            mEntityManager.destroyEntity(rigidBody.getEntity());
            mRigidBodies.Remove(rigidBody);
        }


        public CollisionBody getCollisionBody(int index)
        {
            return mCollisionBodies[index];
        }

        public RigidBody getRigidBody(int index)
        {
            return mRigidBodies[index];
        }

        // Return the current world-space AABB of given collider
        /**
         * @param collider Pointer to a collider
         * @return The AAABB of the collider in world-space
         */
        public AABB getWorldAABB(Collider collider)
        {
            if (collider.getBroadPhaseId() == -1) return new AABB();
            return mCollisionDetection.getWorldAABB(collider);
        }

        // Set the number of iterations for the velocity constraint solver
        /**
         * @param nbIterations Number of iterations for the velocity solver
         */
        public void setNbIterationsVelocitySolver(int nbIterations)
        {
            mNbVelocitySolverIterations = nbIterations;
        }

        // Set the number of iterations for the position constraint solver
        /**
         * @param nbIterations Number of iterations for the position solver
         */
        public void setNbIterationsPositionSolver(int nbIterations)
        {
            mNbPositionSolverIterations = nbIterations;
        }

        // Set the gravity vector of the world
        /**
         * @param gravity The gravity vector (in meter per seconds squared)
         */
        public void setGravity(PVector3 gravity)
        {
            mConfig.gravity = gravity;
        }

        // Set the sleep linear velocity.
        /**
         * When the velocity of a body becomes smaller than the sleep linear/angular
         * velocity for a given amount of time, the body starts sleeping and does not need
         * to be simulated anymore.
         * @param sleepLinearVelocity The sleep linear velocity (in meters per second)
         */
        public void setSleepLinearVelocity(float sleepLinearVelocity)
        {
            mSleepLinearVelocity = sleepLinearVelocity;
        }

        // Set the sleep angular velocity.
        /**
         * When the velocity of a body becomes smaller than the sleep linear/angular
         * velocity for a given amount of time, the body starts sleeping and does not need
         * to be simulated anymore.
         * @param sleepAngularVelocity The sleep angular velocity (in radian per second)
         */
        public void setSleepAngularVelocity(float sleepAngularVelocity)
        {
            mSleepAngularVelocity = sleepAngularVelocity;
        }

        // Set the time a body is required to stay still before sleeping
        /**
         * @param timeBeforeSleep Time a body is required to stay still before sleeping (in seconds)
         */
        public void setTimeBeforeSleep(float timeBeforeSleep)
        {
            mTimeBeforeSleep = timeBeforeSleep;
        }

        // Enable/Disable the gravity
        /**
         * @param isGravityEnabled True if you want to enable the gravity in the world
         * and false otherwise
         */
        public void setIsGravityEnabled(bool isGravityEnabled)
        {
            mIsGravityEnabled = isGravityEnabled;
        }


        // Get the number of iterations for the velocity constraint solver
        /**
         * @return The number of iterations of the velocity constraint solver
         */
        public int getNbIterationsVelocitySolver()
        {
            return mNbVelocitySolverIterations;
        }

// Get the number of iterations for the position constraint solver
        /**
         * @return The number of iterations of the position constraint solver
         */
        public int getNbIterationsPositionSolver()
        {
            return mNbPositionSolverIterations;
        }

        // 设置用于接触点的位置修正技术
        /**
         * @param technique 用于位置修正的技术（Baumgarte 或 Split Impulses）
         */
        public void setContactsPositionCorrectionTechnique(
            configuration.ContactsPositionCorrectionTechnique technique)
        {
            if (technique == configuration.ContactsPositionCorrectionTechnique.BAUMGARTE_CONTACTS)
                mContactSolverSystem.setIsSplitImpulseActive(false);
            else
                mContactSolverSystem.setIsSplitImpulseActive(true);
        }

// Return the gravity vector of the world
        /**
         * @return The current gravity vector (in meter per seconds squared)
         */
        public PVector3 getGravity()
        {
            return mConfig.gravity;
        }

// Return if the gravity is enaled
        /**
         * @return True if the gravity is enabled in the world
         */
        public bool isGravityEnabled()
        {
            return mIsGravityEnabled;
        }

// Return true if the sleeping technique is enabled
        /**
         * @return True if the sleeping technique is enabled and false otherwise
         */
        public bool isSleepingEnabled()
        {
            return mIsSleepingEnabled;
        }

// Return the current sleep linear velocity
        /**
         * @return The sleep linear velocity (in meters per second)
         */
        public float getSleepLinearVelocity()
        {
            return mSleepLinearVelocity;
        }

// Return the current sleep angular velocity
        /**
         * @return The sleep angular velocity (in radian per second)
         */
        public float getSleepAngularVelocity()
        {
            return mSleepAngularVelocity;
        }

// Return the time a body is required to stay still before sleeping
        /**
         * @return Time a body is required to stay still before sleeping (in seconds)
         */
        public float getTimeBeforeSleep()
        {
            return mTimeBeforeSleep;
        }

// Set an event listener object to receive events callbacks.
/**
         * If you use "nullptr" as an argument, the events callbacks will be disabled.
         * @param eventListener Pointer to the event listener object that will receive
         * event callbacks during the simulation
         */
public void setEventListener(EventListener eventListener)
        {
            mEventListener = eventListener;
        }

// Return the number of CollisionBody in the physics world
/**
         * Note that even if a RigidBody is also a collision body, this method does not return the rigid bodies
         * @return The number of collision bodies in the physics world
         */
public int getNbCollisionBodies()
        {
            return mCollisionBodies.Count;
        }

// Return the number of RigidBody in the physics world
        /**
         * @return The number of rigid bodies in the physics world
         */
        public int getNbRigidBodies()
        {
            return mRigidBodies.Count;
        }
    }
}