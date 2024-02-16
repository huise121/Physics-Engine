using System.Collections.Generic;
using UnityEngine;

namespace RP3D
{
    /// <summary>
    /// 执行广义相位碰撞检测。在这一阶段，通常会使用一些快速的算法来识别可能发生碰撞的物体，从而减少后续检测的计算量。
    /// </summary>
    public partial class CollisionDetectionSystem
    {
        /// 在广义相位计算期间重叠的节点
        private readonly List<Pair<int, int>> mBroadPhaseOverlappingNodes = new();
        
        // 计算广相位碰撞检测。
        private void computeBroadPhase()
        {
            // 请求广相位计算所有与上一帧移动或添加的形状重叠的形状。此调用只能在碰撞检测中添加新的重叠对。
            mBroadPhaseSystem.computeOverlappingPairs(mBroadPhaseOverlappingNodes);

            // 如有必要，创建新的重叠对。
            updateOverlappingPairs(mBroadPhaseOverlappingNodes);

            // 移除非重叠对。
            removeNonOverlappingPairs();

            mBroadPhaseOverlappingNodes.Clear();
        }
        
        // 使用广相位中的重叠节点数组，如有必要则创建新的重叠对。
        private void updateOverlappingPairs(List<Pair<int, int>> overlappingNodes) {
            // 对于每对重叠的节点
            var nbOverlappingNodes = overlappingNodes.Count;
            for (var i = 0; i < nbOverlappingNodes; i++) {
                var nodePair = overlappingNodes[i];

                // 跳过具有相同重叠节点的对
                if (nodePair.First == nodePair.Second) continue;
                
                // 获取两个碰撞体
                var collider1Entity = mMapBroadPhaseIdToColliderEntity[nodePair.First];
                var collider2Entity = mMapBroadPhaseIdToColliderEntity[nodePair.Second];

                var collider1Index = mCollidersComponents.getEntityIndex(collider1Entity);
                var collider2Index = mCollidersComponents.getEntityIndex(collider2Entity);

                // 获取两个刚体
                var body1Entity = mCollidersComponents.mBodiesEntities[collider1Index];
                var body2Entity = mCollidersComponents.mBodiesEntities[collider2Index];

                // 如果两个碰撞体来自同一个刚体，则跳过
                if (body1Entity == body2Entity) continue;
                
                var nbEnabledColliderComponents = mCollidersComponents.getNbEnabledComponents();
                var isBody1Enabled = collider1Index < nbEnabledColliderComponents;
                var isBody2Enabled = collider2Index < nbEnabledColliderComponents;
                var isBody1Static = false;
                var isBody2Static = false;
                if (mRigidBodyComponents.HasComponentGetIndex(body1Entity, out var rigidBody1Index))
                    isBody1Static = mRigidBodyComponents.mBodyTypes[rigidBody1Index] == BodyType.STATIC;
                if (mRigidBodyComponents.HasComponentGetIndex(body2Entity, out var rigidBody2Index))
                    isBody2Static = mRigidBodyComponents.mBodyTypes[rigidBody2Index] == BodyType.STATIC;

                var isBody1Active = isBody1Enabled && !isBody1Static;
                var isBody2Active = isBody2Enabled && !isBody2Static;

                if (!isBody1Active && !isBody2Active) continue;
                
                // 检查刚体是否在不能相互碰撞的刚体集合中
                Pair<Entity, Entity> bodiesIndex = OverlappingPairs.computeBodiesIndexPair(body1Entity, body2Entity);
                
                if (mNoCollisionPairs.Contains(bodiesIndex)) continue;
                // 计算重叠对ID
                int pairId = PMath.pairNumbers(PMath.Max(nodePair.First, nodePair.Second),
                    PMath.Min(nodePair.First, nodePair.Second));

                // 检查重叠对是否已经存在
                var overlappingPair = mOverlappingPairs.getOverlappingPair(pairId);
                if (overlappingPair == null) {
                    short shape1CollideWithMaskBits = mCollidersComponents.mCollideWithMaskBits[collider1Index];
                    short shape2CollideWithMaskBits = mCollidersComponents.mCollideWithMaskBits[collider2Index];
                                    
                    short shape1CollisionCategoryBits = mCollidersComponents.mCollisionCategoryBits[collider1Index];
                    short shape2CollisionCategoryBits = mCollidersComponents.mCollisionCategoryBits[collider2Index];

                    // 检查碰撞过滤器是否允许两个形状之间的碰撞
                    if ((shape1CollideWithMaskBits & shape2CollisionCategoryBits) != 0 &&
                        (shape1CollisionCategoryBits & shape2CollideWithMaskBits) != 0) {
                        
                        var shape1 = mCollidersComponents.mColliders[collider1Index];
                        var shape2 = mCollidersComponents.mColliders[collider2Index];

                        // 检查至少一个碰撞形状是否是凸的
                        var isShape1Convex = shape1.getCollisionShape().IsConvex();
                        var isShape2Convex = shape2.getCollisionShape().IsConvex();
                        if (isShape1Convex || isShape2Convex)
                            // 添加新的重叠对
                            mOverlappingPairs.addPair(collider1Index, collider2Index,
                                isShape1Convex && isShape2Convex);
                    }
                } else {
                    // 我们不需要测试重叠对是否重叠，因为刚刚报告它们仍然重叠
                    overlappingPair.NeedToTestOverlap = false;
                }
            }
        }

        
        // 删除不再重叠的对
        private void removeNonOverlappingPairs() {
            // 对于每对凸形状
            for (var i = 0; i < mOverlappingPairs.mConvexPairs.Count; i++) {
                var overlappingPair = mOverlappingPairs.mConvexPairs[i];

                // 检查是否需要测试重叠。如果是，则测试两个形状是否仍然重叠。否则，我们销毁重叠对
                if (overlappingPair.NeedToTestOverlap) {
                    if (mBroadPhaseSystem.testOverlappingShapes(overlappingPair.BroadPhaseId1,
                            overlappingPair.BroadPhaseId2)) {
                        overlappingPair.NeedToTestOverlap = false;
                    } else {
                        // 如果两个碰撞体在上一帧中发生碰撞
                        if (overlappingPair.CollidingInPreviousFrame)
                            // 创建一个新的失去接触对
                            addLostContactPair(overlappingPair);

                        mOverlappingPairs.removePair(i, true);
                        i--;
                    }
                }
            }

            /*// 对于每对凹形状
            for (var i = 0; i < mOverlappingPairs.mConcavePairs.Count; i++) {
                var overlappingPair = mOverlappingPairs.mConcavePairs[i];

                // 检查是否需要测试重叠。如果是，则测试两个形状是否仍然重叠。
                // 否则，我们销毁重叠对
                if (overlappingPair.NeedToTestOverlap) {
                    if (mBroadPhaseSystem.testOverlappingShapes(overlappingPair.BroadPhaseId1,
                            overlappingPair.BroadPhaseId2)) {
                        overlappingPair.NeedToTestOverlap = false;
                    } else {
                        // 如果对的两个碰撞体在上一帧中发生碰撞
                        if (overlappingPair.CollidingInPreviousFrame)
                            // 创建一个新的失去接触对
                            addLostContactPair(overlappingPair);

                        mOverlappingPairs.removePair(i, false);
                        i--;
                    }
                }
            }*/
        }
        
        // 添加一个失去接触的碰撞对（不再接触的碰撞体对） 上一帧碰撞了 这一帧没有碰撞
        private void addLostContactPair(OverlappingPair overlappingPair)
        {
            var collider1Index = mCollidersComponents.getEntityIndex(overlappingPair.Collider1);
            var collider2Index = mCollidersComponents.getEntityIndex(overlappingPair.Collider2);

            var body1Entity = mCollidersComponents.mBodiesEntities[collider1Index];
            var body2Entity = mCollidersComponents.mBodiesEntities[collider2Index];

            var isCollider1Trigger = mCollidersComponents.mIsTrigger[collider1Index];
            var isCollider2Trigger = mCollidersComponents.mIsTrigger[collider2Index];
            var isTrigger = isCollider1Trigger || isCollider2Trigger;

            // Create a lost contact pair
            var lostContactPair = new ContactPair(overlappingPair.PairID, body1Entity, body2Entity,
                overlappingPair.Collider1, overlappingPair.Collider2, mLostContactPairs.Count,
                true, isTrigger);
            mLostContactPairs.Add(lostContactPair);
        }
        
    }
}