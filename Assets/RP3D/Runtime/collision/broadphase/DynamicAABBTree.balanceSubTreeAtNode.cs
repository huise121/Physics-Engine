namespace RP3D
{
    public partial class DynamicAABBTree
    {
        // 使用左旋或右旋平衡给定节点的子树。
        // 旋转方案在Ian Parberry的书 "Introduction to Game Physics with Box2D" 中有描述。
        //
        // 实现了在动态AABB树中平衡一个节点的子树。以下是它的解释：
        // 首先，通过给定的节点ID获取该节点（nodeA）。
        // 如果节点是叶子节点或者节点A的子树高度小于2，则不执行任何旋转操作，直接返回节点ID。
        // 获取节点A的两个子节点的ID：nodeBID和nodeCID，并获取它们对应的节点。
        // 计算左右子树的高度差（平衡因子）。
        // 如果右节点C的高度比左节点B的高度高2个单位以上（balanceFactor > 1），则执行右旋转操作。
        // 获取节点C的两个子节点的ID：nodeFID和nodeGID，并获取它们对应的节点。
        // 将节点C作为新的根节点，更新其父节点、子节点以及子节点的父节点。
        // 根据节点F和节点G的高度，更新节点A和节点C的子节点、AABB包围盒以及高度。
        // 如果左节点B的高度比右节点C的高度高2个单位以上（balanceFactor < -1），则执行左旋转操作。
        // 获取节点B的两个子节点的ID：nodeFID和nodeGID，并获取它们对应的节点。
        // 将节点B作为新的根节点，更新其父节点、子节点以及子节点的父节点。
        // 根据节点F和节点G的高度，更新节点A和节点B的子节点、AABB包围盒以及高度。
        // 如果子树是平衡的（balanceFactor 在 -1 和 1 之间），则直接返回当前节点ID作为根节点。
        // 这段代码通过旋转操作保持了动态AABB树的平衡性，以确保查询和更新的效率。
        private int balanceSubTreeAtNode(int nodeID)
        {
            var nodeA = mNodes[nodeID];

            // 如果节点是叶子节点或者节点A的子树高度小于2
            if (nodeA.IsLeaf() || nodeA.height < 2)
                // 不执行任何旋转操作，直接返回节点ID
                return nodeID;

            // 获取两个子节点
            var nodeBID = nodeA.children[0];
            var nodeCID = nodeA.children[1];
            var nodeB = mNodes[nodeBID];
            var nodeC = mNodes[nodeCID];

            // 计算左右子树的高度差
            var balanceFactor = nodeC.height - nodeB.height;

            // 如果右节点C的高度比左节点B的高度高2个单位以上
            if (balanceFactor > 1)
            {
                var nodeFID = nodeC.children[0];
                var nodeGID = nodeC.children[1];
                var nodeF = mNodes[nodeFID];
                var nodeG = mNodes[nodeGID];

                nodeC.children[0] = nodeID;
                nodeC.parentID = nodeA.parentID;
                nodeA.parentID = nodeCID;

                // 更新父节点的子节点指针
                if (nodeC.parentID != TreeNode.NULL_TREE_NODE)
                {
                    if (mNodes[nodeC.parentID].children[0] == nodeID)
                    {
                        mNodes[nodeC.parentID].children[0] = nodeCID;
                    }
                    else
                    {
                        mNodes[nodeC.parentID].children[1] = nodeCID;
                    }
                }
                else
                {
                    mRootNodeID = nodeCID;
                }

                // 根据节点F和节点G的高度，更新节点A和节点C的子节点、AABB包围盒以及高度
                if (nodeF.height > nodeG.height)
                {
                    // 更新节点C的子节点和节点A的子节点
                    nodeC.children[1] = nodeFID;
                    nodeA.children[1] = nodeGID;
                    nodeG.parentID = nodeID;

                    // 重新计算节点A和C的AABB和高度
                    nodeA.aabb.MergeTwoAABBs(nodeB.aabb, nodeG.aabb);
                    nodeC.aabb.MergeTwoAABBs(nodeA.aabb, nodeF.aabb);

                    nodeA.height = PMath.Max(nodeB.height, nodeG.height) + 1;
                    nodeC.height = PMath.Max(nodeA.height, nodeF.height) + 1;
                }
                else
                {
                    // 更新节点C的子节点和节点A的子节点
                    nodeC.children[1] = nodeGID;
                    nodeA.children[1] = nodeFID;
                    nodeF.parentID = nodeID;

                    // 重新计算节点A和C的AABB和高度
                    nodeA.aabb.MergeTwoAABBs(nodeB.aabb, nodeF.aabb);
                    nodeC.aabb.MergeTwoAABBs(nodeA.aabb, nodeG.aabb);

                    nodeA.height = PMath.Max(nodeB.height, nodeF.height) + 1;
                    nodeC.height = PMath.Max(nodeA.height, nodeG.height) + 1;
                }

                // 返回子树的新根节点
                return nodeCID;
            }

            // 如果左节点B的高度比右节点C的高度高2个单位以上
            if (balanceFactor < -1)
            {
                var nodeFID = nodeB.children[0];
                var nodeGID = nodeB.children[1];
                var nodeF = mNodes[nodeFID];
                var nodeG = mNodes[nodeGID];

                nodeB.children[0] = nodeID;
                nodeB.parentID = nodeA.parentID;
                nodeA.parentID = nodeBID;

                // 更新父节点的子节点指针
                if (nodeB.parentID != TreeNode.NULL_TREE_NODE)
                {
                    if (mNodes[nodeB.parentID].children[0] == nodeID)
                    {
                        mNodes[nodeB.parentID].children[0] = nodeBID;
                    }
                    else
                    {
                        mNodes[nodeB.parentID].children[1] = nodeBID;
                    }
                }
                else
                {
                    mRootNodeID = nodeBID;
                }

                // 根据节点F和节点G的高度，更新节点A和节点B的子节点、AABB包围盒以及高度
                if (nodeF.height > nodeG.height)
                {
                    // 更新节点B的子节点和节点A的子节点
                    nodeB.children[1] = nodeFID;
                    nodeA.children[0] = nodeGID;
                    nodeG.parentID = nodeID;

                    // 重新计算节点A和B的AABB和高度
                    nodeA.aabb.MergeTwoAABBs(nodeC.aabb, nodeG.aabb);
                    nodeB.aabb.MergeTwoAABBs(nodeA.aabb, nodeF.aabb);

                    nodeA.height = (short)(PMath.Max(nodeC.height, nodeG.height) + 1);
                    nodeB.height = (short)(PMath.Max(nodeA.height, nodeF.height) + 1);
                }
                else
                {
                    // 更新节点B的子节点和节点A的子节点
                    nodeB.children[1] = nodeGID;
                    nodeA.children[0] = nodeFID;
                    nodeF.parentID = nodeID;

                    // 重新计算节点A和B的AABB和高度
                    nodeA.aabb.MergeTwoAABBs(nodeC.aabb, nodeF.aabb);
                    nodeB.aabb.MergeTwoAABBs(nodeA.aabb, nodeG.aabb);

                    nodeA.height = PMath.Max(nodeC.height, nodeF.height) + 1;
                    nodeB.height = PMath.Max(nodeA.height, nodeG.height) + 1;
                }

                // 返回子树的新根节点
                return nodeBID;
            }

            // 如果子树是平衡的，返回当前根节点
            return nodeID;
        }
    }
}