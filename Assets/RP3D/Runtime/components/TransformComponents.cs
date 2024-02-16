using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public struct TransformComponent
    {
        public PTransform PTransform;
    }

    public partial class TransformComponents : Components
    {
        public TransformComponents() : base()
        {
            mBodies = new List<Entity>();
            mTransforms = new List<PTransform>();
        }

        public void AddComponent(Entity bodyEntity, bool isSleeping, TransformComponent component)
        {
            var index = prepareAddComponent(isSleeping);
            mBodies.Add(bodyEntity);
            mTransforms.Add(component.PTransform);
            mMapEntityToComponentIndex[bodyEntity] = index;
            mNbComponents++;
            // Implementation...
        }

        protected override void moveComponentToIndex(int srcIndex, int destIndex) {

             Entity entity = mBodies[srcIndex];

            // Copy the data of the source component to the destination location
            mBodies[destIndex] = (mBodies[srcIndex]);
            mTransforms[destIndex] = (mTransforms[srcIndex]);

            // Destroy the source component
            destroyComponent(srcIndex);

            assert(!mMapEntityToComponentIndex.ContainsKey(entity));

            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(entity, destIndex);

            assert(mMapEntityToComponentIndex[mBodies[destIndex]] == destIndex);
        }

        protected override  void swapComponents(int index1, int index2) {

            // Copy component 1 data
            Entity entity1 = (mBodies[index1]);
            PTransform transform1 = (mTransforms[index1]);

            // Destroy component 1
            destroyComponent(index1);

            moveComponentToIndex(index2, index1);

            // Reconstruct component 1 at component 2 location
            mBodies[index2] = (entity1);
            mTransforms[index2] =  (transform1);

            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(entity1, index2);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void destroyComponent(int index)
        {
            base.destroyComponent(index);
            mMapEntityToComponentIndex.Remove(mBodies[index]);
            mBodies[index] = null;
            mTransforms[index] = null;
            // 添加适当的销毁组件的代码
        }


    }
}