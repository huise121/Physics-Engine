using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public static partial class PMath
    {

        /// Function that returns the result of the "value" clamped by
        /// two others values "lowerLimit" and "upperLimit"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int clamp(int value, int lowerLimit, int upperLimit)
        {
            return Min(Max(value, lowerLimit), upperLimit);
        }

        /// Function that returns the result of the "value" clamped by
        /// two others values "lowerLimit" and "upperLimit"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float clamp(float value, float lowerLimit, float upperLimit)
        {
            return Min(Max(value, lowerLimit), upperLimit);
        }

        /// Return the minimum value among three values
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float min3(float a, float b, float c)
        {
            return Min(Min(a, b), c);
        }

        /// Return the maximum value among three values
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float max3(float a, float b, float c)
        {
            return Max(Max(a, b), c);
        }

        /// Return true if two values have the same sign
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool sameSign(float a, float b)
        {
            return a * b >= 0.0f;
        }

        // Return true if two vectors are parallel
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool areParallelVectors(PVector3 vector1, PVector3 vector2)
        {
            return vector1.cross(vector2).LengthSquare() < 0.00001f;
        }


        // Return true if two vectors are orthogonal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool areOrthogonalVectors(PVector3 vector1, PVector3 vector2)
        {
            return Abs(vector1.dot(vector2)) < 0.001f;
        }


        // Clamp a vector such that it is no longer than a given maximum length
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 clamp(PVector3 vector, float maxLength)
        {
            if (vector.LengthSquare() > maxLength * maxLength) return vector.GetUnit() * maxLength;
            return vector;
        }

        // Compute and return a point on segment from "segPointA" and "segPointB" that is closest to point "pointC"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 computeClosestPointOnSegment(PVector3 segPointA, PVector3 segPointB, PVector3 pointC)
        {
            var ab = segPointB - segPointA;

            var abLengthSquare = ab.LengthSquare();

            // If the segment has almost zero length
            if (abLengthSquare < MACHINE_EPSILON)
                // Return one end-point of the segment as the closest point
                return segPointA;

            // Project point C onto "AB" line
            var t = (pointC - segPointA).dot(ab) / abLengthSquare;

            // If projected point onto the line is outside the segment, clamp it to the segment
            if (t < 0.0f) t = 0.0f;
            if (t > 1.0f) t = 1.0f;

            // Return the closest point on the segment
            return segPointA + t * ab;
        }

        // Compute the closest points between two segments
        // This method uses the technique described in the book Real-Time
        // collision detection by Christer Ericson.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void computeClosestPointBetweenTwoSegments(PVector3 seg1PointA, PVector3 seg1PointB,
            PVector3 seg2PointA, PVector3 seg2PointB,out PVector3 closestPointSeg1,out PVector3 closestPointSeg2)
        {
            var d1 = seg1PointB - seg1PointA;
            var d2 = seg2PointB - seg2PointA;
            var r = seg1PointA - seg2PointA;
            var a = d1.LengthSquare();
            var e = d2.LengthSquare();
            var f = d2.dot(r);
            float s, t;

            // If both segments degenerate into points
            if (a <= MACHINE_EPSILON && e <= MACHINE_EPSILON)
            {
                closestPointSeg1 = seg1PointA;
                closestPointSeg2 = seg2PointA;
                return;
            }

            if (a <= MACHINE_EPSILON)
            {
                // If first segment degenerates into a point

                s = 0.0f;

                // Compute the closest point on second segment
                t = clamp(f / e, 0.0f, 1.0f);
            }
            else
            {
                var c = d1.dot(r);

                // If the second segment degenerates into a point
                if (e <= MACHINE_EPSILON)
                {
                    t = 0.0f;
                    s = clamp(-c / a, 0.0f, 1.0f);
                }
                else
                {
                    var b = d1.dot(d2);
                    var denom = a * e - b * b;

                    // If the segments are not parallel
                    if (denom != 0.0f)
                        // Compute the closest point on line 1 to line 2 and
                        // clamp to first segment.
                        s = clamp((b * f - c * e) / denom, 0.0f, 1.0f);
                    else
                        // Pick an arbitrary point on first segment
                        s = 0.0f;

                    // Compute the point on line 2 closest to the closest point
                    // we have just found
                    t = (b * s + f) / e;

                    // If this closest point is inside second segment (t in [0, 1]), we are done.
                    // Otherwise, we clamp the point to the second segment and compute again the
                    // closest point on segment 1
                    if (t < 0.0f)
                    {
                        t = 0.0f;
                        s = clamp(-c / a, 0.0f, 1.0f);
                    }
                    else if (t > 1.0f)
                    {
                        t = 1.0f;
                        s = clamp((b - c) / a, 0.0f, 1.0f);
                    }
                }
            }

            // Compute the closest points on both segments
            closestPointSeg1 = seg1PointA + d1 * s;
            closestPointSeg2 = seg2PointA + d2 * t;
        }

        // Compute the barycentric coordinates u, v, w of a point p inside the triangle (a, b, c)
        // This method uses the technique described in the book Real-Time collision detection by
        // Christer Ericson.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void computeBarycentricCoordinatesInTriangle(PVector3 a, PVector3 b, PVector3 c,
            PVector3 p, out float u, out float v, out float w)
        {
            var v0 = b - a;
            var v1 = c - a;
            var v2 = p - a;

            var d00 = v0.dot(v0);
            var d01 = v0.dot(v1);
            var d11 = v1.dot(v1);
            var d20 = v2.dot(v0);
            var d21 = v2.dot(v1);

            var denom = d00 * d11 - d01 * d01;
            v = (d11 * d20 - d01 * d21) / denom;
            w = (d00 * d21 - d01 * d20) / denom;
            u = 1.0f - v - w;
        }

        // Compute the intersection between a plane and a segment
        // Let the plane define by the equation planeNormal.dot(X) = planeD with X a point on the plane and "planeNormal" the plane normal. This method
        // computes the intersection P between the plane and the segment (segA, segB). The method returns the value "t" such
        // that P = segA + t * (segB - segA). Note that it only returns a value in [0, 1] if there is an intersection. Otherwise,
        // there is no intersection between the plane and the segment.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float computePlaneSegmentIntersection(PVector3 segA, PVector3 segB, float planeD,
            PVector3 planeNormal)
        {
            var parallelEpsilon = 0.0001f;
            var t = -1f;

            var nDotAB = planeNormal.dot(segB - segA);

            // If the segment is not parallel to the plane
            if (Abs(nDotAB) > parallelEpsilon) t = (planeD - planeNormal.dot(segA)) / nDotAB;

            return t;
        }

        // Compute the distance between a point "point" and a line given by the points "linePointA" and "linePointB"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float computePointToLineDistance(PVector3 linePointA, PVector3 linePointB, PVector3 point)
        {
            var distAB = (linePointB - linePointA).Length();

            if (distAB < MACHINE_EPSILON) return (point - linePointA).Length();

            return (point - linePointA).cross(point - linePointB).Length() / distAB;
        }


        //在多个平面上剪裁线段并返回剪裁的线段顶点
        //该方法实现了Sutherland–Hodgman裁剪算法
        public static List<PVector3> clipSegmentWithPlanes(PVector3 segA, PVector3 segB,
            List<PVector3> planesPoints,
            List<PVector3> planesNormals
        )
        {
            var inputVertices = new List<PVector3>();
            var outputVertices = new List<PVector3>();

            inputVertices.Add(segA);
            inputVertices.Add(segB);

            // For each clipping plane
            var nbPlanesPoints = planesPoints.Count;
            for (var p = 0; p < nbPlanesPoints; p++)
            {
                // If there is no more vertices, stop
                if (inputVertices.Count == 0) return inputVertices;


                outputVertices.Clear();

                var v1 = inputVertices[0];
                var v2 = inputVertices[1];

                var v1DotN = (v1 - planesPoints[p]).dot(planesNormals[p]);
                var v2DotN = (v2 - planesPoints[p]).dot(planesNormals[p]);

                // If the second vertex is in front of the clippling plane
                if (v2DotN >= 0.0f)
                {
                    // If the first vertex is not in front of the clippling plane
                    if (v1DotN < 0.0f)
                    {
                        // The second point we keep is the intersection between the segment v1, v2 and the clipping plane
                        var t = computePlaneSegmentIntersection(v1, v2, planesNormals[p].dot(planesPoints[p]),
                            planesNormals[p]);

                        if (t >= 0 && t <= 1.0f)
                            outputVertices.Add(v1 + t * (v2 - v1));
                        else
                            outputVertices.Add(v2);
                    }
                    else
                    {
                        outputVertices.Add(v1);
                    }

                    // Add the second vertex
                    outputVertices.Add(v2);
                }
                else
                {
                    // If the second vertex is behind the clipping plane

                    // If the first vertex is in front of the clippling plane
                    if (v1DotN >= 0.0f)
                    {
                        outputVertices.Add(v1);

                        // The first point we keep is the intersection between the segment v1, v2 and the clipping plane
                        var t = computePlaneSegmentIntersection(v1, v2, -planesNormals[p].dot(planesPoints[p]),
                            -planesNormals[p]);

                        if (t >= 0.0f && t <= 1.0f) outputVertices.Add(v1 + t * (v2 - v1));
                    }
                }
                
                inputVertices.Clear();
                for (int i = 0; i < outputVertices.Count; i++)
                {
                    inputVertices.Add(outputVertices[i]);
                }
            }

            return outputVertices;
        }

        // Clip a polygon against a single plane and return the clipped polygon vertices
        // This method implements the Sutherland–Hodgman polygon clipping algorithm
        public static void clipPolygonWithPlane(List<PVector3> polygonVertices, PVector3 planePoint,
            PVector3 planeNormal, List<PVector3> outClippedPolygonVertices)
        {
            var nbInputVertices = polygonVertices.Count;


            var vStartIndex = nbInputVertices - 1;

            var planeNormalDotPlanePoint = planeNormal.dot(planePoint);

            var vStartDotN = (polygonVertices[vStartIndex] - planePoint).dot(planeNormal);

            // For each edge of the polygon
            for (var vEndIndex = 0; vEndIndex < nbInputVertices; vEndIndex++)
            {
                var vStart = polygonVertices[vStartIndex];
                var vEnd = polygonVertices[vEndIndex];

                var vEndDotN = (vEnd - planePoint).dot(planeNormal);

                // If the second vertex is in front of the clippling plane
                if (vEndDotN >= 0.0f)
                {
                    // If the first vertex is not in front of the clippling plane
                    if (vStartDotN < 0.0f)
                    {
                        // The second point we keep is the intersection between the segment v1, v2 and the clipping plane
                        var t = computePlaneSegmentIntersection(vStart, vEnd, planeNormalDotPlanePoint, planeNormal);

                        if (t >= 0 && t <= 1.0f)
                            outClippedPolygonVertices.Add(vStart + t * (vEnd - vStart));
                        else
                            outClippedPolygonVertices.Add(vEnd);
                    }

                    // Add the second vertex
                    outClippedPolygonVertices.Add(vEnd);
                }
                else
                {
                    // If the second vertex is behind the clipping plane

                    // If the first vertex is in front of the clippling plane
                    if (vStartDotN >= 0.0f)
                    {
                        // The first point we keep is the intersection between the segment v1, v2 and the clipping plane
                        var t = computePlaneSegmentIntersection(vStart, vEnd, -planeNormalDotPlanePoint, -planeNormal);

                        if (t >= 0.0f && t <= 1.0f)
                            outClippedPolygonVertices.Add(vStart + t * (vEnd - vStart));
                        else
                            outClippedPolygonVertices.Add(vStart);
                    }
                }

                vStartIndex = vEndIndex;
                vStartDotN = vEndDotN;
            }
        }

        // Project a point onto a plane that is given by a point and its unit length normal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 projectPointOntoPlane(PVector3 point, PVector3 unitPlaneNormal, PVector3 planePoint)
        {
            return point - unitPlaneNormal.dot(point - planePoint) * unitPlaneNormal;
        }

        // Return the distance between a point and a plane (the plane normal must be normalized)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float computePointToPlaneDistance(PVector3 point, PVector3 planeNormal, PVector3 planePoint)
        {
            return planeNormal.dot(point - planePoint);
        }


        /// 如果一个数字是2的幂则返回true
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isPowerOfTwo(ulong number)
        {
            return number != 0 && (number & (number - 1)) == 0;
        }


        /// 返回大于参数中数字的下一个2的幂
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong nextPowerOfTwo64Bits(ulong number)
        {
            number--;
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            number |= number >> 32;
            number++;
            number += number == 0 ? 1UL : 0UL;
            return number;
        }

        /// Return an unique integer from two integer numbers (pairing function)
        /// Here we assume that the two parameter numbers are sorted such that
        /// number1 = max(number1, number2)
        /// http://szudzik.com/ElegantPairing.pdf
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int pairNumbers(int number1, int number2)
        {
            var nb1 = number1;
            var nb2 = number2;
            return nb1 * nb1 + nb1 + nb2;
        }
    }
}