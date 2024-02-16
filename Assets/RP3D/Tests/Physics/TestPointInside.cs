using RP3D;
using UnityEngine;
using Collider = RP3D.Collider;
using Vector3 = RP3D.PVector3;

public class TestPointInside : MonoBehaviour
{
    PhysicsCommon mPhysicsCommon;

    PhysicsWorld mWorld;
    
    CollisionBody mBoxBody;
    CollisionBody mSphereBody;
    CollisionBody mCapsuleBody;
    CollisionBody mConeBody;
    CollisionBody mConvexMeshBody;
    CollisionBody mConvexMeshBodyEdgesInfo;
    CollisionBody mCylinderBody;
    CollisionBody mCompoundBody;
    
    
    float[] mConvexMeshCubeVertices;
    int[] mConvexMeshCubeIndices;

    
    // PolygonVertexArray mConvexMeshPolygonVertexArray;
    // PolyhedronMesh mConvexMeshPolyhedronMesh;
    // PolygonFace mConvexMeshPolygonFaces;
    
    // Collision shapes
    BoxShape mBoxShape;
    SphereShape mSphereShape;
    CapsuleShape mCapsuleShape;
    // ConvexMeshShape* mConvexMeshShape;

    // Transform
    PTransform mBodyTransform;
    PTransform mShapeTransform;
    PTransform mLocalShapeToWorld;
    PTransform mLocalShape2ToWorld;

    // Colliders
    Collider mBoxCollider;
    Collider mSphereCollider;
    Collider mCapsuleCollider;
    Collider mConvexMeshCollider;

    
    // Start is called before the first frame update
    void Start()
    {
        WorldSettings worldSettings = new WorldSettings();
        mPhysicsCommon = new PhysicsCommon();
        mWorld = mPhysicsCommon.createPhysicsWorld(worldSettings);
        PVector3 position = new PVector3(-3, 2, 7);
        PQuaternion orientation = PQuaternion.FromEulerAngles( PMath.PI_RP3D / 5, PMath.PI_RP3D / 6, PMath.PI_RP3D / 7);
        mBodyTransform = new PTransform(position, orientation);
        
        // Create the bodies
        mBoxBody = mWorld.createCollisionBody(mBodyTransform);
        mSphereBody = mWorld.createCollisionBody(mBodyTransform);
        mCapsuleBody = mWorld.createCollisionBody(mBodyTransform);
        mConeBody = mWorld.createCollisionBody(mBodyTransform);
        mConvexMeshBody = mWorld.createCollisionBody(mBodyTransform);
        mConvexMeshBodyEdgesInfo = mWorld.createCollisionBody(mBodyTransform);
        mCylinderBody = mWorld.createCollisionBody(mBodyTransform);
        mConvexMeshBody = mWorld.createCollisionBody(mBodyTransform);
        mCompoundBody = mWorld.createCollisionBody(mBodyTransform);
        
        // Collision shape transform
        PVector3 shapePosition = new PVector3(1, -4, -3);
        PQuaternion shapeOrientation = PQuaternion.FromEulerAngles(3 * PMath.PI_RP3D / 6 , -PMath.PI_RP3D / 8, PMath.PI_RP3D / 3);
        mShapeTransform = new PTransform(shapePosition, shapeOrientation);
        
        // Compute the the transform from a local shape point to world-space
        mLocalShapeToWorld = mBodyTransform * mShapeTransform;
        
        
        // Create collision shapes
        mBoxShape = mPhysicsCommon.createBoxShape(new PVector3(2, 3, 4));
        mBoxCollider = mBoxBody.addCollider(mBoxShape, mShapeTransform);

        mSphereShape = mPhysicsCommon.createSphereShape(3);
        mSphereCollider = mSphereBody.addCollider(mSphereShape, mShapeTransform);

        mCapsuleShape = mPhysicsCommon.createCapsuleShape(3, 10);
        mCapsuleCollider = mCapsuleBody.addCollider(mCapsuleShape, mShapeTransform);
        
        
        mConvexMeshCubeVertices = new float[24];
        mConvexMeshCubeVertices[0] = -2; mConvexMeshCubeVertices[1] = -3; mConvexMeshCubeVertices[2] = 4;
        mConvexMeshCubeVertices[3] = 2; mConvexMeshCubeVertices[4] = -3; mConvexMeshCubeVertices[5] = 4;
        mConvexMeshCubeVertices[6] = 2; mConvexMeshCubeVertices[7] = -3; mConvexMeshCubeVertices[8] = -4;
        mConvexMeshCubeVertices[9] = -2; mConvexMeshCubeVertices[10] = -3; mConvexMeshCubeVertices[11] = -4;
        mConvexMeshCubeVertices[12] = -2; mConvexMeshCubeVertices[13] = 3; mConvexMeshCubeVertices[14] = 4;
        mConvexMeshCubeVertices[15] = 2; mConvexMeshCubeVertices[16] = 3; mConvexMeshCubeVertices[17] = 4;
        mConvexMeshCubeVertices[18] = 2; mConvexMeshCubeVertices[19] = 3; mConvexMeshCubeVertices[20] = -4;
        mConvexMeshCubeVertices[21] = -2; mConvexMeshCubeVertices[22] = 3; mConvexMeshCubeVertices[23] = -4;

        mConvexMeshCubeIndices = new int[24];
        mConvexMeshCubeIndices[0] = 0; mConvexMeshCubeIndices[1] = 3; mConvexMeshCubeIndices[2] = 2; mConvexMeshCubeIndices[3] = 1;
        mConvexMeshCubeIndices[4] = 4; mConvexMeshCubeIndices[5] = 5; mConvexMeshCubeIndices[6] = 6; mConvexMeshCubeIndices[7] = 7;
        mConvexMeshCubeIndices[8] = 0; mConvexMeshCubeIndices[9] = 1; mConvexMeshCubeIndices[10] = 5; mConvexMeshCubeIndices[11] = 4;
        mConvexMeshCubeIndices[12] = 1; mConvexMeshCubeIndices[13] = 2; mConvexMeshCubeIndices[14] = 6; mConvexMeshCubeIndices[15] = 5;
        mConvexMeshCubeIndices[16] = 2; mConvexMeshCubeIndices[17] = 3; mConvexMeshCubeIndices[18] = 7; mConvexMeshCubeIndices[19] = 6;
        mConvexMeshCubeIndices[20] = 0; mConvexMeshCubeIndices[21] = 4; mConvexMeshCubeIndices[22] = 7; mConvexMeshCubeIndices[23] = 3;

        
        // mConvexMeshPolygonFaces = new PolygonFace[6];
        // PolygonFace face = mConvexMeshPolygonFaces;
        // for (int f = 0; f < 6; f++) {
        //     face.indexBase = f * 4;
        //     face.nbVertices = 4;
        //     face++;
        // }
        // mConvexMeshPolygonVertexArray = new PolygonVertexArray(8, &(mConvexMeshCubeVertices[0]), 3 * sizeof(float),
        //     &(mConvexMeshCubeIndices[0]), sizeof(int), 6, mConvexMeshPolygonFaces,
        //     PolygonVertexArray::VertexDataType::VERTEX_FLOAT_TYPE,
        //     PolygonVertexArray::IndexDataType::INDEX_INTEGER_TYPE);
        // mConvexMeshPolyhedronMesh = mPhysicsCommon.createPolyhedronMesh(mConvexMeshPolygonVertexArray);
        // mConvexMeshShape = mPhysicsCommon.createConvexMeshShape(mConvexMeshPolyhedronMesh);
        // PTransform convexMeshTransform = new PTransform (new PVector3(10, 0, 0), PQuaternion.identity());
        // mConvexMeshCollider = mConvexMeshBody.addCollider(mConvexMeshShape, mShapeTransform);

        // Compound shape is a capsule and a sphere
        PVector3 positionShape2 = new PVector3(4, 2, -3);
        PQuaternion orientationShape2 = PQuaternion.FromEulerAngles(-3 * PMath.PI_RP3D / 8, 1.5f * PMath.PI_RP3D/ 3, PMath.PI_RP3D / 13);
        PTransform shapeTransform2 = new PTransform(positionShape2, orientationShape2);
        mLocalShape2ToWorld = mBodyTransform * shapeTransform2;
        mCompoundBody.addCollider(mCapsuleShape, mShapeTransform);
        mCompoundBody.addCollider(mSphereShape, shapeTransform2);

        testBox();
        testSphere();
        testCapsule();
        testCompound();
    }
    
    
            /// Test the testPointInside() and
        /// CollisionBody::testPointInside() methods
        void testBox() {

            // Tests with CollisionBody
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 0)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, 0, 0)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 0, 0)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, -2.9f, 0)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 2.9f, 0)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -3.9f)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 3.9f)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, -2.9f, -3.9f)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 2.9f, 3.9f)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-1, -2, -1.5f)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-1, 2, -2.5f)));
            rp3d_test(mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 3.5f)));

            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, 0, 0)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 0, 0)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, -3.1f, 0)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 3.1f, 0)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -4.1f)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 4.1f)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, -3.1f, -4.1f)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 3.1f, 4.1f)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-10, -2, -1.5f)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(-1, 4, -2.5f)));
            rp3d_test(!mBoxBody.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 4.5f)));

            // Tests with Collider
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 0)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, 0, 0)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 0, 0)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, -2.9f, 0)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 2.9f, 0)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -3.9f)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 3.9f)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, -2.9f, -3.9f)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 2.9f, 3.9f)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1, -2, -1.5f)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1, 2, -2.5f)));
            rp3d_test(mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 3.5f)));

            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, 0, 0)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 0, 0)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, -3.1f, 0)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 3.1f, 0)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -4.1f)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 4.1f)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, -3.1f, -4.1f)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 3.1f, 4.1f)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-10, -2, -1.5f)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1, 4, -2.5f)));
            rp3d_test(!mBoxCollider.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 4.5f)));
        }
            
            
            
                    /// Test the Collider::testPointInside() and
        /// CollisionBody::testPointInside() methods
        void testSphere() {

            // Tests with CollisionBody
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 0)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(2.9f, 0, 0)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(-2.9f, 0, 0)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 2.9f, 0)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -2.9f, 0)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 2.9f)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 2.9f)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(-1, -2, -1.5f)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(-1, 2, -1.5f)));
            rp3d_test(mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(1, -2, 1.5f)));

            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, 0, 0)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, 0, 0)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 3.1f, 0)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -3.1f, 0)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 3.1f)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, -3.1f)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(-2, -2, -2)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(-2, 2, -1.5f)));
            rp3d_test(!mSphereBody.testPointInside(mLocalShapeToWorld * new Vector3(1.5f, -2, 2.5f)));

            // Tests with Collider
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 0)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(2.9f, 0, 0)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(-2.9f, 0, 0)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 2.9f, 0)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -2.9f, 0)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 2.9f)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 2.9f)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(-1, -2, -1.5f)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(-1, 2, -1.5f)));
            rp3d_test(mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(1, -2, 1.5f)));

            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, 0, 0)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, 0, 0)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 3.1f, 0)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -3.1f, 0)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 3.1f)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, -3.1f)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(-2, -2, -2)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(-2, 2, -1.5f)));
            rp3d_test(!mSphereCollider.testPointInside(mLocalShapeToWorld * new Vector3(1.5f, -2, 2.5f)));
        }

        /// Test the Collider::testPointInside() and
        /// CollisionBody::testPointInside() methods
        void testCapsule() {

            // Tests with CollisionBody
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -6.9f, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 6.9f, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 1.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, -1.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(1.9f, 0, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-1.9f, 0, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 0, 0.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 0, -0.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, 1.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, -1.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(1.9f, 5, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-1.9f, 5, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 5, 0.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 5, -0.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, 1.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, -1.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(1.9f, -5, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-1.9f, -5, 0)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, -5, 0.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, -5, -0.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-1.7f, -4, -0.9f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-1, 2, 0.4f)));
            rp3d_test(mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(1.3f, 1, 1.5f)));

            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -13.1f, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 13.1f, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 3.1f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, -3.1f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, 0, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, 0, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, 3.1f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, -3.1f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, 5, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, 5, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, 5, 2.6f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, 5, -2.7f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, 3.1f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, -3.1f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, -5, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, -5, 0)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, -5, 2.6f)));
            rp3d_test(!mCapsuleBody.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, -5, -2.7f)));

            // Tests with Collider
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -6.9f, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 6.9f, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 1.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, -1.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(1.9f, 0, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-1.9f, 0, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 0, 0.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 0, -0.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, 1.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, -1.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(1.9f, 5, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-1.9f, 5, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 5, 0.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, 5, -0.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, 1.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, -1.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(1.9f, -5, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-1.9f, -5, 0)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, -5, 0.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0.9f, -5, -0.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-1.7f, -4, -0.9f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-1, 2, 0.4f)));
            rp3d_test(mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(1.3f, 1, 1.5f)));

            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -13.1f, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 13.1f, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, 3.1f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 0, -3.1f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, 0, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, 0, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, 3.1f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, 5, -3.1f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, 5, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, 5, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, 5, 2.6f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, 5, -2.7f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, 3.1f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(0, -5, -3.1f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(3.1f, -5, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(-3.1f, -5, 0)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, -5, 2.6f)));
            rp3d_test(!mCapsuleCollider.testPointInside(mLocalShapeToWorld * new Vector3(2.5f, -5, -2.7f)));
        }
        
        
        
                /// Test the Collider::testPointInside() and
        /// CollisionBody::testPointInside() methods
        void testConvexMesh() {

            // Tests with CollisionBody
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 0)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, 0, 0)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 0, 0)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, -2.9f, 0)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 2.9f, 0)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -3.9f)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 3.9f)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, -2.9f, -3.9f)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 2.9f, 3.9f)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-1, -2, -1.5f)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-1, 2, -2.5f)));
            rp3d_test(mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 3.5f)));

            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, 0, 0)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 0, 0)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, -3.1f, 0)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 3.1f, 0)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -4.1f)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 4.1f)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, -3.1f, -4.1f)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 3.1f, 4.1f)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-10, -2, -1.5f)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(-1, 4, -2.5f)));
            rp3d_test(!mConvexMeshBody.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 4.5f)));

            // Tests with Collider
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 0)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, 0, 0)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 0, 0)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, -2.9f, 0)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 2.9f, 0)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -3.9f)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 3.9f)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1.9f, -2.9f, -3.9f)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(1.9f, 2.9f, 3.9f)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1, -2, -1.5f)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1, 2, -2.5f)));
            rp3d_test(mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 3.5f)));

            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, 0, 0)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 0, 0)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, -3.1f, 0)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 3.1f, 0)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -4.1f)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 4.1f)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-2.1f, -3.1f, -4.1f)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(2.1f, 3.1f, 4.1f)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-10, -2, -1.5f)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(-1, 4, -2.5f)));
            rp3d_test(!mConvexMeshCollider.testPointInside(mLocalShapeToWorld * new PVector3(1, -2, 4.5f)));
        }
        
        
                /// Test the CollisionBody::testPointInside() method
        void testCompound() {

            // Points on the capsule
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 3.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, -3.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(2.9f, 0, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-2.9f, 0, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, 2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 0, -2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(1.7f, 0, 1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(1.7f, 0, -1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.7f, 0, -1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.7f, 0, 1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(2.9f, 3.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-2.9f, 3.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 3.9f, 2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, 3.9f, -2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(1.7f, 3.9f, 1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(1.7f, 3.9f, -1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.7f, 3.9f, -1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.7f, 3.9f, 1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(2.9f, -3.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-2.9f, -3.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, -3.9f, 2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(0, -3.9f, -2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(1.7f, -3.9f, 1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(1.7f, -3.9f, -1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.7f, -3.9f, -1.7f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShapeToWorld * new PVector3(-1.7f, -3.9f, 1.7f)));

            // Points on the sphere
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(0, 0, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(2.9f, 0, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(-2.9f, 0, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(0, 2.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(0, -2.9f, 0)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(0, 0, 2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(0, 0, 2.9f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(-1, -2, -1.5f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(-1, 2, -1.5f)));
            rp3d_test(mCompoundBody.testPointInside(mLocalShape2ToWorld * new PVector3(1, -2, 1.5f)));
        }



            public void rp3d_test(bool value)
            {
                Debug.Log(value);
            }
}

