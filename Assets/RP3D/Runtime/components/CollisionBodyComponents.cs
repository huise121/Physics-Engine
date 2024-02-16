using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public struct CollisionBodyComponent
    {
        public CollisionBody body;
    }

    public partial class CollisionBodyComponents : Components
    {
        public CollisionBodyComponents() : base()
        {
            mColliders = new List<List<Entity>>();
            mIsActive = new List<bool>();
            mBodiesEntities = new List<Entity>();
            mBodies = new List<CollisionBody>();
        }

        public void addComponent(Entity bodyEntity, bool isSleeping, CollisionBodyComponent component)
        {
            // Prepare to add new component (allocate memory if necessary and compute insertion index)
            var index = prepareAddComponent(isSleeping);
            mBodiesEntities.Add(bodyEntity);
            //mBodies[index] = component.body;
            mBodies.Add(component.body);


            mColliders.Add(new List<Entity>());
            mIsActive.Add(true);

            // Map the entity with the new component lookup index
            mMapEntityToComponentIndex[bodyEntity] = index;
            mNbComponents++;
        }

        protected override void moveComponentToIndex(int srcIndex, int destIndex)
        {
            var entity = mBodiesEntities[srcIndex];

            // Copy the data of the source component to the destination location
            mBodiesEntities[destIndex] = mBodiesEntities[srcIndex];

            mBodies[destIndex] = mBodies[srcIndex];

            mColliders[destIndex] = mColliders[srcIndex];
            mIsActive[destIndex] = mIsActive[srcIndex];

            // Destroy the source component
            destroyComponent(srcIndex);

            assert(!mMapEntityToComponentIndex.ContainsKey(entity));

            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(entity, destIndex);

            assert(mMapEntityToComponentIndex[mBodiesEntities[destIndex]] == destIndex);
        }

        protected override void swapComponents(int index1, int index2)
        {
            // Copy component 1 data
            var entity1 = mBodiesEntities[index1];
            var body1 = mBodies[index1];
            var colliders1 = mColliders[index1];
            var isActive1 = mIsActive[index1];

            // Destroy component 1
            destroyComponent(index1);

            moveComponentToIndex(index2, index1);

            // Reconstruct component 1 at component 2 location
            mBodiesEntities[index2] = entity1;
            mColliders[index2] = colliders1;
            mBodies[index2] = body1;
            mIsActive[index2] = isActive1;

            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(entity1, index2);
        }


      

       
    }
}