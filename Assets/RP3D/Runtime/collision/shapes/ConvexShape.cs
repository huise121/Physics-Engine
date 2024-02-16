using System;
using System.Runtime.CompilerServices;

namespace RP3D
{
    //"Convex Shape" 的中文翻译是 "凸形"。
    public abstract class ConvexShape : CollisionShape
    {
        /// <summary>
        ///     这是一个表示GJK（Gilbert-Johnson-Keerthi）碰撞检测算法中使用的边缘（margin）的成员变量。在碰撞检测中，
        ///     margin是一小段额外的区域，通常沿着碰撞形状的表面，以确保在物体之间的碰撞检测中有一些余量。
        ///     在GJK算法中，margin被用来处理浮点数精度问题和防止物体之间的早期碰撞检测。通过在形状的表面上添加margin，
        ///     可以确保两个物体在物理世界中不会紧密贴合，从而提高碰撞检测的稳定性。
        ///     具体而言，这个成员变量mMargin表示GJK算法中使用的边缘大小，它是在形状表面上添加的额外距离。
        /// </summary>
        protected float mMargin;

        public ConvexShape(CollisionShapeName name, CollisionShapeType type, float margin) : base(name, type)
        {
            mMargin = margin;
        }

        public ConvexShape(CollisionShapeName name, CollisionShapeType type) : base(name, type)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMargin()
        {
            return mMargin;
        }

        //凸形的
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsConvex()
        {
            return true;
        }

        /// <summary>
        ///     这是一个虚拟方法，用于在给定方向上返回没有对象边距（margin）的本地支持点。在碰撞检测和物理引擎中，支持点是指形状上最远点，沿着给定方向。
        ///     对象边距通常是为了在碰撞检测中引入一些额外的空间，以防止形状之间的过早碰撞。这个方法的目的是获取在指定方向上的支持点，但不考虑对象的边距。
        ///     具体的实现可能会依赖于碰撞形状的类型。
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public abstract PVector3 getLocalSupportPointWithoutMargin(PVector3 direction);
        

        // Return a local support point in a given direction with the object margin
        public PVector3 getLocalSupportPointWithMargin(PVector3 direction)
        {
            // Get the support point without margin
            var supportPoint = getLocalSupportPointWithoutMargin(direction);

            if (mMargin != 0.0f)
            {
                // Add the margin to the support point
                var unitVec = new PVector3(0.0f, -1.0f, 0.0f);
                if (direction.LengthSquare() > float.Epsilon * float.Epsilon) unitVec = direction.GetUnit();
                supportPoint += unitVec * mMargin;
            }

            return supportPoint;
        }
    }
}