namespace RP3D
{
    public partial class SATAlgorithm
    {
        // Test collision between a capsule and a convex mesh
        public bool testCollisionCapsuleVsConvexPolyhedron(NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchIndex)
        {
            var isCapsuleShape1 = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1.GetType() ==
                                  CollisionShapeType.CAPSULE;


            // Get the collision shapes
            var capsuleShape = (CapsuleShape)(isCapsuleShape1
                ? narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1
                : narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2);
            var polyhedron = (ConvexPolyhedronShape)(isCapsuleShape1
                ? narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2
                : narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1);

            var capsuleToWorld = isCapsuleShape1
                ? narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform
                : narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform;
            var polyhedronToWorld = isCapsuleShape1
                ? narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform
                : narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform;

            var polyhedronToCapsuleTransform = capsuleToWorld.GetInverse() * polyhedronToWorld;

            // Compute the end-points of the inner segment of the capsule
            var capsuleSegA = new PVector3(0, -capsuleShape.getHeight() * 0.5f, 0);
            var capsuleSegB = new PVector3(0, capsuleShape.getHeight() * 0.5f, 0);
            var capsuleSegmentAxis = capsuleSegB - capsuleSegA;

            // Minimum penetration depth
            var minPenetrationDepth = float.MinValue;
            var minFaceIndex = 0;
            var isMinPenetrationFaceNormal = false;
            PVector3 separatingAxisCapsuleSpace = PVector3.Zero();
            PVector3 separatingPolyhedronEdgeVertex1 = PVector3.Zero();
            PVector3 separatingPolyhedronEdgeVertex2 = PVector3.Zero();

            // For each face of the convex mesh
            for (var f = 0; f < polyhedron.GetNbFaces(); f++)
            {
                PVector3 outFaceNormalCapsuleSpace = PVector3.Zero();

                // Compute the penetration depth
                var penetrationDepth = computePolyhedronFaceVsCapsulePenetrationDepth(f, polyhedron, capsuleShape,
                    polyhedronToCapsuleTransform,
                    outFaceNormalCapsuleSpace);

                // If the penetration depth is negative, we have found a separating axis
                if (penetrationDepth <= 0.0f) return false;

                // Check if we have found a new minimum penetration axis
                if (penetrationDepth < minPenetrationDepth)
                {
                    minPenetrationDepth = penetrationDepth;
                    minFaceIndex = f;
                    isMinPenetrationFaceNormal = true;
                    separatingAxisCapsuleSpace = outFaceNormalCapsuleSpace;
                }
            }

            // For each direction that is the cross product of the capsule inner segment and an edge of the polyhedron
            for (var e = 0; e < polyhedron.GetNbHalfEdges(); e += 2)
            {
                // Get an edge from the polyhedron (convert it into the capsule local-space)
                HalfEdgeStructure.Edge edge = polyhedron.getHalfEdge(e);
                PVector3 edgeVertex1 = polyhedron.GetVertexPosition(edge.vertexIndex);
                PVector3 edgeVertex2 =
                    polyhedron.GetVertexPosition(polyhedron.getHalfEdge(edge.nextEdgeIndex).vertexIndex);
                var edgeDirectionCapsuleSpace =
                    polyhedronToCapsuleTransform.GetOrientation() * (edgeVertex2 - edgeVertex1);

                HalfEdgeStructure.Edge twinEdge = polyhedron.getHalfEdge(edge.twinEdgeIndex);
                var adjacentFace1Normal = polyhedronToCapsuleTransform.GetOrientation() *
                                          polyhedron.GetFaceNormal(edge.faceIndex);
                var adjacentFace2Normal = polyhedronToCapsuleTransform.GetOrientation() *
                                          polyhedron.GetFaceNormal(twinEdge.faceIndex);

                // Check using the Gauss Map if this edge cross product can be as separating axis
                if (isMinkowskiFaceCapsuleVsEdge(capsuleSegmentAxis, adjacentFace1Normal, adjacentFace2Normal))
                {
                    PVector3 outAxis;

                    // Compute the penetration depth
                    var penetrationDepth = computeEdgeVsCapsuleInnerSegmentPenetrationDepth(polyhedron, capsuleShape,
                        capsuleSegmentAxis, edgeVertex1,
                        edgeDirectionCapsuleSpace,
                        polyhedronToCapsuleTransform,
                        out outAxis);

                    // If the penetration depth is negative, we have found a separating axis
                    if (penetrationDepth <= 0.0f) return false;

                    // Check if we have found a new minimum penetration axis
                    if (penetrationDepth < minPenetrationDepth)
                    {
                        minPenetrationDepth = penetrationDepth;
                        isMinPenetrationFaceNormal = false;
                        separatingAxisCapsuleSpace = outAxis;
                        separatingPolyhedronEdgeVertex1 = edgeVertex1;
                        separatingPolyhedronEdgeVertex2 = edgeVertex2;
                    }
                }
            }

            // Convert the inner capsule segment points into the polyhedron local-space
            var capsuleToPolyhedronTransform = polyhedronToCapsuleTransform.GetInverse();
            var capsuleSegAPolyhedronSpace = capsuleToPolyhedronTransform * capsuleSegA;
            var capsuleSegBPolyhedronSpace = capsuleToPolyhedronTransform * capsuleSegB;

            var normalWorld = capsuleToWorld.GetOrientation() * separatingAxisCapsuleSpace;
            if (isCapsuleShape1) normalWorld = -normalWorld;
            var capsuleRadius = capsuleShape.getRadius();

            // If the separating axis is a face normal
            // We need to clip the inner capsule segment with the adjacent faces of the separating face
            if (isMinPenetrationFaceNormal)
            {
                // If we need to report contacts
                if (narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].reportContacts)
                    return computeCapsulePolyhedronFaceContactPoints(minFaceIndex, capsuleRadius, polyhedron,
                        minPenetrationDepth,
                        polyhedronToCapsuleTransform, normalWorld, separatingAxisCapsuleSpace,
                        capsuleSegAPolyhedronSpace, capsuleSegBPolyhedronSpace,
                        narrowPhaseInfoBatch, batchIndex, isCapsuleShape1);
            }
            else
            {
                // The separating axis is the cross product of a polyhedron edge and the inner capsule segment

                // If we need to report contacts
                if (narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].reportContacts)
                {
                    // Compute the closest points between the inner capsule segment and the
                    // edge of the polyhedron in polyhedron local-space
                    PVector3 closestPointPolyhedronEdge;
                    PVector3  closestPointCapsuleInnerSegment;
                    PMath.computeClosestPointBetweenTwoSegments(capsuleSegAPolyhedronSpace, capsuleSegBPolyhedronSpace,
                        separatingPolyhedronEdgeVertex1, separatingPolyhedronEdgeVertex2,
                        out closestPointCapsuleInnerSegment, out closestPointPolyhedronEdge);

                    // Project closest capsule inner segment point into the capsule bounds
                    var contactPointCapsule = polyhedronToCapsuleTransform * closestPointCapsuleInnerSegment -
                                              separatingAxisCapsuleSpace * capsuleRadius;

                    // Compute smooth triangle mesh contact if one of the two collision shapes is a triangle
                    TriangleShape.computeSmoothTriangleMeshContact(
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2,
                        isCapsuleShape1 ? contactPointCapsule : closestPointPolyhedronEdge,
                        isCapsuleShape1 ? closestPointPolyhedronEdge : contactPointCapsule,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform,
                        minPenetrationDepth, normalWorld);

                    // Create the contact point
                    narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, minPenetrationDepth,
                        isCapsuleShape1 ? contactPointCapsule : closestPointPolyhedronEdge,
                        isCapsuleShape1 ? closestPointPolyhedronEdge : contactPointCapsule);
                }
            }

            return true;
        }

        
        // This method returns true if an edge of a polyhedron and a capsule forms a
        // face of the Minkowski Difference. This test is used to know if two edges
        // (one edge of the polyhedron vs the inner segment of the capsule in this case)
        // have to be test as a possible separating axis
        private bool isMinkowskiFaceCapsuleVsEdge(PVector3 capsuleSegment, PVector3 edgeAdjacentFace1Normal,
            PVector3 edgeAdjacentFace2Normal)
        {
            // Return true if the arc on the Gauss Map corresponding to the polyhedron edge
            // intersect the unit circle plane corresponding to capsule Gauss Map
            return capsuleSegment.dot(edgeAdjacentFace1Normal) * capsuleSegment.dot(edgeAdjacentFace2Normal) < 0.0f;
        }
        

        // Compute the penetration depth between the face of a polyhedron and a capsule along the polyhedron face normal direction
        private float computePolyhedronFaceVsCapsulePenetrationDepth(int polyhedronFaceIndex,
            ConvexPolyhedronShape polyhedron,
            CapsuleShape capsule, PTransform polyhedronToCapsuleTransform,
            PVector3 outFaceNormalCapsuleSpace)
        {
            // Get the face
            HalfEdgeStructure.Face face = polyhedron.GetFace(polyhedronFaceIndex);

            // Get the face normal
            PVector3 faceNormal = polyhedron.GetFaceNormal(polyhedronFaceIndex);

            // Compute the penetration depth (using the capsule support in the direction opposite to the face normal)
            outFaceNormalCapsuleSpace = polyhedronToCapsuleTransform.GetOrientation() * faceNormal;
            PVector3 capsuleSupportPoint = capsule.getLocalSupportPointWithMargin(-outFaceNormalCapsuleSpace);
            PVector3 pointOnPolyhedronFace =
                polyhedronToCapsuleTransform * polyhedron.GetVertexPosition(face.faceVertices[0]);
            var capsuleSupportPointToFacePoint = pointOnPolyhedronFace - capsuleSupportPoint;
            var penetrationDepth = capsuleSupportPointToFacePoint.dot(outFaceNormalCapsuleSpace);

            return penetrationDepth;
        }
        
        
        // Compute the penetration depth when the separating axis is the cross product of polyhedron edge and capsule inner segment
        private float computeEdgeVsCapsuleInnerSegmentPenetrationDepth(ConvexPolyhedronShape polyhedron,
            CapsuleShape capsule,
            PVector3 capsuleSegmentAxis, PVector3 edgeVertex1, PVector3 edgeDirectionCapsuleSpace,
            PTransform polyhedronToCapsuleTransform,out PVector3 outAxis)
        {
            float penetrationDepth = float.MaxValue;

            // Compute the axis to test (cross product between capsule inner segment and polyhedron edge)
            outAxis = capsuleSegmentAxis.cross(edgeDirectionCapsuleSpace);

            // Skip separating axis test if polyhedron edge is parallel to the capsule inner segment
            if (outAxis.LengthSquare() >= 0.00001f)
            {
                var polyhedronCentroid = polyhedronToCapsuleTransform * polyhedron.GetCentroid();
                var pointOnPolyhedronEdge = polyhedronToCapsuleTransform * edgeVertex1;

                // Swap axis direction if necessary such that it points out of the polyhedron
                if (outAxis.dot(pointOnPolyhedronEdge - polyhedronCentroid) < 0) outAxis = -outAxis;

                outAxis.Normalize();

                // Compute the penetration depth
                var capsuleSupportPoint = capsule.getLocalSupportPointWithMargin(-outAxis);
                var capsuleSupportPointToEdgePoint = pointOnPolyhedronEdge - capsuleSupportPoint;
                penetrationDepth = capsuleSupportPointToEdgePoint.dot(outAxis);
            }

            return penetrationDepth;
        }
    }
}