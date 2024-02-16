/// <summary>
/// ContactManifold 表示两个碰撞体之间的接触点集合，也就是碰撞点的集合。
/// 这个数据结构用于存储在碰撞检测过程中发现的接触点，以便在后续的碰撞解析过程中使用。
/// ContactManifold 通常包含以下属性和方法：
/// 碰撞体信息：包括与碰撞相关的两个碰撞体的信息，如碰撞体的身份标识符、碰撞体的材质属性等。
/// 接触点信息：存储了所有的接触点，包括接触点的位置、法线、深度等信息。
/// 接触点的添加和删除：用于向接触点集合中添加新的接触点，或者从集合中移除不再有效的接触点的方法。
/// 碰撞信息的更新：在碰撞检测和碰撞解析过程中，可能需要更新接触点的信息，例如接触点的位置、法线、深度等。
/// 碰撞信息的查询：提供了查询接触点集合中接触点数量、接触点的位置、法线等信息的方法。
/// 通过使用 ContactManifold，能够有效地存储和管理碰撞检测过程中发现的接触点信息，从而在后续的碰撞解析过程中准确地模拟物体之间的碰撞行为。
/// </summary>

namespace RP3D
{
    /// <summary>
    ///     表示两个物体之间一组接触点的类，这些接触点都具有相似的接触法线方向。
    ///     通常，当两个凸形状相互接触时，存在一个接触点组。然而，当凸形状与凹形状碰撞时，
    ///     可能会存在多个接触点组，这些组具有不同的法线方向。
    ///     接触点组的实现方式是为了在帧之间缓存接触点，以提高稳定性（对接触求解器的热启动）。
    /// </summary>
    public class ContactManifold
    {
        /// 减少接触点组中的最大接触点数
        public const int MAX_CONTACT_POINTS_IN_MANIFOLD = 4;

        /// 第一个物体的实体
        public Entity bodyEntity1;
        /// 第二个物体的实体
        public Entity bodyEntity2;
        /// 接触中的第一个碰撞体的实体
        public Entity colliderEntity1;
        /// 接触中的第二个碰撞体的实体
        public Entity colliderEntity2;

        // -------------------- 属性 -------------------- //

        /// 接触点组中第一个接触点在接触点数组中的索引
        public int contactPointsIndex;
        /// 第一个摩擦约束的累积冲量
        public float frictionImpulse1;
        /// 第二个摩擦约束的累积冲量
        public float frictionImpulse2;
        /// 扭摩擦约束的累积冲量
        public float frictionTwistImpulse;
        /// 接触点组的第一个摩擦向量
        public PVector3 frictionVector1;
        /// 接触点组的第二个摩擦向量
        public PVector3 frictionVector2;
        /// 如果接触点组已经添加到岛屿中，则为true
        public bool isAlreadyInIsland;
        /// 缓存中的接触点数量
        public int nbContactPoints;

        // -------------------- 方法 -------------------- //

        public ContactManifold(Entity bodyEntity1, Entity bodyEntity2, Entity colliderEntity1, Entity colliderEntity2,
            int contactPointsIndex, int nbContactPoints)
        {
            this.contactPointsIndex = (contactPointsIndex);
            this.bodyEntity1 = bodyEntity1;
            this.bodyEntity2 = bodyEntity2;
            this.colliderEntity1 = colliderEntity1;
            this.colliderEntity2 = colliderEntity2;
            this.nbContactPoints = nbContactPoints;
            this.frictionImpulse1 =0;
            this.frictionImpulse2 = 0;
            this.frictionTwistImpulse = 0;
            this.isAlreadyInIsland = false;
        }

    }
}