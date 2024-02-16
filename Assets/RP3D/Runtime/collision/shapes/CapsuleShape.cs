using UnityEngine;

namespace RP3D
{
    public class CapsuleShape : ConvexShape
    {
        protected float mHalfHeight;

        public CapsuleShape(float radius, float height) : base(CollisionShapeName.CAPSULE, CollisionShapeType.CAPSULE,
            radius)
        {
            mHalfHeight = height * 0.5f;
        }

        public float getRadius()
        {
            return mMargin;
        }

        public float getHeight()
        {
            return mHalfHeight + mHalfHeight;
        }


        // Return true if the collision shape is a polyhedron
        public override bool IsPolyhedron()
        {
            return false;
        }


        // Return the local bounds of the shape in x, y and z directions
        // This method is used to compute the AABB of the box
        /**
         * @param min The minimum bounds of the shape in local-space coordinates
         * @param max The maximum bounds of the shape in local-space coordinates
         */
        public override void GetLocalBounds(out PVector3 min, out PVector3 max)
        {
            // Maximum bounds
            max.x = mMargin;
            max.y = mHalfHeight + mMargin;
            max.z = mMargin;

            // Minimum bounds
            min.x = -mMargin;
            min.y = -max.y;
            min.z = min.x;
        }

        // 计算返回碰撞形状的体积
        public override float getVolume()
        {
            return PMath.PI_RP3D * mMargin * mMargin * (4.0f * mMargin / 3.0f + 2.0f * mHalfHeight);
        }


        // 返回在给定方向上没有对象边距的局部支撑点。
        // 胶囊是两个球S1和S2的凸包。一组凸对象的凸包在方向"d"上的支撑点是该凸对象的所有支撑点中，
        // 与方向"d"具有最大点积的支撑点"p"。因此，在此方法中，我们计算胶囊的顶部和底部两个球的支撑点，
        // 并返回与方向向量具有最大点积的点。请注意，对象边距隐含为胶囊的半径和高度。
        public override PVector3 getLocalSupportPointWithoutMargin(PVector3 direction)
        {
            // 顶部球的支撑点
            var dotProductTop = mHalfHeight * direction.y;

            // 底部球的支撑点
            var dotProductBottom = -mHalfHeight * direction.y;

            // 返回具有最大点积的点
            if (dotProductTop > dotProductBottom)
                return new PVector3(0, mHalfHeight, 0);
            return new PVector3(0, -mHalfHeight, 0);
        }

        // 返回胶囊形状的局部惯性张量
        public override PVector3 getLocalInertiaTensor(float mass)
        {
            // 胶囊形状的惯性张量公式可在《Game Engine Gems, Volume 1》中找到
            var height = mHalfHeight + mHalfHeight;
            var radiusSquare = mMargin * mMargin;
            var heightSquare = height * height;
            var radiusSquareDouble = radiusSquare + radiusSquare;
            var factor1 = 2.0f * mMargin / (4.0f * mMargin + 3.0f * height);
            var factor2 = 3.0f * height / (4.0f * mMargin + 3.0f * height);
            var sum1 = 0.4f * radiusSquareDouble;
            var sum2 = 0.75f * height * mMargin + 0.5f * heightSquare;
            var sum3 = 0.25f * radiusSquare + 1.0f / 12.0f * heightSquare;
            var IxxAndzz = factor1 * mass * (sum1 + sum2) + factor2 * mass * sum3;
            var Iyy = factor1 * mass * sum1 + factor2 * mass * 0.25f * radiusSquareDouble;

            return new PVector3(IxxAndzz, Iyy, IxxAndzz);
        }

        public override bool Raycast(PRay pRay, out RaycastInfo raycastInfo, Collider collider)
        {
            raycastInfo = new RaycastInfo();

            var n = pRay.point2 - pRay.point1;

            var epsilon = 0.01f;
            var p = new PVector3(0.0f, -mHalfHeight, 0.0f);
            var q = new PVector3(0.0f, mHalfHeight, 0.0f);
            var d = q - p;
            var m = pRay.point1 - p;
            float t;

            var mDotD = m.dot(d);
            var nDotD = n.dot(d);
            var dDotD = d.dot(d);

            // Test if the segment is outside the cylinder
            var vec1DotD = (pRay.point1 - new PVector3(0.0f, -mHalfHeight - mMargin, 0.0f)).dot(d);
            if (vec1DotD < 0.0f && vec1DotD + nDotD < 0.0f) return false;
            var ddotDExtraCaps = 2.0f * mMargin * d.y;
            if (vec1DotD > dDotD + ddotDExtraCaps && vec1DotD + nDotD > dDotD + ddotDExtraCaps) return false;

            var nDotN = n.dot(n);
            var mDotN = m.dot(n);

            var a = dDotD * nDotN - nDotD * nDotD;
            var k = m.dot(m) - mMargin * mMargin;
            var c = dDotD * k - mDotD * mDotD;

            // If the ray is parallel to the capsule axis
            if (Mathf.Abs(a) < epsilon)
            {
                // If the origin is outside the surface of the capusle's cylinder, we return no hit
                if (c > 0.0f) return false;

                // Here we know that the segment intersect an endcap of the capsule

                // If the ray intersects with the "p" endcap of the capsule
                if (mDotD < 0.0f)
                {
                    // Check intersection between the ray and the "p" sphere endcap of the capsule
                    PVector3 hitLocalPoint;
                    float hitFraction;
                    if (RaycastWithSphereEndCap(pRay.point1, pRay.point2, p, pRay.maxFraction, out hitLocalPoint,
                            out hitFraction))
                    {
                        raycastInfo.body = collider.GetBody();
                        raycastInfo.collider = collider;
                        raycastInfo.hitFraction = hitFraction;
                        raycastInfo.worldPoint = hitLocalPoint;
                        raycastInfo.worldNormal = hitLocalPoint - p;
                        return true;
                    }

                    return false;
                }

                if (mDotD > dDotD)
                {
                    // If the ray intersects with the "q" endcap of the cylinder

                    // Check intersection between the ray and the "q" sphere endcap of the capsule
                    PVector3 hitLocalPoint;
                    float hitFraction;
                    if (RaycastWithSphereEndCap(pRay.point1, pRay.point2, q, pRay.maxFraction, out hitLocalPoint,
                            out hitFraction))
                    {
                        raycastInfo.body = collider.GetBody();
                        raycastInfo.collider = collider;
                        raycastInfo.hitFraction = hitFraction;
                        raycastInfo.worldPoint = hitLocalPoint;
                        raycastInfo.worldNormal = hitLocalPoint - q;

                        return true;
                    }

                    return false;
                } // If the origin is inside the cylinder, we return no hit

                return false;
            }

            var b = dDotD * mDotN - nDotD * mDotD;
            var discriminant = b * b - a * c;

            // If the discriminant is negative, no real roots and therfore, no hit
            if (discriminant < 0.0f) return false;

            // Compute the smallest root (first intersection along the ray)
            var t0 = t = (-b - Mathf.Sqrt(discriminant)) / a;

            // If the intersection is outside the finite cylinder of the capsule on "p" endcap side
            var value = mDotD + t * nDotD;
            if (value < 0.0f)
            {
                // Check intersection between the ray and the "p" sphere endcap of the capsule
                PVector3 hitLocalPoint;
                float hitFraction;
                if (RaycastWithSphereEndCap(pRay.point1, pRay.point2, p, pRay.maxFraction, out hitLocalPoint,
                        out hitFraction))
                {
                    raycastInfo.body = collider.GetBody();
                    raycastInfo.collider = collider;
                    raycastInfo.hitFraction = hitFraction;
                    raycastInfo.worldPoint = hitLocalPoint;
                    raycastInfo.worldNormal = hitLocalPoint - p;

                    return true;
                }

                return false;
            }

            if (value > dDotD)
            {
                // If the intersection is outside the finite cylinder on the "q" side

                // Check intersection between the ray and the "q" sphere endcap of the capsule
                PVector3 hitLocalPoint;
                float hitFraction;
                if (RaycastWithSphereEndCap(pRay.point1, pRay.point2, q, pRay.maxFraction, out hitLocalPoint,
                        out hitFraction))
                {
                    raycastInfo.body = collider.GetBody();
                    raycastInfo.collider = collider;
                    raycastInfo.hitFraction = hitFraction;
                    raycastInfo.worldPoint = hitLocalPoint;
                    raycastInfo.worldNormal = hitLocalPoint - q;

                    return true;
                }

                return false;
            }

            t = t0;

            // If the intersection is behind the origin of the ray or beyond the maximum
            // raycasting distance, we return no hit
            if (t < 0.0f || t > pRay.maxFraction) return false;

            // Compute the hit information
            var localHitPoint = pRay.point1 + t * n;
            raycastInfo.body = collider.GetBody();
            raycastInfo.collider = collider;
            raycastInfo.hitFraction = t;
            raycastInfo.worldPoint = localHitPoint;
            var v = localHitPoint - p;
            var w = v.dot(d) / d.LengthSquare() * d;
            var normalDirection = (localHitPoint - (p + w)).GetUnit();
            raycastInfo.worldNormal = normalDirection;

            return true;
        }

        // 胶囊形状上的射线投射方法，用于射线与胶囊两个球形端盖之间的交点计算
        private bool RaycastWithSphereEndCap(PVector3 point1, PVector3 point2, PVector3 sphereCenter, float maxFraction,
            out PVector3 hitLocalPoint, out float hitFraction)
        {
            hitLocalPoint = PVector3.Zero();
            hitFraction = 0.0f;


            var m = point1 - sphereCenter;
            var c = m.dot(m) - mMargin * mMargin;

            // If the origin of the ray is inside the sphere, we return no intersection
            if (c < 0.0f) return false;

            var rayDirection = point2 - point1;
            var b = m.dot(rayDirection);

            // If the origin of the ray is outside the sphere and the ray
            // is pointing away from the sphere, there is no intersection
            if (b > 0.0f) return false;

            var raySquareLength = rayDirection.LengthSquare();

            // Compute the discriminant of the quadratic equation
            var discriminant = b * b - raySquareLength * c;

            // If the discriminant is negative or the ray length is very small, there is no intersection
            if (discriminant < 0.0 || raySquareLength < float.Epsilon) return false;

            // Compute the solution "t" closest to the origin
            var t = -b - Mathf.Sqrt(discriminant);

            //assert(t >= decimal(0.0));

            // If the hit point is withing the segment ray fraction
            if (t < maxFraction * raySquareLength)
            {
                // Compute the intersection information
                t /= raySquareLength;
                hitFraction = t;
                hitLocalPoint = point1 + t * rayDirection;

                return true;
            }

            return false;
        }


        // 如果点在碰撞形状内部则返回 true
        public override bool testPointInside(PVector3 localPoint, Collider collider)
        {
            var diffYCenterSphere1 = localPoint.y - mHalfHeight;
            var diffYCenterSphere2 = localPoint.y + mHalfHeight;
            var xSquare = localPoint.x * localPoint.x;
            var zSquare = localPoint.z * localPoint.z;
            var squareRadius = mMargin * mMargin;

            // Return true if the point is inside the cylinder or one of the two spheres of the capsule
            return (xSquare + zSquare < squareRadius &&
                    localPoint.y < mHalfHeight && localPoint.y > -mHalfHeight) ||
                   xSquare + zSquare + diffYCenterSphere1 * diffYCenterSphere1 < squareRadius ||
                   xSquare + zSquare + diffYCenterSphere2 * diffYCenterSphere2 < squareRadius;
        }


        public override string ToString()
        {
            return $"CapsuleShape halfHeight= {mHalfHeight}, radius= {getRadius()} ";
        }
        
    }
}