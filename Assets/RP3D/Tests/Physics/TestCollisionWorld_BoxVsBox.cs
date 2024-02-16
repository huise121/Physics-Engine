using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;

public partial class TestCollisionWorld
{
    private void testBoxVsBoxCollision()
    {
        var initTransform1 = mBoxBody1.getTransform();
        var initTransform2 = mBoxBody2.getTransform();

        /********************************************************************************
         * Test Box vs Box Face collision                                             *
         *********************************************************************************/

        var transform1 = new Transform(new Vector3(11, 20, 50), Quaternion.identity());
        var transform2 = new Transform(new Vector3(4.5f, 16, 40), Quaternion.identity());

        // Move spheres to collide with each other
        mBoxBody1.setTransform(transform1);
        mBoxBody2.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mBoxBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody2, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mBoxBody2));

        // ----- Test global collision test ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mBoxCollider2));

        // Get collision data
        var collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mBoxCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 4);

        // True if the bodies are swapped in the collision callback response
        var swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        var localBody1Point1 = new Vector3(-3, -2, -2);
        var localBody2Point1 = new Vector3(4, 2, 8);
        var penetrationDepth1 = 0.5f;
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));
        var localBody1Point2 = new Vector3(-3, -2, -3);
        var localBody2Point2 = new Vector3(4, 2, 7);
        var penetrationDepth2 = 0.5f;
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point2 : localBody1Point2,
            swappedBodiesCollisionData ? localBody1Point2 : localBody2Point2,
            penetrationDepth2));
        var localBody1Point3 = new Vector3(-3, -3, -2);
        var localBody2Point3 = new Vector3(4, 1, 8);
        var penetrationDepth3 = 0.5f;
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point3 : localBody1Point3,
            swappedBodiesCollisionData ? localBody1Point3 : localBody2Point3,
            penetrationDepth3));
        var localBody1Point4 = new Vector3(-3, -3, -3);
        var localBody2Point4 = new Vector3(4, 1, 7);
        var penetrationDepth4 = 0.5f;
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point4 : localBody1Point4,
            swappedBodiesCollisionData ? localBody1Point4 : localBody2Point4,
            penetrationDepth4));

        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mBoxCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mBoxCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 4);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point2 : localBody1Point2,
            swappedBodiesCollisionData ? localBody1Point2 : localBody2Point2,
            penetrationDepth2));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point3 : localBody1Point3,
            swappedBodiesCollisionData ? localBody1Point3 : localBody2Point3,
            penetrationDepth3));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point4 : localBody1Point4,
            swappedBodiesCollisionData ? localBody1Point4 : localBody2Point4,
            penetrationDepth4));
        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mBoxCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mBoxCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 4);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point2 : localBody1Point2,
            swappedBodiesCollisionData ? localBody1Point2 : localBody2Point2,
            penetrationDepth2));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point3 : localBody1Point3,
            swappedBodiesCollisionData ? localBody1Point3 : localBody2Point3,
            penetrationDepth3));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point4 : localBody1Point4,
            swappedBodiesCollisionData ? localBody1Point4 : localBody2Point4,
            penetrationDepth4));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mBoxBody2, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mBoxCollider1, mBoxCollider2));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mBoxCollider1, mBoxCollider2);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 4);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mBoxBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point1 : localBody1Point1,
            swappedBodiesCollisionData ? localBody1Point1 : localBody2Point1,
            penetrationDepth1));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point2 : localBody1Point2,
            swappedBodiesCollisionData ? localBody1Point2 : localBody2Point2,
            penetrationDepth2));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point3 : localBody1Point3,
            swappedBodiesCollisionData ? localBody1Point3 : localBody2Point3,
            penetrationDepth3));
        rp3d_test(collisionData.hasContactPointSimilarTo(
            swappedBodiesCollisionData ? localBody2Point4 : localBody1Point4,
            swappedBodiesCollisionData ? localBody1Point4 : localBody2Point4,
            penetrationDepth4));

        // reset the init transforms
        mBoxBody1.setTransform(initTransform1);
        mBoxBody2.setTransform(initTransform2);
    }
}