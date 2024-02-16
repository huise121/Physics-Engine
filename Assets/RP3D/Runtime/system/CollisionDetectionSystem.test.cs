using System.Collections.Generic;

namespace RP3D
{
    public partial class CollisionDetectionSystem
    {
        // 为 testCollision() 方法创建实际的接触曲面和接触点。
        public void createSnapshotContacts(List<ContactPair> contactPairs,
            List<ContactManifold> contactManifolds,
            List<ContactPoint> contactPoints,
            List<ContactManifoldInfo> potentialContactManifolds,
            List<ContactPointInfo> potentialContactPoints)
        {
            // contactManifolds.reserve(contactPairs.Count);
            // contactPoints.reserve(contactManifolds.Count);

            // For each contact pair
            var nbContactPairs = contactPairs.Count;
            for (var p = 0; p < nbContactPairs; p++)
            {
                var contactPair = contactPairs[p];

                contactPair.ContactManifoldsIndex = contactManifolds.Count;
                contactPair.NbContactManifolds = contactPair.NbPotentialContactManifolds;
                contactPair.ContactPointsIndex = contactPoints.Count;

                // For each potential contact manifold of the pair
                for (var m = 0; m < contactPair.NbPotentialContactManifolds; m++)
                {
                    var potentialManifold = potentialContactManifolds[contactPair.PotentialContactManifoldsIndices[m]];

                    // Start index and number of contact points for this manifold
                    var contactPointsIndex = contactPoints.Count;
                    var nbContactPoints = potentialManifold.nbPotentialContactPoints;
                    contactPair.NbTotalContactPoints += nbContactPoints;

                    // Create and add the contact manifold
                    contactManifolds.Add(new ContactManifold(contactPair.Body1Entity, contactPair.Body2Entity,
                        contactPair.Collider1Entity,
                        contactPair.Collider2Entity, contactPointsIndex, nbContactPoints));


                    // For each contact point of the manifold
                    for (var c = 0; c < potentialManifold.nbPotentialContactPoints; c++)
                    {
                        var potentialContactPoint =
                            potentialContactPoints[potentialManifold.potentialContactPointsIndices[c]];

                        // Create a new contact point
                        var contactPoint = new ContactPoint(potentialContactPoint,
                            mWorld.mConfig.persistentContactDistanceThreshold);

                        // Add the contact point
                        contactPoints.Add(contactPoint);
                    }
                }
            }
        }
        
        // 为 testCollision() 方法计算窄相碰撞检测。
        // 如果找到接触点，则返回 true。
        public bool computeNarrowPhaseCollisionSnapshot(NarrowPhaseInput narrowPhaseInput, CollisionCallback callback)
        {
            // Test the narrow-phase collision detection on the batches to be tested
            var collisionFound = testNarrowPhaseCollision(narrowPhaseInput, false);

            // If collision has been found, create contacts
            if (collisionFound)
            {
                var potentialContactPoints = new List<ContactPointInfo>();
                var potentialContactManifolds = new List<ContactManifoldInfo>();
                var contactPairs = new List<ContactPair>();
                var lostContactPairs = new List<ContactPair>(); // Not used during collision snapshots
                var contactManifolds = new List<ContactManifold>();
                var contactPoints = new List<ContactPoint>();

                // 在狭窄阶段碰撞之后处理所有潜在的接触
                processAllPotentialContacts(narrowPhaseInput, true, potentialContactPoints, potentialContactManifolds,
                    contactPairs);

                // 减少几何体接触面的接触点数量
                reducePotentialContactManifolds(contactPairs, potentialContactManifolds, potentialContactPoints);

                // 创建实际的接触几何体和接触点
                createSnapshotContacts(contactPairs, contactManifolds, contactPoints, potentialContactManifolds,
                    potentialContactPoints);

                // 向用户报告接触
                reportContacts(callback, contactPairs, contactManifolds, contactPoints, lostContactPairs);
            }

            return collisionFound;
        }
        
                // Convert the potential overlapping bodies for the testOverlap() methods
        public void computeOverlapSnapshotContactPairs(NarrowPhaseInfoBatch narrowPhaseInfoBatch,
            List<ContactPair> contactPairs,
            SortedSet<int> setOverlapContactPairId)
        {
            // For each narrow phase info object
            for (var i = 0; i < narrowPhaseInfoBatch.GetNbObjects(); i++)
            {
                // If there is a collision
                if (narrowPhaseInfoBatch.narrowPhaseInfos[i].isColliding)
                    // If the contact pair does not already exist
                    if (!setOverlapContactPairId.Contains(narrowPhaseInfoBatch.narrowPhaseInfos[i].overlappingPairId))
                    {
                        var collider1Entity = narrowPhaseInfoBatch.narrowPhaseInfos[i].colliderEntity1;
                        var collider2Entity = narrowPhaseInfoBatch.narrowPhaseInfos[i].colliderEntity2;

                        var collider1Index = mCollidersComponents.getEntityIndex(collider1Entity);
                        var collider2Index = mCollidersComponents.getEntityIndex(collider2Entity);

                        var body1Entity = mCollidersComponents.mBodiesEntities[collider1Index];
                        var body2Entity = mCollidersComponents.mBodiesEntities[collider2Index];

                        var isTrigger = mCollidersComponents.mIsTrigger[collider1Index] ||
                                        mCollidersComponents.mIsTrigger[collider2Index];

                        // Create a new contact pair
                        var contactPair = new ContactPair(narrowPhaseInfoBatch.narrowPhaseInfos[i].overlappingPairId,
                            body1Entity, body2Entity, collider1Entity, collider2Entity,
                            contactPairs.Count, false, isTrigger);
                        contactPairs.Add(contactPair);

                        setOverlapContactPairId.Add(narrowPhaseInfoBatch.narrowPhaseInfos[i].overlappingPairId);
                    }

                narrowPhaseInfoBatch.ResetContactPoints(i);
            }
        }
        
        // 处理测试Overlap()方法中的潜在重叠体
        public void computeOverlapSnapshotContactPairs(NarrowPhaseInput narrowPhaseInput,
            List<ContactPair> contactPairs)
        {
            var setOverlapContactPairId = new SortedSet<int>();

            // 获取用于碰撞测试的窄相位批次
            var sphereVsSphereBatch = narrowPhaseInput.getSphereVsSphereBatch();
            var sphereVsCapsuleBatch = narrowPhaseInput.getSphereVsCapsuleBatch();
            var capsuleVsCapsuleBatch = narrowPhaseInput.getCapsuleVsCapsuleBatch();
            var sphereVsConvexPolyhedronBatch = narrowPhaseInput.getSphereVsConvexPolyhedronBatch();
            var capsuleVsConvexPolyhedronBatch = narrowPhaseInput.getCapsuleVsConvexPolyhedronBatch();
            var convexPolyhedronVsConvexPolyhedronBatch = narrowPhaseInput.getConvexPolyhedronVsConvexPolyhedronBatch();

            // 处理潜在的接触点
            computeOverlapSnapshotContactPairs(sphereVsSphereBatch, contactPairs, setOverlapContactPairId);
            computeOverlapSnapshotContactPairs(sphereVsCapsuleBatch, contactPairs, setOverlapContactPairId);
            computeOverlapSnapshotContactPairs(capsuleVsCapsuleBatch, contactPairs, setOverlapContactPairId);
            computeOverlapSnapshotContactPairs(sphereVsConvexPolyhedronBatch, contactPairs, setOverlapContactPairId);
            computeOverlapSnapshotContactPairs(capsuleVsConvexPolyhedronBatch, contactPairs, setOverlapContactPairId);
            computeOverlapSnapshotContactPairs(convexPolyhedronVsConvexPolyhedronBatch, contactPairs,
                setOverlapContactPairId);
        }
        
        // 对 testOverlap() 方法进行窄相位碰撞检测。
        /// 如果找到接触点，则此方法返回 true。
        public bool computeNarrowPhaseOverlapSnapshot(NarrowPhaseInput narrowPhaseInput, OverlapCallback callback)
        {
            // 在要测试的批次上进行窄相位碰撞检测
            var collisionFound = testNarrowPhaseCollision(narrowPhaseInput, false);
            if (collisionFound && callback != null)
            {
                // 计算重叠的碰撞器
                var contactPairs = new List<ContactPair>();
                var lostContactPairs = new List<ContactPair>(); // 在这种情况下始终为空（快照）
                computeOverlapSnapshotContactPairs(narrowPhaseInput, contactPairs);

                // 报告重叠的碰撞器
                var callbackData = new OverlapCallback.CallbackData(contactPairs, lostContactPairs, false, mWorld);
                callback?.onOverlap(callbackData);
            }
            return collisionFound;
        }
        
        // Return true if two bodies overlap (collide)
        public bool testOverlap(CollisionBody body1, CollisionBody body2)
        {
            var narrowPhaseInput = new NarrowPhaseInput(/*mOverlappingPairs*/);

            // Compute the broad-phase collision detection
            computeBroadPhase();

            // Filter the overlapping pairs to get only the ones with the selected body involved
            var convexPairs = new List<int>();
            var concavePairs = new List<int>();
            filterOverlappingPairs(body1.getEntity(), body2.getEntity(), convexPairs, concavePairs);

            if (convexPairs.Count > 0 || concavePairs.Count > 0)
            {
                // Compute the middle-phase collision detection
                computeMiddlePhaseCollisionSnapshot(convexPairs, concavePairs, narrowPhaseInput, false);

                // Compute the narrow-phase collision detection
                return computeNarrowPhaseOverlapSnapshot(narrowPhaseInput, null);
            }

            return false;
        }

        // Report all the bodies that overlap (collide) in the world
        public void testOverlap(OverlapCallback callback)
        {
            var narrowPhaseInput = new NarrowPhaseInput(/*mOverlappingPairs*/);

            // Compute the broad-phase collision detection
            computeBroadPhase();

            // Compute the middle-phase collision detection
            computeMiddlePhase(narrowPhaseInput, false);

            // Compute the narrow-phase collision detection and report overlapping shapes
            computeNarrowPhaseOverlapSnapshot(narrowPhaseInput, callback);
        }

        // 报告与参数中的物体重叠（碰撞）的所有物体
        public void testOverlap(CollisionBody body, OverlapCallback callback)
        {
            var narrowPhaseInput = new NarrowPhaseInput(/*mOverlappingPairs*/);

            // Compute the broad-phase collision detection
            computeBroadPhase();

            // Filter the overlapping pairs to get only the ones with the selected body involved
            var convexPairs = new List<int>();
            var concavePairs = new List<int>();
            filterOverlappingPairs(body.getEntity(), convexPairs, concavePairs);

            if (convexPairs.Count > 0 || concavePairs.Count > 0)
            {
                // Compute the middle-phase collision detection
                computeMiddlePhaseCollisionSnapshot(convexPairs, concavePairs, narrowPhaseInput, false);

                // Compute the narrow-phase collision detection
                computeNarrowPhaseOverlapSnapshot(narrowPhaseInput, callback);
            }
        }

        // Test collision and report contacts between two bodies.
        public void testCollision(CollisionBody body1, CollisionBody body2, CollisionCallback callback)
        {
            var narrowPhaseInput = new NarrowPhaseInput(/*mOverlappingPairs*/);

            // Compute the broad-phase collision detection
            computeBroadPhase();

            // Filter the overlapping pairs to get only the ones with the selected body involved
            var convexPairs = new List<int>();
            var concavePairs = new List<int>();
            filterOverlappingPairs(body1.getEntity(), body2.getEntity(), convexPairs, concavePairs);

            if (convexPairs.Count > 0 || concavePairs.Count > 0)
            {
                // Compute the middle-phase collision detection
                computeMiddlePhaseCollisionSnapshot(convexPairs, concavePairs, narrowPhaseInput, true);

                // Compute the narrow-phase collision detection and report contacts
                computeNarrowPhaseCollisionSnapshot(narrowPhaseInput, callback);
            }
        }

        // Test collision and report all the contacts involving the body in parameter
        public void testCollision(CollisionBody body, CollisionCallback callback)
        {
            var narrowPhaseInput = new NarrowPhaseInput(/*mOverlappingPairs*/);

            // Compute the broad-phase collision detection
            computeBroadPhase();

            // Filter the overlapping pairs to get only the ones with the selected body involved
            var convexPairs = new List<int>();
            var concavePairs = new List<int>();
            filterOverlappingPairs(body.getEntity(), convexPairs, concavePairs);

            if (convexPairs.Count > 0 || concavePairs.Count > 0)
            {
                // Compute the middle-phase collision detection
                computeMiddlePhaseCollisionSnapshot(convexPairs, concavePairs, narrowPhaseInput, true);

                // Compute the narrow-phase collision detection and report contacts
                computeNarrowPhaseCollisionSnapshot(narrowPhaseInput, callback);
            }
        }

        // Test collision and report contacts between each colliding bodies in the world
        public void testCollision(CollisionCallback callback)
        {
            var narrowPhaseInput = new NarrowPhaseInput(/*mOverlappingPairs*/);

            // Compute the broad-phase collision detection
            computeBroadPhase();

            // Compute the middle-phase collision detection
            computeMiddlePhase(narrowPhaseInput, true);

            // Compute the narrow-phase collision detection and report contacts
            computeNarrowPhaseCollisionSnapshot(narrowPhaseInput, callback);
        }
    }
}