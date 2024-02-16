// "Structure Islands" 类用于管理每帧中的物体岛（islands of bodies）。
// 一个物体岛表示一组孤立的、由一些约束（如接触或关节）连接在一起的唤醒状态的物体。
//
// 以下是这个类的一些特性和功能：
//
// 物体岛（Islands of Bodies）：
// 每个物体岛是一个孤立的物体群，这些物体通过一些约束（如接触或关节）连接在一起。
// 这些物体共同组成了一个物理系统，它们之间的相互作用会影响彼此的运动。
//
// 唤醒状态（Awake Bodies）：
// 物体岛中的物体通常是唤醒状态的，即它们可以根据外部力和约束产生运动。
// 与之相对的是休眠状态的物体，它们不参与物理模拟，从而节省计算资源。
//
// 约束（Constraints）：
// 物体岛中的物体通过一些约束连接在一起。
// 这些约束可以是接触约束（Contacts）或关节约束（Joints），它们会影响物体之间的相互作用，并在碰撞解决和运动仿真中起着重要作用。
//
// 管理功能（Management Functionality）：
// "Structure Islands" 类提供了管理物体岛的功能，包括添加和删除物体、查找物体所属的岛等。
// 这些功能可以帮助物理引擎有效地管理物体之间的相互作用，从而实现准确的物理仿真。
//
// 通过"Structure Islands" 类，提供了一种有效的方式来组织和管理物体之间的相互作用，以及管理物体的运动和碰撞响应。
// 这对于实现高性能和稳定的物理仿真是非常重要的。


using System.Collections.Generic;

namespace RP3D
{
    public class Islands
    {
        #region private:

        // /// 记录了上一帧中的岛的数量，
        // private int mNbIslandsPreviousFrame;
        //
        // /// 记录了上一帧中所有岛中物体的数量
        // private int mNbBodyEntitiesPreviousFrame;

        /// 记录了上一帧中单个岛中最大的物体数量。
        private int mNbMaxBodiesInIslandPreviousFrame;

        /// 记录了当前帧中单个岛中最大的物体数量。
        private int mNbMaxBodiesInIslandCurrentFrame;

        #endregion

        #region public:

        /// 用于存储每个岛中第一个接触 manifold（接触点集合）在接触 manifold 数组中的索引；
        public List<int> contactManifoldsIndices;

        /// 用于存储每个岛中接触 manifold 的数量；
        public List<int> nbContactManifolds;

        /// 用于存储所有岛中物体的实体（entities）的数组；
        public List<Entity> bodyEntities;

        /// 用于存储每个岛中物体实体在 bodyEntities 数组中的起始索引；
        public List<int> startBodyEntitiesIndex;

        /// 用于存储每个岛中物体的数量。
        public List<int> nbBodiesInIsland;
        
        #endregion

        /// Constructor
        public Islands()
        {
            // mNbIslandsPreviousFrame = 16;
            // mNbBodyEntitiesPreviousFrame = 32;
            mNbMaxBodiesInIslandPreviousFrame = 0;
            mNbMaxBodiesInIslandCurrentFrame = 0;

            contactManifoldsIndices = new List<int>();
            nbContactManifolds = new List<int>();
            bodyEntities = new List<Entity>();
            startBodyEntitiesIndex = new List<int>();
            nbBodiesInIsland = new List<int>();
        }


        /// Return the number of islands
        public int getNbIslands()
        {
            return contactManifoldsIndices.Count;
        }

        /// Add an island and return its index
        public int addIsland(int contactManifoldStartIndex)
        {
            var islandIndex = contactManifoldsIndices.Count;

            contactManifoldsIndices.Add(contactManifoldStartIndex);
            nbContactManifolds.Add(0);
            startBodyEntitiesIndex.Add(bodyEntities.Count);
            nbBodiesInIsland.Add(0);

            if (islandIndex > 0 && nbBodiesInIsland[islandIndex - 1] > mNbMaxBodiesInIslandCurrentFrame)
                mNbMaxBodiesInIslandCurrentFrame = nbBodiesInIsland[islandIndex - 1];

            return islandIndex;
        }

        public void addBodyToIsland(Entity bodyEntity)
        {
            var islandIndex = contactManifoldsIndices.Count;

            bodyEntities.Add(bodyEntity);
            nbBodiesInIsland[islandIndex - 1]++;
        }


        /// Clear all the islands
        public void clear()
        {
            var nbIslands = nbContactManifolds.Count;

            if (nbIslands > 0 && nbBodiesInIsland[nbIslands - 1] > mNbMaxBodiesInIslandCurrentFrame)
                mNbMaxBodiesInIslandCurrentFrame = nbBodiesInIsland[nbIslands - 1];

            mNbMaxBodiesInIslandPreviousFrame = mNbMaxBodiesInIslandCurrentFrame;
            //mNbIslandsPreviousFrame = nbIslands;
            //mNbBodyEntitiesPreviousFrame = bodyEntities.Count;
            mNbMaxBodiesInIslandCurrentFrame = 0;
            contactManifoldsIndices.Clear();
            nbContactManifolds.Clear();
            bodyEntities.Clear();
            startBodyEntitiesIndex.Clear();
            nbBodiesInIsland.Clear();
        }

        // public int getNbMaxBodiesInIslandPreviousFrame()
        // {
        //     return mNbMaxBodiesInIslandPreviousFrame;
        // }
    }
}