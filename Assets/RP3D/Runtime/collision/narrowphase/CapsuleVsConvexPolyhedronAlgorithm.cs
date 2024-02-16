using System.Collections.Generic;

namespace RP3D
{
    public class CapsuleVsConvexPolyhedronAlgorithm
    {
        // 计算胶囊与多面体之间的窄相碰撞检测
        // 这个技术基于Dirk Gregorius的"Robust Contact Creation for Physics Simulations"演示文稿
        public bool testCollision(NarrowPhaseInfoBatch narrowPhaseInfoBatch,
            int batchStartIndex, int batchNbItems,
            bool clipWithPreviousAxisIfStillColliding)
        {
            var isCollisionFound = false;

            // 首先，我们运行GJK算法
            var gjkAlgorithm = new GJKAlgorithm();
            var satAlgorithm = new SATAlgorithm(clipWithPreviousAxisIfStillColliding);

            // 运行GJK算法
            var gjkResults = new List<GJKAlgorithm.GJKResult>();
            gjkAlgorithm.TestCollision(narrowPhaseInfoBatch, batchStartIndex, batchNbItems, ref gjkResults);

            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++)
            {
                NarrowPhaseInfo narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];
                // 获取上一帧的碰撞信息
                var lastFrameCollisionInfo = narrowPhaseInfo.lastFrameCollisionInfo;

                lastFrameCollisionInfo.wasUsingGJK = true;
                lastFrameCollisionInfo.wasUsingSAT = false;

                // 如果我们在间隙内找到了接触点（浅层穿透）
                if (gjkResults[batchIndex] == GJKAlgorithm.GJKResult.COLLIDE_IN_MARGIN)
                {
                    // 如果需要报告接触点
                    if (narrowPhaseInfo.reportContacts)
                    {
                        var noContact = false;

                        // GJK发现了一个浅层接触。如果多面体网格的面是与胶囊内部段正交并且与接触点法线平行，
                        // 我们希望创建两个接触点而不是单个接触点（与使用SAT算法的深层接触情况相同）

                        // 获取GJK创建的接触点
                        var contactPoint = narrowPhaseInfo.contactPoints[0];

                        var isCapsuleShape1 =
                            narrowPhaseInfo.collisionShape1.GetType() ==
                            CollisionShapeType.CAPSULE;

                        // 获取碰撞形状
                        var capsuleShape = (CapsuleShape)(isCapsuleShape1
                            ? narrowPhaseInfo.collisionShape1
                            : narrowPhaseInfo.collisionShape2);
                        var polyhedron = (ConvexPolyhedronShape)(isCapsuleShape1
                            ? narrowPhaseInfo.collisionShape2
                            : narrowPhaseInfo.collisionShape1);

                        // 遍历多面体的每个面
                        for (var f = 0; f < polyhedron.GetNbFaces(); f++)
                        {
                            var polyhedronToWorld = isCapsuleShape1
                                ? narrowPhaseInfo.shape2ToWorldTransform
                                : narrowPhaseInfo.shape1ToWorldTransform;
                            var capsuleToWorld = isCapsuleShape1
                                ? narrowPhaseInfo.shape1ToWorldTransform
                                : narrowPhaseInfo.shape2ToWorldTransform;

                            // 获取面法线
                            var faceNormal = polyhedron.GetFaceNormal(f);
                            var faceNormalWorld = polyhedronToWorld.GetOrientation() * faceNormal;

                            var capsuleSegA = new PVector3(0, -capsuleShape.getHeight() * 0.5f, 0);
                            var capsuleSegB = new PVector3(0, capsuleShape.getHeight() * 0.5f, 0);
                            var capsuleInnerSegmentDirection =
                                capsuleToWorld.GetOrientation() * (capsuleSegB - capsuleSegA);
                            capsuleInnerSegmentDirection.Normalize();

                            var isFaceNormalInDirectionOfContactNormal =
                                faceNormalWorld.dot(contactPoint.normal) > 0.0f;
                            var isFaceNormalInContactDirection =
                                (isCapsuleShape1 && !isFaceNormalInDirectionOfContactNormal) ||
                                (!isCapsuleShape1 && isFaceNormalInDirectionOfContactNormal);

                            // 如果多面体面法线与胶囊内部段正交且与接触点法线平行，
                            // 并且面法线在接触法线方向上（从多面体的角度）。
                            if (isFaceNormalInContactDirection && PMath.areOrthogonalVectors(faceNormalWorld,
                                                                   capsuleInnerSegmentDirection)
                                                               && PMath.areParallelVectors(faceNormalWorld,
                                                                   contactPoint.normal))
                            {
                                // 移除先前由GJK计算的接触点
                                narrowPhaseInfoBatch.ResetContactPoints(batchIndex);

                                var capsuleToWorld1 = isCapsuleShape1
                                    ? narrowPhaseInfo.shape1ToWorldTransform
                                    : narrowPhaseInfo.shape2ToWorldTransform;
                                var polyhedronToCapsuleTransform = capsuleToWorld1.GetInverse() * polyhedronToWorld;

                                // 计算胶囊内部段的端点
                                var capsuleSegA1 = new PVector3(0, -capsuleShape.getHeight() * 0.5f, 0);
                                var capsuleSegB1 = new PVector3(0, capsuleShape.getHeight() * 0.5f, 0);

                                // 将内部胶囊段点转换为多面体局部空间
                                var capsuleToPolyhedronTransform = polyhedronToCapsuleTransform.GetInverse();
                                var capsuleSegAPolyhedronSpace = capsuleToPolyhedronTransform * capsuleSegA1;
                                var capsuleSegBPolyhedronSpace = capsuleToPolyhedronTransform * capsuleSegB1;

                                var separatingAxisCapsuleSpace =
                                    polyhedronToCapsuleTransform.GetOrientation() * faceNormal;

                                if (isCapsuleShape1) faceNormalWorld = -faceNormalWorld;

                                // 计算并创建两个接触点
                                var contactsFound = satAlgorithm.computeCapsulePolyhedronFaceContactPoints(f,
                                    capsuleShape.getRadius(), polyhedron, contactPoint.penetrationDepth,
                                    polyhedronToCapsuleTransform, faceNormalWorld, separatingAxisCapsuleSpace,
                                    capsuleSegAPolyhedronSpace, capsuleSegBPolyhedronSpace,
                                    narrowPhaseInfoBatch, batchIndex, isCapsuleShape1);
                                if (!contactsFound)
                                {
                                    noContact = true;
                                    narrowPhaseInfo.isColliding = false;
                                }

                                break;
                            }
                        }

                        if (noContact) continue;
                    }

                    lastFrameCollisionInfo.wasUsingSAT = false;
                    lastFrameCollisionInfo.wasUsingGJK = false;

                    // 发现碰撞
                    narrowPhaseInfo.isColliding = true;
                    isCollisionFound = true;
                    continue;
                }

                // 如果我们有重叠而没有间隙（深层穿透）
                if (gjkResults[batchIndex] == GJKAlgorithm.GJKResult.INTERPENETRATE)
                {
                    // 运行SAT算法以查找分离轴并计算接触点
                    narrowPhaseInfo.isColliding =
                        satAlgorithm.testCollisionCapsuleVsConvexPolyhedron(narrowPhaseInfoBatch, batchIndex);

                    lastFrameCollisionInfo.wasUsingGJK = false;
                    lastFrameCollisionInfo.wasUsingSAT = true;

                    if (narrowPhaseInfo.isColliding) isCollisionFound = true;
                }
            }

            return isCollisionFound;
        }
    }
}