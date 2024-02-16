using System.Collections.Generic;

namespace RP3D
{
    public partial class DynamicAABBTree
    {
        /// 获取要在宽相位重叠中测试的形状数组，并返回重叠形状的成对数组。
        public void ReportAllShapesOverlappingWithShapes(ref List<int> nodesToTest, int startIndex, int endIndex,
            ref List<Pair<int, int>> outOverlappingNodes)
        {
            // 创建一个用于访问节点的堆栈
            var stack = new Stack<int>(64);

            // 对每个要测试重叠的形状
            for (var i = startIndex; i < endIndex; i++)
            {
                stack.Push(mRootNodeID);

                var shapeAABB = getFatAABB(nodesToTest[i]);

                // 当仍有节点需要访问时
                while (stack.Count > 0)
                {
                    // 获取下一个要访问的节点ID
                    var nodeIDToVisit = stack.Pop();

                    // 如果是空节点，则跳过
                    if (nodeIDToVisit == TreeNode.NULL_TREE_NODE) continue;

                    // 获取对应的节点
                    var nodeToVisit = mNodes[nodeIDToVisit];

                    // 如果参数中的AABB与要访问的节点的AABB重叠
                    if (shapeAABB.TestCollision(nodeToVisit.aabb))
                    {
                        // 如果节点是叶子节点
                        if (nodeToVisit.IsLeaf())
                        {
                            // 将节点添加到重叠节点数组中
                            outOverlappingNodes.Add(new Pair<int, int>(nodesToTest[i], nodeIDToVisit));
                        }
                        else
                        {
                            // 如果节点不是叶子节点

                            // 我们需要访问它的子节点
                            stack.Push(nodeToVisit.children[0]);
                            stack.Push(nodeToVisit.children[1]);
                        }
                    }
                }

                stack.Clear();
            }
        }

        
        
        
        /// <summary>
        /// 报告所有与给定AABB重叠的形状
        /// </summary>
        /// <param name="aabb">要检查重叠的AABB</param>
        /// <param name="overlappingNodes">存储重叠形状节点的列表</param>
        public void ReportAllShapesOverlappingWithAABB(AABB aabb,ref List<int> overlappingNodes)
        {
            // 创建一个用于访问节点的堆栈
            var stack = new Stack<int>(64);
            stack.Push(mRootNodeID);

            // 当仍然有待访问的节点时
            while (stack.Count > 0)
            {
                // 获取下一个要访问的节点ID
                var nodeIDToVisit = stack.Pop();

                // 如果是空节点，则跳过
                if (nodeIDToVisit == TreeNode.NULL_TREE_NODE) continue;

                // 获取相应的节点
                var nodeToVisit = mNodes[nodeIDToVisit];

                // 如果参数中的AABB与要访问的节点的AABB重叠
                if (aabb.TestCollision(nodeToVisit.aabb))
                {
                    // 如果节点是叶子节点
                    if (nodeToVisit.IsLeaf())
                    {
                        // 通知广义相位有一个新的潜在重叠对
                        overlappingNodes.Add(nodeIDToVisit);
                    }
                    else
                    {
                        // 如果节点不是叶子节点
                        // 我们需要访问它的子节点
                        stack.Push(nodeToVisit.children[0]);
                        stack.Push(nodeToVisit.children[1]);
                    }
                }
            }
        }

    }
}