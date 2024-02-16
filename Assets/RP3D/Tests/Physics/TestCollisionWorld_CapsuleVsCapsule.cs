using RP3D;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;

public partial class TestCollisionWorld
{
    private void testCapsuleVsCapsuleCollision()
    {
        var initTransform1 = mCapsuleBody1.getTransform();
        var initTransform2 = mCapsuleBody2.getTransform();

        /********************************************************************************
         * Test Capsule (sphere cap) vs Capsule (sphere cap) collision                                             *
         *********************************************************************************/

        var transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
        var transform2 =
            new PTransform(new Vector3(16, 23, 50), Quaternion.FromEulerAngles(0, 0, PMath.PI_RP3D * 0.5f));

        // Move spheres to collide with each other
        mCapsuleBody1.setTransform(transform1);
        mCapsuleBody2.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mCapsuleBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mCapsuleBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mCapsuleBody2, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mCapsuleBody2));

        // ----- Test global collision test ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        var collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        var swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        var localBody1Point = new PVector3(2, 3, 0);
        var localBody2Point = new PVector3(0, 5, 0);
        var penetrationDepth = 1.0f;
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCapsuleBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCapsuleBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCapsuleBody1, mCapsuleBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        /********************************************************************************
         * Test Capsule (sphere cap) vs Capsule (cylinder side) collision                          *
         *********************************************************************************/

        transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
        transform2 = new Transform(new Vector3(10, 27, 50), Quaternion.FromEulerAngles(0, 0, PMath.PI_RP3D * 0.5f));

        // Move spheres to collide with each other
        mCapsuleBody1.setTransform(transform1);
        mCapsuleBody2.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mCapsuleBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mCapsuleBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mCapsuleBody2, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mCapsuleBody2));

        // ----- Test global collision test ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        localBody1Point = new Vector3(0, 5, 0);
        localBody2Point = new Vector3(-3, 0, 0);
        penetrationDepth = 1.0f;
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCapsuleBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCapsuleBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCapsuleBody1, mCapsuleBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mCapsuleCollider1, mCapsuleCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mCapsuleCollider1, mCapsuleCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mCapsuleBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // Reset the init transforms
        mCapsuleBody1.setTransform(initTransform1);
        mCapsuleBody2.setTransform(initTransform2);
    }
}