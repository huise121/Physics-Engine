using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RP3D
{
    //最后一帧碰撞信息
    public class LastFrameCollisionInfo
    {
        // TODO OPTI : Use bit flags instead of bools here

        /// 如果我们有关于上一帧的信息，则为 true
        public bool isValid;

        /// 如果帧信息已过时（碰撞形状在中间阶段不重叠），则为 true
        public bool isObsolete;

        /// 如果两个形状在上一帧中发生碰撞，则为 true
        public bool wasColliding;

        /// 如果我们在上一帧中使用 GJK 算法检测碰撞，则为 true
        public bool wasUsingGJK;

        /// 如果我们在上一帧中使用 SAT 算法检测碰撞，则为 true
        public bool wasUsingSAT;

        /// <summary>
        /// "GJK Separating Axis" 是指 "Gilbert–Johnson–Keerthi Separating Axis"（吉尔伯特-约翰逊-基尔提分离轴）算法中的分离轴。
        /// GJK 算法是一种用于求解凸多边形之间最近点的算法，通常用于碰撞检测中。在碰撞检测中，GJK 算法可以用来检测两个物体是否相交，以及找到它们之间的最近距离。
        /// 在 GJK 算法中，分离轴是一个用于确定两个凸体是否相交的重要概念。如果存在一个分离轴，可以将两个凸体分开，那么它们不相交；否则，它们相交。
        /// "GJK Separating Axis" 是指用于检测两个凸体是否相交的分离轴。该轴可以是两个凸体的表面法线、边缘之间的法线或者两个凸体的顶点之间的向量等。
        /// 利用 GJK 算法，可以计算出这些可能的分离轴，并用于检测两个凸体是否相交。
        /// 在物理引擎中，GJK 算法通常用于高效地检测碰撞，而分离轴则是其中的关键步骤之一。
        /// 通过检测分离轴，可以快速判断两个物体之间是否存在碰撞，从而进行相应的碰撞响应或者避免碰撞。
        /// </summary>
        public PVector3 gjkSeparatingAxis;

        // SAT Algorithm
        public bool satIsAxisFacePolyhedron1;
        public bool satIsAxisFacePolyhedron2;
        public int satMinAxisFaceIndex;
        public int satMinEdge1Index;
        public int satMinEdge2Index;
        
        /// Constructor
        public LastFrameCollisionInfo()
        {
            isValid = false; 
            isObsolete = false; 
            wasColliding = false; 
            wasUsingGJK = false; 
            gjkSeparatingAxis = new PVector3(0, 1, 0);
            satIsAxisFacePolyhedron1 = false; 
            satIsAxisFacePolyhedron2 = false; 
            satMinAxisFaceIndex = 0;
            satMinEdge1Index = 0; 
            satMinEdge2Index = 0;
        }

    }
    
    //重叠配对
    /// <summary>
    /// OverlappingPair（重叠对）是用于表示两个碰撞形状之间的重叠关系的数据结构。
    /// 每当两个碰撞形状相互接触时，就会创建一个 OverlappingPair。
    /// 这个数据结构通常包含了一些与碰撞有关的信息，以及一些用于碰撞检测和碰撞响应的方法。
    ///
    /// OverlappingPair 通常包含以下一些属性和方法：
    /// 碰撞形状（Collision Shapes）：OverlappingPair 包含了两个碰撞形状之间的重叠关系。
    /// 这些碰撞形状可以是凸形状（Convex Shapes）或者凹形状（Concave Shapes）。
    /// 碰撞信息（Collision Information）：OverlappingPair 可能包含了一些与碰撞有关的信息，比如碰撞点、碰撞法线、碰撞深度等。
    /// 碰撞检测方法（Collision Detection Methods）：OverlappingPair 可能包含了一些用于执行碰撞检测的方法。
    /// 这些方法通常用于判断两个碰撞形状之间是否发生了碰撞，并计算碰撞信息。
    /// 碰撞响应方法（Collision Response Methods）：OverlappingPair 可能包含了一些用于执行碰撞响应的方法。
    /// 这些方法通常用于处理碰撞后的物体运动、速度变化等。
    /// </summary>
    public class OverlappingPair
    {
        /// 凸 vs 凸对的ID
        public int PairID { get; }

        ///第一个形状的广义相位ID
        ///TODO OPTI：这个是否被使用？
        public int BroadPhaseId1 { get; private set; }

        ///第二个形状的广义相位ID
        ///TODO OPTI：这个是否被使用？
        public int BroadPhaseId2 { get; private set; }

        ///凸 vs 凸对中第一个碰撞体的实体
        public Entity Collider1 { get; }

        ///凸 vs 凸对中第二个碰撞体的实体
        public Entity Collider2 { get; }

        ///如果需要测试凸 vs 凸重叠的形状对是否仍然重叠，则为true
        public bool NeedToTestOverlap { get; set; }

        ///指向狭义阶段算法的指针
        public NarrowPhaseAlgorithmType NarrowPhaseAlgorithmType { get; private set; }

        ///如果碰撞体在上一帧中发生碰撞，则为true
        public bool CollidingInPreviousFrame { get; set; }

        ///如果碰撞体在当前帧中发生碰撞，则为true
        public bool CollidingInCurrentFrame { get; set; }

        ///构造函数
        public OverlappingPair(int pairId, int broadPhaseId1, int broadPhaseId2, Entity collider1, Entity collider2,
            NarrowPhaseAlgorithmType narrowPhaseAlgorithmType)
        {
            PairID = pairId;
            BroadPhaseId1 = broadPhaseId1;
            BroadPhaseId2 = broadPhaseId2;
            Collider1 = collider1;
            Collider2 = collider2;
            NeedToTestOverlap = false;
            NarrowPhaseAlgorithmType = narrowPhaseAlgorithmType;
            CollidingInPreviousFrame = false;
            CollidingInCurrentFrame = false;
        }
    }

    // 两个凸碰撞体之间的重叠配对
    public class ConvexOverlappingPair : OverlappingPair
    {
        /// 每个重叠碰撞形状的临时相干碰撞数据。
        /// 临时相干数据存储上一帧的碰撞信息。
        /// 如果两个凸形状重叠，我们有一个单独的碰撞数据，但如果一个形状是凹的，
        /// 我们可能会有几个重叠三角形的碰撞数据。
        public LastFrameCollisionInfo lastFrameCollisionInfo;

        /// 构造函数
        public ConvexOverlappingPair(int pairId, int broadPhaseId1, int broadPhaseId2, Entity collider1,
            Entity collider2,
            NarrowPhaseAlgorithmType narrowPhaseAlgorithmType) : base(pairId, broadPhaseId1, broadPhaseId2, collider1,
            collider2, narrowPhaseAlgorithmType)
        {
            lastFrameCollisionInfo = new LastFrameCollisionInfo();
        }
    }

    /*// 一个个凸碰撞体和一个凹碰撞体之间的重叠配对
    public class ConcaveOverlappingPair : OverlappingPair
    {
        /// True if the first shape of the pair is 凸
        public bool isShape1Convex;

        // Temporal Coherence Collision Data：时间相关性碰撞数据是指用于存储与上一帧相关的碰撞信息的数据结构。
        // 通过利用时间上连续的碰撞数据，可以提高碰撞检测的效率，从而实现更快的碰撞检测。
        // Overlapping Collision Shapes：重叠碰撞形状指的是在当前帧中发生碰撞的形状。
        // 这些碰撞形状可能是凸形状，也可能是凹形状。对于凹形状，可能存在多个重叠的三角形。
        // Last Frame Collision Information：上一帧的碰撞信息存储在时间相关性碰撞数据中。
        // 这些信息包括碰撞点、碰撞法线、碰撞深度等。通过存储上一帧的碰撞信息，可以在当前帧中快速获取到上一帧的碰撞数据，从而实现碰撞检测的时间相关性。
        // Map Key：在时间相关性碰撞数据中，使用形状 ID 的组合作为映射的键。
        // 形状 ID 可以唯一标识碰撞形状，因此可以用来区分不同的碰撞形状。
        // 总之，这段注释描述了用于存储重叠碰撞形状的时间相关性碰撞数据的结构和存储方式。
        // 通过存储上一帧的碰撞信息，并使用形状 ID 来索引不同的碰撞形状，可以实现高效的碰撞检测和碰撞响应。
        public Dictionary<long, LastFrameCollisionInfo> lastFrameCollisionInfos;

        /// Constructor
        public ConcaveOverlappingPair(int pairId, int broadPhaseId1, int broadPhaseId2, Entity collider1,
            Entity collider2,
            NarrowPhaseAlgorithmType narrowPhaseAlgorithmType,
            bool isShape1Convex)
            : base(pairId, broadPhaseId1, broadPhaseId2, collider1, collider2, narrowPhaseAlgorithmType)
        {
            this.isShape1Convex = isShape1Convex;
            lastFrameCollisionInfos = new Dictionary<long, LastFrameCollisionInfo>(16);
        }

        // Destroy all the LastFrameCollisionInfo objects
        public void destroyLastFrameCollisionInfos()
        {
            lastFrameCollisionInfos.Clear();
        }

        // 如果给定的形状对尚不存在上一帧的碰撞信息，则添加一个新的上一帧碰撞信息
        public LastFrameCollisionInfo AddLastFrameInfoIfNecessary(int shapeId1, int shapeId2)
        {
            var maxShapeId = PMath.Max(shapeId1, shapeId2);
            var minShapeId = PMath.Min(shapeId1, shapeId2);

            // 尝试获取对应的上一帧碰撞信息
            var shapesId = PMath.pairNumbers(maxShapeId, minShapeId);

            // 如果已经没有这两个形状的碰撞信息
            if (!lastFrameCollisionInfos.ContainsKey(shapesId))
            {
                var lastFrameInfo = new LastFrameCollisionInfo();

                // 将其添加到碰撞信息映射中
                lastFrameCollisionInfos.Add(shapesId, lastFrameInfo);

                return lastFrameInfo;
            }

            // 存在的碰撞信息尚未过时
            var addLastFrameInfoIfNecessary = lastFrameCollisionInfos[shapesId];
            addLastFrameInfoIfNecessary.isObsolete = false;
            return addLastFrameInfoIfNecessary;
        }


        /// 清除过时的 LastFrameCollisionInfo 对象
        public void clearObsoleteLastFrameInfos()
        {
            // 遍历上一帧的每一个碰撞信息
            foreach (var kvp in lastFrameCollisionInfos)
            {
                // 如果碰撞信息已过时
                if (kvp.Value.isObsolete)
                {
                    // 释放内存
                    lastFrameCollisionInfos.Remove(kvp.Key);
                }
                else
                {
                    // 如果碰撞信息不过时，则标记为过时
                    kvp.Value.isObsolete = true;
                }
            }
            
            //lastFrameCollisionInfos.Clear();
        }
    }*/

    
    
    
    /// <summary>
    /// OverlappingPairs 是一个数据结构，用于存储所有重叠的碰撞对。
    /// 每当两个碰撞形状之间存在重叠时，就会创建一个 OverlappingPair 对象，并将其添加到 OverlappingPairs 数据结构中。
    /// 这个数据结构允许引擎轻松地追踪和管理所有的碰撞对，以便在物理仿真中进行适当的碰撞检测和响应。
    /// OverlappingPairs 数据结构通常具有以下特性：
    /// 
    /// 存储重叠碰撞对：OverlappingPairs 数据结构存储了所有重叠的碰撞对。
    /// 每个 OverlappingPair 对象都包含了两个碰撞形状之间的重叠关系，并可能包含其他与碰撞相关的信息。
    /// 
    /// 高效的数据访问：OverlappingPairs 数据结构通常设计为高效地支持对重叠碰撞对的访问和检索。
    /// 这可以通过合适的数据结构和算法来实现，以确保在大量碰撞对的情况下也能够快速地执行碰撞检测和碰撞响应。
    /// 
    /// 碰撞对的添加和移除：OverlappingPairs 数据结构通常提供了方法来添加新的碰撞对，以及移除不再重叠的碰撞对。
    /// 这可以确保数据结构始终保持最新的状态，并且不会包含过时的或不必要的碰撞对。
    /// 
    /// 通过 OverlappingPairs 数据结构，能够有效地管理所有的重叠碰撞对，并在物理仿真中快速准确地检测和处理碰撞，从而实现真实的物体运动和相互作用。
    /// </summary>
    
    public class OverlappingPairs
    {
        public List<ConvexOverlappingPair> mConvexPairs;
        //public List<ConcaveOverlappingPair> mConcavePairs;

        public Dictionary<int, int> mMapConvexPairIdToPairIndex;
        //public Dictionary<int, int> mMapConcavePairIdToPairIndex;
        public ColliderComponents mColliderComponents;
        public CollisionDispatch mCollisionDispatch;


        public OverlappingPairs(ColliderComponents colliderComponents,
            CollisionDispatch collisionDispatch)
        {
            mMapConvexPairIdToPairIndex = new Dictionary<int, int>();
            //mMapConcavePairIdToPairIndex = new Dictionary<int, int>();
            mColliderComponents = colliderComponents;
            mCollisionDispatch = collisionDispatch;
            mConvexPairs = new List<ConvexOverlappingPair>();
            //mConcavePairs = new List<ConcaveOverlappingPair>();
        }

        /// <summary>
        /// 将两个碰撞形状添加到重叠碰撞对
        /// </summary>
        /// <param name="collider1Index"></param>
        /// <param name="collider2Index"></param>
        /// <param name="isConvexVsConvex"></param>
        /// <returns></returns>
        public void addPair(int collider1Index, int collider2Index, bool isConvexVsConvex)
        {
            var collisionShape1 = mColliderComponents.mCollisionShapes[collider1Index];
            var collisionShape2 = mColliderComponents.mCollisionShapes[collider2Index];

            var collider1Entity = mColliderComponents.mCollidersEntities[collider1Index];
            var collider2Entity = mColliderComponents.mCollidersEntities[collider2Index];

            var broadPhase1Id = mColliderComponents.mBroadPhaseIds[collider1Index];
            var broadPhase2Id = mColliderComponents.mBroadPhaseIds[collider2Index];

            // Compute a unique id for the overlapping pair
            var pairId = PMath.pairNumbers(PMath.Max(broadPhase1Id, broadPhase2Id),
                PMath.Min(broadPhase1Id, broadPhase2Id));

            // Select the narrow phase algorithm to use according to the two collision shapes
            if (isConvexVsConvex)
            {
                var algorithmType =
                    mCollisionDispatch.selectNarrowPhaseAlgorithm(collisionShape1.GetType(), collisionShape2.GetType());

                // Map the entity with the new component lookup index
                mMapConvexPairIdToPairIndex.Add(pairId, mConvexPairs.Count);

                var concaveOverlappingPair = new ConvexOverlappingPair(pairId, broadPhase1Id,
                    broadPhase2Id, collider1Entity, collider2Entity, algorithmType);

                // Create and add a new convex pair
                mConvexPairs.Add(concaveOverlappingPair);
                
                //Debug.Log("addPair:" + concaveOverlappingPair.PairID);
                
            }
            /*else
            {
                var isShape1Convex = collisionShape1.IsConvex();

                var algorithmType = mCollisionDispatch.selectNarrowPhaseAlgorithm(
                    isShape1Convex ? collisionShape1.GetType() : collisionShape2.GetType(),
                    CollisionShapeType.CONVEX_POLYHEDRON);
                // Map the entity with the new component lookup index
                mMapConvexPairIdToPairIndex.Add(pairId, mConcavePairs.Count);

                var concaveOverlappingPair = new ConcaveOverlappingPair(pairId, broadPhase1Id, broadPhase2Id,
                    collider1Entity, collider2Entity, algorithmType, isShape1Convex);

                // Create and add a new concave pair
                mConcavePairs.Add(concaveOverlappingPair);
            }*/

            // Add the involved overlapping pair to the two colliders
            
            
            mColliderComponents.mOverlappingPairs[collider1Index].Add(pairId);
            mColliderComponents.mOverlappingPairs[collider2Index].Add(pairId);

            // return pairId;
        }

        // Remove a component at a given index
        public void removePair(int pairId)
        {
            if (mMapConvexPairIdToPairIndex.ContainsKey(pairId))
                removePair(mMapConvexPairIdToPairIndex[pairId], true);
            /*else
                removePair(mMapConcavePairIdToPairIndex[pairId], false);*/
        }

        public void removePair(int pairIndex, bool isConvexVsConvex)
        {
            if (isConvexVsConvex)
            {
                var nbConvexPairs = mConvexPairs.Count;

                // 从两个碰撞体中移除涉及的重叠配对
                mColliderComponents.getOverlappingPairs(mConvexPairs[pairIndex].Collider1).Remove(mConvexPairs[pairIndex].PairID);
                mColliderComponents.getOverlappingPairs(mConvexPairs[pairIndex].Collider2).Remove(mConvexPairs[pairIndex].PairID);

                //记录配对 索引的map
                mMapConvexPairIdToPairIndex.Remove(mConvexPairs[pairIndex].PairID);

                // 如果我们将最后一个项目与要移除的项目交换，则更改对 pairId 和凸配对数组中的索引之间的映射
                if (mConvexPairs.Count > 1 && pairIndex < nbConvexPairs - 1)
                    mMapConvexPairIdToPairIndex[mConvexPairs[nbConvexPairs - 1].PairID] = pairIndex;

                // 我们希望保持数组紧凑。因此，当移除一对时，我们将其替换为数组的最后一个元素。
                var last = mConvexPairs.Last();
                mConvexPairs[pairIndex] = last;
                mConvexPairs.RemoveAt(mConvexPairs.Count - 1);
            }
            /*else
            {
                var nbConcavePairs = mConcavePairs.Count();

                // 从两个碰撞体中移除涉及的重叠配对
                mColliderComponents.getOverlappingPairs(mConcavePairs[pairIndex].Collider1)
                    .Remove(mConcavePairs[pairIndex].PairID);
                mColliderComponents.getOverlappingPairs(mConcavePairs[pairIndex].Collider2)
                    .Remove(mConcavePairs[pairIndex].PairID);

                mMapConcavePairIdToPairIndex.Remove(mConcavePairs[pairIndex].PairID);

                // 销毁所有 LastFrameCollisionInfo 对象
                mConcavePairs[pairIndex].destroyLastFrameCollisionInfos();

                // 如果我们将最后一个项目与要移除的项目交换，则更改对 pairId 和凹配对数组中的索引之间的映射
                if (mConcavePairs.Count > 1 && pairIndex < nbConcavePairs - 1)
                    mMapConcavePairIdToPairIndex[mConcavePairs[nbConcavePairs - 1].PairID] = pairIndex;

                // 我们希望保持数组紧凑。因此，当移除一对时，
                // 我们将其替换为数组的最后一个元素。
                var last = mConcavePairs.Last();
                mConcavePairs[pairIndex] = last;
                mConcavePairs.RemoveAt(mConcavePairs.Count - 1);
            }*/
        }
        

        /*public void clearObsoleteLastFrameCollisionInfos()
        {
            // For each concave overlapping pair
            long nbConcavePairs = mConcavePairs.Count;
            for (var i = 0; i < nbConcavePairs; i++) mConcavePairs[i].clearObsoleteLastFrameInfos();
        }*/

        /// <summary>
        /// 将每个碰撞对的 collidingInPreviousFrame 值设置为 collidingInCurrentFrame 值。
        /// 这些值通常用于记录碰撞对在上一帧和当前帧中是否发生了碰撞。
        /// 具体地说，这个操作可能包括以下步骤：
        /// 迭代所有的碰撞对。
        /// 对于每个碰撞对，将 collidingInPreviousFrame 的值更新为 collidingInCurrentFrame 的值。
        /// 这样做可以将当前帧中的碰撞信息更新到上一帧中，以便在下一帧中进行比较和处理。
        /// 通过将上一帧的碰撞状态更新为当前帧的碰撞状态，可以确保在连续的帧之间保持正确的碰撞信息，从而实现更加稳定和连续的碰撞检测和碰撞响应。
        /// </summary>
        public void updateCollidingInPreviousFrame()
        {
            long nbConvexPairs = mConvexPairs.Count;
            for (var i = 0; i < nbConvexPairs; i++)
                mConvexPairs[i].CollidingInPreviousFrame = mConvexPairs[i].CollidingInCurrentFrame;

            /*var nbConcavePairs = mConcavePairs.Count;
            for (var i = 0; i < nbConcavePairs; i++)
                mConcavePairs[i].CollidingInPreviousFrame = mConcavePairs[i].CollidingInCurrentFrame;*/
        }


        // Return the pair of bodies index
        public static Pair<Entity, Entity> computeBodiesIndexPair(Entity body1Entity, Entity body2Entity)
        {
            // Construct the pair of body index
            var indexPair = body1Entity.Id < body2Entity.Id
                ? new Pair<Entity, Entity>(body1Entity, body2Entity)
                : new Pair<Entity, Entity>(body2Entity, body1Entity);
            return indexPair;
        }

        // Set if we need to test a given pair for overlap
        public void setNeedToTestOverlap(int pairId, bool needToTestOverlap)
        {
            if (mMapConvexPairIdToPairIndex.TryGetValue(pairId, out var index))
                mConvexPairs[index].NeedToTestOverlap = needToTestOverlap;
            /*else if (mMapConcavePairIdToPairIndex.TryGetValue(pairId, out index))
                mConcavePairs[index].NeedToTestOverlap = needToTestOverlap;*/
        }

        // 返回一个重叠对的引用
        public OverlappingPair getOverlappingPair(int pairId)
        {
            if (mMapConvexPairIdToPairIndex.TryGetValue(pairId, out var index))
                return mConvexPairs[index];
            
            /*if (mMapConcavePairIdToPairIndex.TryGetValue(pairId, out index)) 
                return mConcavePairs[index];*/
            return null;
        }
    }
}