using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public partial class CollisionBodyComponents
    {
        #region public:

        private readonly List<bool> mIsActive;
        private readonly List<List<Entity>> mColliders;
        private readonly List<Entity> mBodiesEntities;
        private readonly List<CollisionBody> mBodies;

        #endregion

        
        #region inline:

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void addColliderToBody(Entity bodyEntity, Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mColliders[mMapEntityToComponentIndex[bodyEntity]].Add(colliderEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool getIsActive(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mIsActive[mMapEntityToComponentIndex[bodyEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<Entity> getColliders(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mColliders[mMapEntityToComponentIndex[bodyEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CollisionBody getBody(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mBodies[mMapEntityToComponentIndex[bodyEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setIsActive(Entity bodyEntity, bool isActive)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mIsActive[mMapEntityToComponentIndex[bodyEntity]] = isActive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void removeColliderFromBody(Entity bodyEntity, Entity colliderEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mColliders[mMapEntityToComponentIndex[bodyEntity]].Remove(colliderEntity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void destroyComponent(int index)
        {
            base.destroyComponent(index);
            assert(mMapEntityToComponentIndex[mBodiesEntities[index]] == index);
            mMapEntityToComponentIndex.Remove(mBodiesEntities[index]);

            mBodiesEntities[index] = null;
            mBodies[index] = null;
            mColliders[index] = null;
        }

        #endregion
    }
}