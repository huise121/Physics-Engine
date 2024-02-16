using System.Collections.Generic;

namespace RP3D
{
    /// <summary>
    ///     执行窄相位碰撞检测。在这一阶段，会计算碰撞点、法线、深度等具体的碰撞信息，并进行相应的碰撞响应处理。
    /// </summary>
    public partial class CollisionDetectionSystem
    {
        public const int MAX_CONTACT_POINTS_IN_MANIFOLD = 4;

        
        
        /// 包含至少一个CollisionBody的所有接触对的索引数组
        public List<int> mCollisionBodyContactPairsIndices = new();

        // 计算窄相位碰撞检测
        private void computeNarrowPhase()
        {
            // 交换先前和当前的接触数组
            swapPreviousAndCurrentContacts();

            // 在要测试的批次上测试窄相碰撞检测
            testNarrowPhaseCollision(mNarrowPhaseInput, true);

            // 在窄相碰撞之后处理所有潜在的接触
            processAllPotentialContacts(mNarrowPhaseInput, true, mPotentialContactPoints,
                mPotentialContactManifolds, mCurrentContactPairs);

            // 减少接触点的数量
            reducePotentialContactManifolds(mCurrentContactPairs, mPotentialContactManifolds, mPotentialContactPoints);

            // 将接触对添加到物体
            addContactPairsToBodies();
        }

        // 交换之前和当前的接触数组
        private void swapPreviousAndCurrentContacts()
        {
            var tempCurrentContactPairs = mCurrentContactPairs;
            var tempCurrentContactManifolds = mCurrentContactManifolds;
            var tempmCurrentContactPoints = mCurrentContactPoints;

            mCurrentContactPairs = mPreviousContactPairs;
            mCurrentContactManifolds = mPreviousContactManifolds;
            mCurrentContactPoints = mPreviousContactPoints;

            mPreviousContactPairs = tempCurrentContactPairs;
            mPreviousContactManifolds = tempCurrentContactManifolds;
            mPreviousContactPoints = tempmCurrentContactPoints;
        }

        // 在批处理上执行窄相碰撞检测算法
        private bool testNarrowPhaseCollision(NarrowPhaseInput narrowPhaseInput,
            bool clipWithPreviousAxisIfStillColliding)
        {
            var contactFound = false;

            // Get the narrow-phase collision detection algorithms for each kind of collision shapes
            var sphereVsSphereAlgo = mCollisionDispatch.getSphereVsSphereAlgorithm();
            var sphereVsCapsuleAlgo = mCollisionDispatch.getSphereVsCapsuleAlgorithm();
            var capsuleVsCapsuleAlgo = mCollisionDispatch.getCapsuleVsCapsuleAlgorithm();
            var convexPolyVsConvexPolyAlgo = mCollisionDispatch.getConvexPolyhedronVsConvexPolyhedronAlgorithm();
            var sphereVsConvexPolyAlgo = mCollisionDispatch.getSphereVsConvexPolyhedronAlgorithm();
            var capsuleVsConvexPolyAlgo = mCollisionDispatch.getCapsuleVsConvexPolyhedronAlgorithm();


            // get the narrow-phase batches to test for collision for contacts
            var sphereVsSphereBatchContacts = narrowPhaseInput.getSphereVsSphereBatch();
            var sphereVsCapsuleBatchContacts = narrowPhaseInput.getSphereVsCapsuleBatch();
            var capsuleVsCapsuleBatchContacts = narrowPhaseInput.getCapsuleVsCapsuleBatch();
            var sphereVsConvexPolyhedronBatchContacts = narrowPhaseInput.getSphereVsConvexPolyhedronBatch();
            var capsuleVsConvexPolyhedronBatchContacts = narrowPhaseInput.getCapsuleVsConvexPolyhedronBatch();
            var convexPolyhedronVsConvexPolyhedronBatchContacts =
                narrowPhaseInput.getConvexPolyhedronVsConvexPolyhedronBatch();

            // Compute the narrow-phase collision detection for each kind of collision shapes (for contacts)
            // Compute the narrow-phase collision detection for each kind of collision shapes (for contacts)
            if (sphereVsSphereBatchContacts.GetNbObjects() > 0)
                contactFound |= sphereVsSphereAlgo.testCollision(sphereVsSphereBatchContacts, 0,
                    sphereVsSphereBatchContacts.GetNbObjects());
            if (sphereVsCapsuleBatchContacts.GetNbObjects() > 0)
                contactFound |= sphereVsCapsuleAlgo.testCollision(sphereVsCapsuleBatchContacts, 0,
                    sphereVsCapsuleBatchContacts.GetNbObjects());
            if (capsuleVsCapsuleBatchContacts.GetNbObjects() > 0)
                contactFound |= capsuleVsCapsuleAlgo.testCollision(capsuleVsCapsuleBatchContacts, 0,
                    capsuleVsCapsuleBatchContacts.GetNbObjects());
            if (sphereVsConvexPolyhedronBatchContacts.GetNbObjects() > 0)
                contactFound |= sphereVsConvexPolyAlgo.testCollision(sphereVsConvexPolyhedronBatchContacts, 0,
                    sphereVsConvexPolyhedronBatchContacts.GetNbObjects(), clipWithPreviousAxisIfStillColliding);
            if (capsuleVsConvexPolyhedronBatchContacts.GetNbObjects() > 0)
                contactFound |= capsuleVsConvexPolyAlgo.testCollision(capsuleVsConvexPolyhedronBatchContacts, 0,
                    capsuleVsConvexPolyhedronBatchContacts.GetNbObjects(), clipWithPreviousAxisIfStillColliding);
            if (convexPolyhedronVsConvexPolyhedronBatchContacts.GetNbObjects() > 0)
                contactFound |= convexPolyVsConvexPolyAlgo.testCollision(
                    convexPolyhedronVsConvexPolyhedronBatchContacts, 0,
                    convexPolyhedronVsConvexPolyhedronBatchContacts.GetNbObjects(),
                    clipWithPreviousAxisIfStillColliding);

            return contactFound;
        }

        // 在窄相碰撞检测后处理潜在的接触
        private void processAllPotentialContacts(NarrowPhaseInput narrowPhaseInput, bool updateLastFrameInfo,
            List<ContactPointInfo> potentialContactPoints,
            List<ContactManifoldInfo> potentialContactManifolds,
            List<ContactPair> contactPairs)
        {
            var mapPairIdToContactPairIndex = new Dictionary<int, int>(mPreviousMapPairIdToContactPairIndex.Count);

            // get the narrow-phase batches to test for collision
            var sphereVsSphereBatch = narrowPhaseInput.getSphereVsSphereBatch();
            var sphereVsCapsuleBatch = narrowPhaseInput.getSphereVsCapsuleBatch();
            var capsuleVsCapsuleBatch = narrowPhaseInput.getCapsuleVsCapsuleBatch();
            var sphereVsConvexPolyhedronBatch = narrowPhaseInput.getSphereVsConvexPolyhedronBatch();
            var capsuleVsConvexPolyhedronBatch = narrowPhaseInput.getCapsuleVsConvexPolyhedronBatch();
            var convexPolyhedronVsConvexPolyhedronBatch = narrowPhaseInput.getConvexPolyhedronVsConvexPolyhedronBatch();

            // 处理可能的接触
            processPotentialContacts(sphereVsSphereBatch, updateLastFrameInfo, potentialContactPoints,
                potentialContactManifolds, mapPairIdToContactPairIndex, contactPairs);
            processPotentialContacts(sphereVsCapsuleBatch, updateLastFrameInfo, potentialContactPoints,
                potentialContactManifolds, mapPairIdToContactPairIndex, contactPairs);
            processPotentialContacts(capsuleVsCapsuleBatch, updateLastFrameInfo, potentialContactPoints,
                potentialContactManifolds, mapPairIdToContactPairIndex, contactPairs);
            processPotentialContacts(sphereVsConvexPolyhedronBatch, updateLastFrameInfo, potentialContactPoints,
                potentialContactManifolds, mapPairIdToContactPairIndex, contactPairs);
            processPotentialContacts(capsuleVsConvexPolyhedronBatch, updateLastFrameInfo, potentialContactPoints,
                potentialContactManifolds, mapPairIdToContactPairIndex, contactPairs);
            processPotentialContacts(convexPolyhedronVsConvexPolyhedronBatch, updateLastFrameInfo,
                potentialContactPoints,
                potentialContactManifolds, mapPairIdToContactPairIndex, contactPairs);
        }

        // 将潜在的接触转换为实际接触
        private void processPotentialContacts(NarrowPhaseInfoBatch narrowPhaseInfoBatch, bool updateLastFrameInfo,
            List<ContactPointInfo> potentialContactPoints,
            List<ContactManifoldInfo> potentialContactManifolds,
            Dictionary<int, int> mapPairIdToContactPairIndex,
            List<ContactPair> contactPairs)
        {
            var nbObjects = narrowPhaseInfoBatch.GetNbObjects();
            if (updateLastFrameInfo)
                // 对于每个狭窄阶段信息对象
                for (var i = 0; i < nbObjects; i++)
                {
                    var narrowPhaseInfo = narrowPhaseInfoBatch.narrowPhaseInfos[i];
                    narrowPhaseInfo.lastFrameCollisionInfo.wasColliding = narrowPhaseInfo.isColliding;
                    // 上一帧的碰撞信息现在有效
                    narrowPhaseInfo.lastFrameCollisionInfo.isValid = true;
                    narrowPhaseInfoBatch.narrowPhaseInfos[i] = narrowPhaseInfo;
                }

            // 对于每个狭窄阶段信息对象
            for (var i = 0; i < nbObjects; i++)
                // 如果两个碰撞器正在发生碰撞
                if (narrowPhaseInfoBatch.narrowPhaseInfos[i].isColliding)
                {
                    var pairId = narrowPhaseInfoBatch.narrowPhaseInfos[i].overlappingPairId;
                    var overlappingPair = mOverlappingPairs.getOverlappingPair(pairId);

                    overlappingPair.CollidingInCurrentFrame = true;

                    var collider1Entity = narrowPhaseInfoBatch.narrowPhaseInfos[i].colliderEntity1;
                    var collider2Entity = narrowPhaseInfoBatch.narrowPhaseInfos[i].colliderEntity2;

                    var collider1Index = mCollidersComponents.getEntityIndex(collider1Entity);
                    var collider2Index = mCollidersComponents.getEntityIndex(collider2Entity);

                    var body1Entity = mCollidersComponents.mBodiesEntities[collider1Index];
                    var body2Entity = mCollidersComponents.mBodiesEntities[collider2Index];

                    // 如果我们有一个凸 vs 凸的碰撞（如果我们考虑碰撞器的基本碰撞形状）
                    if (mCollidersComponents.mCollisionShapes[collider1Index].IsConvex() &&
                        mCollidersComponents.mCollisionShapes[collider2Index].IsConvex())
                    {
                        // 创建一个新的 ContactPair
                        var isTrigger = mCollidersComponents.mIsTrigger[collider1Index] ||
                                        mCollidersComponents.mIsTrigger[collider2Index];


                        var newContactPairIndex = contactPairs.Count;

                        var pairContact = new ContactPair(pairId, body1Entity, body2Entity, collider1Entity,
                            collider2Entity,
                            newContactPairIndex, overlappingPair.CollidingInPreviousFrame, isTrigger);
                        contactPairs.Add(pairContact);

                        // 创建一个新的潜在接触面用于重叠对
                        var contactManifoldIndex = potentialContactManifolds.Count;
                        potentialContactManifolds.Add(new ContactManifoldInfo(pairId));
                        var contactManifoldInfo = potentialContactManifolds[contactManifoldIndex];

                        var contactPointIndexStart = potentialContactPoints.Count;

                        // 添加潜在接触点
                        for (var j = 0; j < narrowPhaseInfoBatch.narrowPhaseInfos[i].nbContactPoints; j++)
                            if (contactManifoldInfo.nbPotentialContactPoints < 256)
                            {
                                // 将接触点添加到接触面
                                contactManifoldInfo.potentialContactPointsIndices[
                                        contactManifoldInfo.nbPotentialContactPoints] =
                                    (byte)(contactPointIndexStart + j);
                                contactManifoldInfo.nbPotentialContactPoints++;

                                // 将接触点添加到潜在接触点数组
                                var contactPoint = narrowPhaseInfoBatch.narrowPhaseInfos[i].contactPoints[j];
                                potentialContactPoints.Add(contactPoint);
                            }

                        // 将接触面添加到重叠对接触
                        pairContact.PotentialContactManifoldsIndices[0] = contactManifoldIndex;
                        pairContact.NbPotentialContactManifolds = 1;
                    }
                    else
                    {
                        ContactPair pairContact;
                        // 如果此重叠对尚未存在接触对
                        if (!mapPairIdToContactPairIndex.TryGetValue(pairId, out var pairContactIndex))
                        {
                            // 创建一个新的 ContactPair
                            var isTrigger = mCollidersComponents.mIsTrigger[collider1Index] ||
                                            mCollidersComponents.mIsTrigger[collider2Index];

                            pairContact = new ContactPair(pairId, body1Entity, body2Entity, collider1Entity,
                                collider2Entity,
                                contactPairs.Count, overlappingPair.CollidingInPreviousFrame, isTrigger);
                            contactPairs.Add(pairContact);
                            mapPairIdToContactPairIndex.Add(pairId, contactPairs.Count - 1);
                        }
                        else
                        {
                            // 如果此重叠对已存在接触对，则使用此接触对
                            pairContact = contactPairs[pairContactIndex];
                        }

                        // 添加潜在接触点
                        for (var j = 0; j < narrowPhaseInfoBatch.narrowPhaseInfos[i].nbContactPoints; j++)
                        {
                            var contactPoint = narrowPhaseInfoBatch.narrowPhaseInfos[i].contactPoints[j];

                            // 将接触点添加到潜在接触点数组
                            var contactPointIndex = potentialContactPoints.Count;
                            potentialContactPoints.Add(contactPoint);

                            var similarManifoldFound = false;

                            // 对于重叠对的每个接触面
                            for (var m = 0; m < pairContact.NbPotentialContactManifolds; m++)
                            {
                                var contactManifoldIndex = pairContact.PotentialContactManifoldsIndices[m];

                                var potentialContactManifold = potentialContactManifolds[contactManifoldIndex];
                                if (potentialContactManifold.nbPotentialContactPoints < 256)
                                {
                                    // 获取当前接触面的第一个接触点
                                    int manifoldContactPointIndex =
                                        potentialContactManifold.potentialContactPointsIndices[0];
                                    var manifoldContactPoint = potentialContactPoints[manifoldContactPointIndex];

                                    // 如果我们找到了一个与新接触点相似的接触面
                                    // （具有类似接触法线方向的接触面）
                                    if (manifoldContactPoint.normal.dot(contactPoint.normal) >=
                                        mWorld.mConfig.cosAngleSimilarContactManifold)
                                    {
                                        // 将接触点添加到接触面
                                        potentialContactManifold.potentialContactPointsIndices[
                                                potentialContactManifold.nbPotentialContactPoints] =
                                            (byte)contactPointIndex;
                                        potentialContactManifold.nbPotentialContactPoints++;

                                        similarManifoldFound = true;

                                        break;
                                    }
                                }
                            }

                            // 如果未找到与接触点相似的接触面
                            if (!similarManifoldFound && pairContact.NbPotentialContactManifolds < 12)
                            {
                                // 为重叠对创建一个新的潜在接触面
                                var contactManifoldIndex = potentialContactManifolds.Count;
                                potentialContactManifolds.Add(new ContactManifoldInfo(pairId));
                                var contactManifoldInfo = potentialContactManifolds[contactManifoldIndex];

                                // 将接触点添加到接触面
                                contactManifoldInfo.potentialContactPointsIndices[0] = (byte)contactPointIndex;
                                contactManifoldInfo.nbPotentialContactPoints = 1;

                                // 将接触面添加到重叠对接触
                                pairContact.PotentialContactManifoldsIndices[
                                        pairContact.NbPotentialContactManifolds] =
                                    contactManifoldIndex;
                                pairContact.NbPotentialContactManifolds++;
                            }
                        }
                    }

                    narrowPhaseInfoBatch.ResetContactPoints(i);
                }
        }

        // 清除过时的接触点和接触面，并减少剩余接触面的接触点数量
        private void reducePotentialContactManifolds(List<ContactPair> contactPairs,
            List<ContactManifoldInfo> potentialContactManifolds,
            List<ContactPointInfo> potentialContactPoints)
        {
            // 减少接触对中的潜在接触面的数量
            var nbContactPairs = contactPairs.Count;
            for (var i = 0; i < nbContactPairs; i++)
            {
                var contactPair = contactPairs[i];

                // 当接触对中的接触面过多时
                while (contactPair.NbPotentialContactManifolds > 3)
                {
                    // 查找具有最小接触深度的接触面。
                    var minDepth = float.MaxValue;
                    var minDepthManifoldIndex = -1;
                    for (var j = 0; j < contactPair.NbPotentialContactManifolds; j++)
                    {
                        var manifold = potentialContactManifolds[contactPair.PotentialContactManifoldsIndices[j]];

                        // 获取接触面的最大接触点深度
                        var depth = computePotentialManifoldLargestContactDepth(manifold, potentialContactPoints);

                        if (depth < minDepth)
                        {
                            minDepth = depth;
                            minDepthManifoldIndex = j;
                        }
                    }

                    // 删除非最佳接触面
                    contactPair.RemovePotentialManifoldAtIndex(minDepthManifoldIndex);
                }
            }

            // 减少接触面中潜在接触点的数量
            for (var i = 0; i < nbContactPairs; i++)
            {
                var pairContact = contactPairs[i];

                // 对于每个潜在的接触面
                for (var j = 0; j < pairContact.NbPotentialContactManifolds; j++)
                {
                    var manifold = potentialContactManifolds[pairContact.PotentialContactManifoldsIndices[j]];

                    // 如果接触面中的接触点过多
                    if (manifold.nbPotentialContactPoints > MAX_CONTACT_POINTS_IN_MANIFOLD)
                    {
                        var shape1LocalToWorldTransform =
                            mCollidersComponents.getLocalToWorldTransform(pairContact.Collider1Entity);

                        // 减少接触面中的接触点数量
                        reduceContactPoints(manifold, shape1LocalToWorldTransform, potentialContactPoints);
                    }


                    // 删除接触面中的重复接触点（如果有）
                    removeDuplicatedContactPointsInManifold(manifold, potentialContactPoints);
                }
            }
        }


        // 移除给定接触面中的重复接触点
        private void removeDuplicatedContactPointsInManifold(ContactManifoldInfo manifold,
            List<ContactPointInfo> potentialContactPoints)
        {
            var distThresholdSqr = 0.01f * 0.01f;

            // 对于接触面的每个接触点
            for (var i = 0; i < manifold.nbPotentialContactPoints; i++)
            for (var j = i + 1; j < manifold.nbPotentialContactPoints; j++)
            {
                var point1 = potentialContactPoints[manifold.potentialContactPointsIndices[i]];
                var point2 = potentialContactPoints[manifold.potentialContactPointsIndices[j]];

                // 计算两个接触点之间的距离
                var distSqr = (point2.localPoint1 - point1.localPoint1).LengthSquare();

                // 我们找到了一个重复的接触点
                if (distSqr < distThresholdSqr)
                {
                    // 移除重复的接触点
                    manifold.potentialContactPointsIndices[j] =
                        manifold.potentialContactPointsIndices[manifold.nbPotentialContactPoints - 1];
                    manifold.nbPotentialContactPoints--;

                    j--;
                }
            }
        }


        // 减少潜在接触面的接触点数量
        // 这是基于Dirk Gregorius在他的“Contacts Creation” GDC演示中描述的技术。此方法将接触点的数量减少到最多4个点（但可以更少）。
        private void reduceContactPoints(ContactManifoldInfo manifold, PTransform shape1ToWorldTransform,
            List<ContactPointInfo> potentialContactPoints)
        {
            // 存储在接触体中候选接触点的索引数组。每次找到一个要保留的点时，我们将其从此数组中移除
            var candidatePointsIndices = new byte[256];
            var nbCandidatePoints = manifold.nbPotentialContactPoints;
            for (var i = 0; i < manifold.nbPotentialContactPoints; i++)
                candidatePointsIndices[i] = manifold.potentialContactPointsIndices[i];

            byte nbReducedPoints = 0;

            var pointsToKeepIndices = new byte[4];

            for (var i = 0; i < MAX_CONTACT_POINTS_IN_MANIFOLD; i++) pointsToKeepIndices[i] = 0;

            // 计算我们需要保留的初始接触点。
            // 我们始终保留的第一个点是在给定的恒定方向上的点
            // （为了始终在帧之间具有相同的接触点以获得更好的稳定性）

            var worldToShape1Transform = shape1ToWorldTransform.GetInverse();

            // 计算接触体的接触法线（我们使用第一个接触点）
            // 在第一个碰撞形状的局部空间中
            var contactNormalShape1Space = worldToShape1Transform.GetOrientation() *
                                           potentialContactPoints[candidatePointsIndices[0]].normal;

            // 计算搜索方向
            var searchDirection = new PVector3(1, 1, 1);
            var maxDotProduct = float.MaxValue;
            byte elementIndexToKeep = 0;
            for (byte i = 0; i < nbCandidatePoints; i++)
            {
                var element = potentialContactPoints[candidatePointsIndices[i]];
                var dotProduct = searchDirection.dot(element.localPoint1);
                if (dotProduct > maxDotProduct)
                {
                    maxDotProduct = dotProduct;
                    elementIndexToKeep = i;
                    nbReducedPoints = 1;
                }
            }

            pointsToKeepIndices[0] = candidatePointsIndices[elementIndexToKeep];
            removeItemAtInArray(candidatePointsIndices, elementIndexToKeep, nbCandidatePoints);
            //candidatePointsIndices.removeAt(elementIndexToKeep);

            // 计算我们需要保留的第二个接触点。
            // 我们保留的第二个点是距离第一个点最远的点。

            var maxDistance = 0.0f;
            elementIndexToKeep = 0;
            for (byte i = 0; i < nbCandidatePoints; i++)
            {
                var element = potentialContactPoints[candidatePointsIndices[i]];
                var pointToKeep0 = potentialContactPoints[pointsToKeepIndices[0]];


                var distance = (pointToKeep0.localPoint1 - element.localPoint1).LengthSquare();
                if (distance >= maxDistance)
                {
                    maxDistance = distance;
                    elementIndexToKeep = i;
                    nbReducedPoints = 2;
                }
            }

            pointsToKeepIndices[1] = candidatePointsIndices[elementIndexToKeep];
            removeItemAtInArray(candidatePointsIndices, elementIndexToKeep, nbCandidatePoints);

            // 计算我们需要保留的第三个接触点。
            // 第三个点是与第一个点和第二个点组成面积最大的三角形的点。

            // 我们计算最正或最负的三角形面积（取决于绕线）
            byte thirdPointMaxAreaIndex = 0;
            byte thirdPointMinAreaIndex = 0;
            var minArea = 0.0f;
            var maxArea = 0.0f;
            var isPreviousAreaPositive = true;
            for (byte i = 0; i < nbCandidatePoints; i++)
            {
                var element = potentialContactPoints[candidatePointsIndices[i]];
                var pointToKeep0 = potentialContactPoints[pointsToKeepIndices[0]];
                var pointToKeep1 = potentialContactPoints[pointsToKeepIndices[1]];


                var newToFirst = pointToKeep0.localPoint1 - element.localPoint1;
                var newToSecond = pointToKeep1.localPoint1 - element.localPoint1;

                // 计算三角形面积
                var area1 = newToFirst.cross(newToSecond).dot(contactNormalShape1Space);

                if (area1 >= maxArea)
                {
                    maxArea = area1;
                    thirdPointMaxAreaIndex = i;
                }

                if (area1 <= minArea)
                {
                    minArea = area1;
                    thirdPointMinAreaIndex = i;
                }
            }

            if (maxArea > -minArea)
            {
                isPreviousAreaPositive = true;
                pointsToKeepIndices[2] = candidatePointsIndices[thirdPointMaxAreaIndex];
                removeItemAtInArray(candidatePointsIndices, thirdPointMaxAreaIndex, nbCandidatePoints);
            }
            else
            {
                isPreviousAreaPositive = false;
                pointsToKeepIndices[2] = candidatePointsIndices[thirdPointMinAreaIndex];
                removeItemAtInArray(candidatePointsIndices, thirdPointMinAreaIndex, nbCandidatePoints);
            }

            nbReducedPoints = 3;

            // 通过选择添加最多三角形面积到前一个三角形的三角形来计算第四个点
            // 并具有相反的符号面积（相反的绕线）

            var largestArea = 0.0f; // 最大面积（正或负）
            elementIndexToKeep = 0;
            nbReducedPoints = 4;
            float area;

            // 对于每个剩余的候选点
            for (byte i = 0; i < nbCandidatePoints; i++)
            {
                var element = potentialContactPoints[candidatePointsIndices[i]];


                // 对于第一个三角形的每条边制作的三角形
                for (var j = 0; j < 3; j++)
                {
                    var edgeVertex1Index = j;
                    var edgeVertex2Index = j < 2 ? j + 1 : 0;

                    var pointToKeepEdgeV1 = potentialContactPoints[pointsToKeepIndices[edgeVertex1Index]];
                    var pointToKeepEdgeV2 = potentialContactPoints[pointsToKeepIndices[edgeVertex2Index]];

                    var newToFirst = pointToKeepEdgeV1.localPoint1 - element.localPoint1;
                    var newToSecond = pointToKeepEdgeV2.localPoint1 - element.localPoint1;

                    // 计算三角形面积
                    area = newToFirst.cross(newToSecond).dot(contactNormalShape1Space);

                    // 我们正在寻找面积最大的三角形（正或负）。
                    // 如果之前的区域是正的，现在我们正在看负面积。
                    // 如果之前的区域是负的，现在我们正在看正面积。
                    if (isPreviousAreaPositive && area <= largestArea)
                    {
                        largestArea = area;
                        elementIndexToKeep = i;
                    }
                    else if (!isPreviousAreaPositive && area >= largestArea)
                    {
                        largestArea = area;
                        elementIndexToKeep = i;
                    }
                }
            }

            pointsToKeepIndices[3] = candidatePointsIndices[elementIndexToKeep];
            removeItemAtInArray(candidatePointsIndices, elementIndexToKeep, nbCandidatePoints);

            // 只保留流形中选择的四个接触点
            manifold.potentialContactPointsIndices[0] = pointsToKeepIndices[0];
            manifold.potentialContactPointsIndices[1] = pointsToKeepIndices[1];
            manifold.potentialContactPointsIndices[2] = pointsToKeepIndices[2];
            manifold.potentialContactPointsIndices[3] = pointsToKeepIndices[3];
            manifold.nbPotentialContactPoints = 4;
        }


        // 将接触对添加到相应的物体中
        private void addContactPairsToBodies()
        {
            var nbContactPairs = mCurrentContactPairs.Count;
            for (var p = 0; p < nbContactPairs; p++)
            {
                var contactPair = mCurrentContactPairs[p];

                // 检查联系对涉及的实体是否为刚体
                var isBody1Rigid = mRigidBodyComponents.hasComponent(contactPair.Body1Entity);
                var isBody2Rigid = mRigidBodyComponents.hasComponent(contactPair.Body2Entity);

                // 将关联的联系对添加到对中的两个实体（用于稍后创建岛屿）
                if (isBody1Rigid)
                    mRigidBodyComponents.AddContacPair(contactPair.Body1Entity, p);
                if (isBody2Rigid)
                    mRigidBodyComponents.AddContacPair(contactPair.Body2Entity, p);

                // 如果至少一个实体是 CollisionBody
                if (!isBody1Rigid || !isBody2Rigid)
                    // 将联系对索引添加到 CollisionBody 的对数组中
                    mCollisionBodyContactPairsIndices.Add(p);
            }
        }


        // 返回潜在接触曲面的所有接触点中的最大深度
        private float computePotentialManifoldLargestContactDepth(ContactManifoldInfo manifold,
            List<ContactPointInfo> potentialContactPoints)
        {
            var largestDepth = 0.0f;

            for (var i = 0; i < manifold.nbPotentialContactPoints; i++)
            {
                var depth = potentialContactPoints[manifold.potentialContactPointsIndices[i]].penetrationDepth;

                if (depth > largestDepth) largestDepth = depth;
            }

            return largestDepth;
        }

        // 从数组中移除一个元素（并用数组中的最后一个元素替换它）
        private void removeItemAtInArray(byte[] array, int index, int arraySize)
        {
            array[index] = array[arraySize - 1];
            arraySize--;
        }
    }
}