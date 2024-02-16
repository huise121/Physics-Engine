using System.Collections.Generic;

namespace RP3D
{
    public class SphereVsConvexPolyhedronAlgorithm {
        // 计算球体和凸多面体之间的窄相碰撞检测
        // 该技术基于 Dirk Gregorius 的 "Robust Contact Creation for Physics Simulations" 演示文稿
        public bool testCollision(NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchStartIndex, int batchNbItems,
            bool clipWithPreviousAxisIfStillColliding) {
            // 首先，运行 GJK 算法
            var gjkAlgorithm = new GJKAlgorithm();

            var isCollisionFound = false;

            var gjkResults = new List<GJKAlgorithm.GJKResult>(batchNbItems);
            gjkAlgorithm.TestCollision(narrowPhaseInfoBatch, batchStartIndex, batchNbItems, ref gjkResults);

            // 对于批处理中的每个项
            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++) {
                NarrowPhaseInfo narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];
                // 获取上一帧的碰撞信息
                var lastFrameCollisionInfo = narrowPhaseInfo.lastFrameCollisionInfo;

                lastFrameCollisionInfo.wasUsingGJK = true;
                lastFrameCollisionInfo.wasUsingSAT = false;

                // 如果我们在边缘内找到了一个接触点（浅入侵）
                if (gjkResults[batchIndex] == GJKAlgorithm.GJKResult.COLLIDE_IN_MARGIN) {
                    // 返回 true
                    narrowPhaseInfo.isColliding = true;
                    isCollisionFound = true;
                    continue;
                }

                // 如果即使没有边缘也有重叠（深入侵）
                if (gjkResults[batchIndex] == GJKAlgorithm.GJKResult.INTERPENETRATE) {
                    // 运行 SAT 算法以找到分离轴并计算接触点
                    var satAlgorithm = new SATAlgorithm(clipWithPreviousAxisIfStillColliding);

                    isCollisionFound |=
                        satAlgorithm.testCollisionSphereVsConvexPolyhedron(narrowPhaseInfoBatch, batchIndex, 1);

                    lastFrameCollisionInfo.wasUsingGJK = false;
                    lastFrameCollisionInfo.wasUsingSAT = true;
                }
            }

            return isCollisionFound;
        }
    }

}