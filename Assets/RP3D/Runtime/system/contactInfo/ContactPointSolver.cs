namespace RP3D
{
    /**
     * 用于存储关于接触点的所有信息的接触点解算器内部数据结构。
     */
    public class ContactPointSolver
    {
        /// 外部接触点指针
        public ContactPoint externalContact;
        /// 接触点的法线向量
        public PVector3 normal;
        /// 从刚体 1 中心到接触点的向量
        public PVector3 r1;
        /// 从刚体 2 中心到接触点的向量
        public PVector3 r2;
        /// 侵入深度
        public float penetrationDepth;
        /// 速度恢复偏差
        public float restitutionBias;
        /// 累积的法线冲量
        public float penetrationImpulse;
        /// 用于侵入校正的累积分裂冲量
        public float penetrationSplitImpulse;
        /// 侵入质量的逆矩阵 K
        public float inversePenetrationMass;
        /// r1 与接触法线的叉乘
        public PVector3 i1TimesR1CrossN;
        /// r2 与接触法线的叉乘
        public PVector3 i2TimesR2CrossN;
        /// 如果接触点在上一个时间步存在，则为真
        public bool isRestingContact;
    }
}