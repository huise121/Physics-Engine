using System.Collections.Generic;

namespace RP3D
{
    /// <summary>
    /// 执行中间相位碰撞检测。这一阶段可能会执行一些更复杂的几何计算，以确定物体之间的接触情况，但仍然不涉及具体的碰撞点或碰撞力的计算。
    /// </summary>
    public partial class CollisionDetectionSystem
    {
        // Compute the middle-phase collision detection
        private void computeMiddlePhase(NarrowPhaseInput narrowPhaseInput, bool needToReportContacts)
        {
            // Reserve memory for the narrow-phase input using cached capacity from previous frame
            narrowPhaseInput.reserveMemory();

            /*// Remove the obsolete last frame collision infos and mark all the others as obsolete
            mOverlappingPairs.clearObsoleteLastFrameCollisionInfos();*/

            // For each possible convex vs convex pair of bodies
            var nbConvexVsConvexPairs = mOverlappingPairs.mConvexPairs.Count;
            for (var i = 0; i < nbConvexVsConvexPairs; i++)
            {
                var overlappingPair = mOverlappingPairs.mConvexPairs[i];


                var collider1Entity = overlappingPair.Collider1;
                var collider2Entity = overlappingPair.Collider2;

                var collider1Index = mCollidersComponents.getEntityIndex(collider1Entity);
                var collider2Index = mCollidersComponents.getEntityIndex(collider2Entity);

                var collisionShape1 = mCollidersComponents.mCollisionShapes[collider1Index];
                var collisionShape2 = mCollidersComponents.mCollisionShapes[collider2Index];

                var algorithmType = overlappingPair.NarrowPhaseAlgorithmType;

                var isCollider1Trigger = mCollidersComponents.mIsTrigger[collider1Index];
                var isCollider2Trigger = mCollidersComponents.mIsTrigger[collider2Index];
                var reportContacts = needToReportContacts && !isCollider1Trigger && !isCollider2Trigger;

                // No middle-phase is necessary, simply create a narrow phase info
                // for the narrow-phase collision detection
                narrowPhaseInput.addNarrowPhaseTest(overlappingPair.PairID, collider1Entity, collider2Entity,
                    collisionShape1, collisionShape2,
                    mCollidersComponents.mLocalToWorldTransforms[collider1Index],
                    mCollidersComponents.mLocalToWorldTransforms[collider2Index],
                    algorithmType, reportContacts, overlappingPair.lastFrameCollisionInfo);

                overlappingPair.CollidingInCurrentFrame = false;
            }

            /*// 对于每对可能的凸体与凹体的碰撞对：
            var nbConcavePairs = mOverlappingPairs.mConcavePairs.Count;
            for (var i = 0; i < nbConcavePairs; i++)
            {
                var overlappingPair = mOverlappingPairs.mConcavePairs[i];

                computeConvexVsConcaveMiddlePhase(overlappingPair,
                    narrowPhaseInput, needToReportContacts);

                overlappingPair.CollidingInCurrentFrame = false;
            }*/
        }
        
        /*// 对给定的一对物体计算凹 vs 凸中间阶段算法。
        private void computeConvexVsConcaveMiddlePhase(ConcaveOverlappingPair overlappingPair,
            NarrowPhaseInput narrowPhaseInput, bool reportContacts)
        {
            Entity collider1 = overlappingPair.Collider1;
            Entity collider2 = overlappingPair.Collider2;
        
            int collider1Index = mCollidersComponents.getEntityIndex(collider1);
            int collider2Index = mCollidersComponents.getEntityIndex(collider2);
        
            PTransform shape1LocalToWorldTransform = mCollidersComponents.mLocalToWorldTransforms[collider1Index];
            PTransform shape2LocalToWorldTransform = mCollidersComponents.mLocalToWorldTransforms[collider2Index];
        
            PTransform convexToConcaveTransform;
        
            // Collision shape 1 is convex, collision shape 2 is concave
            ConvexShape convexShape;
            ConcaveShape concaveShape;
            if (overlappingPair.isShape1Convex)
            {
                convexShape = (ConvexShape)mCollidersComponents.mCollisionShapes[collider1Index];
                concaveShape = (ConcaveShape)mCollidersComponents.mCollisionShapes[collider2Index];
                convexToConcaveTransform = shape2LocalToWorldTransform.GetInverse() * shape1LocalToWorldTransform;
            }
            else
            {
                // Collision shape 2 is convex, collision shape 1 is concave
                convexShape = (ConvexShape)mCollidersComponents.mCollisionShapes[collider2Index];
                concaveShape = (ConcaveShape)mCollidersComponents.mCollisionShapes[collider1Index];
                convexToConcaveTransform = shape1LocalToWorldTransform.GetInverse() * shape2LocalToWorldTransform;
            }
        
        
            // Compute the convex shape AABB in the local-space of the concave shape
            var aabb = new AABB();
            convexShape.ComputeAABB(aabb, convexToConcaveTransform);
        
            // Compute the concave shape triangles that are overlapping with the convex mesh AABB
            var triangleVertices = new List<PVector3>(64);
            var triangleVerticesNormals = new List<PVector3>(64);
            var shapeIds = new List<int>(64);
            concaveShape.computeOverlappingTriangles(aabb, triangleVertices, triangleVerticesNormals, shapeIds);
        
        
            var isCollider1Trigger = mCollidersComponents.mIsTrigger[collider1Index];
            var isCollider2Trigger = mCollidersComponents.mIsTrigger[collider2Index];
            reportContacts = reportContacts && !isCollider1Trigger && !isCollider2Trigger;
        
            CollisionShape shape1;
            CollisionShape shape2;
        
            if (overlappingPair.isShape1Convex)
                shape1 = convexShape;
            else
                shape2 = convexShape;
        
            // For each overlapping triangle
            var nbShapeIds = shapeIds.Count;
            for (var i = 0; i < nbShapeIds; i++)
            {
                // Create a triangle collision shape (the allocated memory for the TriangleShape will be released in the
                // destructor of the corresponding NarrowPhaseInfo.
                TriangleShape triangleShape = new TriangleShape(triangleVertices[i * 3],triangleVerticesNormals[i * 3],shapeIds[i],mTriangleHalfEdgeStructure);
        
                if (overlappingPair.isShape1Convex)
                    shape2 = triangleShape;
                else
                    shape1 = triangleShape;
        
                // Add a collision info for the two collision shapes into the overlapping pair (if not present yet)
                LastFrameCollisionInfo lastFrameInfo = overlappingPair.AddLastFrameInfoIfNecessary(shape1.GetId(), shape2.GetId());
        
                // Create a narrow phase info for the narrow-phase collision detection
                narrowPhaseInput.addNarrowPhaseTest(overlappingPair.PairID, collider1, collider2, shape1, shape2,
                    shape1LocalToWorldTransform, shape2LocalToWorldTransform,
                    overlappingPair.NarrowPhaseAlgorithmType, reportContacts, lastFrameInfo);
            }
        }*/
        
        // Compute the middle-phase collision detection
        private void computeMiddlePhaseCollisionSnapshot(List<int> convexPairs, List<int> concavePairs,
            NarrowPhaseInput narrowPhaseInput, bool reportContacts)
        {
            // Reserve memory for the narrow-phase input using cached capacity from previous frame
            narrowPhaseInput.reserveMemory();

            /*// Remove the obsolete last frame collision infos and mark all the others as obsolete
            mOverlappingPairs.clearObsoleteLastFrameCollisionInfos();*/

            // For each possible convex vs convex pair of bodies
            var nbConvexPairs = convexPairs.Count;
            for (var p = 0; p < nbConvexPairs; p++)
            {
                var pairId = convexPairs[p];

                var pairIndex = mOverlappingPairs.mMapConvexPairIdToPairIndex[pairId];

                var collider1Entity = mOverlappingPairs.mConvexPairs[pairIndex].Collider1;
                var collider2Entity = mOverlappingPairs.mConvexPairs[pairIndex].Collider2;

                var collider1Index = mCollidersComponents.getEntityIndex(collider1Entity);
                var collider2Index = mCollidersComponents.getEntityIndex(collider2Entity);


                var collisionShape1 = mCollidersComponents.mCollisionShapes[collider1Index];
                var collisionShape2 = mCollidersComponents.mCollisionShapes[collider2Index];

                var algorithmType = mOverlappingPairs.mConvexPairs[pairIndex].NarrowPhaseAlgorithmType;

                // No middle-phase is necessary, simply create a narrow phase info
                // for the narrow-phase collision detection
                narrowPhaseInput.addNarrowPhaseTest(pairId, collider1Entity, collider2Entity, collisionShape1,
                    collisionShape2,
                    mCollidersComponents.mLocalToWorldTransforms[collider1Index],
                    mCollidersComponents.mLocalToWorldTransforms[collider2Index],
                    algorithmType, reportContacts, mOverlappingPairs.mConvexPairs[pairIndex].lastFrameCollisionInfo);
            }

            /*// 对于每一个可能的凸体与凹体对。
            var nbConcavePairs = concavePairs.Count;
            for (var p = 0; p < nbConcavePairs; p++)
            {
                var pairId = concavePairs[p];
                var pairIndex = mOverlappingPairs.mMapConcavePairIdToPairIndex[pairId];


                computeConvexVsConcaveMiddlePhase(mOverlappingPairs.mConcavePairs[pairIndex], narrowPhaseInput,
                    reportContacts);
            }*/
        }
        
    }
}