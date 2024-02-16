//ContactSolverSystem 是负责解决碰撞约束的系统之一。
//碰撞解决是物理引擎中的重要步骤，用于确定物体之间的接触点、碰撞力以及碰撞后的运动响应。

//ContactSolverSystem 主要负责以下任务：

//接触点的计算：根据碰撞检测阶段找到的碰撞信息，确定物体之间的接触点以及接触法线。

//碰撞力的计算：根据接触点的信息，计算碰撞力的大小和方向，以模拟碰撞时物体之间的相互作用。

//应用碰撞力：将计算得到的碰撞力应用于物体，更新它们的线性和角动量，以实现碰撞后的运动响应。

//解决碰撞约束：使用适当的算法解决碰撞约束，确保物体在碰撞后不会穿透彼此，同时尽可能地保持动能守恒和动量守恒。

//ContactSolverSystem 通常作为物理引擎中的一个系统组件存在，
//并与其他系统（如碰撞检测系统、约束求解系统等）协同工作，以实现整个物理仿真过程。
//通过正确实现和调整 ContactSolverSystem，可以确保物体之间的碰撞行为在仿真中得到准确和合理的模拟。




using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RP3D
{
    public partial class ContactSolverSystem
    {
        private const float BETA = 0.2f;
        private const float BETA_SPLIT_IMPULSE = 0.2f;
        private const float SLOP = 0.01f;

        private float mTimeStep;
        
        /// mRestitutionVelocityThreshold 是一个阈值，用于确定在碰撞中考虑恢复（restitution）的条件。
        /// 如果碰撞中涉及的速度超过这个阈值，系统将考虑恢复，并根据碰撞的材质和对象属性应用恢复效果。
        /// 恢复是指碰撞后物体之间相互分离的速度，通常在碰撞后会产生弹性反弹的效果。
        /// mRestitutionVelocityThreshold 指定了当速度超过这个阈值时，碰撞中的恢复效果才会被考虑。
        /// 如果速度低于此阈值，系统将认为碰撞是非弹性的，不会产生反弹效果。
        /// 这个阈值的选择可能取决于具体的物理场景和模拟需求。
        /// 通常，较高的阈值将导致更多的碰撞被视为弹性碰撞，而较低的阈值则可能导致更多的碰撞被视为非弹性碰撞。
        /// 根据模拟的要求和期望的物理行为，可以调整这个阈值来达到所需的效果。
        /// 在 ContactSolverSystem 中，mRestitutionVelocityThreshold 可能用于在碰撞解算过程中确定碰撞是否具有足够的速度以产生恢复效果。
        private float mRestitutionVelocityThreshold;
        
        
        /// 接触约束
        private List<ContactManifoldSolver> mContactConstraints;
        /// 接触点
        private List<ContactPointSolver> mContactPoints;
        
        /// 接触点约束的数量
        private int mNbContactPoints;
        /// 接触约束的数量
        private int mNbContactManifolds;
        
        /// 岛的引用
        private readonly Islands mIslands;
        
        /// 从窄相阶段获取的接触流形数组的指针
        private List<ContactManifold> mAllContactManifolds;
        
        /// 从窄相阶段获取的接触点数组的指针
        private List<ContactPoint> mAllContactPoints;
        
        /// 动力学组件的引用
        private readonly RigidBodyComponents mRigidBodyComponents;
        /// 碰撞器组件的引用
        private readonly ColliderComponents mColliderComponents;


        public ContactSolverSystem(Islands islands,
            RigidBodyComponents rigidBodyComponents,
            ColliderComponents colliderComponents, float restitutionVelocityThreshold)
        {
            mRestitutionVelocityThreshold = restitutionVelocityThreshold;
            mIslands = islands;
            mRigidBodyComponents = rigidBodyComponents;
            mColliderComponents = colliderComponents;
            mIsSplitImpulseActive = true;
        }

        public void init(List<ContactManifold> contactManifolds, List<ContactPoint> contactPoints, float timeStep)
        {
            mTimeStep = timeStep;
            mAllContactManifolds = contactManifolds.ToList();
            mAllContactPoints = contactPoints.ToList();

            var nbContactManifolds = mAllContactManifolds.Count;
            var nbContactPoints = mAllContactPoints.Count;

            mNbContactManifolds = 0;
            mNbContactPoints = 0;

            if (nbContactManifolds == 0 || nbContactPoints == 0) return;

            mContactPoints = new List<ContactPointSolver>();
            mContactConstraints = new List<ContactManifoldSolver>();

            var nbIslands = mIslands.getNbIslands();
            for (var i = 0; i < nbIslands; i++)
                if (mIslands.nbContactManifolds[i] > 0)
                    initializeForIsland(i);

            warmStart();
        }

        
        public void reset()
        {
            if (mAllContactPoints.Count > 0)
                mAllContactPoints.Clear();
            if (mAllContactManifolds.Count > 0)
                mAllContactManifolds.Clear();
        }

        /// <summary>
        /// 用于预热求解器的方法。预热求解器在每个时间步开始时应用上一步的冲量，以便更快地收敛到线性系统的解决方案。它的主要步骤如下：
        /// 首先，对每个约束进行迭代。
        /// 对于每个约束，检查是否至少有一个接触点是静止的，如果是，则进行下一步；否则，跳过这个约束。
        /// 对于静止的接触点，根据存储的冲量和约束条件，更新相关的刚体速度。这包括：
        /// 更新物体1和物体2的线性速度，以及角速度，以解决碰撞造成的速度变化。
        /// 通过将旧的摩擦冲量投影到新的摩擦向量来更新摩擦冲量。然后，根据新的摩擦冲量，更新摩擦约束的速度变化。
        /// 如果至少有一个接触点是静止的，并且在接触曲面的中心存在摩擦约束，则进一步更新摩擦约束的速度变化。这包括：
        /// 计算冲量并使用它来更新物体的线性和角速度，以解决摩擦造成的速度变化。
        /// 如果没有静止的接触点，则将摩擦冲量初始化为零。
        /// 这个方法的目的是在每个时间步开始时，通过应用上一步的冲量，预热求解器以更快地收敛到线性系统的解决方案。
        /// </summary>
        private void warmStart()
        {
            var contactPointIndex = 0;

            // 对于每个约束
            for (var c = 0; c < mNbContactManifolds; c++)
            {
                var atLeastOneRestingContactPoint = false;

                var cms = mContactConstraints[c];

                for (var i = 0; i < cms.nbContacts; i++)
                {
                    // 如果不是新的接触点（这个接触点在上一个时间步骤已经存在）
                    ContactPointSolver cPointsSolver = mContactPoints[contactPointIndex];
                    
                    if (cPointsSolver.isRestingContact)
                    {
                        var rigidBody1Index = cms.rigidBodyComponentIndexBody1;
                        var rigidBody2Index = cms.rigidBodyComponentIndexBody2;

                        atLeastOneRestingContactPoint = true;

                        // --------- Penetration --------- //

                        // 使用冲量P更新物体1的速度
                        var impulsePenetration = cPointsSolver.normal *
                                                 cPointsSolver.penetrationImpulse;

                        mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index] -= cms.massInverseBody1 *
                            impulsePenetration * cms.linearLockAxisFactorBody1;
                        mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] -=
                            cPointsSolver.i1TimesR1CrossN * cms.angularLockAxisFactorBody1 *
                            cPointsSolver.penetrationImpulse;
                        // 使用冲量P更新物体2的速度
                        mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index] += cms.massInverseBody2 *
                            impulsePenetration * cms.linearLockAxisFactorBody2;
                        mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] +=
                            cPointsSolver.i2TimesR2CrossN * cms.angularLockAxisFactorBody2 *
                            cPointsSolver.penetrationImpulse;
                        
                        
                        var v1 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index];
                        var w1 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index];
                        var v2 = mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index];
                        var w2 = mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index];
                        // if (!v1.IsFinite() || !w1.IsFinite() || !v2.IsFinite() || !w2.IsFinite())
                        // {
                        //     Debug.Log("pTransform:");
                        // }

                    }
                    else
                    {
                        // 如果是新的接触点

                        // 初始化积累的冲量为零
                        cPointsSolver.penetrationImpulse = 0.0f;
                    }

                    contactPointIndex++;
                }

                // 如果我们在接触曲面的中心解决摩擦约束，并且在接触曲面中至少有一个静止接触点
                if (atLeastOneRestingContactPoint)
                {
                    // 投影旧的摩擦冲量（使用旧的摩擦向量）到新的摩擦向量以得到新的摩擦冲量
                    var oldFrictionImpulse = cms.friction1Impulse * cms.oldFrictionVector1 +
                                             cms.friction2Impulse * cms.oldFrictionVector2;
                    cms.friction1Impulse = oldFrictionImpulse.dot(cms.frictionVector1);
                    cms.friction2Impulse = oldFrictionImpulse.dot(cms.frictionVector2);


                    // ------ 第一个摩擦约束在接触曲面的中心 ------ //

                    // 计算冲量P = J^T * lambda
                    var angularImpulseBody1 = -cms.r1CrossT1 * cms.friction1Impulse;
                    var linearImpulseBody2 = cms.frictionVector1 * cms.friction1Impulse;
                    var angularImpulseBody2 = cms.r2CrossT1 * cms.friction1Impulse;

                    var rigidBody1Index = cms.rigidBodyComponentIndexBody1;
                    var rigidBody2Index = cms.rigidBodyComponentIndexBody2;

                    // 使用冲量P更新物体1的速度
                    mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index] -=
                        cms.massInverseBody1 * linearImpulseBody2 * cms.linearLockAxisFactorBody1;
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] +=
                        cms.angularLockAxisFactorBody1 * (cms.inverseInertiaTensorBody1 * angularImpulseBody1);

                    // 使用冲量P更新物体2的速度
                    mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index] +=
                        cms.massInverseBody2 * linearImpulseBody2 * cms.linearLockAxisFactorBody2;
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] +=
                        cms.angularLockAxisFactorBody2 * (cms.inverseInertiaTensorBody2 * angularImpulseBody2);

                    // ------ 第二个摩擦约束在接触曲面的中心 ----- //

                    // 计算冲量P = J^T * lambda
                    angularImpulseBody1 = -cms.r1CrossT2 * cms.friction2Impulse;
                    linearImpulseBody2 = cms.frictionVector2 * cms.friction2Impulse;
                    angularImpulseBody2 = cms.r2CrossT2 * cms.friction2Impulse;


                    // 使用冲量P更新物体1的速度
                    mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody1Index] -=
                        cms.massInverseBody1 * linearImpulseBody2 * cms.linearLockAxisFactorBody1;
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] +=
                        cms.angularLockAxisFactorBody1 * (cms.inverseInertiaTensorBody1 * angularImpulseBody1);


                    // 使用冲量P更新物体2的速度
                    mRigidBodyComponents.mConstrainedLinearVelocities[rigidBody2Index] += cms.massInverseBody2 *
                        linearImpulseBody2 * cms.linearLockAxisFactorBody2.x;
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] +=
                        cms.angularLockAxisFactorBody2 * (cms.inverseInertiaTensorBody2 * angularImpulseBody2);

                    // ------ 接触曲面中心的扭摩擦约束 ------ //

                    // 计算冲量P = J^T * lambda
                    angularImpulseBody1 = -cms.normal * cms.frictionTwistImpulse;
                    angularImpulseBody2 = cms.normal * cms.frictionTwistImpulse;

                    // 使用冲量P更新物体1的速度
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] +=
                        cms.angularLockAxisFactorBody1 * (cms.inverseInertiaTensorBody1 * angularImpulseBody1);

                    // 使用冲量P更新物体2的速度
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] +=
                        cms.angularLockAxisFactorBody2 * (cms.inverseInertiaTensorBody2 * angularImpulseBody2);

                    // 使用冲量P更新物体1的速度
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody1Index] -=
                        cms.angularLockAxisFactorBody1 * (cms.inverseInertiaTensorBody1 * angularImpulseBody2);

                    // 使用冲量P更新物体1的速度
                    mRigidBodyComponents.mConstrainedAngularVelocities[rigidBody2Index] +=
                        cms.angularLockAxisFactorBody2 * (cms.inverseInertiaTensorBody2 * angularImpulseBody2);
                }
                else
                {
                    // 如果是一个新的接触曲面

                    // 初始化积累的冲量为零
                    cms.friction1Impulse = 0.0f;
                    cms.friction2Impulse = 0.0f;
                    cms.frictionTwistImpulse = 0.0f;
                }
            }
        }

        /// <summary>
        /// 这段代码是一个方法，用于将碰撞约束中的冲量信息存储到外部的联系点数据结构中。这个方法的实现如下：
        /// 方法首先定义了一个contactPointIndex变量，用于跟踪遍历过的接触点数量，并初始化为0。
        /// 然后，通过迭代所有的接触约束（碰撞 manifold），并在每个碰撞 manifold 上迭代每个接触点，来处理每个接触点的冲量信息。
        /// 在内部循环中，方法首先将当前接触点的穿透冲量（penetration impulse）存储到外部的联系点数据结构中。
        /// 然后，将当前碰撞 manifold 的摩擦冲量信息（friction impulses）和摩擦向量（friction vectors）存储到外部的联系点数据结构中。
        /// 遍历完成后，所有接触点的冲量信息都已经存储到外部的联系点数据结构中。
        /// 这个方法的目的是将内部使用的碰撞约束中的冲量信息传递给外部数据结构，以便在其他地方使用或者进行进一步处理。
        /// </summary>
        public void storeImpulses()
        {
            var contactPointIndex = 0;
            
            for (var c = 0; c < mNbContactManifolds; c++)
            {
                var cmSolver = mContactConstraints[c];
                for (var i = 0; i < cmSolver.nbContacts; i++)
                {
                    mContactPoints[contactPointIndex].externalContact.SetPenetrationImpulse(
                        mContactPoints[contactPointIndex].penetrationImpulse);
                    contactPointIndex++;
                }
                cmSolver.externalContactManifold.frictionImpulse1 = cmSolver.friction1Impulse;
                cmSolver.externalContactManifold.frictionImpulse2 = cmSolver.friction2Impulse;
                cmSolver.externalContactManifold.frictionTwistImpulse = cmSolver.frictionTwistImpulse;
                cmSolver.externalContactManifold.frictionVector1 = cmSolver.frictionVector1;
                cmSolver.externalContactManifold.frictionVector2 = cmSolver.frictionVector2;
            }
        }
        
        /// <summary>
        /// 计算两个碰撞器之间的碰撞恢复因子。碰撞恢复因子是指碰撞发生后两个物体分开时的速度恢复程度，通常取值范围在0到1之间，表示碰撞后物体的弹性程度。具体解释如下：
        /// 方法接受两个材质（Material）作为参数，每个材质都包含了有关碰撞的物理属性，比如弹性系数（即恢复系数）。
        /// 通过调用getBounciness()方法获取每个材质的弹性系数（或恢复系数），存储在变量restitution1和restitution2中。
        /// 方法返回较大的恢复系数，以确保碰撞后的速度恢复程度更大，从而使碰撞更有弹性。
        /// 这是通过比较restitution1和restitution2来实现的，如果restitution1大于restitution2，则返回restitution1，否则返回restitution2。
        /// 综上所述，这段代码用于确定两个碰撞器之间碰撞的弹性程度，并返回一个合适的碰撞恢复因子，以影响碰撞后物体的速度变化。
        /// </summary>
        public float computeMixedRestitutionFactor(Material material1, Material material2)
        {
            var restitution1 = material1.getBounciness();
            var restitution2 = material2.getBounciness();
            // 返回较大的恢复因子
            return restitution1 > restitution2 ? restitution1 : restitution2;
        }

        /// <summary>
        /// 计算两个材质之间的摩擦系数的几何平均值。摩擦系数是一个无量纲的数值，用于描述两个表面之间的摩擦阻力大小。
        /// 具体解释如下：
        /// 方法接收两个材质（Material）作为参数，每个材质都包含了有关碰撞的物理属性，其中包括摩擦系数。
        /// 通过调用getFrictionCoefficientSqrt()方法获取每个材质的摩擦系数的平方根，并将其相乘。
        /// 返回两个摩擦系数平方根的乘积。这里使用平方根是因为直接乘积会使得摩擦系数的值增长过快，
        /// 而使用平方根能够更好地控制摩擦系数的变化，以更好地模拟物体之间的摩擦关系。
        /// 综上所述，这段代码计算了两个材质之间摩擦系数的几何平均值，并返回结果。
        /// </summary>
        public float computeMixedFrictionCoefficient(Material material1, Material material2)
        {
            // 使用几何平均数来计算混合摩擦系数
            return material1.getFrictionCoefficientSqrt() * material2.getFrictionCoefficientSqrt();
        }
        


        
        
        
        
        /// <summary>
        /// mIsSplitImpulseActive 是一个布尔变量，用于表示是否启用了分裂冲量（split impulse）。
        /// 分裂冲量是一种解决碰撞约束的方法，它允许在每个时间步中多次应用冲量来解决约束，以减少由于单次大冲量引起的震荡和不稳定性。
        /// 通过在多个子步骤中应用小冲量来模拟单个大冲量，可以更好地控制碰撞响应的行为。
        /// 如果 mIsSplitImpulseActive 为 true，则表示分裂冲量是激活状态，系统将使用分裂冲量方法来解决碰撞约束。
        /// 否则，如果 mIsSplitImpulseActive 为 false，则表示分裂冲量未激活，系统将使用单次大冲量的方法来解决碰撞约束。
        /// 在实现中，可以根据需要在启用或禁用分裂冲量时设置和检查 mIsSplitImpulseActive 变量的状态。
        /// </summary>
        private bool mIsSplitImpulseActive;
        
        // 如果用于接触的分裂冲量位置修正技术正在使用，则返回true
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool isSplitImpulseActive()
        {
            return mIsSplitImpulseActive;
        }

        // 激活或停用接触点的分裂脉冲
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setIsSplitImpulseActive(bool isActive)
        {
            mIsSplitImpulseActive = isActive;
        }
        
    }
}