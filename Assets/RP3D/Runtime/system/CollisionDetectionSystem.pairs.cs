using System.Collections.Generic;

namespace RP3D
{
    public partial class CollisionDetectionSystem
    {
        /// 计算从接触对 ID 到 next frame 的接触对映射的方法
        public void computeMapPreviousContactPairs()
        {
            mPreviousMapPairIdToContactPairIndex.Clear();
            var nbCurrentContactPairs = mCurrentContactPairs.Count;
            for (var i = 0; i < nbCurrentContactPairs; i++)
                mPreviousMapPairIdToContactPairIndex[mCurrentContactPairs[i].PairId] =  i;
        }
        
        // 过滤重叠对，仅保留涉及特定刚体的对
        public void filterOverlappingPairs(Entity bodyEntity, List<int> convexPairs, List<int> concavePairs)
        {
            var nbConvexPairs = mOverlappingPairs.mConvexPairs.Count;
            for (var i = 0; i < nbConvexPairs; i++)
                if (mCollidersComponents.getBody(mOverlappingPairs.mConvexPairs[i].Collider1) == bodyEntity ||
                    mCollidersComponents.getBody(mOverlappingPairs.mConvexPairs[i].Collider2) == bodyEntity)
                    convexPairs.Add(mOverlappingPairs.mConvexPairs[i].PairID);

            /*var nbConcavePairs = mOverlappingPairs.mConcavePairs.Count;
            for (var i = 0; i < nbConcavePairs; i++)
                if (mCollidersComponents.getBody(mOverlappingPairs.mConcavePairs[i].Collider1) == bodyEntity ||
                    mCollidersComponents.getBody(mOverlappingPairs.mConcavePairs[i].Collider2) == bodyEntity)
                    concavePairs.Add(mOverlappingPairs.mConcavePairs[i].PairID);*/
        }

        // 筛选重叠对，保留仅涉及两个给定物体的对
        public void filterOverlappingPairs(Entity body1Entity, Entity body2Entity, List<int> convexPairs,
            List<int> concavePairs)
        {
            // 对于每个凸对
            var nbConvexPairs = mOverlappingPairs.mConvexPairs.Count;
            for (var i = 0; i < nbConvexPairs; i++)
            {
                var collider1Body = mCollidersComponents.getBody(mOverlappingPairs.mConvexPairs[i].Collider1);
                var collider2Body = mCollidersComponents.getBody(mOverlappingPairs.mConvexPairs[i].Collider2);

                if ((collider1Body == body1Entity && collider2Body == body2Entity) ||
                    (collider1Body == body2Entity && collider2Body == body1Entity))
                    convexPairs.Add(mOverlappingPairs.mConvexPairs[i].PairID);
            }

            /*// 对于每个凹对
            var nbConcavePairs = mOverlappingPairs.mConcavePairs.Count;
            for (var i = 0; i < nbConcavePairs; i++)
            {
                var collider1Body = mCollidersComponents.getBody(mOverlappingPairs.mConcavePairs[i].Collider1);
                var collider2Body = mCollidersComponents.getBody(mOverlappingPairs.mConcavePairs[i].Collider2);

                if ((collider1Body == body1Entity && collider2Body == body2Entity) ||
                    (collider1Body == body2Entity && collider2Body == body1Entity))
                    concavePairs.Add(mOverlappingPairs.mConcavePairs[i].PairID);
            }*/
        }
    }
}