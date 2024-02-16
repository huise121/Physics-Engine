/********************************************************************************
* 
*   在物理中，BroadPhaseSystem（广义相位系统）用于管理碰撞检测的初始阶段，其主要目标是减少需要进行详细检测的碰撞体数量，以提高效率。
*   这个阶段通常涉及对所有潜在的碰撞体对进行粗略检测，以确定哪些碰撞体可能会发生碰撞。
*   广义相位系统通常会使用一些空间分区技术（如包围盒层次结构）来加速这个过程，例如，当一个物体移动时，它只需要与相邻的空间区域进行碰撞检测，而不是与场景中所有的物体进行检测。
*   这种方法有助于提高碰撞检测的效率，并且在处理大量物体时尤为重要。
*                                                                               *
********************************************************************************/

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    /// 广相系统
    public class BroadPhaseSystem
    {
        /// 动态AABB树
        public DynamicAABBTree mDynamicAABBTree;
        /// 碰撞体组件的引用
        protected ColliderComponents mCollidersComponents;
        /// 变换组件的引用
        protected TransformComponents mTransformsComponents;
        /// 碰撞检测对象的引用
        protected CollisionDetectionSystem mCollisionDetection;
        /// 所有在上一模拟步骤中移动（或创建）的碰撞形状的广相ID集合。
        /// 这些是在下一模拟步骤中需要测试重叠的形状。
        HashSet<int> mMovedShapes;

        public BroadPhaseSystem(CollisionDetectionSystem collisionDetection, ColliderComponents collidersComponents,
            TransformComponents transformComponents)
        {
            mDynamicAABBTree = new DynamicAABBTree(0.08f);
            mCollisionDetection = collisionDetection;
            mCollidersComponents = collidersComponents;
            mTransformsComponents = transformComponents;
            mMovedShapes = new HashSet<int>();
        }

        /// 通知动态AABB树碰撞体需要更新
        protected void updateColliderInternal(int broadPhaseId, Collider collider, AABB aabb, bool forceReInsert)
        {
            // 根据碰撞形状的移动更新动态AABB树
            bool hasBeenReInserted = mDynamicAABBTree.updateObject(broadPhaseId, aabb, forceReInsert);

            // 如果碰撞形状已经移动出其fat AABB（因此已重新插入树中）。
            if (hasBeenReInserted)
            {
                // 将碰撞体添加到在上一模拟步骤中移动（或创建）的形状数组中
                addMovedCollider(broadPhaseId, collider);
            }
        }

        /// 更新一些碰撞体组件的广相状态
        protected void updateCollidersComponents(int startIndex, int nbItems)
        {
            // 确保不更新禁用的组件
            startIndex = PMath.Min(startIndex, mCollidersComponents.getNbEnabledComponents());
            int endIndex = PMath.Min(startIndex + nbItems, mCollidersComponents.getNbEnabledComponents());
            nbItems = endIndex - startIndex;

            // 对于要更新的每个碰撞体组件
            for (int i = startIndex; i < startIndex + nbItems; i++)
            {
                int broadPhaseId = mCollidersComponents.mBroadPhaseIds[i];
                if (broadPhaseId != -1)
                {
                    Entity bodyEntity = mCollidersComponents.mBodiesEntities[i];
                    PTransform transform = mTransformsComponents.getTransform(bodyEntity);

                    // 重新计算碰撞形状的世界空间AABB
                    AABB aabb;
                    mCollidersComponents.mCollisionShapes[i].ComputeAABB(out aabb, transform * mCollidersComponents.mLocalToBodyTransforms[i]);

                    // 如果用户更改了碰撞形状的大小，则需要将广相AABB重置为其新大小
                    bool forceReInsert = mCollidersComponents.mHasCollisionShapeChangedSize[i];

                    // 更新碰撞体的广相状态
                    updateColliderInternal(broadPhaseId, mCollidersComponents.mColliders[i], aabb, forceReInsert);

                    mCollidersComponents.mHasCollisionShapeChangedSize[i] = false;
                }
            }
        }

        /// 添加碰撞体
        public void addCollider(Collider collider, AABB aabb)
        {
            // 将碰撞形状添加到动态AABB树并获取其广相ID
            int nodeId = mDynamicAABBTree.addObject(aabb, collider);

            // 设置碰撞体的广相ID
            mCollidersComponents.setBroadPhaseId(collider.GetEntity(), nodeId);

            // 将碰撞体添加到在上一模拟步骤中移动（或创建）的形状数组中
            addMovedCollider(collider.getBroadPhaseId(), collider);
        }

        /// 移除碰撞体
        public void removeCollider(Collider collider)
        {
            int broadPhaseID = collider.getBroadPhaseId();

            mCollidersComponents.setBroadPhaseId(collider.GetEntity(), -1);

            // 从动态AABB树中移除碰撞形状
            mDynamicAABBTree.removeObject(broadPhaseID);

            // 从在上一模拟步骤中移动（或创建）的形状数组中移除碰撞形状
            removeMovedCollider(broadPhaseID);
        }

        /// 更新碰撞体
        public void updateCollider(Entity colliderEntity)
        {
            // 获取碰撞体组件在数组中的索引
            int index = mCollidersComponents.mMapEntityToComponentIndex[colliderEntity];

            // 更新碰撞体组件
            updateCollidersComponents(index, 1);
        }

        /// 更新所有启用的碰撞体组件
        public void updateColliders()
        {
            // 更新所有启用的碰撞体组件
            if (mCollidersComponents.getNbEnabledComponents() > 0)
            {
                updateCollidersComponents(0, mCollidersComponents.getNbEnabledComponents());
            }
        }

        /// 添加一个在上一模拟中移动的碰撞体，并需要重新测试以检测宽相位重叠。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void addMovedCollider(int broadPhaseID, Collider collider)
        {
            // 将广相ID存储到在上一模拟步骤中移动的形状数组中
            mMovedShapes.Add(broadPhaseID);
            // 通知涉及此形状的重叠对需要测试重叠
            mCollisionDetection.notifyOverlappingPairsToTestOverlap(collider);
        }
        
        /// 从上一次模拟中移动的碰撞体数组中移除一个碰撞体，并需要重新测试以检测宽相位重叠。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void removeMovedCollider(int broadPhaseID)
        {
            // 从集合中移除广相ID
            mMovedShapes.Remove(broadPhaseID);
        }

        /// 计算所有重叠的碰撞形状对。
        public void computeOverlappingPairs(List<Pair<int, int>> overlappingNodes)
        {
            // 获取在上一帧中移动或创建的碰撞体数组
            List<int> shapesToTest = new List<int>(mMovedShapes);
            
            // 请求动态AABB树报告所有与要测试的形状重叠的碰撞形状
            mDynamicAABBTree.ReportAllShapesOverlappingWithShapes(ref shapesToTest, 0, shapesToTest.Count,ref overlappingNodes);
            
            // 重置在上一模拟步骤中移动（或创建）的形状数组
            mMovedShapes.Clear();
        }

        /// 测试两个广相碰撞形状是否重叠
        public bool testOverlappingShapes(int shape1BroadPhaseId, int shape2BroadPhaseId)
        {
            // 获取两个碰撞形状的AABB
            AABB aabb1 = mDynamicAABBTree.getFatAABB(shape1BroadPhaseId);
            AABB aabb2 = mDynamicAABBTree.getFatAABB(shape2BroadPhaseId);
            // 检查两个AABB是否重叠
            return aabb1.TestCollision(aabb2);
        }

        /// 射线投射
        public void raycast(PRay ray, RaycastTest raycastTest, short raycastWithCategoryMaskBits)
        {
            BroadPhaseRaycastCallback broadPhaseRaycastCallback = new BroadPhaseRaycastCallback(mDynamicAABBTree, raycastWithCategoryMaskBits, raycastTest);
            mDynamicAABBTree.raycast(ray, broadPhaseRaycastCallback);
        }

        /// 获取广相ID对应的fat AABB
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AABB getFatAABB(int broadPhaseId)
        {
            return mDynamicAABBTree.getFatAABB(broadPhaseId);
        }


        
        
        
        // public void GetAllNodeMatriciesRecursive(TreeNode n, ref AABB matricies, int depth)
        // {
        //     // 不需要旋转，因为是AABB
        //     // Matrix4x4 matrix = Matrix4x4.Translate(n.Box.Center.ToVector()) * Matrix4x4.Scale(n.Box.Size.ToVector());
        //     // matricies.Add(matrix);
        //
        //     int right = n.children[0];
        //     int left = n.children[1];
        //
        //     // if (n.Right != null) GetAllNodeMatriciesRecursive(n., ref matricies, depth + 1);
        //     // if (n.Left != null) GetAllNodeMatriciesRecursive(n.Left, ref matricies, depth + 1);
        // }
        
        
    }
}
