using RP3D;
using UnityEngine;
using Collider = RP3D.Collider;

public class BaseCollider : MonoBehaviour
{
    public CollisionBody mBody;
    protected CollisionShape mShape;
    protected Collider mCollider;

    [Space(10)] [Header("RigidBody")] 
    public bool createRigidBody = true;
    public bool trigger;
    public BodyType bodyType = BodyType.KINEMATIC;
    
    [Space(10)] [Header("Material")] 
    public float bounciness = 0.2f;
    public float friction = 0.3f;
    
    [Space(10)] [Header("View")] 
    protected int bodyId;
    protected int colliderId;

    private void Update()
    {
        if (mBody != null)
        {
            var ptransform = mBody.getTransform();
            transform.position = new Vector3(ptransform.GetPosition().x, ptransform.GetPosition().y,
                ptransform.GetPosition().z);
            transform.rotation = new Quaternion(ptransform.GetOrientation().x, ptransform.GetOrientation().y,
                ptransform.GetOrientation().z, ptransform.GetOrientation().w);
        }
    }
}