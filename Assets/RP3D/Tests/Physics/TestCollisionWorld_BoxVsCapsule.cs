using RP3D;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;

public partial class TestCollisionWorld
{
    private void testBoxVsCapsuleCollision()
    {
        var initTransform1 = mBoxBody1.getTransform();
        var initTransform2 = mCapsuleBody1.getTransform();

        /********************************************************************************
         * Test Box vs Capsule collision                                             *
         *********************************************************************************/

        var transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
        var transform2 =
            new PTransform(new Vector3(17, 21, 50), Quaternion.FromEulerAngles(0, 0, PMath.PI_RP3D * 0.5f));

        // Move spheres to collide with each other
        mBoxBody1.setTransform(transform1);
        mCapsuleBody1.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mBoxBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mCapsuleBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mCapsuleBody1));

        // ----- Test global collision test ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mCapsuleCollider1));

        // Get collision data
        var collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mCapsuleCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        var swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        var localBody1Point1 = new PVector3(3, 1, 0);
        var localBody2Point1 = new PVector3(0, 5, 0);
        var penetrationDepth1 = 1.0f;
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));
        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mCapsuleCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mCapsuleCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));

        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCapsuleBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mCapsuleCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mCapsuleCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mCapsuleBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mCapsuleCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mCapsuleCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));

        // reset the init transforms
        mBoxBody1.setTransform(initTransform1);
        mCapsuleBody1.setTransform(initTransform2);
    }
}