using System.Runtime.CompilerServices;

//VoronoiSimplex 是一个计算凸包的算法，通常用于解决凸多边形之间的碰撞检测和分离。
//它的基本思想是构建一个包含原始点的简单形状，称为 Voronoi Simplex（Voronoi 单纯形），
//并使用该简单形状来确定点之间的最近点对，从而确定它们之间的分离距离。
//VoronoiSimplex 主要用于判断两个物体是否相交，以及在碰撞中如何分离它们以防止穿透。
//在一些物理引擎中，特别是用于模拟刚体之间碰撞和反应的引擎中，VoronoiSimplex 算法是一个常用的工具。

namespace RP3D
{
    public class VoronoiSimplex
    {
        /// 当前点
        public PVector3[] mPoints;

        /// 单形中的顶点数
        public int mNbPoints;

        /// 使用单形顶点的重心坐标
        public float[] mBarycentricCoords;

        /// pointsLengthSquare[i] = (points[i].length)^2
        public float[] mPointsLengthSquare;

        /// 物体A的支撑点（局部坐标系）
        public PVector3[] mSuppPointsA;

        /// 物体B的支撑点（局部坐标系）
        public PVector3[] mSuppPointsB;

        /// 如果最近点需要重新计算（因为单形已更改），则为True
        public bool mRecomputeClosestPoint;

        /// 当前最接近原点的点
        public PVector3 mClosestPoint;

        /// 物体A上当前最近的点
        public PVector3 mClosestSuppPointA;

        /// 物体B上当前最近的点
        public PVector3 mClosestSuppPointB;

        /// 如果最近计算的最接近点有效，则为True
        public bool mIsClosestPointValid;


        public VoronoiSimplex()
        {
            mNbPoints = 0;
            mRecomputeClosestPoint = false;
            mIsClosestPointValid = false;
            mPoints = new PVector3[4];
            mBarycentricCoords = new float[4];
            mPointsLengthSquare = new float[4];
            mSuppPointsA = new PVector3[4];
            mSuppPointsB = new PVector3[4];
        }

        // 添加一个新的支撑点（A-B）到单形中
        /// suppPointA：物体A在方向-v的支撑点
        /// suppPointB：物体B在方向v的支撑点
        /// point：物体（A-B）的支撑点=> point = suppPointA - suppPointB
        public void addPoint(PVector3 point, PVector3 suppPointA, PVector3 suppPointB)
        {
            mPoints[mNbPoints] = point;
            mSuppPointsA[mNbPoints] = suppPointA;
            mSuppPointsB[mNbPoints] = suppPointB;

            mNbPoints++;
            mRecomputeClosestPoint = true;
        }

        // 从单形中移除一个点
        private void removePoint(int index)
        {
            mNbPoints--;
            mPoints[index] = mPoints[mNbPoints];
            mSuppPointsA[index] = mSuppPointsA[mNbPoints];
            mSuppPointsB[index] = mSuppPointsB[mNbPoints];
        }

        // 减少单形（仅保留参与到最接近原点的点）
        /// bitsUsedPoints被视为表示四个单形顶点是否被用来表示当前最接近原点的一系列位。
        /// - 最右边的位设置为1，如果第一个点被使用
        /// - 第二个最右边的位设置为1，如果第二个点被使用
        /// - 第三个最右边的位设置为1，如果第三个点被使用
        /// - 第四个最右边的位设置为1，如果第四个点被使用
        private void reduceSimplex(int bitsUsedPoints)
        {
            if (mNbPoints >= 4 && (bitsUsedPoints & 8) == 0)
                removePoint(3);

            if (mNbPoints >= 3 && (bitsUsedPoints & 4) == 0)
                removePoint(2);

            if (mNbPoints >= 2 && (bitsUsedPoints & 2) == 0)
                removePoint(1);

            if (mNbPoints >= 1 && (bitsUsedPoints & 1) == 0)
                removePoint(0);
        }

        // 返回点是否在单形中
        public bool isPointInSimplex(PVector3 point)
        {
            // For each four possible points in the simplex
            for (var i = 0; i < mNbPoints; i++)
            {
                // Compute the distance between the points
                var distanceSquare = (mPoints[i] - point).LengthSquare();

                // If the point is very close
                if (distanceSquare <= float.Epsilon) return true;
            }

            return false;
        }

        // 返回单形的点
        private int getSimplex(PVector3[] suppPointsA, PVector3[] suppPointsB, PVector3[] points)
        {
            for (var i = 0; i < mNbPoints; i++)
            {
                points[i] = mPoints[i];
                suppPointsA[i] = mSuppPointsA[i];
                suppPointsB[i] = mSuppPointsB[i];
            }

            // 返回单形中的点数
            return mNbPoints;
        }

        // 如果点集在仿射依赖，则返回true。
        /// 如果集合中的一个点是其他点的仿射组合，则集合是仿射依赖的
        public bool isAffinelyDependent()
        {
            switch (mNbPoints)
            {
                case 0: return false;
                case 1: return false;

                // 如果两点之间的距离小于等于零，则两个点是独立的
                case 2: return (mPoints[1] - mPoints[0]).LengthSquare() <= float.Epsilon;

                // 如果在给定的坐标系中，三个点在同一平面上，那么它们是仿射相关的。
                case 3: return (mPoints[1] - mPoints[0]).cross(mPoints[2] - mPoints[0]).LengthSquare() <= float.Epsilon;

                // 对于四个点，它们在同一平面上，但它们的重心不在该平面上
                case 4:
                    return PMath.Abs(
                               (mPoints[1] - mPoints[0]).dot(
                                   (mPoints[2] - mPoints[0]).cross(mPoints[3] - mPoints[0]))) <=
                           float.Epsilon;
            }

            return false;
        }

        // Compute the closest points "pA" and "pB" of object A and B.
        /// The points are computed as follows :
        /// pA = sum(lambda_i * a_i)    where "a_i" are the support points of object A
        /// pB = sum(lambda_i * b_i)    where "b_i" are the support points of object B
        /// with lambda_i = deltaX_i / deltaX
        public void computeClosestPointsOfAandB(out PVector3 pA,out PVector3 pB)
        {
            pA = mClosestSuppPointA;
            pB = mClosestSuppPointB;
        }

        // Recompute the closest point if the simplex has been modified
        /// This method computes the point "v" of simplex that is closest to the origin.
        /// The method returns true if a closest point has been found.
        private bool recomputeClosestPoint()
        {
            // If we need to recompute the closest point
            if (mRecomputeClosestPoint)
            {
                mRecomputeClosestPoint = false;

                switch (mNbPoints)
                {
                    case 0:

                        // Cannot compute closest point when the simplex is empty
                        mIsClosestPointValid = false;
                        break;

                    case 1:

                    {
                        // There is a single point in the simplex, therefore, this point is
                        // the one closest to the origin
                        mClosestPoint = mPoints[0];
                        mClosestSuppPointA = mSuppPointsA[0];
                        mClosestSuppPointB = mSuppPointsB[0];
                        setBarycentricCoords(1, 0, 0, 0);
                        mIsClosestPointValid = checkClosestPointValid();
                    }
                        break;

                    case 2:

                    {
                        var bitsUsedPoints = 0;
                        float t;

                        // The simplex is a line AB (where A=mPoints[0] and B=mPoints[1].
                        // We need to find the point of that line closest to the origin
                        computeClosestPointOnSegment(mPoints[0], mPoints[1],out bitsUsedPoints, out t);

                        // Compute the closest point
                        mClosestSuppPointA = mSuppPointsA[0] + t * (mSuppPointsA[1] - mSuppPointsA[0]);
                        mClosestSuppPointB = mSuppPointsB[0] + t * (mSuppPointsB[1] - mSuppPointsB[0]);
                        mClosestPoint = mClosestSuppPointA - mClosestSuppPointB;
                        setBarycentricCoords(1.0f - t, t, 0, 0);
                        mIsClosestPointValid = checkClosestPointValid();

                        // Reduce the simplex (remove vertices that are not participating to
                        // the closest point
                        reduceSimplex(bitsUsedPoints);
                    }
                        break;

                    case 3:
                    {
                        // The simplex is a triangle. We need to find the point of that
                        // triangle that is closest to the origin

                        int bitsUsedVertices;
                        PVector3 baryCoords;

                        // Compute the point of the triangle closest to the origin
                        computeClosestPointOnTriangle(mPoints[0], mPoints[1], mPoints[2], out bitsUsedVertices,
                            out baryCoords);
                        mClosestSuppPointA = baryCoords[0] * mSuppPointsA[0] + baryCoords[1] * mSuppPointsA[1] +
                                             baryCoords[2] * mSuppPointsA[2];
                        mClosestSuppPointB = baryCoords[0] * mSuppPointsB[0] + baryCoords[1] * mSuppPointsB[1] +
                                             baryCoords[2] * mSuppPointsB[2];
                        mClosestPoint = mClosestSuppPointA - mClosestSuppPointB;

                        setBarycentricCoords(baryCoords.x, baryCoords.y, baryCoords.z, 0.0f);
                        mIsClosestPointValid = checkClosestPointValid();

                        // Reduce the simplex (remove vertices that are not participating to
                        // the closest point
                        reduceSimplex(bitsUsedVertices);
                    }
                        break;

                    case 4:

                    {
                        // The simplex is a tetrahedron. We need to find the point of that
                        // tetrahedron that is closest to the origin

                        int bitsUsedVertices;
                        PVector2 baryCoordsAB;
                        PVector2 baryCoordsCD;
                        bool isDegenerate;

                        // Compute the point closest to the origin on the tetrahedron
                        var isOutside = computeClosestPointOnTetrahedron(mPoints[0], mPoints[1], mPoints[2], mPoints[3],
                           out bitsUsedVertices,out baryCoordsAB,out baryCoordsCD, out isDegenerate);

                        // If the origin is outside the tetrahedron
                        if (isOutside)
                        {
                            // Compute the point of the tetrahedron closest to the origin
                            mClosestSuppPointA = baryCoordsAB.x * mSuppPointsA[0] + baryCoordsAB.y * mSuppPointsA[1] +
                                                 baryCoordsCD.x * mSuppPointsA[2] + baryCoordsCD.y * mSuppPointsA[3];
                            mClosestSuppPointB = baryCoordsAB.x * mSuppPointsB[0] + baryCoordsAB.y * mSuppPointsB[1] +
                                                 baryCoordsCD.x * mSuppPointsB[2] + baryCoordsCD.y * mSuppPointsB[3];
                            mClosestPoint = mClosestSuppPointA - mClosestSuppPointB;

                            setBarycentricCoords(baryCoordsAB.x, baryCoordsAB.y, baryCoordsCD.x, baryCoordsCD.y);

                            // Reduce the simplex (remove vertices that are not participating to
                            // the closest point
                            reduceSimplex(bitsUsedVertices);
                        }
                        else
                        {
                            // If it is a degenerate case
                            if (isDegenerate)
                            {
                                mIsClosestPointValid = false;
                            }
                            else
                            {
                                // The origin is inside the tetrahedron, therefore, the closest point
                                // is the origin

                                setBarycentricCoords(0.0f, 0.0f, 0.0f, 0.0f);

                                mClosestSuppPointA.SetToZero();
                                mClosestSuppPointB.SetToZero();
                                mClosestPoint.SetToZero();

                                mIsClosestPointValid = true;
                            }

                            break;
                        }

                        mIsClosestPointValid = checkClosestPointValid();
                    }
                        break;
                }
            }

            return mIsClosestPointValid;
        }

        // 计算距离原点最近的线段上的点
        private void computeClosestPointOnSegment(PVector3 a, PVector3 b, out int bitUsedVertices, out float t)
        {
            var AP = -a; // 向量 AP，从 A 指向原点
            var AB = b - a; // 向量 AB，从 A 指向 B
            var APDotAB = AP.dot(AB); // 向量 AP 与向量 AB 的点积

            // 如果最近的点在从 A 沿着 B 的方向上
            if (APDotAB > 0.0f)
            {
                var lengthABSquare = AB.LengthSquare(); // 线段 AB 的长度的平方

                // 如果最近的点在线段 AB 上
                if (APDotAB < lengthABSquare)
                {
                    t = APDotAB / lengthABSquare; // 计算 t 值，即最近点在线段上的位置比例

                    bitUsedVertices = 3; // 0011 (使用了 A 和 B 两个点)
                }
                else
                {
                    // 如果原点在不是朝向 A 的 B 的一侧
                    // 因此，最近的点是 B
                    t = 1.0f;

                    bitUsedVertices = 2; // 0010 (只使用了 B)
                }
            }
            else
            {
                // 如果原点在不是朝向 B 的 A 的一侧
                // 因此，线的最近点是 A
                t = 0.0f;

                bitUsedVertices = 1; // 0001 (只使用了 A)
            }
        }


        // Compute point on a triangle that is closest to the origin
        /// This implementation is based on the one in the book
        /// "Real-Time Collision Detection" by Christer Ericson.
        private void computeClosestPointOnTriangle(PVector3 a, PVector3 b, PVector3 c, out int bitsUsedVertices,
            out PVector3 baryCoordsABC)
        {
            baryCoordsABC = PVector3.Zero();
            // Check if the origin is in the Voronoi region of vertex A
            var ab = b - a;
            var ac = c - a;
            var ap = -a;
            var d1 = ab.dot(ap);
            var d2 = ac.dot(ap);
            if (d1 <= 0.0f && d2 <= 0.0f)
            {
                // The origin is in the Voronoi region of vertex A

                // Set the barycentric coords of the closest point on the triangle
                baryCoordsABC.SetAllValues(1.0f, 0f, 0f);

                bitsUsedVertices = 1; // 0001 (only A is used)
                return;
            }

            // Check if the origin is in the Voronoi region of vertex B
            var bp = -b;
            var d3 = ab.dot(bp);
            var d4 = ac.dot(bp);
            if (d3 >= 0.0f && d4 <= d3)
            {
                // The origin is in the Voronoi region of vertex B

                // Set the barycentric coords of the closest point on the triangle
                baryCoordsABC.SetAllValues(0.0f, 1.0f, 0f);

                bitsUsedVertices = 2; // 0010 (only B is used)
                return;
            }

            // Check if the origin is in the Voronoi region of edge AB
            var vc = d1 * d4 - d3 * d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                // The origin is in the Voronoi region of edge AB
                // We return the projection of the origin on the edge AB
                var v1 = d1 / (d1 - d3);

                // Set the barycentric coords of the closest point on the triangle
                baryCoordsABC.SetAllValues(1.0f - v1, v1, 0);

                bitsUsedVertices = 3; // 0011 (A and B are used)
                return;
            }

            // Check if the origin is in the Voronoi region of vertex C
            var cp = -c;
            var d5 = ab.dot(cp);
            var d6 = ac.dot(cp);
            if (d6 >= 0.0f && d5 <= d6)
            {
                // The origin is in the Voronoi region of vertex C

                // Set the barycentric coords of the closest point on the triangle
                baryCoordsABC.SetAllValues(0.0f, 0.0f, 1.0f);

                bitsUsedVertices = 4; // 0100 (only C is used)
                return;
            }

            // Check if the origin is in the Voronoi region of edge AC
            var vb = d5 * d2 - d1 * d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                // The origin is in the Voronoi region of edge AC
                // We return the projection of the origin on the edge AC
                var w1 = d2 / (d2 - d6);

                // Set the barycentric coords of the closest point on the triangle
                baryCoordsABC.SetAllValues(1.0f - w1, 0, w1);

                bitsUsedVertices = 5; // 0101 (A and C are used)
                return;
            }

            // Check if the origin is in the Voronoi region of edge BC
            var va = d3 * d6 - d5 * d4;
            if (va <= 0.0f && d4 - d3 >= 0.0f && d5 - d6 >= 0.0f)
            {
                // The origin is in the Voronoi region of edge BC
                // We return the projection of the origin on the edge BC
                var w2 = (d4 - d3) / (d4 - d3 + (d5 - d6));

                // Set the barycentric coords of the closest point on the triangle
                baryCoordsABC.SetAllValues(0.0f, 1.0f - w2, w2);

                bitsUsedVertices = 6; // 0110 (B and C are used)
                return;
            }

            // The origin is in the Voronoi region of the face ABC
            var denom = 1.0f / (va + vb + vc);
            var v = vb * denom;
            var w = vc * denom;

            // Set the barycentric coords of the closest point on the triangle
            baryCoordsABC.SetAllValues(1 - v - w, v, w);

            bitsUsedVertices = 7; // 0111 (A, B and C are used)
        }

        // Compute point of a tetrahedron that is closest to the origin
        /// This implementation is based on the one in the book
        /// "Real-Time Collision Detection" by Christer Ericson.
        /// This method returns true if the origin is outside the tetrahedron.
        private bool computeClosestPointOnTetrahedron(PVector3 a, PVector3 b, PVector3 c,
            PVector3 d,out int bitsUsedPoints,out PVector2 baryCoordsAB,
            out PVector2 baryCoordsCD, out bool isDegenerate)
        {
            isDegenerate = false;

            // Start as if the origin was inside the tetrahedron
            bitsUsedPoints = 15; // 1111 (A, B, C and D are used)
            baryCoordsAB = PVector2.Zero();
            baryCoordsCD = PVector2.Zero();

            // Check if the origin is outside each tetrahedron face
            var isOriginOutsideFaceABC = testOriginOutsideOfPlane(a, b, c, d);
            var isOriginOutsideFaceACD = testOriginOutsideOfPlane(a, c, d, b);
            var isOriginOutsideFaceADB = testOriginOutsideOfPlane(a, d, b, c);
            var isOriginOutsideFaceBDC = testOriginOutsideOfPlane(b, d, c, a);

            // If we have a degenerate tetrahedron
            if (isOriginOutsideFaceABC < 0 || isOriginOutsideFaceACD < 0 ||
                isOriginOutsideFaceADB < 0 || isOriginOutsideFaceBDC < 0)
            {
                // The tetrahedron is degenerate
                isDegenerate = true;
                return false;
            }

            if (isOriginOutsideFaceABC == 0 && isOriginOutsideFaceACD == 0 &&
                isOriginOutsideFaceADB == 0 && isOriginOutsideFaceBDC == 0)
                // The origin is inside the tetrahedron
                return true;

            // We know that the origin is outside the tetrahedron, we now need to find
            // which of the four triangle faces is closest to it.

            var closestSquareDistance = float.MaxValue;
            int tempUsedVertices;
            var triangleBaryCoords = PVector3.Zero();

            // If the origin is outside face ABC
            if (isOriginOutsideFaceABC != 0)
            {
                // Compute the closest point on this triangle face
                computeClosestPointOnTriangle(a, b, c, out tempUsedVertices, out triangleBaryCoords);
                var closestPoint = triangleBaryCoords[0] * a + triangleBaryCoords[1] * b +
                                   triangleBaryCoords[2] * c;
                var squareDist = closestPoint.LengthSquare();

                // If the point on that face is the closest to the origin so far
                if (squareDist < closestSquareDistance)
                {
                    // Use it as the current closest point
                    closestSquareDistance = squareDist;
                    baryCoordsAB.SetAllValues(triangleBaryCoords[0], triangleBaryCoords[1]);
                    baryCoordsCD.SetAllValues(triangleBaryCoords[2], 0.0f);
                    bitsUsedPoints = tempUsedVertices;
                }
            }

            // If the origin is outside face ACD
            if (isOriginOutsideFaceACD != 0)
            {
                // Compute the closest point on this triangle face
                computeClosestPointOnTriangle(a, c, d, out tempUsedVertices, out triangleBaryCoords);
                var closestPoint = triangleBaryCoords[0] * a + triangleBaryCoords[1] * c +
                                   triangleBaryCoords[2] * d;
                var squareDist = closestPoint.LengthSquare();

                // If the point on that face is the closest to the origin so far
                if (squareDist < closestSquareDistance)
                {
                    // Use it as the current closest point
                    closestSquareDistance = squareDist;
                    baryCoordsAB.SetAllValues(triangleBaryCoords[0], 0.0f);
                    baryCoordsCD.SetAllValues(triangleBaryCoords[1], triangleBaryCoords[2]);
                    bitsUsedPoints = mapTriangleUsedVerticesToTetrahedron(tempUsedVertices, 0, 2, 3);
                }
            }

            // If the origin is outside face
            if (isOriginOutsideFaceADB != 0)
            {
                // Compute the closest point on this triangle face
                computeClosestPointOnTriangle(a, d, b, out tempUsedVertices, out triangleBaryCoords);
                var closestPoint = triangleBaryCoords[0] * a + triangleBaryCoords[1] * d +
                                   triangleBaryCoords[2] * b;
                var squareDist = closestPoint.LengthSquare();

                // If the point on that face is the closest to the origin so far
                if (squareDist < closestSquareDistance)
                {
                    // Use it as the current closest point
                    closestSquareDistance = squareDist;
                    baryCoordsAB.SetAllValues(triangleBaryCoords[0], triangleBaryCoords[2]);
                    baryCoordsCD.SetAllValues(0.0f, triangleBaryCoords[1]);
                    bitsUsedPoints = mapTriangleUsedVerticesToTetrahedron(tempUsedVertices, 0, 3, 1);
                }
            }

            // If the origin is outside face
            if (isOriginOutsideFaceBDC != 0)
            {
                // Compute the closest point on this triangle face
                computeClosestPointOnTriangle(b, d, c, out tempUsedVertices, out triangleBaryCoords);
                var closestPoint = triangleBaryCoords[0] * b + triangleBaryCoords[1] * d +
                                   triangleBaryCoords[2] * c;
                var squareDist = closestPoint.LengthSquare();

                // If the point on that face is the closest to the origin so far
                if (squareDist < closestSquareDistance)
                {
                    // Use it as the current closest point
                    baryCoordsAB.SetAllValues(0.0f, triangleBaryCoords[0]);
                    baryCoordsCD.SetAllValues(triangleBaryCoords[2], triangleBaryCoords[1]);
                    bitsUsedPoints = mapTriangleUsedVerticesToTetrahedron(tempUsedVertices, 1, 3, 2);
                }
            }

            return true;
        }


        // 0111 (1,2,3) => 0111
        // 0011 (2,1,3) =>
        public int mapTriangleUsedVerticesToTetrahedron(int triangleUsedVertices, int first, int second, int third)
        {
            var tetrahedronUsedVertices = (((1 & triangleUsedVertices) != 0 ? 1 : 0) << first) |
                                          (((2 & triangleUsedVertices) != 0 ? 1 : 0) << second) |
                                          (((4 & triangleUsedVertices) != 0 ? 1 : 0) << third);

            return tetrahedronUsedVertices;
        }


        // 测试点是否在由三角形（a、b、c）定义的平面之外
        /// 如果点d和原点在三角形面（a、b、c）的两侧，则返回1。如果它们在同一侧，则返回0。
        /// 此实现基于Christer Ericson的书籍《Real-Time Collision Detection》中的方法。
        private int testOriginOutsideOfPlane(PVector3 a, PVector3 b, PVector3 c, PVector3 d)
        {
            // (a,b,c)三角形的法线
            var n = (b - a).cross(c - a);

            var signp = (-a).dot(n);
            var signd = (d - a).dot(n);

            // 如果四面体退化为平面（所有点都在同一平面上）
            // 这不应该发生，因为在将点添加到简单形状后，使用此类的用户必须使用
            // isAffinelyDependent() 方法检查简单形状是否不是仿射相关的
            if (signd * signd < float.Epsilon * float.Epsilon) return -1;

            return signp * signd < 0.0f ? 1 : 0;

        }


        // 备份最近点
        public void backupClosestPointInSimplex(out PVector3 v)
        {
            v = mClosestPoint;
        }

        // 返回点的最大平方长度
        public float getMaxLengthSquareOfAPoint()
        {
            var maxLengthSquare = 0.0f;

            for (var i = 0; i < mNbPoints; i++)
            {
                var LengthSquare = mPoints[i].LengthSquare();
                if (LengthSquare > maxLengthSquare) maxLengthSquare = LengthSquare;
            }

            // 返回最大的平方长度
            return maxLengthSquare;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool isFull()
        {
            return mNbPoints == 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool isEmpty()
        {
            return mNbPoints == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setBarycentricCoords(float a, float b, float c, float d)
        {
            mBarycentricCoords[0] = a;
            mBarycentricCoords[1] = b;
            mBarycentricCoords[2] = c;
            mBarycentricCoords[3] = d;
        }

        // Compute the closest point "v" to the origin of the current simplex.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool computeClosestPoint(out PVector3 v)
        {
            var isValid = recomputeClosestPoint();
            v = mClosestPoint;
            return isValid;
        }

        // Return true if the
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool checkClosestPointValid()
        {
            return mBarycentricCoords[0] >= 0.0f && mBarycentricCoords[1] >= 0.0f &&
                   mBarycentricCoords[2] >= 0.0f && mBarycentricCoords[3] >= 0.0f;
        }
    }
}