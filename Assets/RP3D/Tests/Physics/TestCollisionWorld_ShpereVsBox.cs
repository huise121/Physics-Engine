using RP3D;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;

public partial class TestCollisionWorld
{
    private void testSphereVsBoxCollision()
    {
        var initTransform1 = mSphereBody1.getTransform();
        var initTransform2 = mBoxBody1.getTransform();

        /********************************************************************************
         * Test Sphere vs Box Face collision                                             *
         *********************************************************************************/

        var transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
        var transform2 = new Transform(new Vector3(14, 20, 50), Quaternion.identity());

        // Move spheres to collide with each other
        mSphereBody1.setTransform(transform1);
        mBoxBody1.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mSphereBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mSphereBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mBoxBody1));

        // ----- Test global collision test ----- // 

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        var collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        var swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        var localBody1Point = new Vector3(3, 0, 0);
        var localBody2Point = new Vector3(-3, 0, 0);
        var penetrationDepth = 2.0f;
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        /********************************************************************************
         * Test Sphere vs Box Edge collision                                             *
         *********************************************************************************/

        transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
        transform2 = new Transform(new Vector3(14, 16, 50), Quaternion.identity());

        // Move spheres to collide with each other
        mSphereBody1.setTransform(transform1);
        mBoxBody1.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mSphereBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mSphereBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mBoxBody1));

        // ----- Test global collision test ----- // 

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        localBody1Point = PMath.Sqrt(4.5f) * new Vector3(1, -1, 0);
        localBody2Point = new Vector3(-3, 3, 0);
        penetrationDepth = 3.0f - PMath.Sqrt(2);
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        /********************************************************************************
         * Test Sphere vs Box Vertex collision                                             *
         *********************************************************************************/

        transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
        transform2 = new Transform(new Vector3(14, 16, 46), Quaternion.identity());

        // Move spheres to collide with each other
        mSphereBody1.setTransform(transform1);
        mBoxBody1.setTransform(transform2);

        mOverlapCallback.reset();
        mWorld.testOverlap(mSphereBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mSphereBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody1, mOverlapCallback);
        rp3d_test(mOverlapCallback.hasOverlapWithBody(mBoxBody1));

        // ----- Test global collision test ----- // 

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        localBody1Point = PMath.Sqrt(9.0f / 3.0f) * new Vector3(1, -1, -1);
        localBody2Point = new Vector3(-3, 3, 3);
        penetrationDepth = 3.0f - PMath.Sqrt(3);
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 1 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against body 2 only ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // ----- Test collision against selected body 1 and 2 ----- //

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mBoxBody1, mCollisionCallback);

        rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mBoxCollider1));

        // Get collision data
        collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mBoxCollider1);
        rp3d_test(collisionData != null);
        rp3d_test(collisionData.getNbContactPairs() == 1);
        rp3d_test(collisionData.getTotalNbContactPoints() == 1);

        // True if the bodies are swapped in the collision callback response
        swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

        // Test contact points
        rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
            swappedBodiesCollisionData ? localBody1Point : localBody2Point,
            penetrationDepth));

        // Reset the init transforms
        mSphereBody1.setTransform(initTransform1);
        mBoxBody1.setTransform(initTransform2);
    }
}