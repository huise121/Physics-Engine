// 中的 DynamicAABBTree 是一种用于加速碰撞检测和物体查询的数据结构。
// 它是基于动态包围盒层次树（Dynamic AABB Tree）实现的。
//
// DynamicAABBTree 的主要作用是管理场景中物体的边界框（AABB），
// 并提供一种高效的方法来进行碰撞检测、射线检测以及近邻查询等操作。
// 通过将物体的边界框组织成树状结构，DynamicAABBTree 可以快速地确定物体之间是否相交，从而实现高效的碰撞检测。
//
// DynamicAABBTree 被广泛应用于碰撞检测和物理仿真过程中，以提高性能和准确性。
// 它是该物理引擎中重要的组成部分之一，为处理大量物体之间的碰撞关系提供了可靠的解决方案。

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public partial class DynamicAABBTree
    {
        /// FatAABB是初始AABB通过其大小的一定百分比膨胀而成。
        private float mFatAABBInflatePercentage;

        /// <summary>
        /// 在 DynamicAABBTree 中，mFreeNodeID 是用于表示可用节点的索引或标识符。
        /// 这个成员变量通常用于跟踪哪些节点当前是空闲的，即没有被分配给任何物体的节点。
        /// 在动态 AABB 树的操作中，当需要分配一个新的节点来表示某个物体的边界框时，会首先从 mFreeNodeID 中获取一个可用的节点，然后将其分配给该物体使用。
        /// 这样可以提高节点的重复利用率和内存利用率，避免频繁地分配和释放节点造成的内存碎片问题，从而提高数据结构的性能和效率。
        /// </summary>
        private int mFreeNodeID;

        /// <summary>
        /// 在 DynamicAABBTree 中，mNbAllocatedNodes 表示当前已分配的节点数量。
        /// 这个变量用于跟踪动态 AABB 树中当前存在的节点总数。
        /// 在动态 AABB 树中，每个节点都代表空间中的一个包围盒或对象，
        /// 因此通过跟踪已分配的节点数量，可以了解树的规模和复杂度，以及内存使用情况。
        /// </summary>
        private int mNbAllocatedNodes;

        /// 树中的节点数
        public int mNbNodes;

        /// 树节点的列表
        private List<TreeNode> mNodes;

        /// <summary>
        /// 在 DynamicAABBTree 中，mRootNodeID 表示树的根节点的标识符或索引。
        /// 根节点是树的起始点，它连接了树的所有其他节点。
        /// 通过跟踪根节点的索引，可以轻松地访问树的整个结构，并开始执行诸如查询、更新等操作。
        /// </summary>
        private int mRootNodeID;


        public DynamicAABBTree(float fatAABBInflatePercentage)
        {
            mRootNodeID = TreeNode.NULL_TREE_NODE;
            mFatAABBInflatePercentage = fatAABBInflatePercentage;
            init();
        }

        // Initialize the tree
        private void init()
        {
            mRootNodeID = TreeNode.NULL_TREE_NODE;
            mNbNodes = 0;
            mNbAllocatedNodes = 8;
            mNodes = new List<TreeNode>();
            // Construct the nodes

            for (var i = 0; i < mNbAllocatedNodes; i++)
                mNodes.Add(new TreeNode
                {
                    children = new int[2],
                    // dataInt = new int[2],
                    height = -1,
                    aabb = new AABB()
                });
            // Initialize the allocated nodes
            for (var i = 0; i < mNbAllocatedNodes - 1; i++)
            {
                var treeNode = mNodes[i];
                treeNode.nextNodeID = i + 1;
                treeNode.height = -1;
            }

            var node = mNodes[mNbAllocatedNodes - 1];
            node.nextNodeID = TreeNode.NULL_TREE_NODE;
            node.height = -1;
            mFreeNodeID = 0;
        }

        // 清除所有节点并且重置Tree
        private void reset()
        {
            mNodes.Clear();
            init();
        }

        //分配并且返回一个新的节点
        private int allocateNode()
        {
            // If there is no more allocated node to use
            if (mFreeNodeID == TreeNode.NULL_TREE_NODE)
            {
                // Allocate more nodes in the tree
                // var oldNbAllocatedNodes = mNbAllocatedNodes;
                mNbAllocatedNodes *= 2;

                // Initialize the allocated nodes
                for (var i = mNbNodes; i < mNbAllocatedNodes /* - 1*/; i++)
                {
                    var treeNode = new TreeNode
                    {
                        children = new int[2],
                        // dataInt = new int[2],
                        aabb = new AABB(),
                        nextNodeID = i + 1,
                        height = -1
                    };
                    mNodes.Add(treeNode);
                }

                var node = mNodes[mNbAllocatedNodes - 1];
                node.nextNodeID = TreeNode.NULL_TREE_NODE;
                node.height = -1;
                mFreeNodeID = mNbNodes;
            }

            // Get the next free node
            var freeNodeID = mFreeNodeID;
            var treeNode1 = mNodes[freeNodeID];
            mFreeNodeID = treeNode1.nextNodeID;
            treeNode1.parentID = TreeNode.NULL_TREE_NODE;
            treeNode1.height = 0;
            mNbNodes++;

            return freeNodeID;
        }

        // 释放一个节点
        private void releaseNode(int nodeID)
        {
            var treeNode = mNodes[nodeID];
            treeNode.children = new int[2];
            // treeNode.dataInt = new int[2];
            treeNode.aabb = new AABB();
            treeNode.nextNodeID = mFreeNodeID;
            treeNode.height = -1;
            mFreeNodeID = nodeID;
            mNbNodes--;
        }

        // 在树内部添加一个对象
        private int addObjectInternal(AABB aabb)
        {
            // 获取下一个可用节点（如果需要，分配新节点）
            var nodeID = allocateNode();
            // 创建用于树中的"胖"的AABB（通过将AABB的大小膨胀固定百分比的常量）
            var gap = new PVector3(aabb.GetExtent() * mFatAABBInflatePercentage * 0.5f);
            mNodes[nodeID].aabb.SetMin(aabb.GetMin() - gap);
            mNodes[nodeID].aabb.SetMax(aabb.GetMax() + gap);
            // 设置节点在树中的高度
            mNodes[nodeID].height = 0;

            // 将新叶子节点插入树中
            insertLeafNode(nodeID);
            // 返回节点的ID
            return nodeID;
        }

        // Remove an object from the tree
        public void removeObject(int nodeID)
        {
            removeLeafNode(nodeID);
            releaseNode(nodeID);
        }

        // 在物体移动后更新动态树。
        /// 如果移动的物体的新AABB仍然在其宽AABB内部，则不执行任何操作。否则，将相应的节点从树中移除并重新插入。
        /// 如果物体已重新插入树中，则方法返回true。
        /// 如果 "forceReInsert" 参数为true，我们强制现有的AABB采用 "newAABB" 参数的大小，即使它比 "newAABB" 大。
        /// 这可以用于在树中收缩AABB，例如如果相应的碰撞形状已经收缩。
        public bool updateObject(int nodeID, AABB newAABB, bool forceReinsert = false)
        {
            // 如果新的AABB仍然在节点的宽AABB内部
            if (!forceReinsert && mNodes[nodeID].aabb.Contains(newAABB)) return false;
            // 如果新的AABB在宽AABB之外，我们移除相应的节点
            removeLeafNode(nodeID);

            // 通过将AABB膨胀一个常数百分比的AABB大小来计算宽AABB
            mNodes[nodeID].aabb = newAABB;
            var gap = new PVector3(newAABB.GetExtent() * mFatAABBInflatePercentage * 0.5f);
            mNodes[nodeID].aabb.mMinCoordinates -= gap;
            mNodes[nodeID].aabb.mMaxCoordinates += gap;

            // 将节点重新插入树中
            insertLeafNode(nodeID);

            return true;
        }

        // 在树中插入叶节点。在动态树中插入新叶节点的过程在Ian Parberry的书 "Introduction to Game Physics with Box2D" 中有描述。
        private void insertLeafNode(int nodeID)
        {
            // 如果树为空
            if (mRootNodeID == TreeNode.NULL_TREE_NODE)
            {
                mRootNodeID = nodeID;
                mNodes[mRootNodeID].parentID = TreeNode.NULL_TREE_NODE;
                return;
            }

            // 为新节点找到最佳的兄弟节点
            var newNodeAABB = mNodes[nodeID].aabb;
            var currentNodeID = mRootNodeID;
            while (!mNodes[currentNodeID].IsLeaf())
            {
                var leftChild = mNodes[currentNodeID].children[0];
                var rightChild = mNodes[currentNodeID].children[1];

                // 计算合并后的AABB
                var volumeAABB = mNodes[currentNodeID].aabb.GetVolume();
                var mergedAABBs = new AABB();
                mergedAABBs.MergeTwoAABBs(mNodes[currentNodeID].aabb, newNodeAABB);
                var mergedVolume = mergedAABBs.GetVolume();

                // 计算使当前节点成为新节点的兄弟节点的代价
                var costS = 2.0f * mergedVolume;

                // 计算将新节点进一步推入树中的最小成本（继承成本）
                var costI = 2.0f * (mergedVolume - volumeAABB);

                // 计算向左子节点下降的成本
                float costLeft;
                var currentAndLeftAABB = new AABB();
                currentAndLeftAABB.MergeTwoAABBs(newNodeAABB, mNodes[leftChild].aabb);
                if (mNodes[leftChild].IsLeaf())
                {
                    // 如果左子节点是叶子节点
                    costLeft = currentAndLeftAABB.GetVolume() + costI;
                }
                else
                {
                    var leftChildVolume = mNodes[leftChild].aabb.GetVolume();
                    costLeft = costI + currentAndLeftAABB.GetVolume() - leftChildVolume;
                }

                // 计算向右子节点下降的成本
                float costRight;
                var currentAndRightAABB = new AABB();
                currentAndRightAABB.MergeTwoAABBs(newNodeAABB, mNodes[rightChild].aabb);
                if (mNodes[rightChild].IsLeaf())
                {
                    // 如果右子节点是叶子节点
                    costRight = currentAndRightAABB.GetVolume() + costI;
                }
                else
                {
                    var rightChildVolume = mNodes[rightChild].aabb.GetVolume();
                    costRight = costI + currentAndRightAABB.GetVolume() - rightChildVolume;
                }

                // 如果使当前节点成为新节点的兄弟节点的成本小于向左或向右子节点下降的成本
                if (costS < costLeft && costS < costRight) break;

                // 更经济的方式是向当前节点的子节点下降，选择最佳的子节点。
                if (costLeft < costRight)
                    currentNodeID = leftChild;
                else
                    currentNodeID = rightChild;
            }

            var siblingNode = currentNodeID;

            // 为新节点和兄弟节点创建一个新的父节点。
            var oldParentNode = mNodes[siblingNode].parentID;
            var newParentNode = allocateNode();
            mNodes[newParentNode].parentID = oldParentNode;
            mNodes[newParentNode].aabb.MergeTwoAABBs(mNodes[siblingNode].aabb, newNodeAABB);
            mNodes[newParentNode].height = (short)(mNodes[siblingNode].height + 1);

            // 如果兄弟节点不是根节点。
            if (oldParentNode != TreeNode.NULL_TREE_NODE)
            {
                if (mNodes[oldParentNode].children[0] == siblingNode)
                    mNodes[oldParentNode].children[0] = newParentNode;
                else
                    mNodes[oldParentNode].children[1] = newParentNode;
                mNodes[newParentNode].children[0] = siblingNode;
                mNodes[newParentNode].children[1] = nodeID;
                mNodes[siblingNode].parentID = newParentNode;
                mNodes[nodeID].parentID = newParentNode;
            }
            else
            {
                // 如果兄弟节点是根节点。
                mNodes[newParentNode].children[0] = siblingNode;
                mNodes[newParentNode].children[1] = nodeID;
                mNodes[siblingNode].parentID = newParentNode;
                mNodes[nodeID].parentID = newParentNode;
                mRootNodeID = newParentNode;
            }

            // 向树的上层移动，以更新已经改变的AABB。
            currentNodeID = mNodes[nodeID].parentID;
            while (currentNodeID != TreeNode.NULL_TREE_NODE)
            {
                // 平衡当前节点的子树（sub-tree），如果它没有平衡。
                currentNodeID = balanceSubTreeAtNode(currentNodeID);

                var leftChild = mNodes[currentNodeID].children[0];
                var rightChild = mNodes[currentNodeID].children[1];

                // 重新计算树中节点的高度。
                mNodes[currentNodeID].height =
                    (short)(PMath.Max(mNodes[leftChild].height, mNodes[rightChild].height) + 1);

                // 重新计算节点的AABB。
                mNodes[currentNodeID].aabb.MergeTwoAABBs(mNodes[leftChild].aabb, mNodes[rightChild].aabb);

                currentNodeID = mNodes[currentNodeID].parentID;
            }

        }

        // 从树中移除叶子节点
        private void removeLeafNode(int nodeID)
        {
            // 如果我们正在移除根节点（在这种情况下，根节点也是叶子节点）
            if (mRootNodeID == nodeID)
            {
                mRootNodeID = TreeNode.NULL_TREE_NODE;
                return;
            }

            var parentNodeID = mNodes[nodeID].parentID;
            var grandParentNodeID = mNodes[parentNodeID].parentID;
            int siblingNodeID;
            if (mNodes[parentNodeID].children[0] == nodeID)
                siblingNodeID = mNodes[parentNodeID].children[1];
            else
                siblingNodeID = mNodes[parentNodeID].children[0];

            // 如果要移除的节点的父节点不是根节点
            if (grandParentNodeID != TreeNode.NULL_TREE_NODE)
            {
                // 销毁父节点
                if (mNodes[grandParentNodeID].children[0] == parentNodeID)
                {
                    mNodes[grandParentNodeID].children[0] = siblingNodeID;
                }
                else
                {
                    mNodes[grandParentNodeID].children[1] = siblingNodeID;
                }

                mNodes[siblingNodeID].parentID = grandParentNodeID;
                releaseNode(parentNodeID);

                // 现在，我们需要重新计算沿着路径回到根的节点的AABB，并确保树仍然是平衡的
                var currentNodeID = grandParentNodeID;
                while (currentNodeID != TreeNode.NULL_TREE_NODE)
                {
                    // 如果需要，平衡当前子树
                    currentNodeID = balanceSubTreeAtNode(currentNodeID);

                    // 获取当前节点的两个子节点
                    var leftChildID = mNodes[currentNodeID].children[0];
                    var rightChildID = mNodes[currentNodeID].children[1];

                    // 重新计算当前节点的AABB和高度
                    mNodes[currentNodeID].aabb.MergeTwoAABBs(mNodes[leftChildID].aabb,
                        mNodes[rightChildID].aabb);
                    mNodes[currentNodeID].height = (short)(PMath.Max(mNodes[leftChildID].height,
                        mNodes[rightChildID].height) + 1);


                    currentNodeID = mNodes[currentNodeID].parentID;
                }
            }
            else
            {
                // 如果要移除的节点的父节点是根节点

                // 兄弟节点成为新的根节点
                mRootNodeID = siblingNodeID;
                mNodes[siblingNodeID].parentID = TreeNode.NULL_TREE_NODE;
                releaseNode(parentNodeID);
            }
        }

        
        // Ray casting method
        public void raycast(PRay ray, DynamicAABBTreeRaycastCallback callback)
        {
            // 获取射线的最大分数
            var maxFraction = ray.maxFraction;

            // 计算射线方向的倒数
            var rayDirection = ray.point2 - ray.point1;
            var rayDirectionInverse = new PVector3(1.0f / rayDirection.x, 1.0f / rayDirection.y, 1.0f / rayDirection.z);

            var stack = new Stack<int>(128);
            stack.Push(mRootNodeID);

            // 从根节点开始遍历树，寻找与射线AABB相交的碰撞器
            while (stack.Count > 0)
            {
                // 获取栈中的下一个节点
                var nodeID = stack.Pop();

                // 如果是空节点，则跳过
                if (nodeID == TreeNode.NULL_TREE_NODE) continue;

                // 获取相应的节点
                var node = mNodes[nodeID];

                // 测试射线是否与当前节点AABB相交
                if (!node.aabb.testRayIntersect(ray.point1, rayDirectionInverse, maxFraction)) continue;

                // 如果节点是树的叶子节点
                if (node.IsLeaf())
                {
                    // 创建一个临时射线，用于再次进行宽相形状的射线检测
                    var rayTemp = new PRay(ray.point1, ray.point2, maxFraction);

                    // 调用回调函数，它将再次对宽相形状进行射线检测
                    var hitFraction = callback.raycastBroadPhaseShape(nodeID, rayTemp);

                    // 如果用户返回的hitFraction为零，表示射线检测应该在这里停止
                    if (hitFraction == 0.0f) return;

                    // 如果用户返回的是正的分数
                    if (hitFraction > 0.0f)
                        // 更新maxFraction值和射线AABB，使用新的最大分数
                        if (hitFraction < maxFraction)
                            maxFraction = hitFraction;
                    // 如果用户返回的是负的分数，我们继续射线检测，就好像碰撞器不存在
                }
                else // 如果节点有子节点
                {
                    // 将其子节点推入待探索的节点栈中
                    stack.Push(node.children[0]);
                    stack.Push(node.children[1]);
                }
            }
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AABB getRootAABB()
        {
            return getFatAABB(mRootNodeID);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AABB getFatAABB(int nodeID)
        {
            return mNodes[nodeID].aabb;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AABB GetAABB(int index)
        {
            return mNodes[index].aabb;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public int addObject(AABB aabb, int data1, int data2)
        // {
        //     var nodeId = addObjectInternal(aabb);
        //     mNodes[nodeId].dataInt[0] = data1;
        //     mNodes[nodeId].dataInt[1] = data2;
        //     return nodeId;
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int addObject(AABB aabb, Collider data)
        {
            var nodeId = addObjectInternal(aabb);
            mNodes[nodeId].dataPointer = data;
            return nodeId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Collider getNodeDataPointer(int nodeID)
        {
            return mNodes[nodeID].dataPointer;
        }
        
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public int[] getNodeDataInt(int nodeID)
        // {
        //     return mNodes[nodeID].dataInt;
        // }



    }
}