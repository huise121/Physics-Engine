using RP3D;
using Scenes.Test;
using UnityEngine;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;

public partial class TestCollisionWorld 
{
    		void testSphereVsCapsuleCollision() {

			Transform initTransform1 = mSphereBody1.getTransform();
			Transform initTransform2 = mCapsuleBody1.getTransform();

			/********************************************************************************
			* Test Sphere vs Capsule (sphere side) collision                                             *
			*********************************************************************************/

			Transform transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
			Transform transform2 = new Transform(new Vector3(10, 14, 50), Quaternion.identity());

			// Move spheres to collide with each other
			mSphereBody1.setTransform(transform1);
			mCapsuleBody1.setTransform(transform2);

			mOverlapCallback.reset();
            mWorld.testOverlap(mSphereBody1, mOverlapCallback);
            rp3d_test(mOverlapCallback.hasOverlapWithBody(mSphereBody1));

			mOverlapCallback.reset();
            mWorld.testOverlap(mCapsuleBody1, mOverlapCallback);
            rp3d_test(mOverlapCallback.hasOverlapWithBody(mCapsuleBody1));

			// ----- Test global collision test ----- // 

			mCollisionCallback.reset();
            mWorld.testCollision(mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            CollisionData collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
            rp3d_test(collisionData != null);
            rp3d_test(collisionData.getNbContactPairs() == 1);
            rp3d_test(collisionData.getTotalNbContactPoints() == 1);

			// True if the bodies are swapped in the collision callback response
            bool swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

			// Test contact points
			Vector3 localBody1Point = new PVector3(0, -3, 0);
			Vector3 localBody2Point = new PVector3(0, 5, 0);
			float penetrationDepth = 2.0f;
            rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
				swappedBodiesCollisionData ? localBody1Point : localBody2Point,
				penetrationDepth));

			// ----- Test collision against body 1 only ----- //

			mCollisionCallback.reset();
            mWorld.testCollision(mSphereBody1, mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
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
            mWorld.testCollision(mCapsuleBody1, mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
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
            mWorld.testCollision(mSphereBody1, mCapsuleBody1, mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
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
			* Test Sphere vs Box Capsule (cylinder side) collision                          *
			*********************************************************************************/

			transform1 = new Transform(new Vector3(10, 20, 50), Quaternion.identity());
			transform2 = new Transform(new Vector3(14, 19, 50), Quaternion.identity());

			// Move spheres to collide with each other
			mSphereBody1.setTransform(transform1);
			mCapsuleBody1.setTransform(transform2);

			mOverlapCallback.reset();
            mWorld.testOverlap(mSphereBody1, mOverlapCallback);
            rp3d_test(mOverlapCallback.hasOverlapWithBody(mSphereBody1));

			mOverlapCallback.reset();
            mWorld.testOverlap(mCapsuleBody1, mOverlapCallback);
            rp3d_test(mOverlapCallback.hasOverlapWithBody(mCapsuleBody1));

			// ----- Test global collision test ----- // 

			mCollisionCallback.reset();
            mWorld.testCollision(mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
            rp3d_test(collisionData != null);
            rp3d_test(collisionData.getNbContactPairs() == 1);
            rp3d_test(collisionData.getTotalNbContactPoints() == 1);

			// True if the bodies are swapped in the collision callback response
            swappedBodiesCollisionData = collisionData.getBody1().getEntity() != mSphereBody1.getEntity();

			// Test contact points
			localBody1Point = new Vector3(3, 0, 0);
			localBody2Point = new Vector3(-2, 1, 0);
			penetrationDepth = 1.0f;
            rp3d_test(collisionData.hasContactPointSimilarTo(swappedBodiesCollisionData ? localBody2Point : localBody1Point,
				swappedBodiesCollisionData ? localBody1Point : localBody2Point,
				penetrationDepth));

			// ----- Test collision against body 1 only ----- //

			mCollisionCallback.reset();
            mWorld.testCollision(mSphereBody1, mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
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
            mWorld.testCollision(mCapsuleBody1, mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
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
            mWorld.testCollision(mSphereBody1, mCapsuleBody1, mCollisionCallback);

            rp3d_test(mCollisionCallback.areCollidersColliding(mSphereCollider1, mCapsuleCollider1));

			// Get collision data
            collisionData = mCollisionCallback.getCollisionData(mSphereCollider1, mCapsuleCollider1);
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
			mCapsuleBody1.setTransform(initTransform2);
		}
}