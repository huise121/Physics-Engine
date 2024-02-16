using RP3D;
using UnityEngine;

public class PBoxCollider : BaseCollider
{
    [Space(10)] [Header("Base")]
    public Vector3 halfExtents;


    private void Start()
    {
        var world = World.Instance.mWorld;
        mShape = World.Instance.mPhysicsCommon.createBoxShape(new PVector3(halfExtents.x, halfExtents.y,
            halfExtents.z));
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

        gameObject.name = "Box_" + bodyId;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (mBody != null && mBody as RigidBody != null && (mBody as RigidBody).isSleeping()) Gizmos.color = Color.red;

        GizmosExtend.DrawWireCube(transform.position, transform.rotation, halfExtents * 2);
        Gizmos.color = Color.white;
    }
}