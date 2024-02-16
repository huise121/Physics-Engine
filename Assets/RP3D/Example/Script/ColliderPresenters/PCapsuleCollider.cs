using RP3D;
using UnityEngine;

public class PCapsuleCollider : BaseCollider
{
    [Space(10)] [Header("Base")]
    public float radius;
    public float height;

    private void Start()
    {
        var world = World.Instance.mWorld;
        mShape = World.Instance.mPhysicsCommon.createCapsuleShape(radius, height);
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
        
        gameObject.name = "Capsule_" + bodyId;

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (mBody != null && mBody as RigidBody != null && (mBody as RigidBody).isSleeping()) Gizmos.color = Color.red;
        var transform1 = transform;
        var localScale = transform1.localScale;
        GizmosExtend.DrawWireCapsule(transform1.position, transform1.rotation,
            radius,
            height);
    }
}