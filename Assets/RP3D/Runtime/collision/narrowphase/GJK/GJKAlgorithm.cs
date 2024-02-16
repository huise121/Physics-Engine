using System.Collections.Generic;

namespace RP3D
{
    public class GJKAlgorithm
    {
        public enum GJKResult
        {
            SEPARATED, // 表示两个形状在边界之外完全分离，没有发生碰撞。
            COLLIDE_IN_MARGIN, // 表示两个形状在边界上有部分重叠，但只在边缘范围内。这种情况通常描述为浅层穿透，即碰撞发生在形状的边缘或边界附近。
            INTERPENETRATE // 表示两个形状在边界内部发生了重叠，而不仅仅是在边缘范围内。这种情况通常描述为深层穿透，即碰撞发生在形状的内部，而不仅仅是边缘。
        }

        public void TestCollision(NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchStartIndex,
            int batchNbItems,ref List<GJKResult> gjkResults)
        {
            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++)
            {
                PVector3 suppA;
                PVector3 suppB;
                PVector3 w;
                PVector3 pA;
                PVector3 pB;
                float vDotw;
                float prevDistSquare;
                var contactFound = false;

                var shape1 = (ConvexShape)narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1;
                var shape2 = (ConvexShape)narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2;

                var transform1 = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform;
                var transform2 = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform;

                var transform1Inverse = transform1.GetInverse();
                var body2Tobody1 = transform1Inverse * transform2;

                var rotateToBody2 = transform2.GetOrientation().getInverse() * transform1.GetOrientation();

                var margin = shape1.GetMargin() + shape2.GetMargin();
                var marginSquare = margin * margin;

                var simplex = new VoronoiSimplex();

                var lastFrameCollisionInfo =
                    narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].lastFrameCollisionInfo;


                var v = PVector3.Zero();
                if (lastFrameCollisionInfo.isValid && lastFrameCollisionInfo.wasUsingGJK)
                    v = lastFrameCollisionInfo.gjkSeparatingAxis;
                else
                    v.SetAllValues(0, 1, 0);

                var distSquare = float.MaxValue;

                var noIntersection = false;

                do
                {
                    suppA = shape1.getLocalSupportPointWithoutMargin(-v);
                    suppB = body2Tobody1 * shape2.getLocalSupportPointWithoutMargin(rotateToBody2 * v);
                    w = suppA - suppB;
                    vDotw = v.dot(w);

                    if (vDotw > 0.0f && vDotw * vDotw > distSquare * marginSquare)
                    {
                        lastFrameCollisionInfo.gjkSeparatingAxis = v;
                        gjkResults.Add(GJKResult.SEPARATED);
                        noIntersection = true;
                        break;
                    }

                    if (simplex.isPointInSimplex(w) || distSquare - vDotw <= distSquare * 1.0e-3 * 1.0e-3)
                    {
                        contactFound = true;
                        break;
                    }

                    simplex.addPoint(w, suppA, suppB);

                    if (simplex.isAffinelyDependent())
                    {
                        contactFound = true;
                        break;
                    }

                    if (!simplex.computeClosestPoint(out v))
                    {
                        contactFound = true;
                        break;
                    }

                    prevDistSquare = distSquare;
                    distSquare = v.LengthSquare();

                    if (prevDistSquare - distSquare <= float.Epsilon * prevDistSquare)
                    {
                        simplex.backupClosestPointInSimplex(out v);
                        distSquare = v.LengthSquare();
                        contactFound = true;
                        break;
                    }
                } while (!simplex.isFull() && distSquare > float.Epsilon * simplex.getMaxLengthSquareOfAPoint());

                if (noIntersection) continue;


                if (contactFound && distSquare > float.Epsilon)
                {
                    // 计算两个对象的最接近点（不考虑margin）
                    simplex.computeClosestPointsOfAandB(out pA, out pB);

                    // 将这两个点投影到margin上，以获得两个对象带有margin的最接近点
                    var dist = PMath.Sqrt(distSquare);
                    pA = (pA - (shape1.GetMargin() / dist) * v);
                    pB = body2Tobody1.GetInverse() * (pB + shape2.GetMargin() / dist * v);

                    // 计算接触信息
                    var normal = transform1.GetOrientation() * -v.GetUnit();
                    var penetrationDepth = margin - dist;

                    // 如果负的穿透深度（由于数值误差）导致没有接触，则没有接触
                    if (penetrationDepth <= 0.0f)
                    {
                        gjkResults.Add(GJKResult.SEPARATED);
                        continue;
                    }

                    // 不要生成零长度的法线接触点
                    if (normal.LengthSquare() < float.Epsilon)
                    {
                        gjkResults.Add(GJKResult.SEPARATED);
                        continue;
                    }

                    // 如果需要报告接触点
                    if (narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].reportContacts)
                    {
                        // 计算平滑的三角形网格接触点，如果两个碰撞形状之一是三角形
                        TriangleShape.computeSmoothTriangleMeshContact(shape1, shape2, pA, pB, transform1, transform2,
                            penetrationDepth, normal);

                        // 添加一个新的接触点
                        narrowPhaseInfoBatch.AddContactPoint(batchIndex, normal, penetrationDepth, pA, pB);
                    }

                    gjkResults.Add(GJKResult.COLLIDE_IN_MARGIN);

                    continue;
                }

                gjkResults.Add(GJKResult.INTERPENETRATE);
            }
        }

    }
}