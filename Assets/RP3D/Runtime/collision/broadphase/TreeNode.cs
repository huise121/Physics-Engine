//在 DynamicAABBTree 中，TreeNode 是表示树节点的数据结构。
//每个节点包含了 AABB（Axis-Aligned Bounding Box）范围信息以及指向父节点、左子节点和右子节点的索引。
//这种节点结构使得 DynamicAABBTree 能够有效地组织和管理空间中的对象，并支持对它们进行快速的查询和更新操作。

namespace RP3D
{
    public class TreeNode
    {
        /// 空树节点常量
        public const int NULL_TREE_NODE = -1;
        
        public int nextNodeID;
        // 节点要么在树中（有父节点），要么在自由节点数组中（有下一个节点）
        public int parentID;
        
        /// 节点在树中的高度
        public int height;
        
        /// 与节点对应的胖轴对齐边界框（AABB）
        public AABB aabb;
        
        public int[] children;
        
        // public int[] dataInt;
        
        public Collider dataPointer;
        
        /// 如果节点是树的叶子节点，则返回true
        public bool IsLeaf()
        {
            return height == 0;
        }
    }

}