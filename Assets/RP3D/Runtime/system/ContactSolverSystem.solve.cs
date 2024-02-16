using UnityEngine;

namespace RP3D
{
    public partial class ContactSolverSystem
    {
        // 解决接触点
        public void solve()
        {
            float deltaLambda;
            float lambdaTemp;
            var contactPointIndex = 0;

            var beta = mIsSplitImpulseActive ? BETA_SPLIT_IMPULSE : BETA;

            PVector3 linearImpulse;
            PVector3 deltaV;
            float Jv;

            // 针对每个接触流形
            for (var c = 0; c < mNbContactManifolds; c++)
            {
                var sumPenetrationImpulse = 0.0f;

                var cmSolver = mContactConstraints[c];

                var rigidBody1Index = cmSolver.rigidBodyComponentIndexBody1;
                var rigidBody2Index = cmSolver.rigidBodyComponentIndexBody2;
                



                // 获取约束速度
                var v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                var w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                var v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                var w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];

                // if (!v1.IsFinite() || !w1.IsFinite() || !v2.IsFinite() || !w2.IsFinite())
                // {
                //     Debug.Log("pTransform:");
                // }
                
                for (var i = 0; i < cmSolver.nbContacts; i++)
                {
                    // --------- Penetration --------- //
                     v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                     w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                     v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                     w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];

                     // if (!v1.IsFinite() || !w1.IsFinite() || !v2.IsFinite() || !w2.IsFinite())
                     // {
                     //     Debug.Log("pTransform:");
                     // }
                     
                    // 计算 J*v
                    deltaV = v2 + w2.cross(mContactPoints[contactPointIndex].r2) - v1 -
                             w1.cross(mContactPoints[contactPointIndex].r1);
                    var deltaVDotN = deltaV.dot(mContactPoints[contactPointIndex].normal);
                    Jv = deltaVDotN;

                    // 计算约束的偏差 "b"
                    var biasPenetrationDepth = 0.0f;
                    if (mContactPoints[contactPointIndex].penetrationDepth > SLOP)
                        biasPenetrationDepth = -(beta / mTimeStep) * PMath.Max(0.0f,
                            mContactPoints[contactPointIndex].penetrationDepth - SLOP);
                    var b = biasPenetrationDepth + mContactPoints[contactPointIndex].restitutionBias;

                    // 计算拉格朗日乘子 lambda
                    if (mIsSplitImpulseActive)
                        deltaLambda = -(Jv + mContactPoints[contactPointIndex].restitutionBias) *
                                      mContactPoints[contactPointIndex].inversePenetrationMass;
                    else
                        deltaLambda = -(Jv + b) * mContactPoints[contactPointIndex].inversePenetrationMass;

                    lambdaTemp = mContactPoints[contactPointIndex].penetrationImpulse;
                    mContactPoints[contactPointIndex].penetrationImpulse = PMath.Max(
                        mContactPoints[contactPointIndex].penetrationImpulse +
                        deltaLambda, 0.0f);

                    deltaLambda = mContactPoints[contactPointIndex].penetrationImpulse - lambdaTemp;

                    linearImpulse = mContactPoints[contactPointIndex].normal * deltaLambda;

                    var mConstrainedLinearVelocity = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                    mConstrainedLinearVelocity -=
                        cmSolver.massInverseBody1 * linearImpulse * cmSolver.linearLockAxisFactorBody1;
                    mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index] = mConstrainedLinearVelocity;

                    var constrainedAngularVelocity =
                        mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                    constrainedAngularVelocity -= mContactPoints[contactPointIndex].i1TimesR1CrossN *
                                                  cmSolver.angularLockAxisFactorBody1 * deltaLambda;
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] = constrainedAngularVelocity;

                    var constrainedLinearVelocity = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                    constrainedLinearVelocity +=
                        cmSolver.massInverseBody2 * linearImpulse * cmSolver.linearLockAxisFactorBody2;
                    mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index] = constrainedLinearVelocity;

                    var angularVelocity = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];
                    angularVelocity += mContactPoints[contactPointIndex].i2TimesR2CrossN *
                                       cmSolver.angularLockAxisFactorBody2 * deltaLambda;
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] = angularVelocity;
                    
                    v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                    w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                    v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                    w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];
                    

                    sumPenetrationImpulse += mContactPoints[contactPointIndex].penetrationImpulse;

                    // 如果分裂冲量位置修正处于活动状态
                    if (mIsSplitImpulseActive)
                    {
                        // 分裂冲量（位置修正）
                        var v1Split = mRigidBodyComponents.mSplitLinearVelocities[rigidBody1Index];
                        var w1Split = mRigidBodyComponents.mSplitAngularVelocities[rigidBody1Index];
                        var v2Split = mRigidBodyComponents.mSplitLinearVelocities[rigidBody2Index];
                        var w2Split = mRigidBodyComponents.mSplitAngularVelocities[rigidBody2Index];

                        var deltaVSplit = v2Split + w2Split.cross(mContactPoints[contactPointIndex].r2) - v1Split -
                                          w1Split.cross(mContactPoints[contactPointIndex].r1);
                        var JvSplit = deltaVSplit.dot(mContactPoints[contactPointIndex].normal);

                        var deltaLambdaSplit = -(JvSplit + biasPenetrationDepth) *
                                               mContactPoints[contactPointIndex].inversePenetrationMass;
                        var lambdaTempSplit = mContactPoints[contactPointIndex].penetrationSplitImpulse;
                        mContactPoints[contactPointIndex].penetrationSplitImpulse = PMath.Max(
                            mContactPoints[contactPointIndex].penetrationSplitImpulse +
                            deltaLambdaSplit, 0.0f);
                        deltaLambdaSplit = mContactPoints[contactPointIndex].penetrationSplitImpulse - lambdaTempSplit;

                        linearImpulse = mContactPoints[contactPointIndex].normal * deltaLambdaSplit;

                        // 更新主体 1 的速度应用冲量 P
                        mRigidBodyComponents.mSplitLinearVelocities[rigidBody1Index] -=
                            cmSolver.massInverseBody1 * linearImpulse * cmSolver.linearLockAxisFactorBody1;
                        mRigidBodyComponents.mSplitAngularVelocities[rigidBody1Index] -=
                            mContactPoints[contactPointIndex].i1TimesR1CrossN * cmSolver.angularLockAxisFactorBody1 *
                            deltaLambdaSplit;

                        // 更新主体 2 的速度应用冲量 P
                        mRigidBodyComponents.mSplitLinearVelocities[rigidBody2Index] +=
                            cmSolver.massInverseBody2 * linearImpulse * cmSolver.linearLockAxisFactorBody2;
                        mRigidBodyComponents.mSplitAngularVelocities[rigidBody2Index] +=
                            mContactPoints[contactPointIndex].i2TimesR2CrossN * cmSolver.angularLockAxisFactorBody2 *
                            deltaLambdaSplit;
                        
                         v1Split = mRigidBodyComponents.mSplitLinearVelocities[rigidBody1Index];
                         w1Split = mRigidBodyComponents.mSplitAngularVelocities[rigidBody1Index];
                         v2Split = mRigidBodyComponents.mSplitLinearVelocities[rigidBody2Index];
                         w2Split = mRigidBodyComponents.mSplitAngularVelocities[rigidBody2Index];
                        
                    }

                    contactPointIndex++;
                }

                v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];
                
                // if (!v1.IsFinite() || !w1.IsFinite() || !v2.IsFinite() || !w2.IsFinite())
                // {
                //     Debug.Log("pTransform:");
                // }

                // ------ 第一个摩擦约束在接触流形中心 ------ //

                // 计算 J*v
                deltaV = v2 + w2.cross(cmSolver.r2Friction) - v1 - w1.cross(cmSolver.r1Friction);
                Jv = deltaV.dot(cmSolver.frictionVector1);

                // 计算拉格朗日乘子 lambda
                deltaLambda = -Jv * cmSolver.inverseFriction1Mass;
                var frictionLimit = cmSolver.frictionCoefficient * sumPenetrationImpulse;
                lambdaTemp = cmSolver.friction1Impulse;
                cmSolver.friction1Impulse = PMath.Max(-frictionLimit,
                    PMath.Min(cmSolver.friction1Impulse + deltaLambda, frictionLimit));
                deltaLambda = cmSolver.friction1Impulse - lambdaTemp;

                if (float.IsNaN(deltaLambda))
                {
                    Debug.Log("pTransform:");
                }
                
                // 计算冲量 P=J^T * lambda
                var angularImpulseBody1 = -cmSolver.r1CrossT1 * deltaLambda;
                var linearImpulseBody2 = cmSolver.frictionVector1 * deltaLambda;
                var angularImpulseBody2 = cmSolver.r2CrossT1 * deltaLambda;

                // if (float.IsFinite(angularImpulseBody1.x) || float.IsInfinity(angularImpulseBody1.x))
                //     angularImpulseBody1.x = 0;
                // if (float.IsFinite(angularImpulseBody1.y)|| float.IsInfinity(angularImpulseBody1.y))
                //     angularImpulseBody1.y = 0;
                //
                // if (float.IsFinite(angularImpulseBody2.x) || float.IsInfinity(angularImpulseBody2.x))
                //     angularImpulseBody2.x = 0;
                // if (float.IsFinite(angularImpulseBody2.y)|| float.IsInfinity(angularImpulseBody2.y))
                //     angularImpulseBody2.y = 0;
                //
                // if (!angularImpulseBody2.IsFinite())
                // {
                //     Debug.Log("pTransform:");
                // }

                
                // 更新主体 1 的速度应用冲量 P
                mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index] -=
                    cmSolver.massInverseBody1 * linearImpulseBody2 * cmSolver.linearLockAxisFactorBody1;
                
                var angularVelocity1 = cmSolver.angularLockAxisFactorBody1 *
                                       (cmSolver.inverseInertiaTensorBody1 * angularImpulseBody1);
                mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] += angularVelocity1;

                // 更新主体 2 的速度应用冲量 P
                mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index] +=
                    cmSolver.massInverseBody2 * linearImpulseBody2 * cmSolver.linearLockAxisFactorBody2;

                var angularVelocity2 = cmSolver.angularLockAxisFactorBody2 *
                                       (cmSolver.inverseInertiaTensorBody2 * angularImpulseBody2);
                mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] += angularVelocity2;

                v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];
                
                // if (!v1.IsFinite() || !w1.IsFinite() || !v2.IsFinite() || !w2.IsFinite())
                // {
                //     Debug.Log("pTransform:");
                // }
                
                // ------ 第二个摩擦约束在接触流形中心 ----- //
                

                // 计算 J*v
                deltaV.x = v2.x + w2.y * mContactConstraints[c].r2Friction.z - w2.z * mContactConstraints[c].r2Friction.y  - v1.x -
                    w1.y * mContactConstraints[c].r1Friction.z + w1.z * mContactConstraints[c].r1Friction.y;
                deltaV.y = v2.y + w2.z * mContactConstraints[c].r2Friction.x - w2.x * mContactConstraints[c].r2Friction.z  - v1.y -
                    w1.z * mContactConstraints[c].r1Friction.x + w1.x * mContactConstraints[c].r1Friction.z;
                deltaV.z = v2.z + w2.x * mContactConstraints[c].r2Friction.y - w2.y * mContactConstraints[c].r2Friction.x  - v1.z -
                    w1.x * mContactConstraints[c].r1Friction.y + w1.y * mContactConstraints[c].r1Friction.x;
                Jv = deltaV.x * mContactConstraints[c].frictionVector2.x + deltaV.y * mContactConstraints[c].frictionVector2.y +
                     deltaV.z * mContactConstraints[c].frictionVector2.z;


                // 计算拉格朗日乘子 lambda
                deltaLambda = -Jv * cmSolver.inverseFriction2Mass;
                frictionLimit = cmSolver.frictionCoefficient * sumPenetrationImpulse;
                lambdaTemp = cmSolver.friction2Impulse;
                cmSolver.friction2Impulse = PMath.Max(-frictionLimit,
                    PMath.Min(cmSolver.friction2Impulse + deltaLambda, frictionLimit));
                deltaLambda = cmSolver.friction2Impulse - lambdaTemp;

                // 计算冲量 P=J^T * lambda
                angularImpulseBody1 = -cmSolver.r1CrossT2 * deltaLambda;

                linearImpulseBody2 = cmSolver.frictionVector2 * deltaLambda;

                angularImpulseBody2 = cmSolver.r2CrossT2 * deltaLambda;

                // 更新主体 1 的速度应用冲量 P
                mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index] -=
                    cmSolver.massInverseBody1 * linearImpulseBody2 * cmSolver.linearLockAxisFactorBody1;
                angularVelocity1 = cmSolver.angularLockAxisFactorBody1 *
                                   (cmSolver.inverseInertiaTensorBody1 * angularImpulseBody1);
                mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] += angularVelocity1;

   

                // 更新主体 2 的速度应用冲量 P
                mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index] +=
                    cmSolver.massInverseBody2 * linearImpulseBody2 * cmSolver.linearLockAxisFactorBody2;
                angularVelocity2 = cmSolver.angularLockAxisFactorBody2 *
                                   (cmSolver.inverseInertiaTensorBody2 * angularImpulseBody2);
                mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] += angularVelocity2;

                // ------ 扭转摩擦约束在接触流形中心 ------ //
                
                v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];

                // 计算 J*v
                deltaV = w2 - w1;
                Jv = deltaV.dot(cmSolver.normal);

                
                deltaLambda = -Jv * cmSolver.inverseTwistFrictionMass;
                frictionLimit = cmSolver.frictionCoefficient * sumPenetrationImpulse;
                lambdaTemp = cmSolver.frictionTwistImpulse;

                

                cmSolver.frictionTwistImpulse = PMath.Max(-frictionLimit,
                    PMath.Min(cmSolver.frictionTwistImpulse + deltaLambda, frictionLimit));
                deltaLambda = cmSolver.frictionTwistImpulse - lambdaTemp;

                // 计算冲量 P=J^T * lambda
                angularImpulseBody2 = cmSolver.normal * deltaLambda;

                // 更新主体 1 的速度应用冲量 P
                angularVelocity1 = cmSolver.angularLockAxisFactorBody1 *
                                   (cmSolver.inverseInertiaTensorBody1 * angularImpulseBody2);
                mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] -= angularVelocity1;

                // 更新主体 2 的速度应用冲量 P
                angularVelocity2 = cmSolver.angularLockAxisFactorBody2 *
                                   (cmSolver.inverseInertiaTensorBody2 * angularImpulseBody2);
                mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] += angularVelocity2;

                v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];





            }
        }
    }
}