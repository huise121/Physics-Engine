namespace RP3D
{
    public enum TriangleRaycastSide
    {
        /// Raycast against front triangle
        FRONT,

        /// Raycast against back triangle
        BACK,

        /// Raycast against front and back triangle
        FRONT_AND_BACK
    }

    public class TriangleShape : ConvexPolyhedronShape
    {
        public PVector3[] mPoints;
        public PVector3 mNormal;
        public PVector3[] mVerticesNormals;
        public TriangleRaycastSide mRaycastTestType;
        public HalfEdgeStructure mTriangleHalfEdgeStructure;

        public TriangleShape(PVector3[] vertices, PVector3[] verticesNormals, int shapeId,
            HalfEdgeStructure triangleHalfEdgeStructure) : base(CollisionShapeName.TRIANGLE)
        {
            mPoints = new PVector3[3];
            mPoints[0] = vertices[0];
            mPoints[1] = vertices[1];
            mPoints[2] = vertices[2];

            // Compute the triangle normal
            mNormal = (vertices[1] - vertices[0]).cross(vertices[2] - vertices[0]);
            mNormal.Normalize();

            mVerticesNormals = new PVector3[3];
            mVerticesNormals[0] = verticesNormals[0];
            mVerticesNormals[1] = verticesNormals[1];
            mVerticesNormals[2] = verticesNormals[2];

            mRaycastTestType = TriangleRaycastSide.FRONT;

            mId = shapeId;

            mTriangleHalfEdgeStructure = triangleHalfEdgeStructure;
        }

        /// Constructor
        public TriangleShape(PVector3[] vertices, int shapeId, HalfEdgeStructure triangleHalfEdgeStructure) : base(
            CollisionShapeName.TRIANGLE)
        {
            mTriangleHalfEdgeStructure = triangleHalfEdgeStructure;
            mPoints = new PVector3[3];
            mPoints[0] = vertices[0];
            mPoints[1] = vertices[1];
            mPoints[2] = vertices[2];
            mNormal = PVector3.Zero();

            mVerticesNormals = new PVector3[3];
            mVerticesNormals[0] = mNormal;
            mVerticesNormals[1] = mNormal;
            mVerticesNormals[2] = mNormal;

            mRaycastTestType = TriangleRaycastSide.FRONT;

            mId = shapeId;
        }


        public override int GetNbFaces()
        {
            return 2;
        }

        public override uint GetNbVertices()
        {
            return 3;
        }

        public override PVector3 GetVertexPosition(int vertexIndex)
        {
            return mPoints[vertexIndex];
        }

        public override PVector3 GetFaceNormal(int faceIndex)
        {
            return faceIndex == 0 ? mNormal : -mNormal;
        }

        public override uint GetNbHalfEdges()
        {
            return 6;
        }

        public override PVector3 GetCentroid()
        {
            return (mPoints[0] + mPoints[1] + mPoints[2]) / 3.0f;
        }
        
        // Return a local support point in a given direction without the object margin
        public override PVector3 getLocalSupportPointWithoutMargin( PVector3 direction)  {
            PVector3 dotProducts = new PVector3(direction.dot(mPoints[0]), direction.dot(mPoints[1]), direction.dot(mPoints[2]));
            return mPoints[dotProducts.GetMaxAxis()];
        }

        public override HalfEdgeStructure.Edge getHalfEdge(int edgeIndex)
        {
            return mTriangleHalfEdgeStructure.getHalfEdge(edgeIndex);
        }
        
        // Return a given face of the polyhedron
        public override HalfEdgeStructure.Face GetFace(int faceIndex)  {
            return mTriangleHalfEdgeStructure.getFace(faceIndex);
        }


        // Get a smooth contact normal for collision for a triangle of the mesh
        /// This is used to avoid the internal edges issue that occurs when a shape is colliding with
        /// several triangles of a concave mesh. If the shape collide with an edge of the triangle for instance,
        /// the computed contact normal from this triangle edge is not necessarily in the direction of the surface
        /// normal of the mesh at this point. The idea to solve this problem is to use the real (smooth) surface
        /// normal of the mesh at this point as the contact normal. This technique is described in the chapter 5
        /// of the Game Physics Pearl book by Gino van der Bergen and Dirk Gregorius. The vertices normals of the
        /// mesh are either provided by the user or precomputed if the user did not provide them. Note that we only
        /// use the interpolated normal if the contact point is on an edge of the triangle. If the contact is in the
        /// middle of the triangle, we return the true triangle normal.
        public PVector3 computeSmoothLocalContactNormalForTriangle(PVector3 localContactPoint)
        {
            // Compute the barycentric coordinates of the point in the triangle
            float u, v, w;
            PMath.computeBarycentricCoordinatesInTriangle(mPoints[0], mPoints[1], mPoints[2], localContactPoint, out u,
                out v, out w);

            // If the contact is in the middle of the triangle face (not on the edges)
            if (u > float.Epsilon && v > float.Epsilon && w > float.Epsilon)
                // We return the true triangle face normal (not the interpolated one)
                return mNormal;

            // We compute the contact normal as the barycentric interpolation of the three vertices normals
            var interpolatedNormal = u * mVerticesNormals[0] + v * mVerticesNormals[1] + w * mVerticesNormals[2];

            // If the interpolated normal is degenerated
            if (interpolatedNormal.LengthSquare() < float.Epsilon)
                // Return the original normal
                return mNormal;

            return interpolatedNormal.GetUnit();
        }


        // This method implements the technique described in Game Physics Pearl book
// by Gino van der Bergen and Dirk Gregorius to get smooth triangle mesh collision. The idea is
// to replace the contact normal of the triangle shape with the precomputed normal of the triangle
// mesh at this point. Then, we need to recompute the contact point on the other shape in order to
// stay aligned with the new contact normal. This method will return the new smooth world contact
// normal of the triangle and the the local contact point on the other shape.
        public void computeSmoothMeshContact(PVector3 localContactPointTriangle,
            PTransform triangleShapeToWorldTransform,
            PTransform worldToOtherShapeTransform, float penetrationDepth, bool isTriangleShape1,
            PVector3 outNewLocalContactPointOtherShape, PVector3 outSmoothWorldContactTriangleNormal)
        {
            // Get the smooth contact normal of the mesh at the contact point on the triangle
            var triangleLocalNormal = computeSmoothLocalContactNormalForTriangle(localContactPointTriangle);

            // Convert the local contact normal into world-space
            var triangleWorldNormal = triangleShapeToWorldTransform.GetOrientation() * triangleLocalNormal;

            // Penetration axis with direction from triangle to other shape
            var triangleToOtherShapePenAxis = isTriangleShape1
                ? outSmoothWorldContactTriangleNormal
                : -outSmoothWorldContactTriangleNormal;

            // The triangle normal should be the one in the direction out of the current colliding face of the triangle
            if (triangleWorldNormal.dot(triangleToOtherShapePenAxis) < 0.0f)
            {
                triangleWorldNormal = -triangleWorldNormal;
                triangleLocalNormal = -triangleLocalNormal;
            }

            // Compute the final contact normal from shape 1 to shape 2
            outSmoothWorldContactTriangleNormal = isTriangleShape1 ? triangleWorldNormal : -triangleWorldNormal;

            // Re-align the local contact point on the other shape such that it is aligned along the new contact normal
            var otherShapePointTriangleSpace = localContactPointTriangle - triangleLocalNormal * penetrationDepth;
            var otherShapePoint = worldToOtherShapeTransform * triangleShapeToWorldTransform *
                                  otherShapePointTriangleSpace;
            outNewLocalContactPointOtherShape.SetAllValues(otherShapePoint.x, otherShapePoint.y, otherShapePoint.z);
        }


        // This method compute the smooth mesh contact with a triangle in case one of the two collision
// shapes is a triangle. The idea in this case is to use a smooth vertex normal of the triangle mesh
// at the contact point instead of the triangle normal to avoid the internal edge collision issue.
// This method will return the new smooth world contact
// normal of the triangle and the the local contact point on the other shape.
        public static void computeSmoothTriangleMeshContact(CollisionShape shape1, CollisionShape shape2,
            PVector3 localContactPointShape1, PVector3 localContactPointShape2,
            PTransform shape1ToWorld, PTransform shape2ToWorld,
            float penetrationDepth,PVector3 outSmoothVertexNormal)
        {
            // If one the shape is a triangle
            var isShape1Triangle = shape1.GetName() == CollisionShapeName.TRIANGLE;
            if (isShape1Triangle || shape2.GetName() == CollisionShapeName.TRIANGLE)
            {
                var triangleShape = isShape1Triangle ? (TriangleShape)shape1 : (TriangleShape)shape2;

                // Compute the smooth triangle mesh contact normal and recompute the local contact point on the other shape
                triangleShape.computeSmoothMeshContact(isShape1Triangle ? localContactPointShape1 : localContactPointShape2,
                    isShape1Triangle ? shape1ToWorld : shape2ToWorld,
                    isShape1Triangle ? shape2ToWorld.GetInverse() : shape1ToWorld.GetInverse(),
                    penetrationDepth, isShape1Triangle,
                     isShape1Triangle ?  localContactPointShape2 :  localContactPointShape1,
                     outSmoothVertexNormal);
            }

        }

        public override void GetLocalBounds(out PVector3 min, out PVector3 max)
        {
            throw new System.NotImplementedException();
        }

        public override float getVolume()
        {
            throw new System.NotImplementedException();
        }

        public override PVector3 getLocalInertiaTensor(float mass)
        {
            throw new System.NotImplementedException();
        }

        public override bool testPointInside(PVector3 localPoint, Collider collider)
        {
            throw new System.NotImplementedException();
        }
    }
}