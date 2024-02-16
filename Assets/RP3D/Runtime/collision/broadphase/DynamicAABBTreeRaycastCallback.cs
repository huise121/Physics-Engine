namespace RP3D
{
    public abstract class DynamicAABBTreeRaycastCallback
    {
        public abstract float raycastBroadPhaseShape(int nodeId, PRay ray);
    }
}