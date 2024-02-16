namespace RP3D
{
    public class WorldRaycastCallback:RaycastCallback
    {
        public RaycastInfo raycastInfo;
        public Collider shapeToTest;
        public bool isHit = false;

        public override float NotifyRaycastHit(RaycastInfo info)
        {
            if (shapeToTest.GetBody().getEntity() == info.body.getEntity()) {
                raycastInfo.body = info.body;
                raycastInfo.hitFraction = info.hitFraction;
                raycastInfo.collider = info.collider;
                raycastInfo.worldNormal = info.worldNormal;
                raycastInfo.worldPoint = info.worldPoint;
                isHit = true;
            }

            // Return a fraction of 1.0 because we need to gather all hits
            return 1.0f;
        }
        
        public void reset() {
            raycastInfo.body = null;
            raycastInfo.hitFraction = 0.0f;
            raycastInfo.collider = null;
            raycastInfo.worldNormal.SetToZero();
            raycastInfo.worldPoint.SetToZero();
            isHit = false;
        }
    }
}