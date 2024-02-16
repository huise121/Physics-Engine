namespace RP3D
{
    public class ConvexPolyhedronVsConvexPolyhedronAlgorithm : NarrowPhaseAlgorithm {
        // 计算两个凸多面体之间的窄相碰撞检测
        // 这个技术基于Dirk Gregorius的"Robust Contact Creation for Physics Simulations"演示文稿。
        public bool testCollision(NarrowPhaseInfoBatch narrowPhaseInfoBatch,
            int batchStartIndex, int batchNbItems,
            bool clipWithPreviousAxisIfStillColliding) {
            // 运行SAT算法以查找分离轴并计算接触点
            var satAlgorithm = new SATAlgorithm(clipWithPreviousAxisIfStillColliding);

            var isCollisionFound =
                satAlgorithm.testCollisionConvexPolyhedronVsConvexPolyhedron(narrowPhaseInfoBatch, batchStartIndex,
                    batchNbItems);

            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++) {
                NarrowPhaseInfo narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];
                // 获取上一帧的碰撞信息
                var lastFrameCollisionInfo = narrowPhaseInfo.lastFrameCollisionInfo;

                lastFrameCollisionInfo.wasUsingSAT = true;
                lastFrameCollisionInfo.wasUsingGJK = false;
            }
            return isCollisionFound;
        }
    }

}