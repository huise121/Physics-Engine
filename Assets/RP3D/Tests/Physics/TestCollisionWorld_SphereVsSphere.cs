using RP3D;
using UnityEngine;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;

public partial class TestCollisionWorld 
{
    private void testSphereVsSphereCollision()
    {
        var initTransform1 = mSphereBody1.getTransform();
        var initTransform2 = mSphereBody2.getTransform();

        var transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
        var transform2 = new PTransform(new Vector3(17, 20, 50),
            Quaternion.FromEulerAngles(PMath.PI_RP3D / 8.0f, PMath.PI_RP3D / 4.0f, PMath.PI_RP3D / 16.0f));

        // Move spheres to collide with each other
        mSphereBody1.setTransform(transform1);
        mSphereBody2.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mSphereBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mSphereBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mSphereBody2, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mSphereBody2));

        // ----- Test global collision test ----- // 

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mSphereCollider2));

        // Get collision data
        var collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mSphereCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        var swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        var localBody1Point = new PVector3(3, 0, 0);
        var localBody2Point = transform2.GetInverse() * new Vector3(12, 20, 50);
        var penetrationDepth = 1.0f;
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mSphereCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mSphereCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mSphereCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mSphereCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mSphereBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mSphereCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mSphereCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // Reset the init transforms
        mSphereBody1.setTransform(initTransform1);
        mSphereBody2.setTransform(initTransform2);
    }
}