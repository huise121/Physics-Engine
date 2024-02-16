namespace RP3D
{
    public partial class SATAlgorithm
    {
        // Test collision between a sphere and a convex mesh
        public bool testCollisionSphereVsConvexPolyhedron(NarrowPhaseInfoBatch narrowPhaseInfoBatch,
            int batchStartIndex, int batchNbItems)
        {
            var isCollisionFound = false;


            for (var batchIndex = batchStartIndex; batchIndex < batchStartIndex + batchNbItems; batchIndex++)
            {
                var isSphereShape1 = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1.GetType() ==
                                     CollisionShapeType.SPHERE;


                var narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex];

                // Get the capsule collision shapes
                var sphere =
                    (SphereShape)(isSphereShape1 ? narrowPhaseInfo.collisionShape1 : narrowPhaseInfo.collisionShape2);
                var polyhedron =
                    (ConvexPolyhedronShape)(isSphereShape1
                        ? narrowPhaseInfo.collisionShape2
                        : narrowPhaseInfo.collisionShape1);

                var sphereToWorldTransform = isSphereShape1
                    ? narrowPhaseInfo.shape1ToWorldTransform
                    : narrowPhaseInfo.shape2ToWorldTransform;
                var polyhedronToWorldTransform = isSphereShape1
                    ? narrowPhaseInfo.shape2ToWorldTransform
                    : narrowPhaseInfo.shape1ToWorldTransform;

                // Get the transform from sphere local-space to polyhedron local-space
                var worldToPolyhedronTransform = polyhedronToWorldTransform.GetInverse();
                var sphereToPolyhedronSpaceTransform = worldToPolyhedronTransform * sphereToWorldTransform;

                // Transform the center of the sphere into the local-space of the convex polyhedron
                var sphereCenter = sphereToPolyhedronSpaceTransform.GetPosition();

                // Minimum penetration depth
                var minPenetrationDepth = float.MinValue;
                var minFaceIndex = 0;
                var noContact = false;

                // For each face of the convex mesh
                for (var f = 0; f < polyhedron.GetNbFaces(); f++)
                {
                    // Compute the penetration depth of the shapes along the face normal direction
                    var penetrationDepth =
                        computePolyhedronFaceVsSpherePenetrationDepth(f, polyhedron, sphere, sphereCenter);

                    // If the penetration depth is negative, we have found a separating axis
                    if (penetrationDepth <= 0.0f)
                    {
                        noContact = true;
                        break;
                    }

                    // Check if we have found a new minimum penetration axis
                    if (penetrationDepth < minPenetrationDepth)
                    {
                        minPenetrationDepth = penetrationDepth;
                        minFaceIndex = f;
                    }
                }

                if (noContact) continue;

                // If we need to report contacts
                if (narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].reportContacts)
                {
                    var minFaceNormal = polyhedron.GetFaceNormal(minFaceIndex);
                    var minFaceNormalWorld = polyhedronToWorldTransform.GetOrientation() * minFaceNormal;
                    var contactPointSphereLocal = sphereToWorldTransform.GetInverse().GetOrientation() *
                                                  (-minFaceNormalWorld * sphere.GetRadius());
                    var contactPointPolyhedronLocal =
                        sphereCenter + minFaceNormal * (minPenetrationDepth - sphere.GetRadius());

                    var normalWorld = isSphereShape1 ? -minFaceNormalWorld : minFaceNormalWorld;

                    // Compute smooth triangle mesh contact if one of the two collision shapes is a triangle
                    TriangleShape.computeSmoothTriangleMeshContact(
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape1,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].collisionShape2,
                        isSphereShape1 ? contactPointSphereLocal : contactPointPolyhedronLocal,
                        isSphereShape1 ? contactPointPolyhedronLocal : contactPointSphereLocal,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape1ToWorldTransform,
                        narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].shape2ToWorldTransform,
                        minPenetrationDepth, normalWorld);

                    // Create the contact info object
                    narrowPhaseInfoBatch.AddContactPoint(batchIndex, normalWorld, minPenetrationDepth,
                        isSphereShape1 ? contactPointSphereLocal : contactPointPolyhedronLocal,
                        isSphereShape1 ? contactPointPolyhedronLocal : contactPointSphereLocal);
                }

                narrowPhaseInfoBatch.narrowPhaseInfos[batchIndex].isColliding = true;
                isCollisionFound = true;
            }

            return isCollisionFound;
        }
        
        
        // Compute the penetration depth between a face of the polyhedron and a sphere along the polyhedron face normal direction
        private float computePolyhedronFaceVsSpherePenetrationDepth(int faceIndex, ConvexPolyhedronShape polyhedron,
            SphereShape sphere, PVector3 sphereCenter)
        {
            // Get the face
            HalfEdgeStructure.Face face = polyhedron.GetFace(faceIndex);
            // Get the face normal
            PVector3 faceNormal = polyhedron.GetFaceNormal(faceIndex);
            var sphereCenterToFacePoint = polyhedron.GetVertexPosition(face.faceVertices[0]) - sphereCenter;
            var penetrationDepth = sphereCenterToFacePoint.dot(faceNormal) + sphere.GetRadius();
            return penetrationDepth;
        }

    }
}