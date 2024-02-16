using System.Collections.Generic;

namespace RP3D
{
    public class CollisionDispatch
    {
        private const int NB_COLLISION_SHAPE_TYPES = 4;

        // 是否默认使用球 vs 球算法
        protected bool mIsSphereVsSphereDefault = true;
        // 是否默认使用胶囊 vs 胶囊算法
        protected bool mIsCapsuleVsCapsuleDefault = true;
        // 是否默认使用球 vs 胶囊算法
        protected bool mIsSphereVsCapsuleDefault = true;
        // 是否默认使用球 vs 凸多面体算法
        protected bool mIsSphereVsConvexPolyhedronDefault = true;
        // 是否默认使用胶囊 vs 凸多面体算法
        protected bool mIsCapsuleVsConvexPolyhedronDefault = true;
        // 是否默认使用凸多面体 vs 凸多面体算法
        protected bool mIsConvexPolyhedronVsConvexPolyhedronDefault = true;
        // 球 vs 球碰撞算法
        protected SphereVsSphereAlgorithm mSphereVsSphereAlgorithm;
        // 胶囊 vs 胶囊碰撞算法
        protected CapsuleVsCapsuleAlgorithm mCapsuleVsCapsuleAlgorithm;
        // 球 vs 胶囊碰撞算法
        protected SphereVsCapsuleAlgorithm mSphereVsCapsuleAlgorithm;
        // 球 vs 凸多面体碰撞算法
        protected SphereVsConvexPolyhedronAlgorithm mSphereVsConvexPolyhedronAlgorithm;
        // 胶囊 vs 凸多面体碰撞算法
        protected CapsuleVsConvexPolyhedronAlgorithm mCapsuleVsConvexPolyhedronAlgorithm;
        // 凸多面体 vs 凸多面体碰撞算法
        protected ConvexPolyhedronVsConvexPolyhedronAlgorithm mConvexPolyhedronVsConvexPolyhedronAlgorithm;
        
        // 碰撞检测矩阵（要使用的算法）
        protected List<NarrowPhaseAlgorithmType[]> mCollisionMatrix;

        // 选择并返回两种碰撞形状之间要使用的窄相碰撞检测算法
        public NarrowPhaseAlgorithmType selectAlgorithm(int type1, int type2)
        {
            var shape1Type = (CollisionShapeType)type1;
            var shape2Type = (CollisionShapeType)type2;

            if (type1 > type2) return NarrowPhaseAlgorithmType.None;
            if (shape1Type == CollisionShapeType.SPHERE && shape2Type == CollisionShapeType.SPHERE)
                return NarrowPhaseAlgorithmType.SphereVsSphere;
            if (shape1Type == CollisionShapeType.SPHERE && shape2Type == CollisionShapeType.CAPSULE)
                return NarrowPhaseAlgorithmType.SphereVsCapsule;
            if (shape1Type == CollisionShapeType.CAPSULE && shape2Type == CollisionShapeType.CAPSULE)
                return NarrowPhaseAlgorithmType.CapsuleVsCapsule;
            if (shape1Type == CollisionShapeType.SPHERE && shape2Type == CollisionShapeType.CONVEX_POLYHEDRON)
                return NarrowPhaseAlgorithmType.SphereVsConvexPolyhedron;
            if (shape1Type == CollisionShapeType.CAPSULE && shape2Type == CollisionShapeType.CONVEX_POLYHEDRON)
                return NarrowPhaseAlgorithmType.CapsuleVsConvexPolyhedron;
            if (shape1Type == CollisionShapeType.CONVEX_POLYHEDRON &&
                shape2Type == CollisionShapeType.CONVEX_POLYHEDRON)
                return NarrowPhaseAlgorithmType.ConvexPolyhedronVsConvexPolyhedron;

            return NarrowPhaseAlgorithmType.None;
        }

        // 设置球 vs 球窄相碰撞检测算法
        public void setSphereVsSphereAlgorithm(SphereVsSphereAlgorithm algorithm)
        {
            if (mIsSphereVsSphereDefault)
            {
                mSphereVsSphereAlgorithm = null;
                mIsSphereVsSphereDefault = false;
            }

            mSphereVsSphereAlgorithm = algorithm;
            fillInCollisionMatrix();
        }

        // 设置球 vs 胶囊窄相碰撞检测算法
        public void setSphereVsCapsuleAlgorithm(SphereVsCapsuleAlgorithm algorithm)
        {
            if (mIsSphereVsCapsuleDefault)
            {
                mSphereVsCapsuleAlgorithm = null;
                mIsSphereVsCapsuleDefault = false;
            }

            mSphereVsCapsuleAlgorithm = algorithm;
            fillInCollisionMatrix();
        }

        // 设置胶囊 vs 胶囊窄相碰撞检测算法
        public void setCapsuleVsCapsuleAlgorithm(CapsuleVsCapsuleAlgorithm algorithm)
        {
            if (mIsCapsuleVsCapsuleDefault)
            {
                algorithm = null;
                mIsCapsuleVsCapsuleDefault = false;
            }

            mCapsuleVsCapsuleAlgorithm = algorithm;
            fillInCollisionMatrix();
        }

        // 设置球 vs 凸多面体窄相碰撞检测算法
        private void setSphereVsConvexPolyhedronAlgorithm(SphereVsConvexPolyhedronAlgorithm algorithm)
        {
            if (mIsSphereVsConvexPolyhedronDefault)
            {
                mSphereVsConvexPolyhedronAlgorithm = null;
                mIsSphereVsConvexPolyhedronDefault = false;
            }

            mSphereVsConvexPolyhedronAlgorithm = algorithm;
            fillInCollisionMatrix();
        }

        // 设置胶囊 vs 凸多面体窄相碰撞检测算法
        private void setCapsuleVsConvexPolyhedronAlgorithm(CapsuleVsConvexPolyhedronAlgorithm algorithm)
        {
            if (mIsCapsuleVsConvexPolyhedronDefault)
            {
                mCapsuleVsConvexPolyhedronAlgorithm = null;
                mIsCapsuleVsConvexPolyhedronDefault = false;
            }

            mCapsuleVsConvexPolyhedronAlgorithm = algorithm;
            fillInCollisionMatrix();
        }

        // 设置凸多面体 vs 凸多面体窄相碰撞检测算法
        public void setConvexPolyhedronVsConvexPolyhedronAlgorithm(
            ConvexPolyhedronVsConvexPolyhedronAlgorithm algorithm)
        {
            if (mIsConvexPolyhedronVsConvexPolyhedronDefault)
            {
                mConvexPolyhedronVsConvexPolyhedronAlgorithm = null;
                mIsConvexPolyhedronVsConvexPolyhedronDefault = false;
            }

            mConvexPolyhedronVsConvexPolyhedronAlgorithm = algorithm;
            fillInCollisionMatrix();
        }

        public CollisionDispatch()
        {
            mCollisionMatrix = new List<NarrowPhaseAlgorithmType[]>(4);
            mCollisionMatrix.Add(new NarrowPhaseAlgorithmType[4]);
            mCollisionMatrix.Add(new NarrowPhaseAlgorithmType[4]);
            mCollisionMatrix.Add(new NarrowPhaseAlgorithmType[4]);
            mCollisionMatrix.Add(new NarrowPhaseAlgorithmType[4]);

            mSphereVsSphereAlgorithm = new SphereVsSphereAlgorithm();
            mSphereVsCapsuleAlgorithm = new SphereVsCapsuleAlgorithm();
            mCapsuleVsCapsuleAlgorithm = new CapsuleVsCapsuleAlgorithm();
            mSphereVsConvexPolyhedronAlgorithm = new SphereVsConvexPolyhedronAlgorithm();
            mCapsuleVsConvexPolyhedronAlgorithm = new CapsuleVsConvexPolyhedronAlgorithm();
            mConvexPolyhedronVsConvexPolyhedronAlgorithm = new ConvexPolyhedronVsConvexPolyhedronAlgorithm();

            fillInCollisionMatrix();
        }

        // 获取球 vs 球窄相碰撞检测算法
        public SphereVsSphereAlgorithm getSphereVsSphereAlgorithm()
        {
            return mSphereVsSphereAlgorithm;
        }

        // 获取球 vs 胶囊窄相碰撞检测算法
        public SphereVsCapsuleAlgorithm getSphereVsCapsuleAlgorithm()
        {
            return mSphereVsCapsuleAlgorithm;
        }

        // 获取胶囊 vs 胶囊窄相碰撞检测算法
        public CapsuleVsCapsuleAlgorithm getCapsuleVsCapsuleAlgorithm()
        {
            return mCapsuleVsCapsuleAlgorithm;
        }

        // 获取球 vs 凸多面体窄相碰撞检测算法
        public SphereVsConvexPolyhedronAlgorithm getSphereVsConvexPolyhedronAlgorithm()
        {
            return mSphereVsConvexPolyhedronAlgorithm;
        }

        // 获取胶囊 vs 凸多面体窄相碰撞检测算法
        public CapsuleVsConvexPolyhedronAlgorithm getCapsuleVsConvexPolyhedronAlgorithm()
        {
            return mCapsuleVsConvexPolyhedronAlgorithm;
        }

        // 获取凸多面体 vs 凸多面体窄相碰撞检测算法
        public ConvexPolyhedronVsConvexPolyhedronAlgorithm getConvexPolyhedronVsConvexPolyhedronAlgorithm()
        {
            return mConvexPolyhedronVsConvexPolyhedronAlgorithm;
        }

        // 填充碰撞检测矩阵
        private void fillInCollisionMatrix()
        {
            for (var i = 0; i < NB_COLLISION_SHAPE_TYPES; i++)
            for (var j = 0; j < NB_COLLISION_SHAPE_TYPES; j++)
                mCollisionMatrix[i][j] = selectAlgorithm(i, j);
        }

        // 返回要为两个碰撞形状使用的相应窄相碰撞算法类型
        public NarrowPhaseAlgorithmType selectNarrowPhaseAlgorithm(CollisionShapeType shape1Type,
            CollisionShapeType shape2Type)
        {
            var shape1Index = (int)shape1Type;
            var shape2Index = (int)shape2Type;

            if (shape1Index > shape2Index)
                return mCollisionMatrix[shape2Index][shape1Index];

            return mCollisionMatrix[shape1Index][shape2Index];
        }
    }
}