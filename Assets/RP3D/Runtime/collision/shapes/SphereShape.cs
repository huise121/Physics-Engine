namespace RP3D
{
    public class SphereShape : ConvexShape
    {
        public SphereShape(float radius) : base(CollisionShapeName.SPHERE, CollisionShapeType.SPHERE, radius)
        {
        }
        
        // 在给定方向上返回没有对象边距的局部支撑点
        public override PVector3 getLocalSupportPointWithoutMargin(PVector3 direction)  {
            // 返回球的中心（半径已在对象边距中考虑）
            return new PVector3(0.0f, 0.0f, 0.0f);
        }

        public float GetRadius()
        {
            return mMargin;
        }

        public void SetRadius(float radius)
        {
            mMargin = radius;
        }

        public override bool IsPolyhedron()
        {
            return false;
        }

        // 返回形状在 x、y 和 z 方向上的局部边界。
        // 该方法用于计算盒子的AABB（轴对齐边界框）。
        /**
         * @param min 形状在局部坐标系中的最小边界
         * @param max 形状在局部坐标系中的最大边界
         */
        public override void GetLocalBounds(out PVector3 min, out PVector3 max)
        {
            // 最大边界
            max.x = mMargin;
            max.y = mMargin;
            max.z = mMargin;

            // 最小边界
            min.x = -mMargin;
            min.y = min.x;
            min.z = min.x;
        }
        
        /**
         * 用于计算碰撞形状的惯性张量的质量
         * @param mass 用于计算碰撞形状惯性张量的质量
         * @return 返回球体的局部惯性张量，其中 diag 为质量、边距和边距的乘积的0.4倍。
         */
        public override PVector3 getLocalInertiaTensor(float mass)  {
            var diag = 0.4f * mass * mMargin * mMargin;
            return new PVector3(diag, diag, diag);
        }
        
        // 计算并返回碰撞形状的体积
        public override float getVolume()  {
            // 体积计算公式：4/3 * π * r^3，其中 r 为边距（radius）的立方
            return 4.0f / 3.0f * PMath.PI_RP3D * mMargin * mMargin * mMargin;
        }
        
        // 使用碰撞形状更新物体的轴对齐边界框（AABB）
        /**
         * @param[out] aabb 碰撞形状的轴对齐边界框（AABB），在世界坐标系中计算得到
         * @param transform 用于计算碰撞形状的AABB的变换
         */
        public override void ComputeAABB(out AABB aabb, PTransform pTransform)
        {
            aabb = new AABB();
            // 获取在 x、y 和 z 方向上的局部半径
            PVector3 extents = new PVector3(mMargin, mMargin, mMargin);

            // 使用新的最小和最大坐标更新AABB
            aabb.SetMin(pTransform.GetPosition() - extents);
            aabb.SetMax(pTransform.GetPosition() + extents);
        }

        public override bool Raycast(PRay pRay, out RaycastInfo raycastInfo, Collider collider)
        {
            raycastInfo = new RaycastInfo();
            var m = pRay.point1;
            var c = m.dot(m) - mMargin * mMargin;

            // 如果射线起点在球体内部（c < 0.0f），则返回没有交点，因为射线起点已经在球体内部。
            if (c < 0.0f) return false;

            var rayDirection = pRay.point2 - pRay.point1;
            var b = m.dot(rayDirection);

            // If the origin of the ray is outside the sphere and the ray
            // is pointing away from the sphere, there is no intersection
            if (b > 0.0f) return false;

            var raySquareLength = rayDirection.LengthSquare();

            // Compute the discriminant of the quadratic equation
            var discriminant = b * b - raySquareLength * c;

            // If the discriminant is negative or the ray length is very small, there is no intersection
            if (discriminant < 0.0f || raySquareLength < float.Epsilon) return false;

            // Compute the solution "t" closest to the origin
            var t = -b - PMath.Sqrt(discriminant);

            //assert(t >= 0.0f);

            // If the hit point is withing the segment ray fraction
            if (t < pRay.maxFraction * raySquareLength)
            {
                // Compute the intersection information
                t /= raySquareLength;
                raycastInfo.body = collider.GetBody();
                raycastInfo.collider = collider;
                raycastInfo.hitFraction = t;
                raycastInfo.worldPoint = pRay.point1 + t * rayDirection;
                raycastInfo.worldNormal = raycastInfo.worldPoint;

                return true;
            }

            return false;
        }
        
        public override bool testPointInside(PVector3 localPoint, Collider collider) 
        {
            return localPoint.LengthSquare() < mMargin * mMargin;
        }

        public override string ToString()
        {
            return $"SphereShape radius= {GetRadius()} ";
        }
    }
}