using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public partial class ColliderComponents
    {
        #region public:

        ///每个组件的刚体实体数组
        public List<Entity> mBodiesEntities;

        ///每个组件的碰撞体实体数组
        public List<Entity> mCollidersEntities;

        ///每个组件的碰撞体指针数组
        public List<Collider> mColliders;

        ///每个组件的碰撞形状实体数组
        public List<CollisionShape> mCollisionShapes;

        ///碰撞体的局部到世界变换数组
        public List<PTransform> mLocalToWorldTransforms;

        ///碰撞体的局部到其所属刚体的体空间变换数组
        public List<PTransform> mLocalToBodyTransforms;

        ///如果碰撞体是触发器，则为true
        public List<bool> mIsTrigger;

        public List<Material> mMaterials;

        public List<short> mCollideWithMaskBits;
        public List<short> mCollisionCategoryBits;

        ///用于宽相算法的碰撞体的标识符数组
        public List<int> mBroadPhaseIds;

        ///宽相重叠对
        public List<List<int>> mOverlappingPairs;

        ///如果与碰撞体相关联的碰撞形状的大小已被用户更改，则为true
        public List<bool> mHasCollisionShapeChangedSize;

        #endregion


        #region inline:

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity getBody(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mBodiesEntities[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Collider getCollider(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mColliders[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PTransform getLocalToBodyTransform(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mLocalToBodyTransforms[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setLocalToBodyTransform(Entity colliderEntity, PTransform pTransform)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            mLocalToBodyTransforms[mMapEntityToComponentIndex[colliderEntity]] = pTransform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CollisionShape getCollisionShape(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mCollisionShapes[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PTransform getLocalToWorldTransform(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mLocalToWorldTransforms[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setLocalToWorldTransform(Entity colliderEntity, PTransform pTransform)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            mLocalToWorldTransforms[mMapEntityToComponentIndex[colliderEntity]] = pTransform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool getIsTrigger(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mIsTrigger[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setIsTrigger(Entity colliderEntity, bool isTrigger)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            mIsTrigger[mMapEntityToComponentIndex[colliderEntity]] = isTrigger;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int getBroadPhaseId(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mBroadPhaseIds[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setBroadPhaseId(Entity colliderEntity, int broadPhaseId)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            mBroadPhaseIds[mMapEntityToComponentIndex[colliderEntity]] = broadPhaseId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // 返回给定碰撞体的重叠对数组
        public List<int> getOverlappingPairs(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mOverlappingPairs[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short getCollisionCategoryBits(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mCollisionCategoryBits[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short getCollideWithMaskBits(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mCollideWithMaskBits[mMapEntityToComponentIndex[colliderEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setCollisionCategoryBits(Entity colliderEntity, short collisionCategoryBits)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            mCollisionCategoryBits[mMapEntityToComponentIndex[colliderEntity]] = collisionCategoryBits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setCollideWithMaskBits(Entity colliderEntity, short collideWithMaskBits)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            mCollideWithMaskBits[mMapEntityToComponentIndex[colliderEntity]] = collideWithMaskBits;
        }


        // Return a reference to the material of a collider
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Material getMaterial(Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            return mMaterials[mMapEntityToComponentIndex[colliderEntity]];
        }

// Set the material of a collider
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setMaterial(Entity colliderEntity, Material material)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(colliderEntity));
            mMaterials[mMapEntityToComponentIndex[colliderEntity]] = material;
        }

        #endregion
    }
}