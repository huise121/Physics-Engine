using System.Collections.Generic;

namespace RP3D
{
    public partial class SATAlgorithm
    {
        protected float SEPARATING_AXIS_RELATIVE_TOLERANCE = 1.002f;
        protected float SEPARATING_AXIS_ABSOLUTE_TOLERANCE = 0.0005f;

        private readonly bool mClipWithPreviousAxisIfStillColliding;


        public SATAlgorithm(bool clipWithPreviousAxisIfStillColliding)
        {
            mClipWithPreviousAxisIfStillColliding = clipWithPreviousAxisIfStillColliding;
        }

        


        // Compute the two contact points between a polyhedron and a capsule when the separating
        // axis is a face normal of the polyhedron
        public bool computeCapsulePolyhedronFaceContactPoints(int referenceFaceIndex, float capsuleRadius,
            ConvexPolyhedronShape polyhedron,
            float penetrationDepth, PTransform polyhedronToCapsuleTransform,
            PVector3 normalWorld, PVector3 separatingAxisCapsuleSpace,
            PVector3 capsuleSegAPolyhedronSpace, PVector3 capsuleSegBPolyhedronSpace,
            NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchIndex, bool isCapsuleShape1)
        {
            HalfEdgeStructure.Face face = polyhedron.GetFace(referenceFaceIndex);

            // Get the face normal
            PVector3 faceNormal = polyhedron.GetFaceNormal(referenceFaceIndex);

            var firstEdgeIndex = face.edgeIndex;
            var edgeIndex = firstEdgeIndex;

            var planesPoints = new List<PVector3>();
            var planesNormals = new List<PVector3>();

            // For each adjacent edge of the separating face of the polyhedron
            do
            {
                HalfEdgeStructure.Edge edge = polyhedron.getHalfEdge(edgeIndex);
                HalfEdgeStructure.Edge twinEdge = polyhedron.getHalfEdge(edge.twinEdgeIndex);

                // Compute the edge vertices and edge direction
                PVector3 edgeV1 = polyhedron.GetVertexPosition(edge.vertexIndex);
                PVector3 edgeV2 = polyhedron.GetVertexPosition(twinEdge.vertexIndex);
                var edgeDirection = edgeV2 - edgeV1;

                // Compute the normal of the clipping plane for this edge
                // The clipping plane is perpendicular to the edge direction and the reference face normal
                PVector3 clipPlaneNormal = faceNormal.cross(edgeDirection);

                // Construct a clipping plane for each adjacent edge of the separating face of the polyhedron
                planesPoints.Add(polyhedron.GetVertexPosition(edge.vertexIndex));
                planesNormals.Add(clipPlaneNormal);

                edgeIndex = edge.nextEdgeIndex;
            } while (edgeIndex != firstEdgeIndex);

            // First we clip the inner segment of the capsule with the four planes of the adjacent faces
            List<PVector3> clipSegment = PMath.clipSegmentWithPlanes(capsuleSegAPolyhedronSpace, capsuleSegBPolyhedronSpace,
                planesPoints, planesNormals);

            // Project the two clipped points into the polyhedron face
            var delta = faceNormal * (penetrationDepth - capsuleRadius);

            var contactFound = false;

            // For each of the two clipped points
            int nbClipSegments = clipSegment.Count;
            for (var i = 0; i < nbClipSegments; i++)
            {
                // Compute the penetration depth of the two clipped points (to filter out the points that does not correspond to the penetration depth)
                var clipPointPenDepth = (planesPoints[0] - clipSegment[i]).dot(faceNormal);

                // If the clipped point is one that produce this penetration depth, we keep it
                if (clipPointPenDepth > penetrationDepth - capsuleRadius - 0.001f)
                {
                    contactFound = true;

                    var contactPointPolyhedron = clipSegment[i] + delta;

                    // Project the clipped point into the capsule bounds
                    var contactPointCapsule = polyhedronToCapsuleTransform * clipSegment[i] -
                                              separatingAxisCapsuleSpace * capsuleRadius;


                    // Compute smooth triangle mesh contact if one of the two collision shapes is a triangle
                    TriangleShape.computeSmoothTriangleMeshContact(
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2,
                        isCapsuleShape1 ? contactPointCapsule : contactPointPolyhedron,
                        isCapsuleShape1 ? contactPointPolyhedron : contactPointCapsule,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform,
                        penetrationDepth, normalWorld);


                    // Create the contact point
                    narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, penetrationDepth,
                        isCapsuleShape1 ? contactPointCapsule : contactPointPolyhedron,
                        isCapsuleShape1 ? contactPointPolyhedron : contactPointCapsule);
                }
            }

            return contactFound;
        }
        


        // Return the penetration depth between two polyhedra along a face normal axis of the first polyhedron
        private float testSingleFaceDirectionPolyhedronVsPolyhedron(ConvexPolyhedronShape polyhedron1,
            ConvexPolyhedronShape polyhedron2,
            PTransform polyhedron1ToPolyhedron2,
            int faceIndex)
        {
            HalfEdgeStructure.Face face = polyhedron1.GetFace(faceIndex);

            // Get the face normal
            var faceNormal = polyhedron1.GetFaceNormal(faceIndex);

            // Convert the face normal into the local-space of polyhedron 2
            var faceNormalPolyhedron2Space = polyhedron1ToPolyhedron2.GetOrientation() * faceNormal;

            // Get the support point of polyhedron 2 in the inverse direction of face normal
            var supportPoint = polyhedron2.getLocalSupportPointWithoutMargin(-faceNormalPolyhedron2Space);

            // Compute the penetration depth
            var faceVertex = polyhedron1ToPolyhedron2 * polyhedron1.GetVertexPosition(face.faceVertices[0]);
            var penetrationDepth = (faceVertex - supportPoint).dot(faceNormalPolyhedron2Space);

            return penetrationDepth;
        }

        // Test all the normals of a polyhedron for separating axis in the polyhedron vs polyhedron case
        private float testFacesDirectionPolyhedronVsPolyhedron(ConvexPolyhedronShape polyhedron1,
            ConvexPolyhedronShape polyhedron2,
            PTransform polyhedron1ToPolyhedron2,
            ref int minFaceIndex)
        {
            float minPenetrationDepth = float.MaxValue;

            // For each face of the first polyhedron
            for (var f = 0; f < polyhedron1.GetNbFaces(); f++)
            {
                var penetrationDepth = testSingleFaceDirectionPolyhedronVsPolyhedron(polyhedron1, polyhedron2,
                    polyhedron1ToPolyhedron2, f);

                // If the penetration depth is negative, we have found a separating axis
                if (penetrationDepth <= 0.0f)
                {
                    minFaceIndex = f;
                    return penetrationDepth;
                }

                // Check if we have found a new minimum penetration axis
                if (penetrationDepth < minPenetrationDepth)
                {
                    minPenetrationDepth = penetrationDepth;
                    minFaceIndex = f;
                }
            }

            return minPenetrationDepth;
        }


        // Return true if two edges of two polyhedrons build a minkowski face (and can therefore be a separating axis)
        private bool testEdgesBuildMinkowskiFace(ConvexPolyhedronShape polyhedron1, HalfEdgeStructure.Edge edge1,
            ConvexPolyhedronShape polyhedron2, HalfEdgeStructure.Edge edge2,
            PTransform polyhedron1ToPolyhedron2)
        {
            var a = polyhedron1ToPolyhedron2.GetOrientation() * polyhedron1.GetFaceNormal(edge1.faceIndex);
            var b = polyhedron1ToPolyhedron2.GetOrientation() *
                    polyhedron1.GetFaceNormal(polyhedron1.getHalfEdge(edge1.twinEdgeIndex).faceIndex);

            var c = polyhedron2.GetFaceNormal(edge2.faceIndex);
            var d = polyhedron2.GetFaceNormal(polyhedron2.getHalfEdge(edge2.twinEdgeIndex).faceIndex);

            // Compute b.cross(a) using the edge direction
            var edge1Vertex1 = polyhedron1.GetVertexPosition(edge1.vertexIndex);
            var edge1Vertex2 = polyhedron1.GetVertexPosition(polyhedron1.getHalfEdge(edge1.twinEdgeIndex).vertexIndex);
            var bCrossA = polyhedron1ToPolyhedron2.GetOrientation() * (edge1Vertex1 - edge1Vertex2);

            // Compute d.cross(c) using the edge direction
            var edge2Vertex1 = polyhedron2.GetVertexPosition(edge2.vertexIndex);
            var edge2Vertex2 = polyhedron2.GetVertexPosition(polyhedron2.getHalfEdge(edge2.twinEdgeIndex).vertexIndex);
            var dCrossC = edge2Vertex1 - edge2Vertex2;

            // Test if the two arcs of the Gauss Map intersect (therefore forming a minkowski face)
            // Note that we negate the normals of the second polyhedron because we are looking at the
            // Gauss map of the minkowski difference of the polyhedrons
            return testGaussMapArcsIntersect(a, b, -c, -d, bCrossA, dCrossC);
        }


        // Return true if the arcs AB and CD on the Gauss Map (unit sphere) intersect
        /// This is used to know if the edge between faces with normal A and B on first polyhedron
        /// and edge between faces with normal C and D on second polygon create a face on the Minkowski
        /// sum of both polygons. If this is the case, it means that the cross product of both edges
        /// might be a separating axis.
        private bool testGaussMapArcsIntersect(PVector3 a, PVector3 b, PVector3 c, PVector3 d,
                    PVector3 bCrossA, PVector3 dCrossC)
                {
                    var cba = c.dot(bCrossA);
                    var dba = d.dot(bCrossA);
                    var adc = a.dot(dCrossC);
                    var bdc = b.dot(dCrossC);

                    return cba * dba < 0.0f && adc * bdc < 0.0f && cba * bdc > 0.0f;
                }
        }
    
    
}