
namespace RP3D
{
    /// <summary>
    /// 球体与胶囊体之间的碰撞检测算法。
    /// 此技术基于 Dirk Gregorius 的“物理模拟的稳健接触创建”演示文稿。
    /// </summary>
    public class SphereVsCapsuleAlgorithm : NarrowPhaseAlgorithm
    {
        /// <summary>
        /// 计算球体与胶囊体之间的窄相位碰撞检测。
        /// </summary>
        /// <param name="narrowPhaseInfoBatch">窄相位信息批次</param>
        /// <param name="batchStartIndex">批次起始索引</param>
        /// <param name="batchNbItems">批次中的项数</param>
        /// <returns>如果发现碰撞，则返回 true，否则返回 false</returns>
        public bool testCollision(NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchStartIndex, int batchNbItems)
        {
            var isCollisionFound = false;

            for (int batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++)
            {
                NarrowPhaseInfo narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];
                // 检查是否为球体形状1
                bool isSphereShape1 = narrowPhaseInfo.collisionShape1.GetType() == CollisionShapeType.SPHERE;

                // 获取球体和胶囊体形状
                SphereShape sphereShape = (SphereShape)(isSphereShape1 ? narrowPhaseInfo.collisionShape1 : narrowPhaseInfo.collisionShape2);
                CapsuleShape capsuleShape = (CapsuleShape)(isSphereShape1 ? narrowPhaseInfo.collisionShape2 : narrowPhaseInfo.collisionShape1);

                float capsuleHeight = capsuleShape.getHeight();
                float sphereRadius = sphereShape.GetRadius();
                float capsuleRadius = capsuleShape.getRadius();

                // 获取球体和胶囊体的局部空间到世界空间的变换
                PTransform sphereToWorldTransform = isSphereShape1 ? narrowPhaseInfo.shape1ToWorldTransform : narrowPhaseInfo.shape2ToWorldTransform;
                PTransform capsuleToWorldTransform = isSphereShape1 ? narrowPhaseInfo.shape2ToWorldTransform : narrowPhaseInfo.shape1ToWorldTransform;
                PTransform worldToCapsuleTransform = capsuleToWorldTransform.GetInverse();
                PTransform sphereToCapsuleSpaceTransform = worldToCapsuleTransform * sphereToWorldTransform;

                // 将球体中心点转换为胶囊体形状的局部空间
                PVector3 sphereCenter = sphereToCapsuleSpaceTransform.GetPosition();

                // 计算胶囊体内部段的端点
                float capsuleHalfHeight = capsuleHeight * 0.5f;
                PVector3 capsuleSegA = new PVector3(0, -capsuleHalfHeight, 0);
                PVector3 capsuleSegB = new PVector3(0, capsuleHalfHeight, 0);

                // 计算内部胶囊段上距离球体中心最近的点
                PVector3 closestPointOnSegment =
                   PMath.computeClosestPointOnSegment(capsuleSegA, capsuleSegB, sphereCenter);

                // 计算球体中心点与段上最近点之间的距离
                PVector3 sphereCenterToSegment = closestPointOnSegment - sphereCenter;
                float sphereSegmentDistanceSquare = sphereCenterToSegment.LengthSquare();

                // 计算球体和胶囊体（虚拟球体）的半径之和
                var sumRadius = sphereRadius + capsuleRadius;

                // 如果碰撞形状重叠
                if (sphereSegmentDistanceSquare < sumRadius * sumRadius)
                {
                    // 如果需要报告接触
                    if (narrowPhaseInfo.reportContacts)
                    {
                        // 如果球体中心不在胶囊体内部段上
                        float penetrationDepth;
                        PVector3 normalWorld;
                        PVector3 contactPointSphereLocal;
                        PVector3 contactPointCapsuleLocal;
                        if (sphereSegmentDistanceSquare > float.Epsilon)
                        {
                            float sphereSegmentDistance = PMath.Sqrt(sphereSegmentDistanceSquare);
                            sphereCenterToSegment /= sphereSegmentDistance;

                            contactPointSphereLocal = sphereToCapsuleSpaceTransform.GetInverse() *
                                                      (sphereCenter + sphereCenterToSegment * sphereRadius);
                            contactPointCapsuleLocal = closestPointOnSegment - sphereCenterToSegment * capsuleRadius;

                            normalWorld = capsuleToWorldTransform.GetOrientation() * sphereCenterToSegment;

                            penetrationDepth = sumRadius - sphereSegmentDistance;

                            if (!isSphereShape1) normalWorld = -normalWorld;
                        }
                        else
                        {
                            // 如果球体中心在胶囊体内部段上（退化情况）

                            // 任意取垂直于胶囊体内部段的方向作为接触法线

                            // 胶囊体内部段
                            PVector3 capsuleSegment = (capsuleSegB - capsuleSegA).GetUnit();

                            PVector3 vec1 = new PVector3(1, 0, 0);
                            PVector3 vec2 = new PVector3(0, 1, 0);

                            // 获取垂直于胶囊体内部段的向量（vec1 和 vec2 中的其中一个），它们的绝对点乘最小
                            float cosA1 = PMath.Abs(capsuleSegment.x); // abs(vec1.dot(seg2))
                            float cosA2 = PMath.Abs(capsuleSegment.y); // abs(vec2.dot(seg2))

                            penetrationDepth = sumRadius;

                            // 我们选择垂直于胶囊体内部段的任意方向作为接触法线
                            PVector3 normalCapsuleSpace =
                                cosA1 < cosA2 ? capsuleSegment.cross(vec1) : capsuleSegment.cross(vec2);
                            normalWorld = capsuleToWorldTransform.GetOrientation() * normalCapsuleSpace;

                            // 计算两个局部接触点
                            contactPointSphereLocal = sphereToCapsuleSpaceTransform.GetInverse() *
                                                      (sphereCenter + normalCapsuleSpace * sphereRadius);
                            contactPointCapsuleLocal = sphereCenter - normalCapsuleSpace * capsuleRadius;
                        }

                        if (penetrationDepth <= 0.0f)
                            // 无碰撞
                            continue;

                        // 创建接触信息对象
                        narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, penetrationDepth,
                            isSphereShape1 ? contactPointSphereLocal : contactPointCapsuleLocal,
                            isSphereShape1 ? contactPointCapsuleLocal : contactPointSphereLocal);
                    }

                    narrowPhaseInfo.isColliding = true;
                    isCollisionFound = true;
                    continue;
                }
            }

            return isCollisionFound;
        }
    }
}
