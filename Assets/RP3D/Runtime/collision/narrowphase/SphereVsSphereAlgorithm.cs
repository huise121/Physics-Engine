
using UnityEngine;

namespace RP3D
{
    /// <summary>
    /// 球体与球体之间的碰撞检测算法。
    /// </summary>
    public class SphereVsSphereAlgorithm : NarrowPhaseAlgorithm
    {
        /// <summary>
        /// 如果两个边界体发生碰撞，则计算一个接触信息。
        /// </summary>
        /// <param name="narrowPhaseInfoBatch">窄相位信息批次</param>
        /// <param name="batchStartIndex">批次起始索引</param>
        /// <param name="batchNbItems">批次中的项数</param>
        /// <returns>如果发现碰撞，则返回 true，否则返回 false</returns>
        public bool testCollision(NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchStartIndex, int batchNbItems)
        {
            var isCollisionFound = false;

            // 对于批次中的每个项
            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++)
            {
                NarrowPhaseInfo narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];
                // 获取局部空间到世界空间的变换
                var transform1 = narrowPhaseInfo.shape1ToWorldTransform;
                var transform2 = narrowPhaseInfo.shape2ToWorldTransform;

                // 计算两个球体中心之间的距离
                var vectorBetweenCenters = transform2.GetPosition() - transform1.GetPosition();
                var squaredDistanceBetweenCenters = vectorBetweenCenters.LengthSquare();

                var sphereShape1 = (SphereShape)narrowPhaseInfo.collisionShape1;
                var sphereShape2 = (SphereShape)narrowPhaseInfo.collisionShape2;

                var sphere1Radius = sphereShape1.GetRadius();
                var sphere2Radius = sphereShape2.GetRadius();

                // 计算半径之和
                var sumRadiuses = sphere1Radius + sphere2Radius;

                // 计算半径之和的平方
                var sumRadiusesProducts = sumRadiuses * sumRadiuses;

                // 如果球体碰撞形状相交
                if (squaredDistanceBetweenCenters < sumRadiusesProducts)
                {
                    var penetrationDepth = sumRadiuses - PMath.Sqrt(squaredDistanceBetweenCenters);

                    // 确保穿透深度不为零（即使上一个条件测试为真，由于计算的精度问题，穿透深度仍可能为零）
                    if (penetrationDepth > 0)
                    {
                        // 如果需要报告接触
                        if (narrowPhaseInfo.reportContacts)
                        {
                            var transform1Inverse = transform1.GetInverse();
                            var transform2Inverse = transform2.GetInverse();

                            PVector3 intersectionOnBody1;
                            PVector3 intersectionOnBody2;
                            var normal = PVector3.Zero();

                            // 如果两个球体中心不在同一位置
                            if (squaredDistanceBetweenCenters > float.Epsilon)
                            {
                                var centerSphere2InBody1LocalSpace = transform1Inverse * transform2.GetPosition();
                                var centerSphere1InBody2LocalSpace = transform2Inverse * transform1.GetPosition();

                                intersectionOnBody1 = sphere1Radius * centerSphere2InBody1LocalSpace.GetUnit();
                                intersectionOnBody2 = sphere2Radius * centerSphere1InBody2LocalSpace.GetUnit();
                                normal = vectorBetweenCenters.GetUnit();
                            }
                            else
                            {
                                // 如果球体中心在同一位置（退化情况）

                                // 取任意的接触法线方向
                                normal.SetAllValues(0, 1, 0);

                                intersectionOnBody1 = sphere1Radius * (transform1Inverse.GetOrientation() * normal);
                                intersectionOnBody2 = sphere2Radius * (transform2Inverse.GetOrientation() * normal);
                            }
                            
                            
                            //Debug.Log($"normal: {normal} ,{penetrationDepth} ,{intersectionOnBody1} ,{intersectionOnBody2} ,{PhysicsWorld.frameIndex}" );
                            
                            // 创建接触信息对象
                            narrowPhaseInfoBatch.AddContactPoint(batchIndex, normal, penetrationDepth,
                                intersectionOnBody1, intersectionOnBody2);
                        }

                        narrowPhaseInfo.isColliding = true;
                        isCollisionFound = true;
                    }
                }
            }

            return isCollisionFound;
        }
    }
}
