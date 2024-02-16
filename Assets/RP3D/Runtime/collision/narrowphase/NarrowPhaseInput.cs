namespace RP3D
{
    public enum NarrowPhaseAlgorithmType
    {
        None,
        SphereVsSphere,
        SphereVsCapsule,
        CapsuleVsCapsule,
        SphereVsConvexPolyhedron,
        CapsuleVsConvexPolyhedron,
        ConvexPolyhedronVsConvexPolyhedron
    }


    public class NarrowPhaseInput
    {
        // 球与球之间的窄相碰撞信息批处理
        private readonly NarrowPhaseInfoBatch mSphereVsSphereBatch; 
        // 球与胶囊之间的窄相碰撞信息批处理
        private readonly NarrowPhaseInfoBatch mSphereVsCapsuleBatch; 
        // 胶囊与胶囊之间的窄相碰撞信息批处理
        private readonly NarrowPhaseInfoBatch mCapsuleVsCapsuleBatch; 
        // 球与凸多面体之间的窄相碰撞信息批处理
        private readonly NarrowPhaseInfoBatch mSphereVsConvexPolyhedronBatch; 
        // 胶囊与凸多面体之间的窄相碰撞信息批处理
        private readonly NarrowPhaseInfoBatch mCapsuleVsConvexPolyhedronBatch; 
        // 凸多面体与凸多面体之间的窄相碰撞信息批处理
        private readonly NarrowPhaseInfoBatch mConvexPolyhedronVsConvexPolyhedronBatch; 

        public NarrowPhaseInput(/*OverlappingPairs overlappingPairs*/)
        {
            // 初始化各个类型的碰撞批处理信息
            mSphereVsSphereBatch = new NarrowPhaseInfoBatch(/*overlappingPairs*/);
            mSphereVsCapsuleBatch = new NarrowPhaseInfoBatch(/*overlappingPairs*/);
            mCapsuleVsCapsuleBatch = new NarrowPhaseInfoBatch(/*overlappingPairs*/);
            mSphereVsConvexPolyhedronBatch = new NarrowPhaseInfoBatch(/*overlappingPairs*/);
            mCapsuleVsConvexPolyhedronBatch = new NarrowPhaseInfoBatch(/*overlappingPairs*/);
            mConvexPolyhedronVsConvexPolyhedronBatch = new NarrowPhaseInfoBatch(/*overlappingPairs*/);
        }

        // 获取球与球之间的窄相碰撞信息批处理
        public NarrowPhaseInfoBatch getSphereVsSphereBatch()
        {
            return mSphereVsSphereBatch;
        }

        // 获取球与胶囊之间的窄相碰撞信息批处理
        public NarrowPhaseInfoBatch getSphereVsCapsuleBatch()
        {
            return mSphereVsCapsuleBatch;
        }

        // 获取胶囊与胶囊之间的窄相碰撞信息批处理
        public NarrowPhaseInfoBatch getCapsuleVsCapsuleBatch()
        {
            return mCapsuleVsCapsuleBatch;
        }

        // 获取球与凸多面体之间的窄相碰撞信息批处理
        public NarrowPhaseInfoBatch getSphereVsConvexPolyhedronBatch()
        {
            return mSphereVsConvexPolyhedronBatch;
        }

        // 获取胶囊与凸多面体之间的窄相碰撞信息批处理
        public NarrowPhaseInfoBatch getCapsuleVsConvexPolyhedronBatch()
        {
            return mCapsuleVsConvexPolyhedronBatch;
        }

        // 获取凸多面体与凸多面体之间的窄相碰撞信息批处理
        public NarrowPhaseInfoBatch getConvexPolyhedronVsConvexPolyhedronBatch()
        {
            return mConvexPolyhedronVsConvexPolyhedronBatch;
        }

        // 将待测试的形状添加到窄相碰撞检测批处理中
        public void addNarrowPhaseTest(int pairId, Entity collider1, Entity collider2, CollisionShape shape1,
            CollisionShape shape2, PTransform shape1Transform, PTransform shape2Transform,
            NarrowPhaseAlgorithmType narrowPhaseAlgorithmType, bool reportContacts,
            LastFrameCollisionInfo lastFrameInfo)
        {
            // 根据碰撞算法类型将待测试的形状添加到相应的批处理中
            switch (narrowPhaseAlgorithmType)
            {
                case NarrowPhaseAlgorithmType.SphereVsSphere:
                    mSphereVsSphereBatch.AddNarrowPhaseInfo(pairId, collider1, collider2, shape1, shape2,
                        shape1Transform,
                        shape2Transform, reportContacts, lastFrameInfo);
                    break;
                case NarrowPhaseAlgorithmType.SphereVsCapsule:
                    mSphereVsCapsuleBatch.AddNarrowPhaseInfo(pairId, collider1, collider2, shape1, shape2,
                        shape1Transform,
                        shape2Transform, reportContacts, lastFrameInfo);
                    break;
                case NarrowPhaseAlgorithmType.CapsuleVsCapsule:
                    mCapsuleVsCapsuleBatch.AddNarrowPhaseInfo(pairId, collider1, collider2, shape1, shape2,
                        shape1Transform,
                        shape2Transform, reportContacts, lastFrameInfo);
                    break;
                case NarrowPhaseAlgorithmType.SphereVsConvexPolyhedron:
                    mSphereVsConvexPolyhedronBatch.AddNarrowPhaseInfo(pairId, collider1, collider2, shape1, shape2,
                        shape1Transform, shape2Transform, reportContacts, lastFrameInfo);
                    break;
                case NarrowPhaseAlgorithmType.CapsuleVsConvexPolyhedron:
                    mCapsuleVsConvexPolyhedronBatch.AddNarrowPhaseInfo(pairId, collider1, collider2, shape1, shape2,
                        shape1Transform, shape2Transform, reportContacts, lastFrameInfo);
                    break;
                case NarrowPhaseAlgorithmType.ConvexPolyhedronVsConvexPolyhedron:
                    mConvexPolyhedronVsConvexPolyhedronBatch.AddNarrowPhaseInfo(pairId, collider1, collider2, shape1,
                        shape2, shape1Transform, shape2Transform, reportContacts, lastFrameInfo);
                    break;
                case NarrowPhaseAlgorithmType.None:
                    // 不应该发生
                    // assert(false);
                    break;
            }
        }

        // 预留内存
        public void reserveMemory()
        {
            mSphereVsSphereBatch.reserveMemory();
            mSphereVsCapsuleBatch.reserveMemory();
            mCapsuleVsCapsuleBatch.reserveMemory();
            mSphereVsConvexPolyhedronBatch.reserveMemory();
            mCapsuleVsConvexPolyhedronBatch.reserveMemory();
            mConvexPolyhedronVsConvexPolyhedronBatch.reserveMemory();
        }

        // 清空窄相碰撞检测批处理
        public void clear()
        {
            mSphereVsSphereBatch.clear();
            mSphereVsCapsuleBatch.clear();
            mCapsuleVsCapsuleBatch.clear();
            mSphereVsConvexPolyhedronBatch.clear();
            mCapsuleVsConvexPolyhedronBatch.clear();
            mConvexPolyhedronVsConvexPolyhedronBatch.clear();
        }
    }
}