using UnityEngine;

namespace RP3D
{
    public class AABB
    {
        public PVector3 mMinCoordinates;
        public PVector3 mMaxCoordinates;

        public AABB()
        {
        }

        public AABB(PVector3 min, PVector3 max)
        {
            mMinCoordinates = min;
            mMaxCoordinates = max;
        }

        public void SetMin(PVector3 min)
        {
            mMinCoordinates = min;
        }

        public PVector3 GetMin()
        {
            return mMinCoordinates;
        }

        public void SetMax(PVector3 max)
        {
            mMaxCoordinates = max;
        }

        public PVector3 GetMax()
        {
            return mMaxCoordinates;
        }

        public PVector3 GetCenter()
        {
            return (mMinCoordinates + mMaxCoordinates) * 0.5f;
        }

        public PVector3 GetExtent()
        {
            return mMaxCoordinates - mMinCoordinates;
        }

        public void Inflate(float dx, float dy, float dz)
        {
            mMaxCoordinates += new PVector3(dx, dy, dz);
            mMinCoordinates -= new PVector3(dx, dy, dz);
        }

        public bool TestCollision(AABB aabb)
        {
            if (mMaxCoordinates.x < aabb.mMinCoordinates.x ||
                aabb.mMaxCoordinates.x < mMinCoordinates.x) return false;
            if (mMaxCoordinates.y < aabb.mMinCoordinates.y ||
                aabb.mMaxCoordinates.y < mMinCoordinates.y) return false;
            if (mMaxCoordinates.z < aabb.mMinCoordinates.z ||
                aabb.mMaxCoordinates.z < mMinCoordinates.z) return false;
            return true;
        }

        public float GetVolume()
        {
            var diff = mMaxCoordinates - mMinCoordinates;
            return diff.x * diff.y * diff.z;
        }

        public bool TestCollisionTriangleAABB(PVector3[] trianglePoints)
        {
            if (PMath.min3(trianglePoints[0].x, trianglePoints[1].x, trianglePoints[2].x) > mMaxCoordinates.x)
                return false;
            if (PMath.min3(trianglePoints[0].y, trianglePoints[1].y, trianglePoints[2].y) > mMaxCoordinates.y)
                return false;
            if (PMath.min3(trianglePoints[0].z, trianglePoints[1].z, trianglePoints[2].z) > mMaxCoordinates.z)
                return false;
            if (PMath.max3(trianglePoints[0].x, trianglePoints[1].x, trianglePoints[2].x) < mMinCoordinates.x)
                return false;
            if (PMath.max3(trianglePoints[0].y, trianglePoints[1].y, trianglePoints[2].y) < mMinCoordinates.y)
                return false;
            if (PMath.max3(trianglePoints[0].z, trianglePoints[1].z, trianglePoints[2].z) < mMinCoordinates.z)
                return false;
            return true;
        }

        public bool Contains(PVector3 point)
        {
            return point.x >= mMinCoordinates.x - float.Epsilon && point.x <= mMaxCoordinates.x + float.Epsilon &&
                   point.y >= mMinCoordinates.y - float.Epsilon && point.y <= mMaxCoordinates.y + float.Epsilon &&
                   point.z >= mMinCoordinates.z - float.Epsilon && point.z <= mMaxCoordinates.z + float.Epsilon;
        }

        // Apply a scale factor to the AABB
        public void ApplyScale(PVector3 scale)
        {
            mMinCoordinates = mMinCoordinates * scale;
            mMaxCoordinates = mMaxCoordinates * scale;
        }

        // Merge the AABB in parameter with the current one
        public void MergeWithAABB(AABB aabb)
        {
            mMinCoordinates.x = Mathf.Min(mMinCoordinates.x, aabb.mMinCoordinates.x);
            mMinCoordinates.y = Mathf.Min(mMinCoordinates.y, aabb.mMinCoordinates.y);
            mMinCoordinates.z = Mathf.Min(mMinCoordinates.z, aabb.mMinCoordinates.z);
            mMaxCoordinates.x = Mathf.Max(mMaxCoordinates.x, aabb.mMaxCoordinates.x);
            mMaxCoordinates.y = Mathf.Max(mMaxCoordinates.y, aabb.mMaxCoordinates.y);
            mMaxCoordinates.z = Mathf.Max(mMaxCoordinates.z, aabb.mMaxCoordinates.z);
        }

        // Replace the current AABB with a new AABB that is the union of two AABBs in parameters
        public void MergeTwoAABBs(AABB aabb1, AABB aabb2)
        {
            mMinCoordinates.x = Mathf.Min(aabb1.mMinCoordinates.x, aabb2.mMinCoordinates.x);
            mMinCoordinates.y = Mathf.Min(aabb1.mMinCoordinates.y, aabb2.mMinCoordinates.y);
            mMinCoordinates.z = Mathf.Min(aabb1.mMinCoordinates.z, aabb2.mMinCoordinates.z);
            mMaxCoordinates.x = Mathf.Max(aabb1.mMaxCoordinates.x, aabb2.mMaxCoordinates.x);
            mMaxCoordinates.y = Mathf.Max(aabb1.mMaxCoordinates.y, aabb2.mMaxCoordinates.y);
            mMaxCoordinates.z = Mathf.Max(aabb1.mMaxCoordinates.z, aabb2.mMaxCoordinates.z);
        }

        // Return true if the current AABB contains the AABB given in parameter
        public bool Contains(AABB aabb)
        {
            var isInside = true;
            isInside = isInside && mMinCoordinates.x <= aabb.mMinCoordinates.x;
            isInside = isInside && mMinCoordinates.y <= aabb.mMinCoordinates.y;
            isInside = isInside && mMinCoordinates.z <= aabb.mMinCoordinates.z;
            isInside = isInside && mMaxCoordinates.x >= aabb.mMaxCoordinates.x;
            isInside = isInside && mMaxCoordinates.y >= aabb.mMaxCoordinates.y;
            isInside = isInside && mMaxCoordinates.z >= aabb.mMaxCoordinates.z;
            return isInside;
        }


        // Create and return an AABB for a triangle
        public AABB CreateAABBForTriangle(PVector3[] trianglePoints)
        {
            var minCoords = new PVector3(trianglePoints[0].x, trianglePoints[0].y, trianglePoints[0].z);
            var maxCoords = new PVector3(trianglePoints[0].x, trianglePoints[0].y, trianglePoints[0].z);

            if (trianglePoints[1].x < minCoords.x) minCoords.x = trianglePoints[1].x;
            if (trianglePoints[1].y < minCoords.y) minCoords.y = trianglePoints[1].y;
            if (trianglePoints[1].z < minCoords.z) minCoords.z = trianglePoints[1].z;

            if (trianglePoints[2].x < minCoords.x) minCoords.x = trianglePoints[2].x;
            if (trianglePoints[2].y < minCoords.y) minCoords.y = trianglePoints[2].y;
            if (trianglePoints[2].z < minCoords.z) minCoords.z = trianglePoints[2].z;

            if (trianglePoints[1].x > maxCoords.x) maxCoords.x = trianglePoints[1].x;
            if (trianglePoints[1].y > maxCoords.y) maxCoords.y = trianglePoints[1].y;
            if (trianglePoints[1].z > maxCoords.z) maxCoords.z = trianglePoints[1].z;

            if (trianglePoints[2].x > maxCoords.x) maxCoords.x = trianglePoints[2].x;
            if (trianglePoints[2].y > maxCoords.y) maxCoords.y = trianglePoints[2].y;
            if (trianglePoints[2].z > maxCoords.z) maxCoords.z = trianglePoints[2].z;

            return new AABB(minCoords, maxCoords);
        }


        public bool Raycast(PRay pRay, out PVector3 hitPoint)
        {
            hitPoint = PVector3.Zero();

            var tMin = 0.0f;
            var tMax = float.MaxValue;

            var epsilon = 0.00001f;

            var rayDirection = pRay.point2 - pRay.point1;

            // For all three slabs
            for (var i = 0; i < 3; i++)
                // If the ray is parallel to the slab
                if (Mathf.Abs(rayDirection[i]) < epsilon)
                {
                    // If origin of the ray is not inside the slab, no hit
                    if (pRay.point1[i] < mMinCoordinates[i] || pRay.point1[i] > mMaxCoordinates[i]) return false;
                }
                else
                {
                    var rayDirectionInverse = 1.0f / rayDirection[i];
                    var t1 = (mMinCoordinates[i] - pRay.point1[i]) * rayDirectionInverse;
                    var t2 = (mMaxCoordinates[i] - pRay.point1[i]) * rayDirectionInverse;

                    if (t1 > t2)
                        // Swap t1 and t2
                        (t2, t1) = (t1, t2);

                    tMin = Mathf.Max(tMin, t1);
                    tMax = Mathf.Min(tMax, t2);

                    // Exit with no collision 
                    if (tMin > tMax) return false;
                }

            // Compute the hit point
            hitPoint = pRay.point1 + tMin * rayDirection;

            return true;
        }


        // Return true if the ray intersects the AABB
        public bool testRayIntersect(PVector3 rayOrigin, PVector3 rayDirectionInverse, float rayMaxFraction)
        {
            // This algorithm relies on the IEE floating point properties (division by zero). If the rayDirection is zero, rayDirectionInverse and
            // therfore t1 and t2 will be +-INFINITY. If the i coordinate of the ray's origin is inside the AABB (mMinCoordinates[i] < rayOrigin[i] < mMaxCordinates[i)), we have
            // t1 = -t2 = +- INFINITY. Since max(n, -INFINITY) = min(n, INFINITY) = n for all n, tMin and tMax will stay unchanged. Secondly, if the i
            // coordinate of the ray's origin is outside the box (rayOrigin[i] < mMinCoordinates[i] or rayOrigin[i] > mMaxCoordinates[i]) we have
            // t1 = t2 = +- INFINITY and therefore either tMin = +INFINITY or tMax = -INFINITY. One of those values will stay until the end and make the
            // method to return false. Unfortunately, if the ray lies exactly on a slab (rayOrigin[i] = mMinCoordinates[i] or rayOrigin[i] = mMaxCoordinates[i]) we
            // have t1 = (mMinCoordinates[i] - rayOrigin[i]) * rayDirectionInverse[i] = 0 * INFINITY = NaN which is a problem for the remaining of the algorithm.
            // This will cause the method to return true when the ray is not intersecting the AABB and therefore cause to traverse more nodes than necessary in
            // the BVH tree. Because this should be rare, it is not really a big issue.
            // Reference: https://tavianator.com/2011/ray_box.html

            var t1 = (mMinCoordinates[0] - rayOrigin[0]) * rayDirectionInverse[0];
            var t2 = (mMaxCoordinates[0] - rayOrigin[0]) * rayDirectionInverse[0];

            var tMin = PMath.Min(t1, t2);
            var tMax = PMath.Max(t1, t2);
            tMax = PMath.Min(tMax, rayMaxFraction);

            for (var i = 1; i < 3; i++)
            {
                t1 = (mMinCoordinates[i] - rayOrigin[i]) * rayDirectionInverse[i];
                t2 = (mMaxCoordinates[i] - rayOrigin[i]) * rayDirectionInverse[i];

                tMin = PMath.Max(tMin, PMath.Min(t1, t2));
                tMax = PMath.Min(tMax, PMath.Max(t1, t2));
            }

            return tMax >= PMath.Max(tMin, 0.0f);
        }
    }
}