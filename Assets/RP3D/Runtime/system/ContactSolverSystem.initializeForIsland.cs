using UnityEngine;

namespace RP3D
{
    public partial class ContactSolverSystem
    {
        /// <summary>
        ///     用于初始化一个物理岛（Physics Island）中的接触约束（Contact Constraints）。
        ///     在物理引擎中，将物理对象分组为物理岛可以提高性能，因为在每个物理岛内部的物体可以同时进行约束求解，从而减少了求解的规模。
        ///     以下是这段代码的主要步骤：
        ///     对于物理岛中的每个接触曲面（Contact Manifold），获取其索引和接触点的数量。
        ///     对于每个接触曲面，获取其关联的刚体索引和碰撞体索引，并获取对应的物体位置、速度等信息。
        ///     对于每个接触曲面中的每个接触点，获取接触点在世界空间的位置，以及其他相关属性。
        ///     计算接触点的相对速度（deltaV）以及用于约束求解的相关矩阵和质量信息。
        ///     根据相对速度和材质属性计算恢复偏置（restitution bias）。
        ///     计算接触曲面的摩擦向量，并计算用于摩擦约束求解的相关质量信息。
        ///     将每个接触曲面的约束信息存储到 ContactManifoldSolver 对象中，并添加到 mContactConstraints 列表中。
        ///     这段代码的目的是在物理模拟开始之前准备好接触约束，以便在求解过程中使用。
        /// </summary>
        /// <param name="islandIndex"></param>
        private void initializeForIsland(int islandIndex)
        {
            // 对于岛中的每个接触曲面
            var contactManifoldsIndex = mIslands.contactManifoldsIndices[islandIndex];
            var nbContactManifolds = mIslands.nbContactManifolds[islandIndex];
            for (var m = contactManifoldsIndex; m < contactManifoldsIndex + nbContactManifolds; m++)
            {
                if (m == 34)
                {
                    Debug.Log("test");
                }
                
                var externalManifold = mAllContactManifolds[m];

                var rigidBodyIndex1 = mRigidBodyComponents.getEntityIndex(externalManifold.bodyEntity1);
                var rigidBodyIndex2 = mRigidBodyComponents.getEntityIndex(externalManifold.bodyEntity2);

                var collider1Index = mColliderComponents.getEntityIndex(externalManifold.colliderEntity1);
                var collider2Index = mColliderComponents.getEntityIndex(externalManifold.colliderEntity2);

                // 获取两个物体的位置
                var x1 = mRigidBodyComponents.mCentersOfMassWorld[rigidBodyIndex1];
                var x2 = mRigidBodyComponents.mCentersOfMassWorld[rigidBodyIndex2];

                var cManifoldSolver = new ContactManifoldSolver
                {
                    rigidBodyComponentIndexBody1 = rigidBodyIndex1,
                    rigidBodyComponentIndexBody2 = rigidBodyIndex2,
                    inverseInertiaTensorBody1 = mRigidBodyComponents.mInverseInertiaTensorsWorld[rigidBodyIndex1],
                    inverseInertiaTensorBody2 = mRigidBodyComponents.mInverseInertiaTensorsWorld[rigidBodyIndex2],
                    massInverseBody1 = mRigidBodyComponents.mInverseMasses[rigidBodyIndex1],
                    massInverseBody2 = mRigidBodyComponents.mInverseMasses[rigidBodyIndex2],
                    linearLockAxisFactorBody1 = mRigidBodyComponents.mLinearLockAxisFactors[rigidBodyIndex1],
                    linearLockAxisFactorBody2 = mRigidBodyComponents.mLinearLockAxisFactors[rigidBodyIndex2],
                    angularLockAxisFactorBody1 = mRigidBodyComponents.mAngularLockAxisFactors[rigidBodyIndex1],
                    angularLockAxisFactorBody2 = mRigidBodyComponents.mAngularLockAxisFactors[rigidBodyIndex2],
                    nbContacts = externalManifold.nbContactPoints,
                    frictionCoefficient = computeMixedFrictionCoefficient(
                        mColliderComponents.mMaterials[collider1Index], mColliderComponents.mMaterials[collider2Index]),
                    externalContactManifold = externalManifold,
                    normal = PVector3.Zero(),
                    frictionPointBody1 = PVector3.Zero(),
                    frictionPointBody2 = PVector3.Zero()
                };

                // 获取物体的速度
                var v1 = mRigidBodyComponents.mLinearVelocities[rigidBodyIndex1];
                var w1 = mRigidBodyComponents.mAngularVelocities[rigidBodyIndex1];
                var v2 = mRigidBodyComponents.mLinearVelocities[rigidBodyIndex2];
                var w2 = mRigidBodyComponents.mAngularVelocities[rigidBodyIndex2];

                var collider1LocalToWorldTransform = mColliderComponents.mLocalToWorldTransforms[collider1Index];
                var collider2LocalToWorldTransform = mColliderComponents.mLocalToWorldTransforms[collider2Index];

                // 对于接触曲面的每个接触点
                var contactPointsStartIndex = externalManifold.contactPointsIndex;
                var nbContactPoints = externalManifold.nbContactPoints;

                for (var c = contactPointsStartIndex; c < contactPointsStartIndex + nbContactPoints; c++)
                {
                    var externalContact = mAllContactPoints[c];
                    externalContact.SetIsRestingContact(true);

                    var p1 = collider1LocalToWorldTransform * externalContact.GetLocalPointOnShape1();
                    var p2 = collider2LocalToWorldTransform * externalContact.GetLocalPointOnShape2();

                    var contactPointSolver = new ContactPointSolver
                    {
                        externalContact = externalContact,
                        normal = externalContact.GetNormal(),
                        r1 = new PVector3(p1.x - x1.x, p1.y - x1.y, p1.z - x1.z),
                        r2 = new PVector3(p2.x - x2.x, p2.y - x2.y, p2.z - x2.z),
                        penetrationDepth = externalContact.GetPenetrationDepth(),
                        isRestingContact = externalContact.GetIsRestingContact(),
                        penetrationSplitImpulse = 0.0f,
                        penetrationImpulse = externalContact.GetPenetrationImpulse()
                    };

                    // 将接触点信息添加到列表中
                    mContactPoints.Add(contactPointSolver);

                    cManifoldSolver.frictionPointBody1 += p1;
                    cManifoldSolver.frictionPointBody2 += p2;

                    // 计算速度差异
                    var cpSolver = mContactPoints[mNbContactPoints];


                    var r1CrossW1 = w1.cross(cpSolver.r1);
                    var r2CrossW2 =  w2.cross(cpSolver.r2);

                    var v1MinusV2 =  v2 - v1;

                    var deltaV = v1MinusV2 - r1CrossW1 + r2CrossW2;

                    var r1CrossN = cpSolver.r1.cross(cpSolver.normal);
                    var r2CrossN = cpSolver.r2.cross(cpSolver.normal);

                    cpSolver.i1TimesR1CrossN = cManifoldSolver.inverseInertiaTensorBody1 * r1CrossN;
                    cpSolver.i2TimesR2CrossN = cManifoldSolver.inverseInertiaTensorBody2 * r2CrossN;

                    // 计算穿透约束的逆质量矩阵 K
                    var a1 = cManifoldSolver.massInverseBody1 + cManifoldSolver.massInverseBody2;
                    var a2 = cpSolver.i1TimesR1CrossN.cross(cpSolver.r1).dot(cpSolver.normal);
                    var a3 = cpSolver.i2TimesR2CrossN.cross(cpSolver.r2).dot(cpSolver.normal);
                    var massPenetration = a1 + a2 + a3;
                    cpSolver.inversePenetrationMass = massPenetration > 0.0f ? 1.0f / massPenetration : 0.0f;

                    // 计算恢复偏置 "b"
                    cpSolver.restitutionBias = 0.0f;
                    var deltaVDotN = deltaV.dot(cpSolver.normal);
                    var restitutionFactor = computeMixedRestitutionFactor(
                        mColliderComponents.mMaterials[collider1Index], mColliderComponents.mMaterials[collider2Index]);
                    
                    if (deltaVDotN < -mRestitutionVelocityThreshold)
                        cpSolver.restitutionBias = restitutionFactor * deltaVDotN;

                    cManifoldSolver.normal += cpSolver.normal;

                    mNbContactPoints++;
                }

                cManifoldSolver.frictionPointBody1 /= cManifoldSolver.nbContacts;
                cManifoldSolver.frictionPointBody2 /= cManifoldSolver.nbContacts;
                cManifoldSolver.r1Friction = cManifoldSolver.frictionPointBody1 - x1;
                cManifoldSolver.r2Friction = cManifoldSolver.frictionPointBody2 - x2;

                cManifoldSolver.oldFrictionVector1 = externalManifold.frictionVector1;
                cManifoldSolver.oldFrictionVector2 = externalManifold.frictionVector2;
                

                // 使用上一步的累积冲量初始化累积冲量
                cManifoldSolver.friction1Impulse = externalManifold.frictionImpulse1;
                cManifoldSolver.friction2Impulse = externalManifold.frictionImpulse2;
                cManifoldSolver.frictionTwistImpulse = externalManifold.frictionTwistImpulse;

                cManifoldSolver.normal.Normalize();

                var deltaVFrictionPoint = v2 + w2.cross(cManifoldSolver.r2Friction) -
                                          v1 - w1.cross(cManifoldSolver.r1Friction);
                // 计算摩擦向量
                computeFrictionVectors(deltaVFrictionPoint,ref cManifoldSolver);
                
                // Debug.Log(
                //     cManifoldSolver.oldFrictionVector1 + "  " + 
                //           cManifoldSolver.oldFrictionVector2 + " now: " + 
                //           cManifoldSolver.frictionVector1+ "  " + 
                //           cManifoldSolver.frictionVector2);


                // 计算中心接触曲面处摩擦约束的逆质量矩阵 K
                cManifoldSolver.r1CrossT1 = cManifoldSolver.r1Friction.cross(cManifoldSolver.frictionVector1);
                cManifoldSolver.r1CrossT2 = cManifoldSolver.r1Friction.cross(cManifoldSolver.frictionVector2);
                cManifoldSolver.r2CrossT1 = cManifoldSolver.r2Friction.cross(cManifoldSolver.frictionVector1);
                cManifoldSolver.r2CrossT2 = cManifoldSolver.r2Friction.cross(cManifoldSolver.frictionVector2);

                float friction1Mass = cManifoldSolver.massInverseBody1 + cManifoldSolver.massInverseBody2 +
                                    (cManifoldSolver.inverseInertiaTensorBody1 * cManifoldSolver.r1CrossT1)
                                    .cross(cManifoldSolver.r1Friction).dot(cManifoldSolver.frictionVector1) +
                                    (cManifoldSolver.inverseInertiaTensorBody2 * cManifoldSolver.r2CrossT1)
                                    .cross(cManifoldSolver.r2Friction).dot(cManifoldSolver.frictionVector1);

                float friction2Mass = cManifoldSolver.massInverseBody1 + cManifoldSolver.massInverseBody2 +
                                      (cManifoldSolver.inverseInertiaTensorBody1 * cManifoldSolver.r1CrossT2)
                                      .cross(cManifoldSolver.r1Friction).dot(cManifoldSolver.frictionVector2) +
                                      (cManifoldSolver.inverseInertiaTensorBody2 * cManifoldSolver.r2CrossT2)
                                      .cross(cManifoldSolver.r2Friction).dot(cManifoldSolver.frictionVector2);

                float frictionTwistMass =
                    cManifoldSolver.normal.dot(cManifoldSolver.inverseInertiaTensorBody1 * cManifoldSolver.normal) +
                    cManifoldSolver.normal.dot(
                        cManifoldSolver.inverseInertiaTensorBody2 * cManifoldSolver.normal);
                
                cManifoldSolver.inverseFriction1Mass = friction1Mass > 0.0f ? 1.0f / friction1Mass : 0.0f;
                cManifoldSolver.inverseFriction2Mass = friction2Mass > 0.0f ? 1.0f / friction2Mass : 0.0f;
                cManifoldSolver.inverseTwistFrictionMass = frictionTwistMass > 0.0f ? 1.0f / frictionTwistMass : 0.0f;

                mContactConstraints.Add(cManifoldSolver);
                mNbContactManifolds++;
            }
        }
        
        
        /// <summary>
        /// 用于计算摩擦力的方向向量的方法。在物理仿真中，摩擦力是由两个物体之间的接触面和两者之间的相对速度所确定的。
        /// 摩擦力的方向与接触面的法向量垂直，并且可以在接触面上定义一个切平面，摩擦力的方向则可以沿着切平面的方向。

        /// 以下是该方法的主要步骤：
        /// 首先，计算相对速度与接触法线的点积，以获取速度在法线方向上的分量。这个分量被称为法向速度（normalVelocity），即速度在接触面的垂直方向上的分量。
        /// 然后，从相对速度中减去法向速度，得到切向速度（tangentVelocity），即速度在接触面的切向方向上的分量。
        /// 如果切向速度的长度不为零，表示有切向运动，那么第一个摩擦向量（frictionVector1）即为切向速度的单位向量。
        /// 如果切向速度的长度为零，表示没有切向运动，那么第一个摩擦向量可以选择接触法线的任意一个正交向量。
        /// 第二个摩擦向量（frictionVector2）通过接触法线与第一个摩擦向量的叉乘得到，确保它与第一个摩擦向量和接触法线都正交。
        /// 这样计算得到的两个摩擦向量可以用来表示摩擦力的方向，它们分别位于接触面上，并且与接触法线和彼此正交。
        /// </summary>
        private void computeFrictionVectors(PVector3 deltaVelocity,ref ContactManifoldSolver contact)
        {
            // 计算切平面中的速度差向量
            var deltaVDotNormal = deltaVelocity.dot(contact.normal);
            var normalVelocity = deltaVDotNormal * contact.normal;
            var tangentVelocity = new PVector3(deltaVelocity.x - normalVelocity.x, deltaVelocity.y - normalVelocity.y,
                deltaVelocity.z - normalVelocity.z);

            // 如果切平面中的速度差不为零
            var lengthTangentVelocity = tangentVelocity.Length();
            if (lengthTangentVelocity > float.Epsilon)
                // 计算第一个摩擦向量在切向上的方向
                // 速度差
                contact.frictionVector1 = tangentVelocity / lengthTangentVelocity;
            else
                // 获取法向量的任意正交向量作为第一个摩擦向量
                contact.frictionVector1 = contact.normal.GetOneUnitOrthogonalVector();

            // 第二个摩擦向量由第一个摩擦向量和接触法线的叉乘得到
            contact.frictionVector2 = contact.normal.cross(contact.frictionVector1);
        }
    }
}