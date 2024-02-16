using RP3D;
using UnityEngine;

public class PSphereCollider : BaseCollider
{
    [Space(10)] [Header("Base")]
    public float radius;

    private void Start()
    {
        var world = World.Instance.mWorld;


        mShape = World.Instance.mPhysicsCommon.createSphereShape(radius);
        var mPreviousTransform = new PTransform();
        var rotation = transform.rotation;
        mPreviousTransform.SetOrientation(new PQuaternion(rotation.x, rotation.y, rotation.z, rotation.w));
        var position = transform.position;
        mPreviousTransform.SetPosition(new PVector3(position.x, position.y, position.z));


        if (createRigidBody)
        {
            var body = world.createRigidBody(mPreviousTransform);
            mCollider = body.addCollider(mShape, PTransform.Identity());
            body.updateMassPropertiesFromColliders();
            mCollider.setIsTrigger(trigger);
            body.setType(bodyType);
            mCollider.getMaterial().setBounciness(bounciness);
            mCollider.getMaterial().setFrictionCoefficient(friction);
            mBody = body;
        }
        else
        {
            mBody = world.createCollisionBody(mPreviousTransform);
            mCollider = mBody.addCollider(mShape, PTransform.Identity());
        }
        bodyId = (int)mBody.getEntity().Id;
        colliderId = (int)mCollider.mEntity.Id;
        
        gameObject.name = "Sphere_" + bodyId;

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (mBody != null && mBody as RigidBody != null && (mBody as RigidBody).isSleeping()) Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}