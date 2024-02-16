// Material 通常用于描述碰撞材料的属性。碰撞材料是一种在物理引擎中用于模拟物体之间碰撞行为的特性。
//
// Material 类一般包含以下一些属性：
//
// 摩擦系数（Friction Coefficient）：描述了物体之间的摩擦力大小。摩擦系数可以分为静态摩擦系数和动态摩擦系数，分别用于描述物体在静止状态和运动状态下的摩擦力。
//
// 恢复系数（Restitution Coefficient）：描述了碰撞时物体之间能量损失的程度。恢复系数为 0 表示碰撞是完全非弹性的，物体在碰撞后将停止运动；而恢复系数为 1 表示碰撞是完全弹性的，物体在碰撞后将完全弹开。
//
// 用户自定义数据（User-defined Data）：一个可选的用户自定义数据指针，可以用于存储额外的材料属性或其他与碰撞相关的信息。
//
// 通过设置不同的摩擦系数和恢复系数，可以模拟出各种不同的碰撞效果，如滑动摩擦、静态摩擦、弹簧效应等。Material 类的使用使得物理引擎中的碰撞行为可以更加灵活地进行调整和控制，以满足不同应用场景的需求。


namespace RP3D
{
    public class Material
    {
        /// Material 类中的 mFrictionCoefficientSqrt 属性通常表示摩擦系数的平方根。
        /// 在一些物理引擎中，通常使用摩擦系数的平方根而不是直接的摩擦系数值。
        /// 这样做的一个原因是为了优化计算性能。
        ///
        /// 在物理引擎中，摩擦系数通常在 0 到 1 之间，其中 0 表示无摩擦，1 表示最大摩擦。
        /// 因此，摩擦系数的平方根值在 0 到 1 之间，这样可以减少一些复杂的计算，同时仍然能够表示摩擦的大小。
        ///
        /// 具体而言，mFrictionCoefficientSqrt 可能被用于计算碰撞中的摩擦力，而不必在每次碰撞中都进行平方根运算，从而提高计算效率。
        /// 这在处理大量碰撞时尤其有用，因为减少不必要的计算可以提高性能。
        private float mFrictionCoefficientSqrt;

        /// mBounciness 属性通常表示碰撞中的恢复系数（或称为弹性系数）。
        /// 恢复系数用于描述物体在碰撞过程中能量损失的程度，也就是物体在碰撞后弹性恢复的程度。
        ///
        /// 具体地说，mBounciness 表示碰撞中的恢复系数，它是一个在 0 到 1 之间的值，其中 0 表示碰撞是完全非弹性的，物体在碰撞后将不会反弹；
        /// 而 1 表示碰撞是完全弹性的，物体在碰撞后将以与碰撞前相同的速度反弹。
        ///
        /// 通过调整 mBounciness 属性，可以模拟出不同的碰撞效果。例如，当 mBounciness 较低时，碰撞后的物体会减速并几乎停止；
        /// 而当 mBounciness 较高时，碰撞后的物体会以较高的速度反弹。这对于模拟弹簧、球类运动等现象都非常有用。
        ///
        /// 总之，mBounciness 属性是用于描述碰撞恢复程度的重要参数，它影响着碰撞后物体的运动行为。
        private float mBounciness;

        /// mMassDensity 属性通常表示物体的质量密度。
        /// 质量密度是一个物理量，表示单位体积内的质量量。
        ///
        /// 具体来说，mMassDensity 表示物体的质量除以其体积的值。
        /// 因此，质量密度可以用以下公式表示：
        /// Mass Density= Mass / VolumeMass
        ///
        /// 在物理仿真中，质量密度是一个重要的物理量，它用于计算物体的质量，以便参与碰撞检测、碰撞响应和动力学模拟。
        /// 通过调整物体的质量密度，可以控制物体的质量分布，从而影响物体的运动行为和相互作用。
        ///
        /// 通常情况下，物体的质量可以通过以下公式计算得出：
        /// Mass = Mass Density × Volume
        ///
        /// 因此，mMassDensity 属性被用来确定物体的质量，进而影响其在物理仿真中的行为。
        private float mMassDensity;

        public Material(float frictionCoefficient, float bounciness, float massDensity = 1.0f)
        {
            mFrictionCoefficientSqrt = PMath.Sqrt(frictionCoefficient);
            mBounciness = bounciness;
            mMassDensity = massDensity;
        }

        public float getBounciness()
        {
            return mBounciness;
        }

        public void setBounciness(float bounciness)
        {
            mBounciness = bounciness;
        }

        public float getFrictionCoefficient()
        {
            return mFrictionCoefficientSqrt * mFrictionCoefficientSqrt;
        }

        public void setFrictionCoefficient(float frictionCoefficient)
        {
            mFrictionCoefficientSqrt = PMath.Sqrt(frictionCoefficient);
        }

        public float getFrictionCoefficientSqrt()
        {
            return mFrictionCoefficientSqrt;
        }

        public float getMassDensity()
        {
            return mMassDensity;
        }

        public void setMassDensity(float massDensity)
        {
            mMassDensity = massDensity;
        }

        public override string ToString()
        {
            return $"frictionCoefficient={mFrictionCoefficientSqrt * mFrictionCoefficientSqrt},bounciness={mBounciness}";
        }
    }
}