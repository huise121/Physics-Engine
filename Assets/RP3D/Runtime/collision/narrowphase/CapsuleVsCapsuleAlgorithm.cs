
namespace RP3D
{
    /// <summary>
    /// 这个类实现了两个胶囊体之间的碰撞检测算法。
    /// 在 testCollision 方法中，遍历了窄相位信息批次中的每个项，计算了两个胶囊体之间的碰撞情况。
    /// 具体的碰撞检测算法逻辑在方法内部进行了详细的处理。
    /// </summary>
    public class CapsuleVsCapsuleAlgorithm : NarrowPhaseAlgorithm
    {
        /// <summary>
        /// 计算两个包围体碰撞时的接触信息。
        /// </summary>
        /// <param name="narrowPhaseInfoBatch">窄相位信息批次</param>
        /// <param name="batchStartIndex">批次起始索引</param>
        /// <param name="batchNbItems">批次中的项数</param>
        /// <returns>如果发现碰撞，则返回 true，否则返回 false</returns>
        public bool testCollision(NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchStartIndex, int batchNbItems)
        {
            var isCollisionFound = false;

            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++)
            {
                NarrowPhaseInfo narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];
                // 获取从胶囊体1局部空间到胶囊体2局部空间的变换
                var capsule1ToCapsule2SpaceTransform =
                    narrowPhaseInfo.shape2ToWorldTransform.GetInverse() *
                    narrowPhaseInfo.shape1ToWorldTransform;

                var capsuleShape1 = (CapsuleShape)narrowPhaseInfo.collisionShape1;
                var capsuleShape2 = (CapsuleShape)narrowPhaseInfo.collisionShape2;

                var capsule1Height = capsuleShape1.getHeight();
                var capsule2Height = capsuleShape2.getHeight();
                var capsule1Radius = capsuleShape1.getRadius();
                var capsule2Radius = capsuleShape2.getRadius();

                // 计算第一个胶囊体内部段的端点
                var capsule1HalfHeight = capsule1Height * 0.5f;
                var capsule1SegA = new PVector3(0, -capsule1HalfHeight, 0);
                var capsule1SegB = new PVector3(0, capsule1HalfHeight, 0);
                capsule1SegA = capsule1ToCapsule2SpaceTransform * capsule1SegA;
                capsule1SegB = capsule1ToCapsule2SpaceTransform * capsule1SegB;

                // 计算第二个胶囊体内部段的端点
                var capsule2HalfHeight = capsule2Height * 0.5f;
                var capsule2SegA = new PVector3(0, -capsule2HalfHeight, 0);
                var capsule2SegB = new PVector3(0, capsule2HalfHeight, 0);

                // 两个内部胶囊段
                var seg1 = capsule1SegB - capsule1SegA;
                var seg2 = capsule2SegB - capsule2SegA;

                // 计算两个胶囊体（虚拟球体）的半径之和
                var sumRadius = capsule1Radius + capsule2Radius;

                // 如果两个胶囊体平行（我们创建两个接触点）
                bool areCapsuleInnerSegmentsParallel = PMath.areParallelVectors(seg1, seg2);
                if (areCapsuleInnerSegmentsParallel)
                {
                    // 如果两个段之间的距离大于两个胶囊体的半径之和（我们没有重叠）
                    float segmentsPerpendicularDistance =
                       PMath.computePointToLineDistance(capsule1SegA, capsule1SegB, capsule2SegA);
                    if (segmentsPerpendicularDistance >= sumRadius)
                        // 胶囊体平行，但它们的内部段距离大于两个胶囊体的半径之和。
                        // 因此，我们没有重叠。如果内部段重叠，则不报告任何碰撞。
                        continue;

                    // 计算通过第一个胶囊体内部段的极端点的平面
                    var d1 = seg1.dot(capsule1SegA);
                    var d2 = -seg1.dot(capsule1SegB);

                    // 使用通过第一个胶囊体内部段的极端点的两个平面剪切第二个胶囊体的内部段
                    float t1 = PMath.computePlaneSegmentIntersection(capsule2SegB, capsule2SegA, d1, seg1);
                    float t2 = PMath.computePlaneSegmentIntersection(capsule2SegA, capsule2SegB, d2, -seg1);

                    // 如果段重叠（剪切段有效）
                    if (t1 > 0.0f && t2 > 0.0f)
                    {
                        if (narrowPhaseInfo.reportContacts)
                        {
                            // 剪切第二个胶囊体的内部段
                            if (t1 > 1.0f) t1 = 1.0f;
                            var clipPointA = capsule2SegB - t1 * seg2;
                            if (t2 > 1.0f) t2 = 1.0f;
                            var clipPointB = capsule2SegA + t2 * seg2;

                            // 投影点capsule2SegA到第一个胶囊体内部段的线上
                            var seg1Normalized = seg1.GetUnit();
                            var pointOnInnerSegCapsule1 = capsule1SegA +
                                                          seg1Normalized.dot(capsule2SegA - capsule1SegA) *
                                                          seg1Normalized;

                            PVector3 normalCapsule2SpaceNormalized;
                            PVector3 segment1ToSegment2 = PVector3.Zero();

                            // 如果内部胶囊段的垂直距离不为零（内部段不重叠）
                            if (segmentsPerpendicularDistance > float.Epsilon)
                            {
                                // 从段1到段2计算一个垂直向量
                                segment1ToSegment2 = capsule2SegA - pointOnInnerSegCapsule1;
                                normalCapsule2SpaceNormalized = segment1ToSegment2.GetUnit();
                            }
                            else
                            {
                                // 如果胶囊段不平行

                                // 我们不能使用两段之间的向量作为接触法线。我们需要使用两段的叉积来计算新的接触法线。
                                normalCapsule2SpaceNormalized = seg1.cross(seg2);
                                normalCapsule2SpaceNormalized.Normalize();
                            }

                            PTransform capsule2ToCapsule1SpaceTransform = capsule1ToCapsule2SpaceTransform.GetInverse();
                            PVector3 contactPointACapsule1Local = capsule2ToCapsule1SpaceTransform *
                                                                      (clipPointA - segment1ToSegment2 +
                                                                       normalCapsule2SpaceNormalized * capsule1Radius);
                            PVector3 contactPointBCapsule1Local = capsule2ToCapsule1SpaceTransform *
                                                                  (clipPointB - segment1ToSegment2 +
                                                                   normalCapsule2SpaceNormalized * capsule1Radius);
                            PVector3 contactPointACapsule2Local =
                                clipPointA - normalCapsule2SpaceNormalized * capsule2Radius;
                            PVector3 contactPointBCapsule2Local =
                                clipPointB - normalCapsule2SpaceNormalized * capsule2Radius;

                            var penetrationDepth = sumRadius - segmentsPerpendicularDistance;

                            PVector3 normalWorld =
                                narrowPhaseInfo.shape2ToWorldTransform
                                    .GetOrientation() * normalCapsule2SpaceNormalized;

                            // 创建接触信息对象
                            narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, penetrationDepth,
                                contactPointACapsule1Local, contactPointACapsule2Local);
                            narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, penetrationDepth,
                                contactPointBCapsule1Local, contactPointBCapsule2Local);
                        }

                        NarrowPhaseInfo tempInfp = narrowPhaseInfo;
                        tempInfp.isColliding = true;
                        isCollisionFound = true;
                        continue;
                    }
                }

                // 计算两个内部胶囊段之间的最近点
                PVector3 closestPointCapsule1Seg;
                PVector3 closestPointCapsule2Seg;
                PMath.computeClosestPointBetweenTwoSegments(capsule1SegA, capsule1SegB, capsule2SegA, capsule2SegB,
                    out closestPointCapsule1Seg, out closestPointCapsule2Seg);

                // 计算两段之间的距离
                PVector3 closestPointsSeg1ToSeg2 = closestPointCapsule2Seg - closestPointCapsule1Seg;
                float closestPointsDistanceSquare = closestPointsSeg1ToSeg2.LengthSquare();

                // 如果重叠
                if (closestPointsDistanceSquare < sumRadius * sumRadius)
                {
                    if (narrowPhaseInfo.reportContacts)
                    {
                        // 如果两段之间的距离不为零
                        if (closestPointsDistanceSquare > float.Epsilon)
                        {
                            float closestPointsDistance = PMath.Sqrt(closestPointsDistanceSquare);
                            closestPointsSeg1ToSeg2 /= closestPointsDistance;

                            PVector3 contactPointCapsule1Local = capsule1ToCapsule2SpaceTransform.GetInverse() *
                                                                 (closestPointCapsule1Seg +
                                                                  closestPointsSeg1ToSeg2 * capsule1Radius);
                            PVector3 contactPointCapsule2Local =
                                closestPointCapsule2Seg - closestPointsSeg1ToSeg2 * capsule2Radius;

                            PVector3 normalWorld =
                                narrowPhaseInfo.shape2ToWorldTransform
                                    .GetOrientation() * closestPointsSeg1ToSeg2;

                            float penetrationDepth = sumRadius - closestPointsDistance;

                            // 创建接触信息对象
                            narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, penetrationDepth,
                                contactPointCapsule1Local, contactPointCapsule2Local);
                        }
                        else
                        {
                            // 段重叠

                            // 如果胶囊段平行
                            if (areCapsuleInnerSegmentsParallel)
                            {
                                // 段平行，不重叠，它们的距离为零。
                                // 因此，胶囊体只是在内部段的顶部接触。
                                var squareDistCapsule2PointToCapsuleSegA =
                                    (capsule1SegA - closestPointCapsule2Seg).LengthSquare();

                                PVector3 capsule1SegmentMostExtremePoint =
                                    squareDistCapsule2PointToCapsuleSegA > float.Epsilon
                                        ? capsule1SegA
                                        : capsule1SegB;
                                PVector3 normalCapsuleSpace2 = closestPointCapsule2Seg - capsule1SegmentMostExtremePoint;
                                normalCapsuleSpace2.Normalize();

                                PVector3 contactPointCapsule1Local = capsule1ToCapsule2SpaceTransform.GetInverse() *
                                                                     (closestPointCapsule1Seg +
                                                                      normalCapsuleSpace2 * capsule1Radius);
                                PVector3 contactPointCapsule2Local =
                                    closestPointCapsule2Seg - normalCapsuleSpace2 * capsule2Radius;

                                PVector3 normalWorld =
                                    narrowPhaseInfo.shape2ToWorldTransform
                                        .GetOrientation() * normalCapsuleSpace2;

                                // 创建接触信息对象
                                narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, sumRadius,
                                    contactPointCapsule1Local, contactPointCapsule2Local);
                            }
                            else
                            {
                                // 如果内部段不平行

                                // 我们不能使用两段之间的向量作为接触法线。我们需要使用两段的叉积来计算新的接触法线。
                                PVector3 normalCapsuleSpace2 = seg1.cross(seg2);
                                normalCapsuleSpace2.Normalize();

                                // 计算两个形状上的接触点
                                PVector3 contactPointCapsule1Local = capsule1ToCapsule2SpaceTransform.GetInverse() *
                                                                     (closestPointCapsule1Seg +
                                                                      normalCapsuleSpace2 * capsule1Radius);
                                PVector3 contactPointCapsule2Local =
                                    closestPointCapsule2Seg - normalCapsuleSpace2 * capsule2Radius;

                                PVector3 normalWorld =
                                    narrowPhaseInfo.shape2ToWorldTransform
                                        .GetOrientation() * normalCapsuleSpace2;

                                // 创建接触信息对象
                                narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, sumRadius,
                                    contactPointCapsule1Local, contactPointCapsule2Local);
                            }
                        }
                    }

                    narrowPhaseInfo.isColliding = true;
                    isCollisionFound = true;
                }
            }

            return isCollisionFound;
        }
    }
}
