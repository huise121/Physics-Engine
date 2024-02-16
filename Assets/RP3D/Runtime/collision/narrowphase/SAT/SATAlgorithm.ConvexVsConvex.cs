using System.Collections.Generic;

namespace RP3D
{
    public partial class SATAlgorithm
    {
        // 测试2个凸形状 之间的碰撞
        public bool testCollisionConvexPolyhedronVsConvexPolyhedron(NarrowPhaseInfoBatch narrowPhaseInfoBatch,
            int batchStartIndex, int batchNbItems)
        {
            var isCollisionFound = false;

            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++)
            {
                NarrowPhaseInfo tempNarrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];
                
                var polyhedron1 =
                    (ConvexPolyhedronShape)tempNarrowPhaseInfo.collisionShape1;
                var polyhedron2 =
                    (ConvexPolyhedronShape)tempNarrowPhaseInfo.collisionShape2;

                var polyhedron1ToPolyhedron2 = tempNarrowPhaseInfo.shape2ToWorldTransform.GetInverse() * tempNarrowPhaseInfo.shape1ToWorldTransform;
                var polyhedron2ToPolyhedron1 = polyhedron1ToPolyhedron2.GetInverse();

                var minPenetrationDepth = float.MaxValue;
                var minFaceIndex = 0;
                var isMinPenetrationFaceNormal = false;
                var isMinPenetrationFaceNormalPolyhedron1 = false;
                var minSeparatingEdge1Index = 0;
                var minSeparatingEdge2Index = 0;
                var separatingEdge1A = PVector3.Zero();
                var separatingEdge1B = PVector3.Zero();
                var separatingEdge2A = PVector3.Zero();
                var separatingEdge2B = PVector3.Zero();
                var minEdgeVsEdgeSeparatingAxisPolyhedron2Space = PVector3.Zero();
                
                var isShape1Triangle = polyhedron1.GetName() == CollisionShapeName.TRIANGLE;

                var lastFrameCollisionInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].lastFrameCollisionInfo;

                // If the last frame collision info is valid and was also using SAT algorithm
                if (lastFrameCollisionInfo.isValid && lastFrameCollisionInfo.wasUsingSAT)
                {
                    // We perform temporal coherence, we check if there is still an overlapping along the previous minimum separating
                    // axis. If it is the case, we directly report the collision without executing the whole SAT algorithm again. If
                    // the shapes are still separated along this axis, we directly exit with no collision.

                    // If the previous separating axis (or axis with minimum penetration depth)
                    // was a face normal of polyhedron 1
                    if (lastFrameCollisionInfo.satIsAxisFacePolyhedron1)
                    {
                        var penetrationDepth = testSingleFaceDirectionPolyhedronVsPolyhedron(polyhedron1, polyhedron2,
                            polyhedron1ToPolyhedron2,
                            lastFrameCollisionInfo.satMinAxisFaceIndex);

                        // If the previous axis was a separating axis and is still a separating axis in this frame
                        if (!lastFrameCollisionInfo.wasColliding && penetrationDepth <= 0.0f)
                            // Return no collision without running the whole SAT algorithm
                            continue;

                        // The two shapes were overlapping in the previous frame and still seem to overlap in this one
                        if (lastFrameCollisionInfo.wasColliding && mClipWithPreviousAxisIfStillColliding &&
                            penetrationDepth > 0.0f)
                        {
                            minPenetrationDepth = penetrationDepth;
                            minFaceIndex = lastFrameCollisionInfo.satMinAxisFaceIndex;
                            isMinPenetrationFaceNormal = true;
                            isMinPenetrationFaceNormalPolyhedron1 = true;

                            // Compute the contact points between two faces of two convex polyhedra.
                            if (computePolyhedronVsPolyhedronFaceContactPoints(isMinPenetrationFaceNormalPolyhedron1,
                                    polyhedron1, polyhedron2,
                                    polyhedron1ToPolyhedron2, polyhedron2ToPolyhedron1, minFaceIndex,
                                    narrowPhaseInfoBatch, batchIndex))
                            {
                                lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = isMinPenetrationFaceNormalPolyhedron1;
                                lastFrameCollisionInfo.satIsAxisFacePolyhedron2 =
                                    !isMinPenetrationFaceNormalPolyhedron1;
                                lastFrameCollisionInfo.satMinAxisFaceIndex = minFaceIndex;

                                // The shapes are still overlapping in the previous axis (the contact manifold is not empty).
                                // Therefore, we can return without running the whole SAT algorithm
                                narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].isColliding = true;
                                isCollisionFound = true;
                                continue;
                            }

                            // The contact manifold is empty. Therefore, we have to run the whole SAT algorithm again
                        }
                    }
                    else if (lastFrameCollisionInfo.satIsAxisFacePolyhedron2)
                    {
                        // If the previous separating axis (or axis with minimum penetration depth)
                        // was a face normal of polyhedron 2

                        var penetrationDepth = testSingleFaceDirectionPolyhedronVsPolyhedron(polyhedron2, polyhedron1,
                            polyhedron2ToPolyhedron1,
                            lastFrameCollisionInfo.satMinAxisFaceIndex);

                        // If the previous axis was a separating axis and is still a separating axis in this frame
                        if (!lastFrameCollisionInfo.wasColliding && penetrationDepth <= 0.0f)
                            // Return no collision without running the whole SAT algorithm
                            continue;

                        // The two shapes were overlapping in the previous frame and still seem to overlap in this one
                        if (lastFrameCollisionInfo.wasColliding && mClipWithPreviousAxisIfStillColliding &&
                            penetrationDepth > 0.0f)
                        {
                            minPenetrationDepth = penetrationDepth;
                            minFaceIndex = lastFrameCollisionInfo.satMinAxisFaceIndex;
                            isMinPenetrationFaceNormal = true;
                            isMinPenetrationFaceNormalPolyhedron1 = false;

                            // Compute the contact points between two faces of two convex polyhedra.
                            if (computePolyhedronVsPolyhedronFaceContactPoints(isMinPenetrationFaceNormalPolyhedron1,
                                    polyhedron1, polyhedron2,
                                    polyhedron1ToPolyhedron2, polyhedron2ToPolyhedron1, minFaceIndex,
                                    narrowPhaseInfoBatch, batchIndex))
                            {
                                lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = isMinPenetrationFaceNormalPolyhedron1;
                                lastFrameCollisionInfo.satIsAxisFacePolyhedron2 =
                                    !isMinPenetrationFaceNormalPolyhedron1;
                                lastFrameCollisionInfo.satMinAxisFaceIndex = minFaceIndex;

                                // The shapes are still overlapping in the previous axis (the contact manifold is not empty).
                                // Therefore, we can return without running the whole SAT algorithm
                                narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].isColliding = true;
                                isCollisionFound = true;
                                continue;
                            }

                            // The contact manifold is empty. Therefore, we have to run the whole SAT algorithm again
                        }
                    }
                    else
                    {
                        // If the previous separating axis (or axis with minimum penetration depth) was the cross product of two edges

                        var edge1 = polyhedron1.getHalfEdge(lastFrameCollisionInfo.satMinEdge1Index);
                        var edge2 = polyhedron2.getHalfEdge(lastFrameCollisionInfo.satMinEdge2Index);

                        var edge1A = polyhedron1ToPolyhedron2 * polyhedron1.GetVertexPosition(edge1.vertexIndex);
                        var edge1B = polyhedron1ToPolyhedron2 *
                                     polyhedron1.GetVertexPosition(polyhedron1.getHalfEdge(edge1.nextEdgeIndex)
                                         .vertexIndex);
                        var edge1Direction = edge1B - edge1A;
                        var edge2A = polyhedron2.GetVertexPosition(edge2.vertexIndex);
                        var edge2B =
                            polyhedron2.GetVertexPosition(polyhedron2.getHalfEdge(edge2.nextEdgeIndex).vertexIndex);
                        var edge2Direction = edge2B - edge2A;

                        // If the two edges build a minkowski face (and the cross product is
                        // therefore a candidate for separating axis
                        if (testEdgesBuildMinkowskiFace(polyhedron1, edge1, polyhedron2, edge2,
                                polyhedron1ToPolyhedron2))
                        {
                            PVector3 separatingAxisPolyhedron2Space;

                            // Compute the penetration depth along the previous axis
                            var polyhedron1Centroid = polyhedron1ToPolyhedron2 * polyhedron1.GetCentroid();
                            var penetrationDepth = computeDistanceBetweenEdges(edge1A, edge2A, polyhedron1Centroid,
                                polyhedron2.GetCentroid(),
                                edge1Direction, edge2Direction, isShape1Triangle,out separatingAxisPolyhedron2Space);

                            // If the shapes were not overlapping in the previous frame and are still not
                            // overlapping in the current one
                            if (!lastFrameCollisionInfo.wasColliding && penetrationDepth <= 0.0f)
                                // We have found a separating axis without running the whole SAT algorithm
                                continue;

                            // If the shapes were overlapping on the previous axis and still seem to overlap in this frame
                            if (lastFrameCollisionInfo.wasColliding && mClipWithPreviousAxisIfStillColliding &&
                                penetrationDepth > 0.0f &&
                                penetrationDepth < float.MaxValue)
                            {
                                // Compute the closest points between the two edges (in the local-space of poylhedron 2)
                                PVector3 closestPointPolyhedron1Edge;
                                PVector3 closestPointPolyhedron2Edge;
                                PMath.computeClosestPointBetweenTwoSegments(edge1A, edge1B, edge2A, edge2B,
                                    out closestPointPolyhedron1Edge, out closestPointPolyhedron2Edge);

                                // Here we try to project the closest point on edge1 onto the segment of edge 2 to see if
                                // the projected point falls onto the segment. We also try to project the closest point
                                // on edge 2 to see if it falls onto the segment of edge 1. If one of the point does not
                                // fall onto the opposite segment, it means the edges are not colliding (the contact manifold
                                // is empty). Therefore, we need to run the whole SAT algorithm again.
                                var vec1 = closestPointPolyhedron1Edge - edge2A;
                                var vec2 = closestPointPolyhedron2Edge - edge1A;
                                var edge1LengthSquare = edge1Direction.LengthSquare();
                                var edge2LengthSquare = edge2Direction.LengthSquare();
                                var t1 = vec1.dot(edge2Direction) / edge2LengthSquare;
                                var t2 = vec2.dot(edge1Direction) / edge1LengthSquare;
                                if (t1 >= 0.0f && t1 <= 1 && t2 >= 0.0 && t2 <= 1.0)
                                {
                                    // If we need to report contact points
                                    if (narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].reportContacts)
                                    {
                                        // Compute the contact point on polyhedron 1 edge in the local-space of polyhedron 1
                                        var closestPointPolyhedron1EdgeLocalSpace =
                                            polyhedron2ToPolyhedron1 * closestPointPolyhedron1Edge;

                                        // Compute the world normal
                                        var normalWorld =
                                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform
                                                .GetOrientation() * separatingAxisPolyhedron2Space;

                                        // Compute smooth triangle mesh contact if one of the two collision shapes is a triangle
                                        TriangleShape.computeSmoothTriangleMeshContact(
                                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1,
                                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2,
                                            closestPointPolyhedron1EdgeLocalSpace, closestPointPolyhedron2Edge,
                                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform,
                                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform,
                                            penetrationDepth, normalWorld);

                                        // Create the contact point
                                        narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, penetrationDepth,
                                            closestPointPolyhedron1EdgeLocalSpace, closestPointPolyhedron2Edge);
                                    }

                                    // The shapes are overlapping on the previous axis (the contact manifold is not empty). Therefore
                                    // we return without running the whole SAT algorithm
                                    narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].isColliding = true;
                                    isCollisionFound = true;
                                    continue;
                                }

                                // The contact manifold is empty. Therefore, we have to run the whole SAT algorithm again
                            }
                        }
                    }
                }

                minPenetrationDepth = float.MaxValue;
                isMinPenetrationFaceNormal = false;

                // Test all the face normals of the polyhedron 1 for separating axis
                int faceIndex1 = 0;
                var penetrationDepth1 = testFacesDirectionPolyhedronVsPolyhedron(polyhedron1, polyhedron2,
                    polyhedron1ToPolyhedron2, ref faceIndex1);
                if (penetrationDepth1 <= 0.0f)
                {
                    lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = true;
                    lastFrameCollisionInfo.satIsAxisFacePolyhedron2 = false;
                    lastFrameCollisionInfo.satMinAxisFaceIndex = faceIndex1;

                    // We have found a separating axis
                    continue;
                }

                // Test all the face normals of the polyhedron 2 for separating axis
                int faceIndex2 = 0;
                var penetrationDepth2 = testFacesDirectionPolyhedronVsPolyhedron(polyhedron2, polyhedron1,
                    polyhedron2ToPolyhedron1, ref faceIndex2);
                if (penetrationDepth2 <= 0.0f)
                {
                    lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = false;
                    lastFrameCollisionInfo.satIsAxisFacePolyhedron2 = true;
                    lastFrameCollisionInfo.satMinAxisFaceIndex = faceIndex2;

                    // We have found a separating axis
                    continue;
                }

                // Here we know that we have found penetration along both axis of a face of polyhedron1 and a face of
                // polyhedron2. If the two penetration depths are almost the same, we need to make sure we always prefer
                // one axis to the other for consistency between frames. This is to prevent the contact manifolds to switch
                // from one reference axis to the other for a face to face resting contact for instance. This is better for
                // stability. To do this, we use a relative and absolute bias to move penetrationDepth2 a little bit to the right.
                // Now if:
                //  penetrationDepth1 < penetrationDepth2: Nothing happens and we use axis of polygon 1
                //  penetrationDepth1 ~ penetrationDepth2: Until penetrationDepth1 becomes significantly less than penetrationDepth2 we still use axis of polygon 1
                //  penetrationDepth1 >> penetrationDepth2: penetrationDepth2 is now significantly less than penetrationDepth1 and we use polygon 2 axis
                if (penetrationDepth1 < penetrationDepth2 * SEPARATING_AXIS_RELATIVE_TOLERANCE +
                    SEPARATING_AXIS_ABSOLUTE_TOLERANCE)
                {
                    // We use penetration axis of polygon 1
                    isMinPenetrationFaceNormal = true;
                    minPenetrationDepth = PMath.Min(penetrationDepth1, penetrationDepth2);
                    minFaceIndex = faceIndex1;
                    isMinPenetrationFaceNormalPolyhedron1 = true;
                }
                else
                {
                    // We use penetration axis of polygon 2
                    isMinPenetrationFaceNormal = true;
                    minPenetrationDepth = PMath.Min(penetrationDepth1, penetrationDepth2);
                    minFaceIndex = faceIndex2;
                    isMinPenetrationFaceNormalPolyhedron1 = false;
                }

                var separatingAxisFound = false;

                // Test the cross products of edges of polyhedron 1 with edges of polyhedron 2 for separating axis
                for (var i = 0; i < polyhedron1.GetNbHalfEdges(); i += 2)
                {
                    // Get an edge of polyhedron 1
                    var edge1 = polyhedron1.getHalfEdge(i);

                    var edge1A = polyhedron1ToPolyhedron2 * polyhedron1.GetVertexPosition(edge1.vertexIndex);
                    var edge1B = polyhedron1ToPolyhedron2 *
                                 polyhedron1.GetVertexPosition(polyhedron1.getHalfEdge(edge1.nextEdgeIndex)
                                     .vertexIndex);
                    var edge1Direction = edge1B - edge1A;

                    for (var j = 0; j < polyhedron2.GetNbHalfEdges(); j += 2)
                    {
                        // Get an edge of polyhedron 2
                        var edge2 = polyhedron2.getHalfEdge(j);

                        var edge2A = polyhedron2.GetVertexPosition(edge2.vertexIndex);
                        var edge2B =
                            polyhedron2.GetVertexPosition(polyhedron2.getHalfEdge(edge2.nextEdgeIndex).vertexIndex);
                        var edge2Direction = edge2B - edge2A;

                        // If the two edges build a minkowski face (and the cross product is
                        // therefore a candidate for separating axis
                        if (testEdgesBuildMinkowskiFace(polyhedron1, edge1, polyhedron2, edge2,
                                polyhedron1ToPolyhedron2))
                        {
                            var separatingAxisPolyhedron2Space = PVector3.Zero();

                            // Compute the penetration depth
                            var polyhedron1Centroid = polyhedron1ToPolyhedron2 * polyhedron1.GetCentroid();
                            var penetrationDepth = computeDistanceBetweenEdges(edge1A, edge2A, polyhedron1Centroid,
                                polyhedron2.GetCentroid(),
                                edge1Direction, edge2Direction, isShape1Triangle,out separatingAxisPolyhedron2Space);

                            if (penetrationDepth <= 0.0f)
                            {
                                lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = false;
                                lastFrameCollisionInfo.satIsAxisFacePolyhedron2 = false;
                                lastFrameCollisionInfo.satMinEdge1Index = i;
                                lastFrameCollisionInfo.satMinEdge2Index = j;

                                // We have found a separating axis
                                separatingAxisFound = true;
                                break;
                            }

                            // If the current minimum penetration depth is along a face normal axis (isMinPenetrationFaceNormal=true) and we have found a new
                            // smaller pentration depth along an edge-edge cross-product axis we want to favor the face normal axis because contact manifolds between
                            // faces have more contact points and therefore more stable than the single contact point of an edge-edge collision. It means that if the new minimum
                            // penetration depth from the edge-edge contact is only a little bit smaller than the current minPenetrationDepth (from a face contact), we favor
                            // the face contact and do not generate an edge-edge contact. However, if the new penetration depth from the edge-edge contact is really smaller than
                            // the current one, we generate an edge-edge contact.
                            // To do this, we use a relative and absolute bias to increase a little bit the new penetration depth from the edge-edge contact during the comparison test
                            if ((isMinPenetrationFaceNormal &&
                                 penetrationDepth1 * SEPARATING_AXIS_RELATIVE_TOLERANCE +
                                 SEPARATING_AXIS_ABSOLUTE_TOLERANCE < minPenetrationDepth) ||
                                (!isMinPenetrationFaceNormal && penetrationDepth < minPenetrationDepth))
                            {
                                minPenetrationDepth = penetrationDepth;
                                isMinPenetrationFaceNormalPolyhedron1 = false;
                                isMinPenetrationFaceNormal = false;
                                minSeparatingEdge1Index = i;
                                minSeparatingEdge2Index = j;
                                separatingEdge1A = edge1A;
                                separatingEdge1B = edge1B;
                                separatingEdge2A = edge2A;
                                separatingEdge2B = edge2B;
                                minEdgeVsEdgeSeparatingAxisPolyhedron2Space = separatingAxisPolyhedron2Space;
                            }
                        }
                    }

                    if (separatingAxisFound) break;
                }

                if (separatingAxisFound) continue;

                // Here we know the shapes are overlapping on a given minimum separating axis.
                // Now, we will clip the shapes along this axis to find the contact points


                // If the minimum separating axis is a face normal
                if (isMinPenetrationFaceNormal)
                {
                    // Compute the contact points between two faces of two convex polyhedra.
                    var contactsFound = computePolyhedronVsPolyhedronFaceContactPoints(
                        isMinPenetrationFaceNormalPolyhedron1, polyhedron1,
                        polyhedron2, polyhedron1ToPolyhedron2, polyhedron2ToPolyhedron1,
                        minFaceIndex, narrowPhaseInfoBatch, batchIndex);

                    // There should be clipping points here. If it is not the case, it might be
                    // because of a numerical issue
                    if (!contactsFound)
                    {
                        lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = isMinPenetrationFaceNormalPolyhedron1;
                        lastFrameCollisionInfo.satIsAxisFacePolyhedron2 = !isMinPenetrationFaceNormalPolyhedron1;
                        lastFrameCollisionInfo.satMinAxisFaceIndex = minFaceIndex;

                        // Return no collision
                        continue;
                    }

                    lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = isMinPenetrationFaceNormalPolyhedron1;
                    lastFrameCollisionInfo.satIsAxisFacePolyhedron2 = !isMinPenetrationFaceNormalPolyhedron1;
                    lastFrameCollisionInfo.satMinAxisFaceIndex = minFaceIndex;
                }
                else
                {
                    // If we have an edge vs edge contact

                    // If we need to report contacts
                    if (narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].reportContacts)
                    {
                        // Compute the closest points between the two edges (in the local-space of poylhedron 2)
                        PVector3 closestPointPolyhedron1Edge;
                        PVector3 closestPointPolyhedron2Edge;
                        PMath.computeClosestPointBetweenTwoSegments(separatingEdge1A, separatingEdge1B,
                            separatingEdge2A,separatingEdge2B,
                             out closestPointPolyhedron1Edge, out closestPointPolyhedron2Edge);

                        // Compute the contact point on polyhedron 1 edge in the local-space of polyhedron 1
                        var closestPointPolyhedron1EdgeLocalSpace =
                            polyhedron2ToPolyhedron1 * closestPointPolyhedron1Edge;

                        // Compute the world normal
                        var normalWorld =
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform.GetOrientation() *
                            minEdgeVsEdgeSeparatingAxisPolyhedron2Space;

                        // Compute smooth triangle mesh contact if one of the two collision shapes is a triangle
                        TriangleShape.computeSmoothTriangleMeshContact(
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1,
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2,
                            closestPointPolyhedron1EdgeLocalSpace, closestPointPolyhedron2Edge,
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform,
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform,
                            minPenetrationDepth, normalWorld);

                        // Create the contact point
                        narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, minPenetrationDepth,
                            closestPointPolyhedron1EdgeLocalSpace, closestPointPolyhedron2Edge);
                    }

                    lastFrameCollisionInfo.satIsAxisFacePolyhedron1 = false;
                    lastFrameCollisionInfo.satIsAxisFacePolyhedron2 = false;
                    lastFrameCollisionInfo.satMinEdge1Index = minSeparatingEdge1Index;
                    lastFrameCollisionInfo.satMinEdge2Index = minSeparatingEdge2Index;
                }

                narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].isColliding = true;
                isCollisionFound = true;
            }

            return isCollisionFound;
        }
        
        
        // Compute the contact points between two faces of two convex polyhedra.
        /// The method returns true if contact points have been found
        private bool computePolyhedronVsPolyhedronFaceContactPoints(bool isMinPenetrationFaceNormalPolyhedron1,
            ConvexPolyhedronShape polyhedron1, ConvexPolyhedronShape polyhedron2,
            PTransform polyhedron1ToPolyhedron2, PTransform polyhedron2ToPolyhedron1,
            int minFaceIndex, NarrowPhaseInfoBatch narrowPhaseInfoBatch, int batchIndex)
        {
            ConvexPolyhedronShape referencePolyhedron;
            ConvexPolyhedronShape incidentPolyhedron;
            var referenceToIncidentTransform = isMinPenetrationFaceNormalPolyhedron1
                ? polyhedron1ToPolyhedron2
                : polyhedron2ToPolyhedron1;
            var incidentToReferenceTransform = isMinPenetrationFaceNormalPolyhedron1
                ? polyhedron2ToPolyhedron1
                : polyhedron1ToPolyhedron2;

            if (isMinPenetrationFaceNormalPolyhedron1)
            {
                referencePolyhedron = polyhedron1;
                incidentPolyhedron = polyhedron2;
            }
            else
            {
                referencePolyhedron = polyhedron2;
                incidentPolyhedron = polyhedron1;
            }

            var axisReferenceSpace = referencePolyhedron.GetFaceNormal(minFaceIndex);
            var axisIncidentSpace = referenceToIncidentTransform.GetOrientation() * axisReferenceSpace;

            // Compute the world normal
            var normalWorld = isMinPenetrationFaceNormalPolyhedron1
                ? narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform.GetOrientation() *
                  axisReferenceSpace
                : -(narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform.GetOrientation() *
                    axisReferenceSpace);

            // Get the reference face
            HalfEdgeStructure.Face referenceFace = referencePolyhedron.GetFace(minFaceIndex);

            // Find the incident face on the other polyhedron (most anti-parallel face)
            int incidentFaceIndex = incidentPolyhedron.findMostAntiParallelFace(axisIncidentSpace);

            // Get the incident face
            HalfEdgeStructure.Face incidentFace = incidentPolyhedron.GetFace(incidentFaceIndex);

            var nbIncidentFaceVertices = incidentFace.faceVertices.Count;
            var nbMaxElements = nbIncidentFaceVertices * 2 * referenceFace.faceVertices.Count;

            var verticesTemp1 = new List<PVector3>(nbMaxElements);
            var verticesTemp2 = new List<PVector3>(nbMaxElements);

            // Get all the vertices of the incident face (in the reference local-space)
            for (var i = 0; i < nbIncidentFaceVertices; i++)
            {
                var faceVertexIncidentSpace = incidentPolyhedron.GetVertexPosition(incidentFace.faceVertices[i]);
                verticesTemp1.Add(incidentToReferenceTransform * faceVertexIncidentSpace);
            }

            // For each edge of the reference we use it to clip the incident face polygon using Sutherland-Hodgman algorithm
            var firstEdgeIndex = referenceFace.edgeIndex;
            var areVertices1Input = false;
            int nbOutputVertices;
            int currentEdgeIndex;

            // Get the adjacent edge
            HalfEdgeStructure.Edge currentEdge = referencePolyhedron.getHalfEdge(firstEdgeIndex);
            var edgeV1 = referencePolyhedron.GetVertexPosition(currentEdge.vertexIndex);

            do
            {
                // Switch the input/output arrays of vertices
                areVertices1Input = !areVertices1Input;

                // Get the adjacent edge
                HalfEdgeStructure.Edge nextEdge = referencePolyhedron.getHalfEdge(currentEdge.nextEdgeIndex);

                // Compute the edge vertices and edge direction
                var edgeV2 = referencePolyhedron.GetVertexPosition(nextEdge.vertexIndex);
                var edgeDirection = edgeV2 - edgeV1;

                // Compute the normal of the clipping plane for this edge
                // The clipping plane is perpendicular to the edge direction and the reference face normal
                var planeNormal = axisReferenceSpace.cross(edgeDirection);


                // Clip the incident face with one adjacent plane (corresponding to one edge) of the reference face
                PMath.clipPolygonWithPlane(areVertices1Input ? verticesTemp1 : verticesTemp2, edgeV1, planeNormal,
                    areVertices1Input ? verticesTemp2 : verticesTemp1);

                currentEdgeIndex = currentEdge.nextEdgeIndex;

                // Go to the next adjacent edge of the reference face
                currentEdge = nextEdge;
                edgeV1 = edgeV2;

                // Clear the input array of vertices before the next loop
                if (areVertices1Input)
                {
                    verticesTemp1.Clear();
                    nbOutputVertices = verticesTemp2.Count;
                }
                else
                {
                    verticesTemp2.Clear();
                    nbOutputVertices = verticesTemp1.Count;
                }
            } while (currentEdgeIndex != firstEdgeIndex && nbOutputVertices > 0);

            // Reference to the output clipped polygon vertices
            var clippedPolygonVertices = areVertices1Input ? verticesTemp2 : verticesTemp1;

            // We only keep the clipped points that are below the reference face
            var referenceFaceVertex =
                referencePolyhedron.GetVertexPosition(referencePolyhedron.getHalfEdge(firstEdgeIndex).vertexIndex);
            var contactPointsFound = false;
            var nbClipPolygonVertices = clippedPolygonVertices.Count;
            for (var i = 0; i < nbClipPolygonVertices; i++)
            {
                // Compute the penetration depth of this contact point (can be different from the minPenetration depth which is
                // the maximal penetration depth of any contact point for this separating axis
                var penetrationDepth = (referenceFaceVertex - clippedPolygonVertices[i]).dot(axisReferenceSpace);

                // If the clip point is below the reference face
                if (penetrationDepth > 0.0f)
                {
                    contactPointsFound = true;

                    // If we need to report contacts
                    if (narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].reportContacts)
                    {
                        var outWorldNormal = normalWorld;

                        // Convert the clip incident polyhedron vertex into the incident polyhedron local-space
                        var contactPointIncidentPolyhedron = referenceToIncidentTransform * clippedPolygonVertices[i];

                        // Project the contact point onto the reference face
                        PVector3 contactPointReferencePolyhedron = PMath.projectPointOntoPlane(clippedPolygonVertices[i],
                            axisReferenceSpace, referenceFaceVertex);

                        // Compute smooth triangle mesh contact if one of the two collision shapes is a triangle
                        TriangleShape.computeSmoothTriangleMeshContact(
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1,
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2,
                            isMinPenetrationFaceNormalPolyhedron1
                                ? contactPointReferencePolyhedron
                                : contactPointIncidentPolyhedron,
                            isMinPenetrationFaceNormalPolyhedron1
                                ? contactPointIncidentPolyhedron
                                : contactPointReferencePolyhedron,
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform,
                            narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform,
                            penetrationDepth, outWorldNormal);

                        // Create a new contact point
                        narrowPhaseInfoBatch.AddContactPoint(batchIndex, outWorldNormal, penetrationDepth,
                            isMinPenetrationFaceNormalPolyhedron1
                                ? contactPointReferencePolyhedron
                                : contactPointIncidentPolyhedron,
                            isMinPenetrationFaceNormalPolyhedron1
                                ? contactPointIncidentPolyhedron
                                : contactPointReferencePolyhedron);
                    }
                }
            }

            return contactPointsFound;
        }
        
        


        // Compute and return the distance between the two edges in the direction of the candidate separating axis
        private float computeDistanceBetweenEdges(PVector3 edge1A, PVector3 edge2A,
            PVector3 polyhedron1Centroid, PVector3 polyhedron2Centroid,
            PVector3 edge1Direction, PVector3 edge2Direction,
            bool isShape1Triangle,out PVector3 outSeparatingAxisPolyhedron2Space)
        {
            outSeparatingAxisPolyhedron2Space = PVector3.Zero();
            // If the two edges are parallel
            if (PMath.areParallelVectors(edge1Direction, edge2Direction))
                // Return a large penetration depth to skip those edges
                return float.MaxValue;

            // Compute the candidate separating axis (cross product between two polyhedrons edges)
            var axis = edge1Direction.cross(edge2Direction).GetUnit();

            // Make sure the axis direction is going from first to second polyhedron
            float dotProd;
            if (isShape1Triangle)
                // The shape 1 is a triangle. It is safer to use a vector from
                // centroid to edge of the shape 2 because for a triangle, we
                // can have a vector that is orthogonal to the axis
                dotProd = axis.dot(edge2A - polyhedron2Centroid);
            else
                // The shape 2 might be a triangle. It is safer to use a vector from
                // centroid to edge of the shape 2 because for a triangle, we
                // can have a vector that is orthogonal to the axis
                dotProd = axis.dot(polyhedron1Centroid - edge1A);
            if (dotProd > 0.0f) axis = -axis;

            outSeparatingAxisPolyhedron2Space = axis;

            // Compute and return the distance between the edges
            return -axis.dot(edge2A - edge1A);
        }
        
        
    }
}