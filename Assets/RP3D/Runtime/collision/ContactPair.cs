//在物理引擎中，ContactPair 通常表示两个物体之间的碰撞或接触关系。
//当两个物体发生碰撞时，物理引擎会生成一个 ContactPair 对象，其中包含有关碰撞的信息，如碰撞点、碰撞法线、碰撞深度等。


namespace RP3D
{
    
    public enum EventType
    {
        ContactStart,
        ContactStay,
        ContactExit
    }
    
    // 结构体 ContactPair
    /// <summary>
    /// 该结构体表示在狭义阶段期间接触的一对形状。
    /// </summary>
    public class ContactPair
    {
        public const int NB_MAX_CONTACT_MANIFOLDS = 3;
        public const int NB_MAX_POTENTIAL_CONTACT_MANIFOLDS = 4 * NB_MAX_CONTACT_MANIFOLDS;
        // -------------------- 属性 -------------------- //

        /// 重叠对的 Id
        public int PairId;
        /// 潜在接触曲面的数量
        public byte NbPotentialContactManifolds;
        /// 潜在接触曲面的索引
        public int[] PotentialContactManifoldsIndices;
        /// 第一个接触体的实体
        public Entity Body1Entity;
        /// 第二个接触体的实体
        public Entity Body2Entity;
        /// 接触中第一个碰撞体的实体
        public Entity Collider1Entity;
        /// 接触中第二个碰撞体的实体
        public Entity Collider2Entity;
        /// 如果接触曲面已经在岛屿中，则为 true
        public bool IsAlreadyInIsland;
        /// 在对的数组中的接触对的索引
        public int ContactPairIndex;
        /// 在数组中的第一个接触曲面的索引
        public int ContactManifoldsIndex;
        /// 接触曲面的数量
        public int NbContactManifolds;
        /// 在接触点数组中的第一个接触点的索引
        public int ContactPointsIndex;
        /// 在接触对的所有曲面中的总接触点数量
        public int NbTotalContactPoints;
        /// 如果对的碰撞体在上一帧已经发生碰撞，则为 true
        public bool CollidingInPreviousFrame;
        /// 如果两个涉及的碰撞体中有一个是触发器，则为 true
        public bool IsTrigger;
        

        // -------------------- 方法 -------------------- //
        /// 构造函数
        public ContactPair(int pairId, Entity body1Entity, Entity body2Entity, Entity collider1Entity,
                           Entity collider2Entity, int contactPairIndex, bool collidingInPreviousFrame, bool isTrigger)
        {
            PairId = pairId;
            NbPotentialContactManifolds = 0;
            PotentialContactManifoldsIndices = new int[NB_MAX_POTENTIAL_CONTACT_MANIFOLDS];
            Body1Entity = body1Entity;
            Body2Entity = body2Entity;
            Collider1Entity = collider1Entity;
            Collider2Entity = collider2Entity;
            IsAlreadyInIsland = false;
            ContactPairIndex = contactPairIndex;
            ContactManifoldsIndex = 0;
            NbContactManifolds = 0;
            ContactPointsIndex = 0;
            NbTotalContactPoints = 0;
            CollidingInPreviousFrame = collidingInPreviousFrame;
            IsTrigger = isTrigger;
        }
        
        // 从数组中移除给定索引处的潜在接触曲面
        public void RemovePotentialManifoldAtIndex(int index)
        {
            if (index < NbPotentialContactManifolds)
            {
                PotentialContactManifoldsIndices[index] = PotentialContactManifoldsIndices[NbPotentialContactManifolds - 1];
                NbPotentialContactManifolds--;
            }
        }
    }
}
