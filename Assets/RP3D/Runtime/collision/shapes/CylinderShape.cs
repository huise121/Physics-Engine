namespace RP3D
{
    public class CylinderShape : ConvexShape
    {
        protected float mHalfHeight;


        public CylinderShape(float radius, float height) : base(CollisionShapeName.CYLINDER,
            CollisionShapeType.CYLINDER)
        {
        }


        public override bool IsPolyhedron()
        {
            throw new System.NotImplementedException();
        }

        public override void GetLocalBounds(out PVector3 min, out PVector3 max)
        {
            throw new System.NotImplementedException();
        }

        public override float getVolume()
        {
            throw new System.NotImplementedException();
        }

        public override PVector3 getLocalInertiaTensor(float mass)
        {
            throw new System.NotImplementedException();
        }

        public override bool Raycast(PRay pRay, out RaycastInfo raycastInfo, Collider collider)
        {
            throw new System.NotImplementedException();
        }

        public override bool testPointInside(PVector3 localPoint, Collider collider)
        {
            throw new System.NotImplementedException();
        }

        public override PVector3 getLocalSupportPointWithoutMargin(PVector3 direction)
        {
            throw new System.NotImplementedException();
        }
    }
}