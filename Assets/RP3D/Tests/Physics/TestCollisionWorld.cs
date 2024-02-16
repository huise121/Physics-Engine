using System.Diagnostics;
using NUnit.Framework;
using RP3D;
using Scenes.Test;
using UnityEngine;
using Collider = RP3D.Collider;
using Debug = UnityEngine.Debug;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;


public partial class TestCollisionWorld 
{
    private PhysicsCommon mPhysicsCommon;

    // Physics world
    private PhysicsWorld mWorld;

    // Bodies
    private CollisionBody mBoxBody1;
    private CollisionBody mBoxBody2;
    protected CollisionBody mSphereBody1;
    private CollisionBody mSphereBody2;
    private CollisionBody mCapsuleBody1;
    private CollisionBody mCapsuleBody2;
    private CollisionBody mConvexMeshBody1;
    private CollisionBody mConvexMeshBody2;
    private CollisionBody mConcaveMeshBody;

    // Collision shapes
    private BoxShape mBoxShape1;
    private BoxShape mBoxShape2;
    private SphereShape mSphereShape1;
    private SphereShape mSphereShape2;
    private CapsuleShape mCapsuleShape1;

    private CapsuleShape mCapsuleShape2;
    // ConvexMeshShape mConvexMeshShape1;
    // ConvexMeshShape mConvexMeshShape2;
    // ConcaveMeshShape mConcaveMeshShape;

    // Colliders
    private Collider mBoxCollider1;
    private Collider mBoxCollider2;
    private Collider mSphereCollider1;
    private Collider mSphereCollider2;
    private Collider mCapsuleCollider1;
    private Collider mCapsuleCollider2;
    private Collider mConvexMeshCollider1;
    private Collider mConvexMeshCollider2;
    private Collider mConcaveMeshCollider;

    //PolygonVertexArray mConvexMesh1PolygonVertexArray;
    //PolygonVertexArray mConvexMesh2PolygonVertexArray;
    //PolyhedronMesh mConvexMesh1PolyhedronMesh;
    //PolyhedronMesh mConvexMesh2PolyhedronMesh;
    //PolygonFace mConvexMeshPolygonFaces;

    //TriangleVertexArray mConcaveMeshTriangleVertexArray;
    private float[] mConvexMesh1CubeVertices;
    private float[] mConvexMesh2CubeVertices;
    private int[] mConvexMeshCubeIndices;

    private float[] mConcaveMeshPlaneVertices;
    private int[] mConcaveMeshPlaneIndices;

    //TriangleMesh mConcaveTriangleMesh;

    // Collision callback
    //WorldCollisionCallback mCollisionCallback;

    // Overlap callback
    //WorldOverlapCallback mOverlapCallback;


    private WorldCollisionCallback mCollisionCallback;
    private WorldOverlapCallback mOverlapCallback;

    // Start is called before the first frame update
    private void Init()
    {
        mCollisionCallback = new WorldCollisionCallback();
        mOverlapCallback = new WorldOverlapCallback();


        var worldSettings = new WorldSettings();
        mPhysicsCommon = new PhysicsCommon();
        mWorld = mPhysicsCommon.createPhysicsWorld(worldSettings);
        var mConvexMesh1CubeVertices = new float[8 * 3];
        var mConvexMesh2CubeVertices = new float[8 * 3];
        var mConvexMeshCubeIndices = new int[24];
        var mConcaveMeshPlaneVertices = new float[36 * 3];
        var mConcaveMeshPlaneIndices = new int[25 * 2 * 3];


        // ---------- Boxes ---------- //
        var boxTransform1 = new Transform(new Vector3(-20, 20, 0), Quaternion.identity());
        mBoxBody1 = mWorld.createCollisionBody(boxTransform1);
        mBoxShape1 = mPhysicsCommon.createBoxShape(new Vector3(3, 3, 3));
        mBoxCollider1 = mBoxBody1.addCollider(mBoxShape1, Transform.Identity());

        var boxTransform2 = new Transform(new Vector3(-10, 20, 0), Quaternion.identity());
        mBoxBody2 = mWorld.createCollisionBody(boxTransform2);
        mBoxShape2 = mPhysicsCommon.createBoxShape(new Vector3(4, 2, 8));
        mBoxCollider2 = mBoxBody2.addCollider(mBoxShape2, Transform.Identity());

        // ---------- Spheres ---------- //
        mSphereShape1 = mPhysicsCommon.createSphereShape(3.0f);
        var sphereTransform1 = new Transform(new Vector3(10, 20, 0), Quaternion.identity());
        mSphereBody1 = mWorld.createCollisionBody(sphereTransform1);
        mSphereCollider1 = mSphereBody1.addCollider(mSphereShape1, Transform.Identity());

        mSphereShape2 = mPhysicsCommon.createSphereShape(5.0f);
        var sphereTransform2 = new Transform(new Vector3(20, 20, 0), Quaternion.identity());
        mSphereBody2 = mWorld.createCollisionBody(sphereTransform2);
        mSphereCollider2 = mSphereBody2.addCollider(mSphereShape2, Transform.Identity());

        // ---------- Capsules ---------- //
        mCapsuleShape1 = mPhysicsCommon.createCapsuleShape(2, 6);
        var capsuleTransform1 = new Transform(new Vector3(-10, 0, 0), Quaternion.identity());
        mCapsuleBody1 = mWorld.createCollisionBody(capsuleTransform1);
        mCapsuleCollider1 = mCapsuleBody1.addCollider(mCapsuleShape1, Transform.Identity());

        mCapsuleShape2 = mPhysicsCommon.createCapsuleShape(3, 4);
        var capsuleTransform2 = new Transform(new Vector3(-20, 0, 0), Quaternion.identity());
        mCapsuleBody2 = mWorld.createCollisionBody(capsuleTransform2);
        mCapsuleCollider2 = mCapsuleBody2.addCollider(mCapsuleShape2, Transform.Identity());

        // ---------- Convex Meshes ---------- //
        mConvexMesh1CubeVertices[0] = -3;
        mConvexMesh1CubeVertices[1] = -3;
        mConvexMesh1CubeVertices[2] = 3;
        mConvexMesh1CubeVertices[3] = 3;
        mConvexMesh1CubeVertices[4] = -3;
        mConvexMesh1CubeVertices[5] = 3;
        mConvexMesh1CubeVertices[6] = 3;
        mConvexMesh1CubeVertices[7] = -3;
        mConvexMesh1CubeVertices[8] = -3;
        mConvexMesh1CubeVertices[9] = -3;
        mConvexMesh1CubeVertices[10] = -3;
        mConvexMesh1CubeVertices[11] = -3;
        mConvexMesh1CubeVertices[12] = -3;
        mConvexMesh1CubeVertices[13] = 3;
        mConvexMesh1CubeVertices[14] = 3;
        mConvexMesh1CubeVertices[15] = 3;
        mConvexMesh1CubeVertices[16] = 3;
        mConvexMesh1CubeVertices[17] = 3;
        mConvexMesh1CubeVertices[18] = 3;
        mConvexMesh1CubeVertices[19] = 3;
        mConvexMesh1CubeVertices[20] = -3;
        mConvexMesh1CubeVertices[21] = -3;
        mConvexMesh1CubeVertices[22] = 3;
        mConvexMesh1CubeVertices[23] = -3;

        mConvexMeshCubeIndices[0] = 0;
        mConvexMeshCubeIndices[1] = 3;
        mConvexMeshCubeIndices[2] = 2;
        mConvexMeshCubeIndices[3] = 1;
        mConvexMeshCubeIndices[4] = 4;
        mConvexMeshCubeIndices[5] = 5;
        mConvexMeshCubeIndices[6] = 6;
        mConvexMeshCubeIndices[7] = 7;
        mConvexMeshCubeIndices[8] = 0;
        mConvexMeshCubeIndices[9] = 1;
        mConvexMeshCubeIndices[10] = 5;
        mConvexMeshCubeIndices[11] = 4;
        mConvexMeshCubeIndices[12] = 1;
        mConvexMeshCubeIndices[13] = 2;
        mConvexMeshCubeIndices[14] = 6;
        mConvexMeshCubeIndices[15] = 5;
        mConvexMeshCubeIndices[16] = 2;
        mConvexMeshCubeIndices[17] = 3;
        mConvexMeshCubeIndices[18] = 7;
        mConvexMeshCubeIndices[19] = 6;
        mConvexMeshCubeIndices[20] = 0;
        mConvexMeshCubeIndices[21] = 4;
        mConvexMeshCubeIndices[22] = 7;
        mConvexMeshCubeIndices[23] = 3;

        // mConvexMeshPolygonFaces = new rp3d::PolygonVertexArray::PolygonFace[6];
        // rp3d::PolygonVertexArray::PolygonFace* face = mConvexMeshPolygonFaces;
        // for (int f = 0; f < 6; f++) {
        //              face.indexBase = f * 4;
        // 	face.nbVertices = 4;
        // 	face++;
        // }
        //          mConvexMesh1PolygonVertexArray = new rp3d::PolygonVertexArray(8, &(mConvexMesh1CubeVertices[0]), 3 * sizeof(float),
        // 		&(mConvexMeshCubeIndices[0]), sizeof(int), 6, mConvexMeshPolygonFaces,
        // 		rp3d::PolygonVertexArray::VertexDataType::VERTEX_FLOAT_TYPE,
        // 		rp3d::PolygonVertexArray::IndexDataType::INDEX_INTEGER_TYPE);
        //          mConvexMesh1PolyhedronMesh = mPhysicsCommon.createPolyhedronMesh(mConvexMesh1PolygonVertexArray);
        //          mConvexMeshShape1 = mPhysicsCommon.createConvexMeshShape(mConvexMesh1PolyhedronMesh);
        // Transform convexMeshTransform1 = new Transform(new Vector3(10, 0, 0), Quaternion.identity());
        // mConvexMeshBody1 = mWorld.createCollisionBody(convexMeshTransform1);
        // mConvexMeshCollider1 = mConvexMeshBody1.addCollider(mConvexMeshShape1, Transform.Identity());

        //        mConvexMesh2CubeVertices[0] = -4; mConvexMesh2CubeVertices[1] = -2; mConvexMesh2CubeVertices[2] = 8;
        //        mConvexMesh2CubeVertices[3] = 4; mConvexMesh2CubeVertices[4] = -2; mConvexMesh2CubeVertices[5] = 8;
        //        mConvexMesh2CubeVertices[6] = 4; mConvexMesh2CubeVertices[7] = -2; mConvexMesh2CubeVertices[8] = -8;
        //        mConvexMesh2CubeVertices[9] = -4; mConvexMesh2CubeVertices[10] = -2; mConvexMesh2CubeVertices[11] = -8;
        //        mConvexMesh2CubeVertices[12] = -4; mConvexMesh2CubeVertices[13] = 2; mConvexMesh2CubeVertices[14] = 8;
        //        mConvexMesh2CubeVertices[15] = 4; mConvexMesh2CubeVertices[16] = 2; mConvexMesh2CubeVertices[17] = 8;
        //        mConvexMesh2CubeVertices[18] = 4; mConvexMesh2CubeVertices[19] = 2; mConvexMesh2CubeVertices[20] = -8;
        //        mConvexMesh2CubeVertices[21] = -4; mConvexMesh2CubeVertices[22] = 2; mConvexMesh2CubeVertices[23] = -8;
        //
        //        mConvexMesh2PolygonVertexArray = new rp3d::PolygonVertexArray(8, &(mConvexMesh2CubeVertices[0]), 3 * sizeof(float),
        // &(mConvexMeshCubeIndices[0]), sizeof(int), 6, mConvexMeshPolygonFaces,
        // rp3d::PolygonVertexArray::VertexDataType::VERTEX_FLOAT_TYPE,
        // rp3d::PolygonVertexArray::IndexDataType::INDEX_INTEGER_TYPE);
        //        mConvexMesh2PolyhedronMesh = mPhysicsCommon.createPolyhedronMesh(mConvexMesh2PolygonVertexArray);
        //        mConvexMeshShape2 = mPhysicsCommon.createConvexMeshShape(mConvexMesh2PolyhedronMesh);
        //        Transform convexMeshTransform2(Vector3(20, 0, 0), Quaternion.identity());
        //        mConvexMeshBody2 = mWorld.createCollisionBody(convexMeshTransform2);
        //        mConvexMeshCollider2 = mConvexMeshBody2.addCollider(mConvexMeshShape2, Transform.Identity());

        // ---------- Concave Meshes ---------- //
        for (var i = 0; i < 6; i++)
        for (var j = 0; j < 6; j++)
        {
            mConcaveMeshPlaneVertices[i * 6 * 3 + j * 3] = -2.5f + i;
            mConcaveMeshPlaneVertices[i * 6 * 3 + j * 3 + 1] = 0;
            mConcaveMeshPlaneVertices[i * 6 * 3 + j * 3 + 2] = -2.5f + j;
        }

        var triangleIndex = 0;
        for (var i = 0; i < 5; i++)
        for (var j = 0; j < 5; j++)
        {
            // Triangle 1
            mConcaveMeshPlaneIndices[triangleIndex * 3] = i * 6 + j;
            mConcaveMeshPlaneIndices[triangleIndex * 3 + 1] = (i + 1) * 6 + j + 1;
            mConcaveMeshPlaneIndices[triangleIndex * 3 + 2] = i * 6 + j + 1;
            triangleIndex++;

            // Triangle 2
            mConcaveMeshPlaneIndices[triangleIndex * 3] = i * 6 + j;
            mConcaveMeshPlaneIndices[triangleIndex * 3 + 1] = (i + 1) * 6 + j;
            mConcaveMeshPlaneIndices[triangleIndex * 3 + 2] = (i + 1) * 6 + j + 1;
            triangleIndex++;
        }

        //          mConcaveMeshTriangleVertexArray = new rp3d::TriangleVertexArray(36, &(mConcaveMeshPlaneVertices[0]), 3 * sizeof(float),
        //                  50, &(mConcaveMeshPlaneIndices[0]), 3 * sizeof(int),
        //                  rp3d::TriangleVertexArray::VertexDataType::VERTEX_FLOAT_TYPE,
        // 		rp3d::TriangleVertexArray::IndexDataType::INDEX_INTEGER_TYPE);
        //
        // // Add the triangle vertex array of the subpart to the triangle mesh
        //          Transform concaveMeshTransform(Vector3(0, -20, 0), Quaternion::identity());
        //          mConcaveTriangleMesh = mPhysicsCommon.createTriangleMesh();
        //          mConcaveTriangleMesh.addSubpart(mConcaveMeshTriangleVertexArray);
        //          mConcaveMeshShape = mPhysicsCommon.createConcaveMeshShape(mConcaveTriangleMesh);
        //          mConcaveMeshBody = mWorld.createCollisionBody(concaveMeshTransform);
        //          mConcaveMeshCollider = mConcaveMeshBody.addCollider(mConcaveMeshShape, rp3d::Transform::identity());


        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        testNoCollisions();
        testNoOverlap();
        testBoxVsBoxCollision();
        testCapsuleVsCapsuleCollision();
        testSphereVsSphereCollision();
        testSphereVsCapsuleCollision();
        
        testBoxVsCapsuleCollision();
        testSphereVsBoxCollision();
         
        
        
        stopwatch.Stop();
        
        // 输出执行时间（毫秒）
        double milliseconds = stopwatch.ElapsedMilliseconds;
        Debug.Log($"程序执行时间: {milliseconds} 毫秒");
    }

    [Test]
    public void testNoCollisions()
    {
        Init();
        // 当世界中的所有形状被创建时，它们之间都没有接触。
        // 在这里，我们测试在开始时，是否完全没有任何碰撞。

        // ---------- Global test ---------- //

        mCollisionCallback.reset();
        mWorld.testCollision(mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        // ---------- Single body test ---------- //

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody2, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody2, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        // Two bodies test

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mBoxBody2, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mSphereBody1, mSphereBody2, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mSphereBody1, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody1, mSphereBody2, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody2, mSphereBody1, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());

        mCollisionCallback.reset();
        mWorld.testCollision(mBoxBody2, mSphereBody2, mCollisionCallback);
        rp3d_test(!mCollisionCallback.hasContacts());
    }
    [Test]
    public void testNoOverlap()
    {
        Init();
        // 当世界中的所有形状被创建时，它们之间都没有接触。
        // 在这里，我们测试在开始时，是否完全没有任何重叠。


        // ---------- Single body test ---------- //

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody1, mOverlapCallback);
        rp3d_test(!mOverlapCallback.hasOverlapWithBody(mBoxBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mBoxBody2, mOverlapCallback);
        rp3d_test(!mOverlapCallback.hasOverlapWithBody(mBoxBody2));

        mOverlapCallback.reset();
        mWorld.testOverlap(mSphereBody1, mOverlapCallback);
        rp3d_test(!mOverlapCallback.hasOverlapWithBody(mSphereBody1));

        mOverlapCallback.reset();
        mWorld.testOverlap(mSphereBody2, mOverlapCallback);
        rp3d_test(!mOverlapCallback.hasOverlapWithBody(mSphereBody2));

        // Two bodies test

        rp3d_test(!mWorld.testOverlap(mBoxBody1, mBoxBody2));
        rp3d_test(!mWorld.testOverlap(mSphereBody1, mSphereBody2));
        rp3d_test(!mWorld.testOverlap(mBoxBody1, mSphereBody1));
        rp3d_test(!mWorld.testOverlap(mBoxBody1, mSphereBody2));
        rp3d_test(!mWorld.testOverlap(mBoxBody2, mSphereBody1));
        rp3d_test(!mWorld.testOverlap(mBoxBody2, mSphereBody2));
    }

    
    public void rp3d_test(bool value)
    {
        Debug.Log(value);
    }
}