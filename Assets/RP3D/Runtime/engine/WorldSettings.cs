namespace RP3D
{
    /// <summary>
    /// 世界设置结构体
    /// </summary>
    public struct WorldSettings
    {
        /// <summary>
        /// 世界的名称
        /// </summary>
        public string worldName;

        /// <summary>
        /// 世界的重力矢量（单位：牛顿）
        /// </summary>
        public PVector3 gravity;

        /// <summary>
        /// 两个接触点被视为有效持续接触的距离阈值（单位：米）
        /// </summary>
        public float persistentContactDistanceThreshold;

        /// <summary>
        /// 刚体的默认摩擦系数
        /// </summary>
        public float defaultFrictionCoefficient;

        /// <summary>
        /// 刚体的默认弹性系数
        /// </summary>
        public float defaultBounciness;

        /// <summary>
        /// 接触速度弹性的速度阈值
        /// </summary>
        public float restitutionVelocityThreshold;

        /// <summary>
        /// 如果启用了睡眠技术，则为true
        /// </summary>
        public bool isSleepingEnabled;

        /// <summary>
        /// 通过顺序脉冲技术解决速度约束时的默认迭代次数
        /// </summary>
        public ushort defaultVelocitySolverNbIterations;

        /// <summary>
        /// 通过顺序脉冲技术解决位置约束时的默认迭代次数
        /// </summary>
        public ushort defaultPositionSolverNbIterations;

        /// <summary>
        /// 被认为处于休眠状态的物体保持静止的时间（单位：秒）
        /// </summary>
        public float defaultTimeBeforeSleep;

        /// <summary>
        /// 具有线性速度小于睡眠线性速度（单位：米/秒）的物体可能进入睡眠模式
        /// </summary>
        public float defaultSleepLinearVelocity;

        /// <summary>
        /// 具有角速度小于睡眠角速度（单位：弧度/秒）的物体可能进入睡眠模式
        /// </summary>
        public float defaultSleepAngularVelocity;

        /// <summary>
        /// 用于测试两个接触曲面是否相似（相同的接触法线），以便合并它们。如果两个曲面法线之间的角度的余弦大于下面的值，
        /// 则认为曲面相似。
        /// </summary>
        public float cosAngleSimilarContactManifold;
    }
}