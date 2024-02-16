namespace RP3D
{
    public partial class DynamicAABBTree
    {
        //**********************************NDEBUG start************************************

        // Check if the tree structure is valid (for debugging purpose)
        private void check()
        {
            // Recursively check each node
            checkNode(mRootNodeID);

            var nbFreeNodes = 0;
            var freeNodeID = mFreeNodeID;

            // Check the free nodes
            while (freeNodeID != TreeNode.NULL_TREE_NODE)
            {
                freeNodeID = mNodes[freeNodeID].nextNodeID;
                nbFreeNodes++;
            }
        }

        // Check if the node structure is valid (for debugging purpose)
        private void checkNode(int nodeID)
        {
            if (nodeID == TreeNode.NULL_TREE_NODE) return;

            // Get the children nodes
            var pNode = mNodes[nodeID];
            var leftChild = pNode.children[0];
            var rightChild = pNode.children[1];

            // If the current node is a leaf
            if (!pNode.IsLeaf())
            {
                // Check the height of node
                var height = 1 + PMath.Max(mNodes[leftChild].height, mNodes[rightChild].height);

                // Check the AABB of the node
                var aabb = new AABB();
                aabb.MergeTwoAABBs(mNodes[leftChild].aabb, mNodes[rightChild].aabb);

                // Recursively check the children nodes
                checkNode(leftChild);
                checkNode(rightChild);
            }
        }

        // Compute the height of the tree
        private int computeHeight()
        {
            return computeHeight(mRootNodeID);
        }

        // Compute the height of a given node in the tree
        private int computeHeight(int nodeID)
        {
            var node = mNodes[nodeID];

            // If the node is a leaf, its height is zero
            if (node.IsLeaf()) return 0;

            // Compute the height of the left and right sub-tree
            var leftHeight = computeHeight(node.children[0]);
            var rightHeight = computeHeight(node.children[1]);

            // Return the height of the node
            return 1 + PMath.Max(leftHeight, rightHeight);
        }
    }
}