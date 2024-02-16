using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public partial class TransformComponents
    {
        private  List<Entity> mBodies;
        private  List<PTransform> mTransforms;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PTransform getTransform(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mTransforms[mMapEntityToComponentIndex[bodyEntity]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setTransform(Entity bodyEntity, PTransform pTransform)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mTransforms[mMapEntityToComponentIndex[bodyEntity]] = pTransform;
        }
    }
}