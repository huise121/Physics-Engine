using System.Collections.Generic;
using UnityEngine;

namespace RP3D
{
    public partial class PhysicsWorld
    {
        public static int mNbWorlds;
        public List<int> mProcessContactPairsOrderIslands;
        private readonly List<CollisionBody> mCollisionBodies;
        public EntityManager mEntityManager;
        public CollisionBodyComponents mCollisionBodyComponents;
        public TransformComponents mTransformComponents;
        public ColliderComponents mCollidersComponents;
        public RigidBodyComponents mRigidBodyComponents;
        public CollisionDetectionSystem mCollisionDetection;
        public List<RigidBody> mRigidBodies;
        public DynamicsSystem mDynamicsSystem;
        private readonly ContactSolverSystem mContactSolverSystem;
        
        /// 线性速度阈值以进入休眠状态
        private  float mSleepLinearVelocity;
        /// 角速度阈值以进入休眠状态
        private  float mSleepAngularVelocity;
        
        private bool mIsSleepingEnabled;
        
        private bool mIsGravityEnabled = true;
        
        public WorldSettings mConfig;
        public EventListener mEventListener;
        
        public Islands mIslands;
        
        private float mTimeBeforeSleep;

        /// 用于顺序冲量技术的速度求解器迭代次数
        private int mNbVelocitySolverIterations;
        /// 用于顺序冲量技术的位置求解器迭代次数
        private int mNbPositionSolverIterations;

        public PhysicsWorld(PhysicsCommon physicsCommon, WorldSettings worldSettings)
        {
            mConfig = worldSettings;
            mTimeBeforeSleep = mConfig.defaultTimeBeforeSleep;
            mSleepLinearVelocity = mConfig.defaultSleepLinearVelocity;
            mSleepAngularVelocity = mConfig.defaultSleepAngularVelocity;
            mNbVelocitySolverIterations = mConfig.defaultVelocitySolverNbIterations;
            mNbPositionSolverIterations = mConfig.defaultPositionSolverNbIterations;
            mIsSleepingEnabled = mConfig.isSleepingEnabled;
            
            mIsGravityEnabled = true;
            
            mEntityManager = new EntityManager();
            mCollisionBodies = new List<CollisionBody>();
            mRigidBodies = new List<RigidBody>();
            
            mCollisionBodyComponents = new CollisionBodyComponents();
            mTransformComponents = new TransformComponents();
            mCollidersComponents = new ColliderComponents();
            mRigidBodyComponents = new RigidBodyComponents();
            
            mCollisionDetection = new CollisionDetectionSystem(this, mCollidersComponents, mTransformComponents,
                /*mCollisionBodyComponents,*/
                mRigidBodyComponents/*, physicsCommon.mTriangleShapeHalfEdgeStructure*/);
           
            mDynamicsSystem = new DynamicsSystem(mRigidBodyComponents, mTransformComponents, mCollidersComponents, mIsGravityEnabled, mConfig.gravity);
            
            mIslands = new Islands();
            
            mContactSolverSystem = new ContactSolverSystem(mIslands,
                mRigidBodyComponents,mCollidersComponents, mConfig.restitutionVelocityThreshold);
            
            mProcessContactPairsOrderIslands = new List<int>();
            
            mEventListener = null;
            mNbWorlds++;
        }


        public void update(float timeStep)
        {
            timeStep = 0.0166666675f;
     
            // 计算碰撞检测
            mCollisionDetection.computeCollisionDetection();

            // 创建岛屿
            createIslands();

            // 创建实际的接触凸体和接触点
            mCollisionDetection.createContacts();

            // 向用户报告接触点信息
            mCollisionDetection.reportContactsAndTriggers();

            // 重新计算刚体的逆惯性张量
            updateBodiesInverseWorldInertiaTensors();

            // 积分速度
            mDynamicsSystem.integrateRigidBodiesVelocities(timeStep);

            // 解决接触和约束
            solveContactsAndConstraints(timeStep);

            // 积分每个刚体的位置和方向
            mDynamicsSystem.integrateRigidBodiesPositions(timeStep, false);

            // 更新刚体的状态（位置和速度）
            mDynamicsSystem.updateBodiesState();

            // 更新碰撞体
            mCollisionDetection.updateColliders();

            // 如果启用了睡眠，更新休眠刚体
            if (mIsSleepingEnabled) updateSleepingBodies(timeStep);

            // 重置施加在刚体上的外部力和扭矩
            mDynamicsSystem.resetBodiesForceAndTorque();

            // 重置岛屿
            mIslands.clear();
            mProcessContactPairsOrderIslands.Clear();
        }


        // 解决接触和约束
        private void solveContactsAndConstraints(float timeStep)
        {
            // ---------- 解决关节和接触的速度约束 ---------- //
            // 初始化接触求解器
            mContactSolverSystem.init(mCollisionDetection.mCurrentContactManifolds,
                mCollisionDetection.mCurrentContactPoints, timeStep);
            // 对于速度求解器的每次迭代

            if (mCollisionDetection.mCurrentContactManifolds.Count > 0 ||
                mCollisionDetection.mCurrentContactPoints.Count > 0)
            {

                for (var i = 0; i < mNbVelocitySolverIterations; i++)
                {
                    mContactSolverSystem.solve();
                }
                // Debug.Log("------------------solve-----------------------" + PhysicsWorld.frameIndex);
            }
            mContactSolverSystem.storeImpulses();
            // 重置接触求解器
            mContactSolverSystem.reset();
        }
        

        // 射线投射方法
        /**
         * @param ray 用于射线投射的射线
         * @param raycastCallback 包含回调方法的类的指针
         * @param raycastWithCategoryMaskBits 与要进行射线投射的刚体类别对应的位掩码
         */
        public void raycast(PRay ray, RaycastCallback raycastCallback, short raycastWithCategoryMaskBits = short.MaxValue)
        {
            mCollisionDetection.raycast(raycastCallback, ray, raycastWithCategoryMaskBits);
        }

        /// Return true if two bodies overlap (collide)
        public bool testOverlap(CollisionBody body1, CollisionBody body2)
        {
            return mCollisionDetection.testOverlap(body1, body2);
        }

        /// Report all the bodies that overlap (collide) with the body in parameter
        public void testOverlap(CollisionBody body, OverlapCallback overlapCallback)
        {
            mCollisionDetection.testOverlap(body, overlapCallback);
        }

        /// Report all the bodies that overlap (collide) in the world
        public void testOverlap(OverlapCallback overlapCallback)
        {
            mCollisionDetection.testOverlap(overlapCallback);
        }

        /// Test collision and report contacts between two bodies.
        public void testCollision(CollisionBody body1, CollisionBody body2, CollisionCallback callback)
        {
            mCollisionDetection.testCollision(body1, body2, callback);
        }

        /// Test collision and report all the contacts involving the body in parameter
        public void testCollision(CollisionBody body, CollisionCallback callback)
        {
            mCollisionDetection.testCollision(body, callback);
        }

        /// Test collision and report contacts between each colliding bodies in the world
        public void testCollision(CollisionCallback callback)
        {
            mCollisionDetection.testCollision(callback);
        }

  


 
        
        //Solve the position error correction of the constraints
        // public void solvePositionCorrection() {
        //     // ---------- Solve the position error correction for the constraints ---------- //
        //
        //     // For each iteration of the position (error correction) solver
        //     for (int i=0; i<mNbPositionSolverIterations; i++) {
        //
        //         // Solve the position constraints
        //         mConstraintSolverSystem.solvePositionConstraints();
        //     }
        // }
       
        
    }
}