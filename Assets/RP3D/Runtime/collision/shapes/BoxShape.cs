namespace RP3D
{
    public class BoxShape : ConvexPolyhedronShape
    {
        private PVector3 mHalfExtents;

        protected PhysicsCommon mPhysicsCommon;

        public BoxShape(PVector3 halfExtents, PhysicsCommon physicsCommon) : base(CollisionShapeName.BOX)
        {
            mPhysicsCommon = physicsCommon;
            mHalfExtents = halfExtents;
        }

        public PVector3 getHalfExtents()
        {
            return mHalfExtents;
        }

        public void setHalfExtents(PVector3 halfExtents)
        {
            mHalfExtents = halfExtents;
            //notifyColliderAboutChangedSize();
        }

        public override void GetLocalBounds(out PVector3 min, out PVector3 max)
        {
            // Maximum bounds
            max = mHalfExtents;
            // Minimum bounds
            min = -max;
        }


        // 返回在给定方向上没有对象边距的局部支撑点
        public override PVector3 getLocalSupportPointWithoutMargin(PVector3 direction)
        {
            return new PVector3(direction.x < 0.0f ? -mHalfExtents.x : mHalfExtents.x,
                direction.y < 0.0f ? -mHalfExtents.y : mHalfExtents.y,
                direction.z < 0.0f ? -mHalfExtents.z : mHalfExtents.z);
        }

        public override bool testPointInside(PVector3 localPoint, Collider collider)
        {
            return localPoint.x < mHalfExtents[0] && localPoint.x > -mHalfExtents[0] &&
                   localPoint.y < mHalfExtents[1] && localPoint.y > -mHalfExtents[1] &&
                   localPoint.z < mHalfExtents[2] && localPoint.z > -mHalfExtents[2];
        }

        public override PVector3 getLocalInertiaTensor(float mass)
        {
            var factor = 1.0f / 3.0f * mass;
            var xSquare = mHalfExtents.x * mHalfExtents.x;
            var ySquare = mHalfExtents.y * mHalfExtents.y;
            var zSquare = mHalfExtents.z * mHalfExtents.z;
            return new PVector3(factor * (ySquare + zSquare), factor * (xSquare + zSquare),
                factor * (xSquare + ySquare));
        }

        public override bool Raycast(PRay pRay, out RaycastInfo raycastInfo, Collider collider)
        {
            raycastInfo = new RaycastInfo();

            var rayDirection = pRay.point2 - pRay.point1;
            var tMin = float.MinValue;
            var tMax = float.MaxValue;
            var normalDirection = PVector3.Zero();

            var currentNormal = PVector3.Zero();

            // 对于三个平板的每一个
            for (var i = 0; i < 3; i++)
                // 如果射线与平板平行
                if (PMath.Abs(rayDirection[i]) < float.Epsilon)
                {
                    // 如果射线的起点不在平板内部，则没有击中
                    if (pRay.point1[i] > mHalfExtents[i] || pRay.point1[i] < -mHalfExtents[i]) return false;
                }
                else
                {
                    // 计算射线与平板近处和远处平面的交点
                    var oneOverD = 1.0f / rayDirection[i];
                    var t1 = (-mHalfExtents[i] - pRay.point1[i]) * oneOverD;
                    var t2 = (mHalfExtents[i] - pRay.point1[i]) * oneOverD;
                    currentNormal[0] = i == 0 ? -mHalfExtents[i] : 0.0f;
                    currentNormal[1] = i == 1 ? -mHalfExtents[i] : 0.0f;
                    currentNormal[2] = i == 2 ? -mHalfExtents[i] : 0.0f;

                    // 如果需要，交换 t1 和 t2，使得 t1 是与近处平面的交点，t2 是与远处平面的交点
                    if (t1 > t2)
                    {
                        (t1, t2) = (t2, t1);
                        currentNormal = -currentNormal;
                    }

                    // 计算平板交点间隔与之前平板的交点的交集
                    if (t1 > tMin)
                    {
                        tMin = t1;
                        normalDirection = currentNormal;
                    }

                    tMax = PMath.Min(tMax, t2);

                    // 如果 tMin 大于最大射线距离，我们返回没有击中
                    if (tMin > pRay.maxFraction) return false;

                    // 如果平板交点为空，则没有击中
                    if (tMin > tMax) return false;
                }

            // 如果 tMin 为负，我们返回没有击中
            if (tMin < 0.0f || tMin > pRay.maxFraction) return false;

            // 射线与三个平板相交，计算击中点
            var localHitPoint = pRay.point1 + tMin * rayDirection;
            raycastInfo.body = collider.GetBody();
            raycastInfo.collider = collider;
            raycastInfo.hitFraction = tMin;
            raycastInfo.worldPoint = localHitPoint;
            raycastInfo.worldNormal = normalDirection;
            return true;
        }

        // 返回多面体的面数量
        public override int GetNbFaces()
        {
            return 6;
        }


        // 返回多面体的顶点数量
        public override uint GetNbVertices()
        {
            return 8;
        }

        // 返回给定顶点的位置
        public override PVector3 GetVertexPosition(int vertexIndex)
        {
            switch (vertexIndex)
            {
                case 0: return new PVector3(-mHalfExtents.x, -mHalfExtents.y, mHalfExtents.z);
                case 1: return new PVector3(mHalfExtents.x, -mHalfExtents.y, mHalfExtents.z);
                case 2: return new PVector3(mHalfExtents.x, mHalfExtents.y, mHalfExtents.z);
                case 3: return new PVector3(-mHalfExtents.x, mHalfExtents.y, mHalfExtents.z);
                case 4: return new PVector3(-mHalfExtents.x, -mHalfExtents.y, -mHalfExtents.z);
                case 5: return new PVector3(mHalfExtents.x, -mHalfExtents.y, -mHalfExtents.z);
                case 6: return new PVector3(mHalfExtents.x, mHalfExtents.y, -mHalfExtents.z);
                case 7: return new PVector3(-mHalfExtents.x, mHalfExtents.y, -mHalfExtents.z);
            }

            return PVector3.Zero();
        }

        // 返回多面体的给定面的法向量
        public override PVector3 GetFaceNormal(int faceIndex)
        {
            switch (faceIndex)
            {
                case 0: return new PVector3(0, 0, 1);
                case 1: return new PVector3(1, 0, 0);
                case 2: return new PVector3(0, 0, -1);
                case 3: return new PVector3(-1, 0, 0);
                case 4: return new PVector3(0, -1, 0);
                case 5: return new PVector3(0, 1, 0);
            }

            return PVector3.Zero();
        }

        // 返回多面体的半边数量
        public override uint GetNbHalfEdges()
        {
            return 24;
        }

        // 返回多面体的质心
        public override PVector3 GetCentroid()
        {
            return PVector3.Zero();
        }

        // 计算并返回碰撞形状的体积
        public override float getVolume()
        {
            return 8 * mHalfExtents.x * mHalfExtents.y * mHalfExtents.z;
        }

        public override string ToString()
        {
            return $"BoxShape extents= {mHalfExtents}";
        }


        // Return a given face of the polyhedron
        public HalfEdgeStructure.Face etFace(int faceIndex)
        {
            return mPhysicsCommon.mBoxShapeHalfEdgeStructure.getFace(faceIndex);
        }

        // Return a given vertex of the polyhedron
        public HalfEdgeStructure.Vertex getVertex(int vertexIndex)
        {
            return mPhysicsCommon.mBoxShapeHalfEdgeStructure.getVertex(vertexIndex);
        }

        // Return a given half-edge of the polyhedron
        public override HalfEdgeStructure.Edge getHalfEdge(int edgeIndex)
        {
            return mPhysicsCommon.mBoxShapeHalfEdgeStructure.getHalfEdge(edgeIndex);
        }

        // Return a given face of the polyhedron
        public override HalfEdgeStructure.Face GetFace(int faceIndex)
        {
            return mPhysicsCommon.mBoxShapeHalfEdgeStructure.getFace(faceIndex);
        }
    }
}