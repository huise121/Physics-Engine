namespace RP3D
{
    // 结构体 ContactManifoldSolver
    /**
     * 用于存储关于接触曲面的所有信息的接触曲面解算器内部数据结构。
     */
    public class ContactManifoldSolver
    {
        /// 外部接触曲面指针
        public ContactManifold externalContactManifold;
        /// 刚体 1 在动力学组件数组中的索引
        public int rigidBodyComponentIndexBody1;
        /// 刚体 2 在动力学组件数组中的索引
        public int rigidBodyComponentIndexBody2;
        /// 刚体 1 的质量的逆
        public float massInverseBody1;
        /// 刚体 2 的质量的逆
        public float massInverseBody2;
        /// 刚体 1 的线性锁定轴因子
        public PVector3 linearLockAxisFactorBody1;
        /// 刚体 2 的线性锁定轴因子
        public PVector3 linearLockAxisFactorBody2;
        /// 刚体 1 的角锁定轴因子
        public PVector3 angularLockAxisFactorBody1;
        /// 刚体 2 的角锁定轴因子
        public PVector3 angularLockAxisFactorBody2;
        /// 刚体 1 的逆惯性张量
        public Matrix3x3 inverseInertiaTensorBody1;
        /// 刚体 2 的逆惯性张量
        public Matrix3x3 inverseInertiaTensorBody2;
        /// 两个物体的摩擦系数
        public float frictionCoefficient;

        // - 摩擦约束应用于曲面中心时使用的变量 -//

        /// 接触曲面的平均法线向量
        public PVector3 normal;
        /// 应用摩擦约束的刚体 1 上的点
        public PVector3 frictionPointBody1;
        /// 应用摩擦约束的刚体 2 上的点
        public PVector3 frictionPointBody2;
        /// 摩擦约束的 r1 向量
        public PVector3 r1Friction;
        /// 摩擦约束的 r2 向量
        public PVector3 r2Friction;
        /// r1 与第一个摩擦向量的叉乘
        public PVector3 r1CrossT1;
        /// r1 与第二个摩擦向量的叉乘
        public PVector3 r1CrossT2;
        /// r2 与第一个摩擦向量的叉乘
        public PVector3 r2CrossT1;
        /// r2 与第二个摩擦向量的叉乘
        public PVector3 r2CrossT2;
        /// 第一个摩擦约束的矩阵 K
        public float inverseFriction1Mass;
        /// 第二个摩擦约束的矩阵 K
        public float inverseFriction2Mass;
        /// 扭摩擦约束的矩阵 K
        public float inverseTwistFrictionMass;
        /// 接触曲面中心的第一个摩擦方向
        public PVector3 frictionVector1;
        /// 接触曲面中心的第二个摩擦方向
        public PVector3 frictionVector2;
        /// 接触曲面中心的旧的第一个摩擦方向
        public PVector3 oldFrictionVector1;
        /// 接触曲面中心的旧的第二个摩擦方向
        public PVector3 oldFrictionVector2;
        /// 接触曲面中心的第一个摩擦方向冲量
        public float friction1Impulse;
        /// 接触曲面中心的第二个摩擦方向冲量
        public float friction2Impulse;
        /// 接触曲面中心的扭摩擦冲量
        public float frictionTwistImpulse;
        /// 接触点数量
        public int nbContacts;
    }
}