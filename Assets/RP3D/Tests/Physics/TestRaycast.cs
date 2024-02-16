using System.Collections.Generic;
using RP3D;
using UnityEngine;
using Collider = RP3D.Collider;
using Vector3 = RP3D.PVector3;
using Transform = RP3D.PTransform;
using Quaternion = RP3D.PQuaternion;
using Ray = RP3D.PRay;

namespace Scenes.Test
{
    
    public enum Category {
        CATEGORY1 = 0x0001,
        CATEGORY2 = 0x0002
    };
    public class TestRaycast : MonoBehaviour
    {
        private PhysicsCommon mPhysicsCommon;

        // Raycast callback class
        private WorldRaycastCallback mCallback;


        // Epsilon
        private float epsilon;

        // Physics world
        private PhysicsWorld mWorld;

        // Bodies
        private CollisionBody mBoxBody;
        private CollisionBody mSphereBody;
        private CollisionBody mCapsuleBody;
        private CollisionBody mConvexMeshBody;
        private CollisionBody mCylinderBody;
        private CollisionBody mCompoundBody;
        private CollisionBody mConcaveMeshBody;
        private CollisionBody mHeightFieldBody;

        // Transform
        private Transform mBodyTransform;
        private Transform mShapeTransform;
        private Transform mLocalShapeToWorld;
        private Transform mLocalShape2ToWorld;

        // Collision shapes
        private BoxShape mBoxShape;
        private SphereShape mSphereShape;

        private CapsuleShape mCapsuleShape;
        // ConvexMeshShape mConvexMeshShape;
        // ConcaveMeshShape mConcaveMeshShape;
        // HeightFieldShape mHeightFieldShape;

        // Collider
        private Collider mBoxCollider;
        private Collider mSphereCollider;
        private Collider mCapsuleCollider;
        private Collider mConvexMeshCollider;
        private Collider mCompoundSphereCollider;
        private Collider mCompoundCapsuleCollider;
        private Collider mConcaveMeshCollider;
        private Collider mHeightFieldCollider;

        // Triangle meshes
        // TriangleMesh mConcaveTriangleMesh;

        private List<Vector3> mConcaveMeshVertices;

        private List<int> mConcaveMeshIndices;

        // TriangleVertexArray mConcaveMeshVertexArray;
        private float[] mHeightFieldData;

        // PolygonFace[] mPolygonFaces;
        // PolygonVertexArray mPolygonVertexArray;
        // PolyhedronMesh mPolyhedronMesh;
        private float[] mPolyhedronVertices;
        private int[] mPolyhedronIndices;


        private void Start()
        {

            mPolyhedronVertices = new float[24];
            mPolyhedronIndices = new int[24];
            mCallback = new WorldRaycastCallback();
            
            mConcaveMeshIndices = new List<int>();
            mConcaveMeshVertices = new List<PVector3>();

            epsilon = 0.0001f;

            var worldSettings = new WorldSettings();
            mPhysicsCommon = new PhysicsCommon();
            mWorld = mPhysicsCommon.createPhysicsWorld(worldSettings);


            // Body transform
            var position = new Vector3(-3, 2, 7);
            var orientation = Quaternion.FromEulerAngles(PMath.PI_RP3D / 5, PMath.PI_RP3D / 6, PMath.PI_RP3D / 7);
            mBodyTransform = new Transform(position, orientation);

            // Create the bodies
            mBoxBody = mWorld.createCollisionBody(mBodyTransform);
            mSphereBody = mWorld.createCollisionBody(mBodyTransform);
            mCapsuleBody = mWorld.createCollisionBody(mBodyTransform);
            mConvexMeshBody = mWorld.createCollisionBody(mBodyTransform);
            mCylinderBody = mWorld.createCollisionBody(mBodyTransform);
            mCompoundBody = mWorld.createCollisionBody(mBodyTransform);
            mConcaveMeshBody = mWorld.createCollisionBody(mBodyTransform);
            mHeightFieldBody = mWorld.createCollisionBody(mBodyTransform);

            // Collision shape transform
            var shapePosition = new Vector3(1, -4, -3);
            var shapeOrientation =
                Quaternion.FromEulerAngles(3 * PMath.PI_RP3D / 6, -PMath.PI_RP3D / 8, PMath.PI_RP3D / 3);
            mShapeTransform = new Transform(shapePosition, shapeOrientation);

            // Compute the the transform from a local shape point to world-space
            mLocalShapeToWorld = mBodyTransform * mShapeTransform;

            // Create collision shapes
            mBoxShape = mPhysicsCommon.createBoxShape(new Vector3(2, 3, 4));
            mBoxCollider = mBoxBody.addCollider(mBoxShape, mShapeTransform);

            mSphereShape = mPhysicsCommon.createSphereShape(3);
            mSphereCollider = mSphereBody.addCollider(mSphereShape, mShapeTransform);

            mCapsuleShape = mPhysicsCommon.createCapsuleShape(2, 5);
            mCapsuleCollider = mCapsuleBody.addCollider(mCapsuleShape, mShapeTransform);

            mPolyhedronVertices[0] = -2; mPolyhedronVertices[1] = -3; mPolyhedronVertices[2] = 4;
            mPolyhedronVertices[3] = 2; mPolyhedronVertices[4] = -3; mPolyhedronVertices[5] = 4;
            mPolyhedronVertices[6] = 2; mPolyhedronVertices[7] = -3; mPolyhedronVertices[8] = -4;
            mPolyhedronVertices[9] = -2; mPolyhedronVertices[10] = -3; mPolyhedronVertices[11] = -4;
            mPolyhedronVertices[12] = -2; mPolyhedronVertices[13] = 3; mPolyhedronVertices[14] = 4;
            mPolyhedronVertices[15] = 2; mPolyhedronVertices[16] = 3; mPolyhedronVertices[17] = 4;
            mPolyhedronVertices[18] = 2; mPolyhedronVertices[19] = 3; mPolyhedronVertices[20] = -4;
            mPolyhedronVertices[21] = -2; mPolyhedronVertices[22] = 3; mPolyhedronVertices[23] = -4;

            mPolyhedronIndices[0] = 0; mPolyhedronIndices[1] = 3; mPolyhedronIndices[2] = 2; mPolyhedronIndices[3] = 1;
            mPolyhedronIndices[4] = 4; mPolyhedronIndices[5] = 5; mPolyhedronIndices[6] = 6; mPolyhedronIndices[7] = 7;
            mPolyhedronIndices[8] = 0; mPolyhedronIndices[9] = 1; mPolyhedronIndices[10] = 5; mPolyhedronIndices[11] = 4;
            mPolyhedronIndices[12] = 1; mPolyhedronIndices[13] = 2; mPolyhedronIndices[14] = 6; mPolyhedronIndices[15] = 5;
            mPolyhedronIndices[16] = 2; mPolyhedronIndices[17] = 3; mPolyhedronIndices[18] = 7; mPolyhedronIndices[19] = 6;
            mPolyhedronIndices[20] = 0; mPolyhedronIndices[21] = 4; mPolyhedronIndices[22] = 7; mPolyhedronIndices[23] = 3;

            // Polygon faces descriptions for the polyhedron
            // PolygonVertexArray::PolygonFace* face = mPolygonFaces;
            // for (int f = 0; f < 6; f++) {
            //     face.indexBase = f * 4;
            //     face.nbVertices = 4;
            //     face++;
            // }
            //
            // // Create the polygon vertex array
            // mPolygonVertexArray = new PolygonVertexArray(8, mPolyhedronVertices, 3 * sizeof(float),
            //                              mPolyhedronIndices, sizeof(int), 6, mPolygonFaces,
            //                              PolygonVertexArray::VertexDataType::VERTEX_FLOAT_TYPE,
            //                              PolygonVertexArray::IndexDataType::INDEX_INTEGER_TYPE);
            //
            // mPolyhedronMesh = mPhysicsCommon.createPolyhedronMesh(mPolygonVertexArray);
            // mConvexMeshShape = mPhysicsCommon.createConvexMeshShape(mPolyhedronMesh);
            // mConvexMeshCollider = mConvexMeshBody.addCollider(mConvexMeshShape, mShapeTransform);

            // Compound shape is a cylinder and a sphere
            var positionShape2 = new Vector3(4, 2, -3);
            var orientationShape2 =
                Quaternion.FromEulerAngles(-3 * PMath.PI_RP3D / 8, 1.5f * PMath.PI_RP3D / 3, PMath.PI_RP3D / 13);
            var shapeTransform2 = new Transform(positionShape2, orientationShape2);
            mLocalShape2ToWorld = mBodyTransform * shapeTransform2;
            mCompoundCapsuleCollider = mCompoundBody.addCollider(mCapsuleShape, mShapeTransform);
            mCompoundSphereCollider = mCompoundBody.addCollider(mSphereShape, shapeTransform2);

            // Concave Mesh shape
            // mConcaveMeshVertices.Add(new Vector3(-2, -3, -4));
            // mConcaveMeshVertices.Add(new Vector3(2, -3, -4));
            // mConcaveMeshVertices.Add(new Vector3(2, -3, 4));
            // mConcaveMeshVertices.Add(new Vector3(-2, -3, 4));
            // mConcaveMeshVertices.Add(new Vector3(-2, 3, -4));
            // mConcaveMeshVertices.Add(new Vector3(2, 3, -4));
            // mConcaveMeshVertices.Add(new Vector3(2, 3, 4));
            // mConcaveMeshVertices.Add(new Vector3(-2, 3, 4));

            // mConcaveMeshIndices.Add((0); mConcaveMeshIndices.Add((1); mConcaveMeshIndices.Add((2);
            // mConcaveMeshIndices.Add((0); mConcaveMeshIndices.Add((2); mConcaveMeshIndices.Add((3);
            // mConcaveMeshIndices.Add((1); mConcaveMeshIndices.Add((5); mConcaveMeshIndices.Add((2);
            // mConcaveMeshIndices.Add((2); mConcaveMeshIndices.Add((5); mConcaveMeshIndices.Add((6);
            // mConcaveMeshIndices.Add((2); mConcaveMeshIndices.Add((7); mConcaveMeshIndices.Add((3);
            // mConcaveMeshIndices.Add((2); mConcaveMeshIndices.Add((6); mConcaveMeshIndices.Add((7);
            // mConcaveMeshIndices.Add((0); mConcaveMeshIndices.Add((3); mConcaveMeshIndices.Add((4);
            // mConcaveMeshIndices.Add((3); mConcaveMeshIndices.Add((7); mConcaveMeshIndices.Add((4);
            // mConcaveMeshIndices.Add((0); mConcaveMeshIndices.Add((4); mConcaveMeshIndices.Add((1);
            // mConcaveMeshIndices.Add((1); mConcaveMeshIndices.Add((4); mConcaveMeshIndices.Add((5);
            // mConcaveMeshIndices.Add((5); mConcaveMeshIndices.Add((7); mConcaveMeshIndices.Add((6);
            // mConcaveMeshIndices.Add((4); mConcaveMeshIndices.Add((7); mConcaveMeshIndices.Add((5);
            // VertexDataType vertexType = sizeof(decimal) == 4 ? TriangleVertexArray::VertexDataType::VERTEX_FLOAT_TYPE :
            //                                                                         TriangleVertexArray::VertexDataType::VERTEX_DOUBLE_TYPE;
            // mConcaveMeshVertexArray =
            //         new TriangleVertexArray(8, &(mConcaveMeshVertices[0]), sizeof(Vector3),
            //                                       12, &(mConcaveMeshIndices[0]), 3 * sizeof(uint),
            //                                       vertexType, TriangleVertexArray::IndexDataType::INDEX_INTEGER_TYPE);
            //
            // // Add the triangle vertex array of the subpart to the triangle mesh
            // mConcaveTriangleMesh = mPhysicsCommon.createTriangleMesh();
            // mConcaveTriangleMesh.addSubpart(mConcaveMeshVertexArray);
            // mConcaveMeshShape = mPhysicsCommon.createConcaveMeshShape(mConcaveTriangleMesh);
            // mConcaveMeshCollider = mConcaveMeshBody.addCollider(mConcaveMeshShape, mShapeTransform);


            // Heightfield shape (plane height field at height=4)
            // for (int i=0; i<100; i++) mHeightFieldData[i] = 4;
            // mHeightFieldShape = mPhysicsCommon.createHeightFieldShape(10, 10, 0, 4, mHeightFieldData, HeightFieldShape::HeightDataType::HEIGHT_FLOAT_TYPE);
            // mHeightFieldCollider = mHeightFieldBody.addCollider(mHeightFieldShape, mShapeTransform);

            // Assign colliders to the different categories
            mBoxCollider.setCollisionCategoryBits(0x0001);
            mSphereCollider.setCollisionCategoryBits(0x0001);
            mCapsuleCollider.setCollisionCategoryBits(0x0001);
            //mConvexMeshCollider.setCollisionCategoryBits(0x0001);
            mCompoundSphereCollider.setCollisionCategoryBits(0x0001);
            mCompoundCapsuleCollider.setCollisionCategoryBits(0x0001);
            //mConcaveMeshCollider.setCollisionCategoryBits(0x0001);
            //mHeightFieldCollider.setCollisionCategoryBits(0x0001);


            testBox();

            testSphere();

            testCapsule();
        }

        /// Test the Collider::raycast(), CollisionBody::raycast() and
        /// PhysicsWorld::raycast() methods.
        private void testBox()
        {
            // ----- Test feedback data ----- //
            var point1 = mLocalShapeToWorld * new Vector3(1, 2, 10);
            var point2 = mLocalShapeToWorld * new Vector3(1, 2, -20);
            var ray = new Ray(point1, point2);
            var hitPoint = mLocalShapeToWorld * new Vector3(1, 2, 4);

            mCallback.shapeToTest = mBoxCollider;

            // PhysicsWorld::raycast()
            mCallback.reset();
            mWorld.raycast(ray, mCallback);
            rp3d_test(mCallback.isHit);
            rp3d_test(mCallback.raycastInfo.body == mBoxBody);
            rp3d_test(mCallback.raycastInfo.collider == mBoxCollider);
            rp3d_test(approxEqual(mCallback.raycastInfo.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.z, hitPoint.z, epsilon));

            // Correct category filter mask
            mCallback.reset();
            mWorld.raycast(ray, mCallback, 0x0001);
            rp3d_test(mCallback.isHit);

            // Wrong category filter mask
            mCallback.reset();
            mWorld.raycast(ray, mCallback, 0x0002);
            rp3d_test(!mCallback.isHit);

            // CollisionBody::raycast()
            var raycastInfo2  = new RaycastInfo();
            rp3d_test(mBoxBody.raycast(ray,out raycastInfo2));
            rp3d_test(raycastInfo2.body == mBoxBody);
            rp3d_test(raycastInfo2.collider == mBoxCollider);
            rp3d_test(approxEqual(raycastInfo2.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.z, hitPoint.z, epsilon));

            // Collider::raycast()
            var raycastInfo3 = new RaycastInfo();
            rp3d_test(mBoxCollider.Raycast(ray, out raycastInfo3));
            rp3d_test(raycastInfo3.body == mBoxBody);
            rp3d_test(raycastInfo3.collider == mBoxCollider);
            rp3d_test(approxEqual(raycastInfo3.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.z, hitPoint.z, epsilon));

            var ray1 = new Ray(mLocalShapeToWorld * new Vector3(0, 0, 0), mLocalShapeToWorld * new Vector3(5, 7, -1));
            var ray2 = new Ray(mLocalShapeToWorld * new Vector3(5, 11, 7),
                mLocalShapeToWorld * new Vector3(17, 29, 28));
            var ray3 = new Ray(mLocalShapeToWorld * new Vector3(1, 2, 3), mLocalShapeToWorld * new Vector3(-11, 2, 24));
            var ray4 = new Ray(mLocalShapeToWorld * new Vector3(10, 10, 10),
                mLocalShapeToWorld * new Vector3(22, 28, 31));
            var ray5 = new Ray(mLocalShapeToWorld * new Vector3(3, 1, -5),
                mLocalShapeToWorld * new Vector3(-30, 1, -5));
            var ray6 = new Ray(mLocalShapeToWorld * new Vector3(4, 4, 1), mLocalShapeToWorld * new Vector3(4, -20, 1));
            var ray7 = new Ray(mLocalShapeToWorld * new Vector3(1, -4, 5),
                mLocalShapeToWorld * new Vector3(1, -4, -20));
            var ray8 = new Ray(mLocalShapeToWorld * new Vector3(-4, 4, 0), mLocalShapeToWorld * new Vector3(20, 4, 0));
            var ray9 = new Ray(mLocalShapeToWorld * new Vector3(0, -4, -7),
                mLocalShapeToWorld * new Vector3(0, 50, -7));
            var ray10 = new Ray(mLocalShapeToWorld * new Vector3(-3, 0, -6),
                mLocalShapeToWorld * new Vector3(-3, 0, 20));
            var ray11 = new Ray(mLocalShapeToWorld * new Vector3(3, 1, 2), mLocalShapeToWorld * new Vector3(-20, 1, 2));
            var ray12 = new Ray(mLocalShapeToWorld * new Vector3(1, 4, -1),
                mLocalShapeToWorld * new Vector3(1, -20, -1));
            var ray13 = new Ray(mLocalShapeToWorld * new Vector3(-1, 2, 5),
                mLocalShapeToWorld * new Vector3(-1, 2, -20));
            var ray14 = new Ray(mLocalShapeToWorld * new Vector3(-3, 2, -2),
                mLocalShapeToWorld * new Vector3(20, 2, -2));
            var ray15 = new Ray(mLocalShapeToWorld * new Vector3(0, -4, 1), mLocalShapeToWorld * new Vector3(0, 20, 1));
            var ray16 = new Ray(mLocalShapeToWorld * new Vector3(-1, 2, -5),
                mLocalShapeToWorld * new Vector3(-1, 2, 20));

            // ----- Test raycast miss ----- //
            rp3d_test(!mBoxBody.raycast(ray1,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray1, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray1, mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray1.point1, ray1.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray1.point1, ray1.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray2,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray2, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray2, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray3,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray3, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray3, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray4,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray4, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray4, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray5,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray5, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray5, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray6,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray6, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray6, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray7,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray7, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray7, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray8,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray8, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray8, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray9,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray9, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray9, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mBoxBody.raycast(ray10,out raycastInfo3));
            rp3d_test(!mBoxCollider.Raycast(ray10, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray10, mCallback);
            rp3d_test(!mCallback.isHit);

            mCallback.reset();
            mWorld.raycast(new Ray(ray11.point1, ray11.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray12.point1, ray12.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray13.point1, ray13.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray14.point1, ray14.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray15.point1, ray15.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray16.point1, ray16.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);

            // ----- Test raycast hits ----- //
            rp3d_test(mBoxBody.raycast(ray11,out raycastInfo3));
            rp3d_test(mBoxCollider.Raycast(ray11, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray11, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray11.point1, ray11.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mBoxBody.raycast(ray12,out raycastInfo3));
            rp3d_test(mBoxCollider.Raycast(ray12, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray12, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray12.point1, ray12.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mBoxBody.raycast(ray13,out raycastInfo3));
            rp3d_test(mBoxCollider.Raycast(ray13, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray13, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray13.point1, ray13.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mBoxBody.raycast(ray14,out raycastInfo3));
            rp3d_test(mBoxCollider.Raycast(ray14, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray14, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray14.point1, ray14.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mBoxBody.raycast(ray15,out raycastInfo3));
            rp3d_test(mBoxCollider.Raycast(ray15, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray15, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray15.point1, ray15.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mBoxBody.raycast(ray16,out raycastInfo3));
            rp3d_test(mBoxCollider.Raycast(ray16, out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray16, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray16.point1, ray16.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);
        }
        
        
        
                /// Test the Collider::raycast(), CollisionBody::raycast() and
        /// PhysicsWorld::raycast() methods.
        void testSphere() {

            // ----- Test feedback data ----- //
            Vector3 point1 = mLocalShapeToWorld * new Vector3(-5 , 0, 0);
            Vector3 point2 = mLocalShapeToWorld * new Vector3(5, 0, 0);
            Ray ray = new Ray(point1, point2);
            Vector3 hitPoint = mLocalShapeToWorld * new Vector3(-3, 0, 0);

            mCallback.shapeToTest = mSphereCollider;

            // PhysicsWorld::raycast()
            mCallback.reset();
            mWorld.raycast(ray, mCallback);
            rp3d_test(mCallback.isHit);
            rp3d_test(mCallback.raycastInfo.body == mSphereBody);
            rp3d_test(mCallback.raycastInfo.collider == mSphereCollider);
            rp3d_test(approxEqual(mCallback.raycastInfo.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.z, hitPoint.z, epsilon));

            // Correct category filter mask
            mCallback.reset();
            mWorld.raycast(ray, mCallback,(short) Category.CATEGORY1);
            rp3d_test(mCallback.isHit);

            // Wrong category filter mask
            mCallback.reset();
            mWorld.raycast(ray, mCallback, (short)Category.CATEGORY2);
            rp3d_test(!mCallback.isHit);

            // CollisionBody::raycast()
            RaycastInfo raycastInfo2;
            rp3d_test(mSphereBody.raycast(ray,out raycastInfo2));
            rp3d_test(raycastInfo2.body == mSphereBody);
            rp3d_test(raycastInfo2.collider == mSphereCollider);
            rp3d_test(approxEqual(raycastInfo2.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.z, hitPoint.z, epsilon));

            // Collider::raycast()
            RaycastInfo raycastInfo3;
            rp3d_test(mSphereCollider.Raycast(ray,out raycastInfo3));
            rp3d_test(raycastInfo3.body == mSphereBody);
            rp3d_test(raycastInfo3.collider == mSphereCollider);
            rp3d_test(approxEqual(raycastInfo3.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.z, hitPoint.z, epsilon));

            Ray ray1 = new Ray(mLocalShapeToWorld * new Vector3(0, 0, 0), mLocalShapeToWorld * new Vector3(5, 7, -1));
            Ray ray2 = new Ray(mLocalShapeToWorld * new Vector3(5, 11, 7), mLocalShapeToWorld * new Vector3(4, 6, 7));
            Ray ray3 = new Ray(mLocalShapeToWorld * new Vector3(1, 2, 2), mLocalShapeToWorld * new Vector3(-4, 0, 7));
            Ray ray4 = new Ray(mLocalShapeToWorld * new Vector3(10, 10, 10), mLocalShapeToWorld * new Vector3(4, 6, 7));
            Ray ray5 = new Ray(mLocalShapeToWorld * new Vector3(4, 1, -5), mLocalShapeToWorld * new Vector3(-30, 1, -5));
            Ray ray6 = new Ray(mLocalShapeToWorld * new Vector3(4, 4, 1), mLocalShapeToWorld * new Vector3(4, -30, 1));
            Ray ray7 = new Ray(mLocalShapeToWorld * new Vector3(1, -4, 5), mLocalShapeToWorld * new Vector3(1, -4, -30));
            Ray ray8 = new Ray(mLocalShapeToWorld * new Vector3(-4, 4, 0), mLocalShapeToWorld * new Vector3(30, 4, 0));
            Ray ray9 = new Ray(mLocalShapeToWorld * new Vector3(0, -4, -4), mLocalShapeToWorld * new Vector3(0, 30, -4));
            Ray ray10 = new Ray(mLocalShapeToWorld * new Vector3(-4, 0, -6), mLocalShapeToWorld * new Vector3(-4, 0, 30));
            Ray ray11 = new Ray(mLocalShapeToWorld * new Vector3(4, 1, 2), mLocalShapeToWorld * new Vector3(-30, 1, 2));
            Ray ray12 = new Ray(mLocalShapeToWorld * new Vector3(1, 4, -1), mLocalShapeToWorld * new Vector3(1, -30, -1));
            Ray ray13 = new Ray(mLocalShapeToWorld * new Vector3(-1, 2, 5), mLocalShapeToWorld * new Vector3(-1, 2, -30));
            Ray ray14 = new Ray(mLocalShapeToWorld * new Vector3(-5, 2, -2), mLocalShapeToWorld * new Vector3(30, 2, -2));
            Ray ray15 = new Ray(mLocalShapeToWorld * new Vector3(0, -4, 1), mLocalShapeToWorld * new Vector3(0, 30, 1));
            Ray ray16 = new Ray(mLocalShapeToWorld * new Vector3(-1, 2, -11), mLocalShapeToWorld * new Vector3(-1, 2, 30));

            // ----- Test raycast miss ----- //
            rp3d_test(!mSphereBody.raycast(ray1,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray1,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray1, mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray1.point1, ray1.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray1.point1, ray1.point2, 100.0f), mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray2,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray2,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray2, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray3,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray3,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray3, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray4,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray4,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray4, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray5,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray5,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray5, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray6,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray6,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray6, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray7,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray7,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray7, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray8,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray8,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray8, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray9,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray9,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray9, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mSphereBody.raycast(ray10,out raycastInfo3));
            rp3d_test(!mSphereCollider.Raycast(ray10,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray10, mCallback);
            rp3d_test(!mCallback.isHit);

            mCallback.reset();
            mWorld.raycast(new Ray(ray11.point1, ray11.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray12.point1, ray12.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray13.point1, ray13.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray14.point1, ray14.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray15.point1, ray15.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray16.point1, ray16.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);

            // ----- Test raycast hits ----- //
            rp3d_test(mSphereBody.raycast(ray11,out raycastInfo3));
            rp3d_test(mSphereCollider.Raycast(ray11,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray11, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray11.point1, ray11.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mSphereBody.raycast(ray12,out raycastInfo3));
            rp3d_test(mSphereCollider.Raycast(ray12,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray12, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray12.point1, ray12.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mSphereBody.raycast(ray13,out raycastInfo3));
            rp3d_test(mSphereCollider.Raycast(ray13,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray13, mCallback);
            mCallback.reset();
            mWorld.raycast(new Ray(ray13.point1, ray13.point2, 0.8f), mCallback);

            rp3d_test(mSphereBody.raycast(ray14,out raycastInfo3));
            rp3d_test(mSphereCollider.Raycast(ray14,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray14, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray14.point1, ray14.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mSphereBody.raycast(ray15,out raycastInfo3));
            rp3d_test(mSphereCollider.Raycast(ray15,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray15, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray15.point1, ray15.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mSphereBody.raycast(ray16,out raycastInfo3));
            rp3d_test(mSphereCollider.Raycast(ray16,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray16, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray16.point1, ray16.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);
        }

        /// Test the Collider::raycast(), CollisionBody::raycast() and
        /// PhysicsWorld::raycast() methods.
        void testCapsule() {

            // ----- Test feedback data ----- //
            Vector3 point1A = mLocalShapeToWorld * new Vector3(4 , 1, 0);
            Vector3 point1B = mLocalShapeToWorld * new Vector3(-6, 1, 0);
            Ray ray = new Ray(point1A, point1B);
            Vector3 hitPoint = mLocalShapeToWorld * new Vector3(2, 1, 0);

            Vector3 point2A = mLocalShapeToWorld * new Vector3(0 , 6.5f, 0);
            Vector3 point2B = mLocalShapeToWorld * new Vector3(0, -3.5f, 0);
            Ray rayTop = new Ray(point2A, point2B);
            Vector3 hitPointTop = mLocalShapeToWorld * new Vector3(0, 4.5f, 0);

            Vector3 point3A = mLocalShapeToWorld * new Vector3(0 , -6.5f, 0);
            Vector3 point3B = mLocalShapeToWorld * new Vector3(0, 3.5f, 0);
            Ray rayBottom = new Ray(point3A, point3B);
            Vector3 hitPointBottom = mLocalShapeToWorld * new Vector3(0, -4.5f, 0);

            mCallback.shapeToTest = mCapsuleCollider;

            // PhysicsWorld::raycast()
            mCallback.reset();
            mWorld.raycast(ray, mCallback);
            rp3d_test(mCallback.isHit);
            rp3d_test(mCallback.raycastInfo.body == mCapsuleBody);
            rp3d_test(mCallback.raycastInfo.collider == mCapsuleCollider);
            rp3d_test(approxEqual(mCallback.raycastInfo.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(mCallback.raycastInfo.worldPoint.z, hitPoint.z, epsilon));

            // Correct category filter mask
            mCallback.reset();
            mWorld.raycast(ray, mCallback,(short)Category.CATEGORY1);
            rp3d_test(mCallback.isHit);

            // Wrong category filter mask
            mCallback.reset();
            mWorld.raycast(ray, mCallback,(short) Category.CATEGORY2);
            rp3d_test(!mCallback.isHit);

            // CollisionBody::raycast()
            RaycastInfo raycastInfo2;
            rp3d_test(mCapsuleBody.raycast(ray,out raycastInfo2));
            rp3d_test(raycastInfo2.body == mCapsuleBody);
            rp3d_test(raycastInfo2.collider == mCapsuleCollider);
            rp3d_test(approxEqual(raycastInfo2.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(raycastInfo2.worldPoint.z, hitPoint.z, epsilon));

            // Collider::raycast()
            RaycastInfo raycastInfo3;
            rp3d_test(mCapsuleCollider.Raycast(ray,out raycastInfo3));
            rp3d_test(raycastInfo3.body == mCapsuleBody);
            rp3d_test(raycastInfo3.collider == mCapsuleCollider);
            rp3d_test(approxEqual(raycastInfo3.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.x, hitPoint.x, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.y, hitPoint.y, epsilon));
            rp3d_test(approxEqual(raycastInfo3.worldPoint.z, hitPoint.z, epsilon));

            RaycastInfo raycastInfo4;
            rp3d_test(mCapsuleCollider.Raycast(rayTop,out raycastInfo4));
            rp3d_test(raycastInfo4.body == mCapsuleBody);
            rp3d_test(raycastInfo4.collider == mCapsuleCollider);
            rp3d_test(approxEqual(raycastInfo4.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo4.worldPoint.x, hitPointTop.x, epsilon));
            rp3d_test(approxEqual(raycastInfo4.worldPoint.y, hitPointTop.y, epsilon));
            rp3d_test(approxEqual(raycastInfo4.worldPoint.z, hitPointTop.z, epsilon));

            // Collider::raycast()
            RaycastInfo raycastInfo5;
            rp3d_test(mCapsuleCollider.Raycast(rayBottom,out raycastInfo5));
            rp3d_test(raycastInfo5.body == mCapsuleBody);
            rp3d_test(raycastInfo5.collider == mCapsuleCollider);
            rp3d_test(approxEqual(raycastInfo5.hitFraction, 0.2f, epsilon));
            rp3d_test(approxEqual(raycastInfo5.worldPoint.x, hitPointBottom.x, epsilon));
            rp3d_test(approxEqual(raycastInfo5.worldPoint.y, hitPointBottom.y, epsilon));
            rp3d_test(approxEqual(raycastInfo5.worldPoint.z, hitPointBottom.z, epsilon));

            Ray ray1 = new Ray(mLocalShapeToWorld * new Vector3(0, 0, 0), mLocalShapeToWorld * new Vector3(5, 7, -1));
            Ray ray2 = new Ray(mLocalShapeToWorld * new Vector3(5, 11, 7), mLocalShapeToWorld * new Vector3(9, 17, 14));
            Ray ray3 = new Ray(mLocalShapeToWorld * new Vector3(1, 3, -1), mLocalShapeToWorld * new Vector3(-3, 3, 6));
            Ray ray4 = new Ray(mLocalShapeToWorld * new Vector3(10, 10, 10), mLocalShapeToWorld * new Vector3(14, 16, 17));
            Ray ray5 = new Ray(mLocalShapeToWorld * new Vector3(4, 1, -5), mLocalShapeToWorld * new Vector3(1, 1, -5));
            Ray ray6 = new Ray(mLocalShapeToWorld * new Vector3(4, 9, 1), mLocalShapeToWorld * new Vector3(4, 7, 1));
            Ray ray7 = new Ray(mLocalShapeToWorld * new Vector3(1, -9, 5), mLocalShapeToWorld * new Vector3(1, -9, 3));
            Ray ray8 = new Ray(mLocalShapeToWorld * new Vector3(-4, 9, 0), mLocalShapeToWorld * new Vector3(-3, 9, 0));
            Ray ray9 = new Ray(mLocalShapeToWorld * new Vector3(0, -9, -4), mLocalShapeToWorld * new Vector3(0, -4, -4));
            Ray ray10 = new Ray(mLocalShapeToWorld * new Vector3(-4, 0, -6), mLocalShapeToWorld * new Vector3(-4, 0, 2));
            Ray ray11 = new Ray(mLocalShapeToWorld * new Vector3(4, 1, 1.5f), mLocalShapeToWorld * new Vector3(-30, 1, 1.5f));
            Ray ray12 = new Ray(mLocalShapeToWorld * new Vector3(1, 9, -1), mLocalShapeToWorld * new Vector3(1, -30, -1));
            Ray ray13 = new Ray(mLocalShapeToWorld * new Vector3(-1, 2, 3), mLocalShapeToWorld * new Vector3(-1, 2, -30));
            Ray ray14 = new Ray(mLocalShapeToWorld * new Vector3(-3, 2, -1.7f), mLocalShapeToWorld * new Vector3(30, 2, -1.7f));
            Ray ray15 = new Ray(mLocalShapeToWorld * new Vector3(0, -9, 1), mLocalShapeToWorld * new Vector3(0, 30, 1));
            Ray ray16 = new Ray(mLocalShapeToWorld * new Vector3(-1, 2, -7), mLocalShapeToWorld * new Vector3(-1, 2, 30));

            // ----- Test raycast miss ----- //
            rp3d_test(!mCapsuleBody.raycast(ray1,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray1,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray1, mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray1.point1, ray1.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray1.point1, ray1.point2, 100.0f), mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray2,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray2,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray2, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray3,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray3,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray3, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray4,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray4,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray4, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray5,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray5,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray5, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray6,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray6,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray6, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray7,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray7,out raycastInfo3));
            mWorld.raycast(ray7, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray8,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray8,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray8, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray9,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray9,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray9, mCallback);
            rp3d_test(!mCallback.isHit);

            rp3d_test(!mCapsuleBody.raycast(ray10,out raycastInfo3));
            rp3d_test(!mCapsuleCollider.Raycast(ray10,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray10, mCallback);
            rp3d_test(!mCallback.isHit);

            mCallback.reset();
            mWorld.raycast(new Ray(ray11.point1, ray11.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray12.point1, ray12.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray13.point1, ray13.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray14.point1, ray14.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray15.point1, ray15.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray16.point1, ray16.point2, 0.01f), mCallback);
            rp3d_test(!mCallback.isHit);

            // ----- Test raycast hits ----- //
            rp3d_test(mCapsuleBody.raycast(ray11,out raycastInfo3));
            rp3d_test(mCapsuleCollider.Raycast(ray11,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray11, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray11.point1, ray11.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mCapsuleBody.raycast(ray12,out raycastInfo3));
            rp3d_test(mCapsuleCollider.Raycast(ray12,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray12, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray12.point1, ray12.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mCapsuleBody.raycast(ray13,out raycastInfo3));
            rp3d_test(mCapsuleCollider.Raycast(ray13,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray13, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray13.point1, ray13.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mCapsuleBody.raycast(ray14,out raycastInfo3));
            rp3d_test(mCapsuleCollider.Raycast(ray14,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray14, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray14.point1, ray14.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mCapsuleBody.raycast(ray15,out raycastInfo3));
            rp3d_test(mCapsuleCollider.Raycast(ray15,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray15, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray15.point1, ray15.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);

            rp3d_test(mCapsuleBody.raycast(ray16,out raycastInfo3));
            rp3d_test(mCapsuleCollider.Raycast(ray16,out raycastInfo3));
            mCallback.reset();
            mWorld.raycast(ray16, mCallback);
            rp3d_test(mCallback.isHit);
            mCallback.reset();
            mWorld.raycast(new Ray(ray16.point1, ray16.point2, 0.8f), mCallback);
            rp3d_test(mCallback.isHit);
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