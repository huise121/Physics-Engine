using UnityEngine;

namespace RP3D
{
    public class DynamicsSystem
    {
        private RigidBodyComponents mRigidBodyComponents;
        private TransformComponents mTransformComponents;
        private ColliderComponents mColliderComponents;
        private bool mIsGravityEnabled;
        private PVector3 mGravity;

        public DynamicsSystem(RigidBodyComponents mRigidBodyComponents,TransformComponents mTransformComponents,ColliderComponents mCollidersComponents,
            bool mIsGravityEnabled,PVector3 gravity)
        {
            this.mRigidBodyComponents = mRigidBodyComponents;
            this.mTransformComponents = mTransformComponents;
            this.mColliderComponents = mCollidersComponents;
            this.mIsGravityEnabled = mIsGravityEnabled;
            this.mGravity = gravity;
        }

        
        //对刚体的速度进行积分。
        ///此方法仅设置临时速度，但不更新物体的实际速度。此方法中更新的速度可能违反约束，并将在约束中进行更正，以及接触解算器。
        public void integrateRigidBodiesPositions(float timeStep, bool isSplitImpulseActive) {
            float isSplitImpulseFactor = isSplitImpulseActive ? 1.0f : 0.0f;

            int nbRigidBodyComponents = mRigidBodyComponents.getNbEnabledComponents();
            for (int i=0; i < nbRigidBodyComponents; i++) {

                // Get the constrained velocity
                PVector3 newLinVelocity = mRigidBodyComponents.mConstrainedLinearVelocities[i];
                PVector3 newAngVelocity = mRigidBodyComponents.mConstrainedAngularVelocities[i];

                // Add the split impulse velocity from Contact Solver (only used
                // to update the position)
                newLinVelocity += isSplitImpulseFactor * mRigidBodyComponents.mSplitLinearVelocities[i];
                newAngVelocity += isSplitImpulseFactor * mRigidBodyComponents.mSplitAngularVelocities[i];

                // Get current position and orientation of the body
                PVector3 currentPosition = mRigidBodyComponents.mCentersOfMassWorld[i];
                PQuaternion currentOrientation = mTransformComponents.getTransform(mRigidBodyComponents.mBodiesEntities[i]).GetOrientation();

                // Update the new constrained position and orientation of the body
                mRigidBodyComponents.mConstrainedPositions[i] = currentPosition + newLinVelocity * timeStep;
                mRigidBodyComponents.mConstrainedOrientations[i] = currentOrientation + new PQuaternion(0, newAngVelocity) *
                    currentOrientation * 0.5f * timeStep;
                PQuaternion q = mRigidBodyComponents.mConstrainedOrientations[i];
            }
        }

        // 更新刚体的位置和方向
        public void updateBodiesState()
        {

            int nbRigidBodyComponents = mRigidBodyComponents.getNbEnabledComponents();
            for (int i = 0; i < nbRigidBodyComponents; i++)
            {

                // 更新刚体的线性和角速度
                mRigidBodyComponents.mLinearVelocities[i] = mRigidBodyComponents.mConstrainedLinearVelocities[i];
                mRigidBodyComponents.mAngularVelocities[i] = mRigidBodyComponents.mConstrainedAngularVelocities[i];
                // 更新刚体的质心位置
                mRigidBodyComponents.mCentersOfMassWorld[i] = mRigidBodyComponents.mConstrainedPositions[i];
                // 更新刚体的方向
                PQuaternion constrainedOrientation = mRigidBodyComponents.mConstrainedOrientations[i];
                
                mTransformComponents.getTransform(mRigidBodyComponents.mBodiesEntities[i])
                    .SetOrientation(constrainedOrientation.getUnit());
            }

            // 更新刚体的位置（使用新的质心和新的方向）
            for (int i = 0; i < nbRigidBodyComponents; i++)
            {

                PTransform pTransform = mTransformComponents.getTransform(mRigidBodyComponents.mBodiesEntities[i]);
                PVector3 centerOfMassWorld = mRigidBodyComponents.mCentersOfMassWorld[i];
                PVector3 centerOfMassLocal = mRigidBodyComponents.mCentersOfMassLocal[i];
                pTransform.SetPosition(centerOfMassWorld - pTransform.GetOrientation() * centerOfMassLocal);

                

                // if (!pTransform.GetPosition().IsZero())
                // {
                //     Debug.Log("pTransform:" + pTransform.GetPosition());
                // }

                // if (!pTransform.GetPosition().IsZero())

            }
            

            // 更新碰撞体的局部到世界变换
            int nbColliderComponents = mColliderComponents.getNbEnabledComponents();
            for (int i = 0; i < nbColliderComponents; i++)
            {
                // 更新碰撞体的局部到世界变换
                mColliderComponents.mLocalToWorldTransforms[i] =
                    mTransformComponents.getTransform(mColliderComponents.mBodiesEntities[i]) *
                    mColliderComponents.mLocalToBodyTransforms[i];
            }
        }

        /// 对刚体的速度进行积分。
        /// 该方法仅设置临时速度，但不更新实际的刚体速度。在此方法中更新的速度可能违反约束，将在约束和接触求解器中进行校正。
        public void integrateRigidBodiesVelocities(float timeStep)
        {
            resetSplitVelocities();
            int nbEnabledRigidBodyComponents = mRigidBodyComponents.getNbEnabledComponents();
            for (int i = 0; i < nbEnabledRigidBodyComponents; i++)
            {
                PVector3 linearVelocity = mRigidBodyComponents.mLinearVelocities[i];
                PVector3 angularVelocity = mRigidBodyComponents.mAngularVelocities[i];
                
                mRigidBodyComponents.mConstrainedLinearVelocities[i] = linearVelocity + timeStep * mRigidBodyComponents.mInverseMasses[i] *
                    mRigidBodyComponents.mLinearLockAxisFactors[i] * mRigidBodyComponents.mExternalForces[i];
                mRigidBodyComponents.mConstrainedAngularVelocities[i] = angularVelocity + timeStep * mRigidBodyComponents.mAngularLockAxisFactors[i] *
                    (mRigidBodyComponents.mInverseInertiaTensorsWorld[i] * mRigidBodyComponents.mExternalTorques[i]);
                
                // if (!mRigidBodyComponents.mConstrainedLinearVelocities[i].IsFinite() || !mRigidBodyComponents.mConstrainedAngularVelocities[i].IsFinite()) {
                //     Debug.Log("pTransform:");
                // }
                
            }
            
            int nbRigidBodyComponents = mRigidBodyComponents.getNbEnabledComponents();

            if (mIsGravityEnabled) {

                for (int i=0; i < nbRigidBodyComponents; i++) {

                    //如果重力需要应用到这个刚体上
                    if (mRigidBodyComponents.mIsGravityEnabled[i]) {

                        //积分重力作用力
                        mRigidBodyComponents.mConstrainedLinearVelocities[i] += timeStep *
                            mRigidBodyComponents.mInverseMasses[i] * mRigidBodyComponents.mLinearLockAxisFactors[i] *
                            mRigidBodyComponents.mMasses[i] * mGravity;
                    }
                }
            }
            
            for (int i=0; i < nbRigidBodyComponents; i++) {

                float linDampingFactor = mRigidBodyComponents.mLinearDampings[i];
                float angDampingFactor = mRigidBodyComponents.mAngularDampings[i];
                float linearDamping = 1.0f / (1.0f + linDampingFactor * timeStep);
                float angularDamping = 1.0f / (1.0f + angDampingFactor * timeStep);
                mRigidBodyComponents.mConstrainedLinearVelocities[i] *= linearDamping;
                mRigidBodyComponents.mConstrainedAngularVelocities[i] *= angularDamping;
            }
        }
        
        // 重置施加在刚体上的外部力和扭矩
        public void resetBodiesForceAndTorque() {
            int nbRigidBodyComponents = mRigidBodyComponents.getNbComponents();
            for (int i=0; i < nbRigidBodyComponents; i++) {
                mRigidBodyComponents.mExternalForces[i].SetToZero();
                mRigidBodyComponents.mExternalTorques[i].SetToZero();
            }
        }
        
        // 重置刚体的分裂速度
        public void resetSplitVelocities() {

            int nbRigidBodyComponents = mRigidBodyComponents.getNbEnabledComponents();
            for(int i=0; i < nbRigidBodyComponents; i++) {
                mRigidBodyComponents.mSplitLinearVelocities[i].SetToZero();
                mRigidBodyComponents.mSplitAngularVelocities[i].SetToZero();
            }
        }

        


    }
}