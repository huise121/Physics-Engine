using System;

namespace RP3D
{
    // 类 ConvexPolyhedronShape
    /// <summary>
    ///     这个抽象类表示一个与狭义碰撞检测期间使用的具有凸多面体形状的关联身体。
    /// </summary>
    public abstract class ConvexPolyhedronShape : ConvexShape
    {
        protected PVector3 mScale;

        // -------------------- 方法 -------------------- //
        protected ConvexPolyhedronShape(CollisionShapeName name, PVector3 scaling) : base(name,
            CollisionShapeType.CONVEX_POLYHEDRON)
        {
            mScale = scaling;
        }

        protected ConvexPolyhedronShape(CollisionShapeName name) : base(name, CollisionShapeType.CONVEX_POLYHEDRON)
        {
        }


        /// <summary>
        ///     返回多面体的面数
        /// </summary>
        public abstract int GetNbFaces();

        // /// <summary>
        // /// 返回多面体的给定面
        // /// </summary>
        public abstract HalfEdgeStructure.Face GetFace(int faceIndex);

        /// <summary>
        ///     返回多面体的顶点数
        /// </summary>
        public abstract uint GetNbVertices();

        // /// <summary>
        // /// 返回多面体的给定顶点
        // /// </summary>
        // public abstract HalfEdgeStructure.Vertex GetVertex(uint vertexIndex);

        /// <summary>
        ///     返回给定顶点的位置
        /// </summary>
        public abstract PVector3 GetVertexPosition(int vertexIndex);

        /// <summary>
        ///     返回多面体的给定面的法线向量
        /// </summary>
        public abstract PVector3 GetFaceNormal(int faceIndex);

        /// <summary>
        ///     返回多面体的半边数
        /// </summary>
        public abstract uint GetNbHalfEdges();

        // /// <summary>
        // /// 返回多面体的给定半边
        // /// </summary>
        // public abstract HalfEdgeStructure.Edge GetHalfEdge(uint edgeIndex);

        /// <summary>
        ///     如果碰撞形状是多面体，则返回true
        /// </summary>
        public override bool IsPolyhedron()
        {
            return true;
        }

        /// <summary>
        ///     返回多面体的质心
        /// </summary>
        public abstract PVector3 GetCentroid();

        // /// <summary>
        // /// 查找并返回给定方向矢量上最反向的多面体面的索引
        // /// </summary>
        // public uint FindMostAntiParallelFace(Vector3 direction) {
        //     float minDotProduct = float.MaxValue;
        //     uint mostAntiParallelFace = 0;
        //
        //     // 对于多面体的每个面
        //     uint nbFaces = GetNbFaces();
        //     for (uint i = 0; i < nbFaces; i++) {
        //         // 获取面法线
        //         float dotProduct = Vector3.Dot(GetFaceNormal(i), direction);
        //         if (dotProduct < minDotProduct) {
        //             minDotProduct = dotProduct;
        //             mostAntiParallelFace = i;
        //         }
        //     }
        //
        //     return mostAntiParallelFace;
        // }

        public override bool Raycast(PRay pRay, out RaycastInfo raycastInfo, Collider collider)
        {
            throw new NotImplementedException();
        }

        public abstract HalfEdgeStructure.Edge getHalfEdge(int edgeIndex);


        // Find and return the index of the polyhedron face with the most anti-parallel face
// normal given a direction vector. This is used to find the incident face on
// a polyhedron of a given reference face of another polyhedron
        public int findMostAntiParallelFace(PVector3 direction)
        {
            var minDotProduct = float.MaxValue;
            var mostAntiParallelFace = 0;

            // For each face of the polyhedron
            var nbFaces = GetNbFaces();
            for (var i = 0; i < nbFaces; i++)
            {
                // Get the face normal
                var dotProduct = GetFaceNormal(i).dot(direction);
                if (dotProduct < minDotProduct)
                {
                    minDotProduct = dotProduct;
                    mostAntiParallelFace = i;
                }
            }

            return mostAntiParallelFace;
        }
    }
}