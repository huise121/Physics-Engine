using RP3D;
using UnityEngine;
using Collider = RP3D.Collider;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;
using RigidBody = RP3D.RigidBody;

namespace Scenes.Test
{
    public class TestRigidBody: MonoBehaviour
    {
        PhysicsCommon mPhysicsCommon;
        PhysicsWorld mWorld;

        RigidBody mRigidBody1;
        RigidBody mRigidBody2Box;
        RigidBody mRigidBody2Sphere;
        //RigidBody mRigidBody2Convex;
        RigidBody mRigidBody3;

        Collider mBoxCollider;
        Collider mSphereCollider;
        Collider mConvexMeshCollider;

        // PolygonVertexArray mConvexMeshPolygonVertexArray;
        // PolyhedronMesh mConvexMeshPolyhedronMesh;
        // PolygonFace mConvexMeshPolygonFaces;
        float[] mConvexMeshCubeVertices;
        int[] mConvexMeshCubeIndices;

        private void Start()
        {
            init();
            
            testGettersSetters();
            testMassPropertiesMethods();
            testApplyForcesAndTorques();
        }

        void init()
        {
            float[] mConvexMeshCubeVertices = new float[8 * 3];
            int[] mConvexMeshCubeIndices = new int[24];
            
            var worldSettings = new WorldSettings();
            worldSettings.gravity = new PVector3(0, -9.81f, 0);
            worldSettings.persistentContactDistanceThreshold = 0.03f;
            worldSettings.defaultFrictionCoefficient = 0.3f;
            worldSettings.defaultBounciness = 0.5f;
            worldSettings.restitutionVelocityThreshold = 0.5f;
            worldSettings.isSleepingEnabled = true;
            worldSettings.defaultVelocitySolverNbIterations = 6;
            worldSettings.defaultPositionSolverNbIterations = 3;
            worldSettings.defaultTimeBeforeSleep = 1.0f;
            worldSettings.defaultSleepLinearVelocity = 0.02f;
            worldSettings.defaultSleepAngularVelocity = 3.0f * (PMath.PI_RP3D / 180.0f);
            worldSettings.cosAngleSimilarContactManifold = 0.95f;
            
            
            mPhysicsCommon = new PhysicsCommon();
            mWorld = mPhysicsCommon.createPhysicsWorld(worldSettings);
            
            
            Transform transform1 = new PTransform(new Vector3(1, 2, 3), Quaternion.identity());
            mRigidBody1 = mWorld.createRigidBody(transform1);

            //  Box

             Transform transform2 = new PTransform(new Vector3(0, 0, 0), Quaternion.identity());
            mRigidBody2Box = mWorld.createRigidBody(transform2);
            BoxShape boxShape = mPhysicsCommon.createBoxShape(new Vector3(2, 2, 2));
            mBoxCollider = mRigidBody2Box.addCollider(boxShape, Transform.Identity());

            //  Sphere

            mRigidBody2Sphere = mWorld.createRigidBody(transform2);
            SphereShape sphereShape = mPhysicsCommon.createSphereShape(4);
            mSphereCollider = mRigidBody2Sphere.addCollider(sphereShape, Transform.Identity());

            //  Convex Meshes (in the shape of a box)

            // mConvexMeshCubeVertices[0] = -3; mConvexMeshCubeVertices[1] = -3; mConvexMeshCubeVertices[2] = 3;
            // mConvexMeshCubeVertices[3] = 3; mConvexMeshCubeVertices[4] = -3; mConvexMeshCubeVertices[5] = 3;
            // mConvexMeshCubeVertices[6] = 3; mConvexMeshCubeVertices[7] = -3; mConvexMeshCubeVertices[8] = -3;
            // mConvexMeshCubeVertices[9] = -3; mConvexMeshCubeVertices[10] = -3; mConvexMeshCubeVertices[11] = -3;
            // mConvexMeshCubeVertices[12] = -3; mConvexMeshCubeVertices[13] = 3; mConvexMeshCubeVertices[14] = 3;
            // mConvexMeshCubeVertices[15] = 3; mConvexMeshCubeVertices[16] = 3; mConvexMeshCubeVertices[17] = 3;
            // mConvexMeshCubeVertices[18] = 3; mConvexMeshCubeVertices[19] = 3; mConvexMeshCubeVertices[20] = -3;
            // mConvexMeshCubeVertices[21] = -3; mConvexMeshCubeVertices[22] = 3; mConvexMeshCubeVertices[23] = -3;
            //
            // mConvexMeshCubeIndices[0] = 0; mConvexMeshCubeIndices[1] = 3; mConvexMeshCubeIndices[2] = 2; mConvexMeshCubeIndices[3] = 1;
            // mConvexMeshCubeIndices[4] = 4; mConvexMeshCubeIndices[5] = 5; mConvexMeshCubeIndices[6] = 6; mConvexMeshCubeIndices[7] = 7;
            // mConvexMeshCubeIndices[8] = 0; mConvexMeshCubeIndices[9] = 1; mConvexMeshCubeIndices[10] = 5; mConvexMeshCubeIndices[11] = 4;
            // mConvexMeshCubeIndices[12] = 1; mConvexMeshCubeIndices[13] = 2; mConvexMeshCubeIndices[14] = 6; mConvexMeshCubeIndices[15] = 5;
            // mConvexMeshCubeIndices[16] = 2; mConvexMeshCubeIndices[17] = 3; mConvexMeshCubeIndices[18] = 7; mConvexMeshCubeIndices[19] = 6;
            // mConvexMeshCubeIndices[20] = 0; mConvexMeshCubeIndices[21] = 4; mConvexMeshCubeIndices[22] = 7; mConvexMeshCubeIndices[23] = 3;

            // mConvexMeshPolygonFaces = new rp3d::PolygonVertexArray::PolygonFace[6];
            // rp3d::PolygonVertexArray::PolygonFace* face = mConvexMeshPolygonFaces;
            // for (int f = 0; f < 6; f++) {
            //     face.indexBase = f * 4;
            //     face.nbVertices = 4;
            //     face++;
            // }
            // mConvexMeshPolygonVertexArray = new rp3d::PolygonVertexArray(8, &(mConvexMeshCubeVertices[0]), 3 * sizeof(float),
            //         &(mConvexMeshCubeIndices[0]), sizeof(int), 6, mConvexMeshPolygonFaces,
            //         rp3d::PolygonVertexArray::VertexDataType::VERTEX_FLOAT_TYPE,
            //         rp3d::PolygonVertexArray::IndexDataType::INDEX_INTEGER_TYPE);
            // mConvexMeshPolyhedronMesh = mPhysicsCommon.createPolyhedronMesh(mConvexMeshPolygonVertexArray);
            // ConvexMeshShape* convexMeshShape = mPhysicsCommon.createConvexMeshShape(mConvexMeshPolyhedronMesh);
            // Transform transform3(Vector3(10, 0, 0), Quaternion::identity());
            // mRigidBody2Convex = mWorld.createRigidBody(transform3);
            // mConvexMeshCollider = mRigidBody2Convex.addCollider(convexMeshShape, Transform::identity());

            //  Rigidbody 3

            float angleRad = 20 * PMath.PI_RP3D / 180.0f;
            Transform transform4 = new PTransform(new Vector3(1, 2, 3), Quaternion.FromEulerAngles(angleRad, angleRad, angleRad));
            mRigidBody3 = mWorld.createRigidBody(transform4);
            BoxShape boxShape3 = mPhysicsCommon.createBoxShape(new Vector3(2, 2, 2));
            mRigidBody3.addCollider(boxShape3, Transform.Identity());
        }
        
        
        void testGettersSetters() {

           mRigidBody1.setMass(34);
           rp3d_test(mRigidBody1.getMass() == 34);

           mRigidBody1.setLinearDamping(0.6f);
           rp3d_test(approxEqual(mRigidBody1.getLinearDamping(), 0.6f));

           mRigidBody1.setAngularDamping(0.6f);
           rp3d_test(approxEqual(mRigidBody1.getAngularDamping(), 0.6f));

           mRigidBody1.setLinearLockAxisFactor(new PVector3(0.2f, 0.3f, 0.4f));
           rp3d_test(PMath.approxEqual(mRigidBody1.getLinearLockAxisFactor(), new PVector3(0.2f, 0.3f, 0.4f)));

           mRigidBody1.setAngularLockAxisFactor(new PVector3(0.2f, 0.3f, 0.4f));
           rp3d_test(PMath.approxEqual(mRigidBody1.getAngularLockAxisFactor(), new PVector3(0.2f, 0.3f, 0.4f)));

           mRigidBody1.setLinearVelocity(new PVector3(2, 3, 4));
           rp3d_test(PMath.approxEqual(mRigidBody1.getLinearVelocity(), new PVector3(2, 3, 4)));

           mRigidBody1.setAngularVelocity(new PVector3(2, 3, 4));
           rp3d_test(PMath.approxEqual(mRigidBody1.getAngularVelocity(), new PVector3(2, 3, 4)));

           mRigidBody1.setTransform(new PTransform(new PVector3(5, 4, 3), Quaternion.FromEulerAngles(1.7f, 1.8f, 1.9f)));
           rp3d_test(PMath.approxEqual(mRigidBody1.getTransform().GetPosition(), new PVector3(5, 4, 3)));
           rp3d_test(PMath.approxEqual(mRigidBody1.getTransform().GetOrientation().x, Quaternion.FromEulerAngles(1.7f, 1.8f, 1.9f).x));
           rp3d_test(PMath.approxEqual(mRigidBody1.getTransform().GetOrientation().y, Quaternion.FromEulerAngles(1.7f, 1.8f, 1.9f).y));
           rp3d_test(PMath.approxEqual(mRigidBody1.getTransform().GetOrientation().z, Quaternion.FromEulerAngles(1.7f, 1.8f, 1.9f).z));
           rp3d_test(PMath.approxEqual(mRigidBody1.getTransform().GetOrientation().w, Quaternion.FromEulerAngles(1.7f, 1.8f, 1.9f).w));

           mRigidBody1.setLocalCenterOfMass(new PVector3(10, 20, 30));
           rp3d_test(PMath.approxEqual(mRigidBody1.getLocalCenterOfMass(), new PVector3(10, 20, 30)));

           mRigidBody1.setType(BodyType.KINEMATIC);
           rp3d_test(mRigidBody1.getType() == BodyType.KINEMATIC);

           mRigidBody1.setLocalInertiaTensor(new PVector3(2, 4, 6));
           rp3d_test(PMath.approxEqual(mRigidBody1.getLocalInertiaTensor(), new PVector3(2, 4, 6)));
        }

        void testMassPropertiesMethods() {

            // Box collider
            mBoxCollider.getMaterial().setMassDensity(3);
            mRigidBody2Box.updateMassFromColliders();
            rp3d_test(PMath.approxEqual(mRigidBody2Box.getMass(), 64 * 3));

            mRigidBody2Box.setLocalCenterOfMass(new PVector3(1, 2, 3));
            mRigidBody2Box.setMass(1);
            mRigidBody2Box.updateMassPropertiesFromColliders();
            
            rp3d_test(PMath.approxEqual(mRigidBody2Box.getMass(), 64 * 3));
            rp3d_test(PMath.approxEqual(mRigidBody2Box.getLocalCenterOfMass(), Vector3.Zero()));

            mRigidBody2Box.setLocalCenterOfMass(new PVector3(1, 2, 3));
            mRigidBody2Box.updateLocalCenterOfMassFromColliders();
            rp3d_test(PMath.approxEqual(mRigidBody2Box.getLocalCenterOfMass(), Vector3.Zero()));

            mRigidBody2Box.setLocalInertiaTensor(new PVector3(1, 2, 3));
            mRigidBody2Box.updateLocalInertiaTensorFromColliders();
            float tensorBox = 1.0f / 6.0f * 64 * 3 * 4 * 4;
            rp3d_test(PMath.approxEqual(mRigidBody2Box.getLocalInertiaTensor(), new PVector3(tensorBox, tensorBox, tensorBox)));

            // Sphere collider
            mSphereCollider.getMaterial().setMassDensity(3);
            mRigidBody2Sphere.updateMassFromColliders();
            float sphereMass = 4.0f / 3.0f * PMath.PI_RP3D * 64 * 3;
            rp3d_test(PMath.approxEqual(mRigidBody2Sphere.getMass(), sphereMass));

            mRigidBody2Sphere.setLocalCenterOfMass(new PVector3(1, 2, 3));
            mRigidBody2Sphere.setMass(1);
            mRigidBody2Sphere.updateMassPropertiesFromColliders();
            rp3d_test(PMath.approxEqual(mRigidBody2Sphere.getMass(), sphereMass));
            rp3d_test(PMath.approxEqual(mRigidBody2Sphere.getLocalCenterOfMass(), Vector3.Zero()));

            mRigidBody2Sphere.setLocalCenterOfMass(new PVector3(1, 2, 3));
            mRigidBody2Sphere.updateLocalCenterOfMassFromColliders();
            rp3d_test(PMath.approxEqual(mRigidBody2Sphere.getLocalCenterOfMass(), Vector3.Zero()));

            mRigidBody2Sphere.setLocalInertiaTensor(new PVector3(1, 2, 3));
            mRigidBody2Sphere.updateLocalInertiaTensorFromColliders();
            float tensorSphere = 2.0f / 5.0f * sphereMass * 4 * 4;
            rp3d_test(PMath.approxEqual(mRigidBody2Sphere.getLocalInertiaTensor(), new PVector3(tensorSphere, tensorSphere, tensorSphere)));

            // Convex mesh collider
            //mConvexMeshCollider.getMaterial().setMassDensity(3);
            // mRigidBody2Convex.updateMassFromColliders();
            // rp3d_test(PVector3.approxEqual(mRigidBody2Convex.getMass(), 648));
            //
            // mRigidBody2Convex.setLocalCenterOfMass(new PVector3(1, 2, 3));
            // mRigidBody2Convex.setMass(1);
            // //mConvexMeshCollider.getMaterial().setMassDensity(2);
            // mRigidBody2Convex.updateMassPropertiesFromColliders();
            // rp3d_test(PVector3.approxEqual(mRigidBody2Convex.getMass(), 432));
            // rp3d_test(PVector3.approxEqual(mRigidBody2Convex.getLocalCenterOfMass(), Vector3.Zero()));
            //
            // mRigidBody2Convex.setLocalCenterOfMass(new PVector3(1, 2, 3));
            // mRigidBody2Convex.updateLocalCenterOfMassFromColliders();
            // rp3d_test(PVector3.approxEqual(mRigidBody2Convex.getLocalCenterOfMass(), Vector3.Zero()));
            //
            // mRigidBody2Convex.setLocalInertiaTensor(new PVector3(1, 2, 3));
            // mRigidBody2Convex.updateLocalInertiaTensorFromColliders();
            // tensorBox = 1.0f / 6.0f * 432 * 6 * 6;
            // rp3d_test(PVector3.approxEqual(mRigidBody2Convex.getLocalInertiaTensor(), new PVector3(tensorBox, tensorBox, tensorBox)));
        }

        void testApplyForcesAndTorques() {

            PTransform worldTransform = mRigidBody3.getTransform();
            PQuaternion orientation = worldTransform.GetOrientation();

            mRigidBody3.applyLocalForceAtCenterOfMass(new PVector3(4, 5, 6));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), orientation * new PVector3(4, 5, 6)));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), orientation * Vector3.Zero()));

            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();

            mRigidBody3.applyWorldForceAtCenterOfMass(new PVector3(4, 5, 6));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), new PVector3(4, 5, 6)));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), Vector3.Zero()));
            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();

            mRigidBody3.applyLocalForceAtLocalPosition(new PVector3(0, 0, 3), new PVector3(2, 0, 0));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), orientation * new PVector3(0, 0, 3)));

            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), orientation * new PVector3(0, -3 * 2, 0), 0.0001f));
            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();

            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), Vector3.Zero()));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), Vector3.Zero()));

            mRigidBody3.applyLocalForceAtWorldPosition(new PVector3(0, 0, 3), worldTransform * new PVector3(2, 0, 0));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), orientation * new PVector3(0, 0, 3)));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), orientation * new PVector3(0, -3 * 2, 0), 0.0001f));
            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();

            mRigidBody3.applyWorldForceAtLocalPosition(orientation * new PVector3(0, 0, 3), new PVector3(2, 0, 0));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), orientation * new PVector3(0, 0, 3)));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), orientation * new PVector3(0, -3 * 2, 0), 0.0001f));
            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();

            mRigidBody3.applyWorldForceAtWorldPosition(orientation * new PVector3(0, 0, 3), worldTransform * new PVector3(2, 0, 0));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), orientation * new PVector3(0, 0, 3)));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), orientation * new PVector3(0, -3 * 2, 0), 0.0001f));
            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();

            mRigidBody3.applyWorldTorque(new PVector3(0, 4, 0));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), Vector3.Zero()));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), new PVector3(0, 4, 0), 0.0001f));
            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();

            mRigidBody3.applyLocalTorque(new PVector3(0, 4, 0));
            rp3d_test(PMath.approxEqual(mRigidBody3.getForce(), Vector3.Zero()));
            rp3d_test(PMath.approxEqual(mRigidBody3.getTorque(), orientation * new PVector3(0, 4, 0), 0.0001f));
            mRigidBody3.resetForce();
            mRigidBody3.resetTorque();
        }
        
        public void rp3d_test(bool value)
        {
            Debug.Log(value);
        }

        public bool approxEqual(float a, float b, float epsilon = float.Epsilon)
        {
            return PMath.Abs(a - b) < epsilon;
        }
        
    }
}