using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public struct ColliderComponent
    {
        public Entity bodyEntity;
        public Collider collider;
        public AABB localBounds;
        public PTransform LocalToBodyPTransform;
        public CollisionShape collisionShape;
        public short collisionCategoryBits;
        public short collideWithMaskBits;
        public PTransform LocalToWorldPTransform;
        public Material material;
    }

    public partial class ColliderComponents : Components
    {
        public ColliderComponents() : base()
        {
            mCollideWithMaskBits = new List<short>(INIT_NB_ALLOCATED_COMPONENTS);
            mCollisionCategoryBits = new List<short>(INIT_NB_ALLOCATED_COMPONENTS);
            mCollideWithMaskBits = new List<short>(INIT_NB_ALLOCATED_COMPONENTS);
            mCollisionCategoryBits = new List<short>(INIT_NB_ALLOCATED_COMPONENTS);
            mOverlappingPairs = new List<List<int>>(INIT_NB_ALLOCATED_COMPONENTS);
            mBodiesEntities = new List<Entity>(INIT_NB_ALLOCATED_COMPONENTS);
            mCollidersEntities = new List<Entity>(INIT_NB_ALLOCATED_COMPONENTS);
            mColliders = new List<Collider>();
            mCollisionShapes = new List<CollisionShape>(INIT_NB_ALLOCATED_COMPONENTS);
            mLocalToWorldTransforms = new List<PTransform>(INIT_NB_ALLOCATED_COMPONENTS);
            mLocalToBodyTransforms = new List<PTransform>(INIT_NB_ALLOCATED_COMPONENTS);
            mIsTrigger = new List<bool>(INIT_NB_ALLOCATED_COMPONENTS);
            mBroadPhaseIds = new List<int>(INIT_NB_ALLOCATED_COMPONENTS);
            mHasCollisionShapeChangedSize = new List<bool>(INIT_NB_ALLOCATED_COMPONENTS);
            mMaterials = new List<Material>();
        }

        public void addComponent(Entity colliderEntity, bool isSleeping, ColliderComponent component)
        {
            var index = prepareAddComponent(isSleeping);
            mOverlappingPairs.Add(new List<int>());
            mCollidersEntities.Add(colliderEntity);
            mBodiesEntities.Add(component.bodyEntity);
            mColliders.Add(component.collider);
            mBroadPhaseIds.Add(-1);
            mLocalToBodyTransforms.Add(component.LocalToBodyPTransform);
            mCollisionShapes.Add(component.collisionShape);
            mCollisionCategoryBits.Add(component.collisionCategoryBits);
            mCollideWithMaskBits.Add(component.collideWithMaskBits);

            mLocalToWorldTransforms.Add(component.LocalToWorldPTransform);
            mHasCollisionShapeChangedSize.Add(false);
            mIsTrigger.Add(false);
            mMaterials.Add(component.material);

            mMapEntityToComponentIndex.Add(colliderEntity, index);
            mNbComponents++;
        }

        protected override void moveComponentToIndex(int srcIndex, int destIndex)
        {
            var colliderEntity = mCollidersEntities[srcIndex];

            // Copy the data of the source component to the destination location
            mCollidersEntities[destIndex] = mCollidersEntities[srcIndex];
            mBodiesEntities[destIndex] = mBodiesEntities[srcIndex];
            mColliders[destIndex] = mColliders[srcIndex];
            mBroadPhaseIds[destIndex] = mBroadPhaseIds[srcIndex];
            mLocalToBodyTransforms[destIndex] = mLocalToBodyTransforms[srcIndex];
            mCollisionShapes[destIndex] = mCollisionShapes[srcIndex];
            mCollisionCategoryBits[destIndex] = mCollisionCategoryBits[srcIndex];
            mCollideWithMaskBits[destIndex] = mCollideWithMaskBits[srcIndex];
            mLocalToWorldTransforms[destIndex] = mLocalToWorldTransforms[srcIndex];
            mOverlappingPairs[destIndex] = mOverlappingPairs[srcIndex];
            mHasCollisionShapeChangedSize[destIndex] = mHasCollisionShapeChangedSize[srcIndex];
            mIsTrigger[destIndex] = mIsTrigger[srcIndex];
            mMaterials[destIndex] = mMaterials[srcIndex];

            // Destroy the source component
            destroyComponent(srcIndex);

            assert(!mMapEntityToComponentIndex.ContainsKey(colliderEntity));

            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(colliderEntity, destIndex);

            assert(mMapEntityToComponentIndex[mCollidersEntities[destIndex]] == destIndex);
        }

        protected override void swapComponents(int index1, int index2)
        {
            // var colliderEntity1 = mCollidersEntities[index1];
            // var colliderEntity2 = mCollidersEntities[index2];
            //
            //
            // mCollidersEntities.Swap(index1,index2);
            // mBodiesEntities.Swap(index1,index2);
            // mColliders.Swap(index1,index2);
            // mBroadPhaseIds.Swap(index1,index2);
            // mLocalToBodyTransforms.Swap(index1,index2);
            // mCollisionShapes.Swap(index1,index2);
            // mCollisionCategoryBits.Swap(index1,index2);
            // mCollideWithMaskBits.Swap(index1,index2);
            // mLocalToWorldTransforms.Swap(index1,index2);
            // mOverlappingPairs.Swap(index1,index2);
            // mHasCollisionShapeChangedSize.Swap(index1,index2);
            // mIsTrigger.Swap(index1,index2);
            // mMaterials.Swap(index1,index2);
            //
            //
            // mMapEntityToComponentIndex[colliderEntity1] = index2;
            // mMapEntityToComponentIndex[colliderEntity2] = index1;
            
            // Copy component 1 data
            var colliderEntity1 = mCollidersEntities[index1];
            var bodyEntity1 = mBodiesEntities[index1];
            var collider1 = mColliders[index1];
            var broadPhaseId1 = mBroadPhaseIds[index1];
            var localToBodyTransform1 = mLocalToBodyTransforms[index1];
            var collisionShape1 = mCollisionShapes[index1];
            var collisionCategoryBits1 = mCollisionCategoryBits[index1];
            var collideWithMaskBits1 = mCollideWithMaskBits[index1];
            var localToWorldTransform1 = mLocalToWorldTransforms[index1];
            var overlappingPairs = mOverlappingPairs[index1];
            var hasCollisionShapeChangedSize = mHasCollisionShapeChangedSize[index1];
            var isTrigger = mIsTrigger[index1];
            var material = mMaterials[index1];
            
            // Destroy component 1
            destroyComponent(index1);
            
            moveComponentToIndex(index2, index1);
            
            // Reconstruct component 1 at component 2 location
            mCollidersEntities[index2] = colliderEntity1;
            mBodiesEntities[index2] = bodyEntity1;
            mColliders[index2] = collider1;
            mBroadPhaseIds[index2] = broadPhaseId1;
            mLocalToBodyTransforms[index2] = localToBodyTransform1;
            mCollisionShapes[index2] = collisionShape1;
            mCollisionCategoryBits[index2] = collisionCategoryBits1;
            mCollideWithMaskBits[index2] = collideWithMaskBits1;
            mLocalToWorldTransforms[index2] = localToWorldTransform1;
            mOverlappingPairs[index2] = overlappingPairs;
            mHasCollisionShapeChangedSize[index2] = hasCollisionShapeChangedSize;
            mIsTrigger[index2] = isTrigger;
            mMaterials[index2] = material;
            
            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(colliderEntity1, index2);
        }

        public override void destroyComponent(int index)
        {
            base.destroyComponent(index);
            assert(mMapEntityToComponentIndex[mCollidersEntities[index]] == index);
            mMapEntityToComponentIndex.Remove(mCollidersEntities[index]);
            mOverlappingPairs[index] = null;
            mCollidersEntities[index] = null;
            mBodiesEntities[index] = null;
            mColliders[index] = null;
            mLocalToBodyTransforms[index] = null;
            mCollisionShapes[index] = null;
            mLocalToWorldTransforms[index] = null;
            mOverlappingPairs[index] = null;
            mMaterials[index] = null;
        }

       
    }
}