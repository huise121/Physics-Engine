using UnityEngine;

namespace RP3D
{
    public class BroadPhaseRaycastCallback:DynamicAABBTreeRaycastCallback
    {
        private DynamicAABBTree mDynamicAABBTree;
        private short mRaycastWithCategoryMaskBits;
        
        RaycastTest mRaycastTest;


        
        public BroadPhaseRaycastCallback( DynamicAABBTree dynamicAABBTree,  short raycastWithCategoryMaskBits,
            RaycastTest raycastTest)
        {
            mDynamicAABBTree = dynamicAABBTree;
            mRaycastWithCategoryMaskBits = raycastWithCategoryMaskBits;
            mRaycastTest = raycastTest;
        }
        
        public override  float raycastBroadPhaseShape(int nodeId, PRay ray) {
            float hitFraction = -1.0f;

            // 从节点获取碰撞体
            Collider collider = mDynamicAABBTree.getNodeDataPointer(nodeId);

            // 检查射线过滤掩码是否允许对该形状进行射线检测
            if ((mRaycastWithCategoryMaskBits & collider.getCollisionCategoryBits()) != 0) {

                // 询问碰撞检测执行射线检测测试以针对该节点的碰撞体，因为射线与广义相交形状
                // 进行了重叠
                hitFraction = mRaycastTest.raycastAgainstShape(collider, ray);
            }
            return hitFraction;
        }


    }
}