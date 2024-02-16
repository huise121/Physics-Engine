namespace RP3D
{
    /// <summary>
    /// 联系点信息结构体
    /// </summary>
    public struct ContactPointInfo
    {
        // -------------------- 属性 -------------------- //

        /// <summary>
        /// 碰撞联系处的世界空间中的归一化法线向量
        /// </summary>
        public PVector3 normal;

        /// <summary>
        /// 刚体1上的联系点，位于刚体1的局部空间中
        /// </summary>
        public PVector3 localPoint1;

        /// <summary>
        /// 刚体2上的联系点，位于刚体2的局部空间中
        /// </summary>
        public PVector3 localPoint2;

        /// <summary>
        /// 碰撞的渗透深度
        /// </summary>
        public float penetrationDepth;
    }
}