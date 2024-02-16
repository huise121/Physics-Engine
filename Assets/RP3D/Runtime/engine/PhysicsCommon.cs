using System.Collections.Generic;
using UnityEngine;

namespace RP3D
{
    public class PhysicsCommon
    {
        /// Half-edge structure of a box polyhedron
        public HalfEdgeStructure mBoxShapeHalfEdgeStructure;
        
        
        private  HashSet<BoxShape> mBoxShapes;
        private HashSet<CapsuleShape> mCapsuleShapes;
        private HashSet<SphereShape> mSphereShapes;
        private HashSet<PhysicsWorld> mPhysicsWorlds;
        
        
        public HalfEdgeStructure mTriangleShapeHalfEdgeStructure;



        public PhysicsCommon()
        {
            mBoxShapes = new HashSet<BoxShape>();
            mCapsuleShapes = new HashSet<CapsuleShape>();
            mSphereShapes = new HashSet<SphereShape>();
            mPhysicsWorlds = new HashSet<PhysicsWorld>();
            mTriangleShapeHalfEdgeStructure = new HalfEdgeStructure(2,3,6);
            mBoxShapeHalfEdgeStructure = new HalfEdgeStructure(6,8,24);
            initBoxShapeHalfEdgeStructure();
            initTriangleShapeHalfEdgeStructure();
        }


        
        

        public PhysicsWorld createPhysicsWorld(WorldSettings worldSettings)
        {
            var world = new PhysicsWorld(this,worldSettings);
            mPhysicsWorlds.Add(world);
            return world;
        }

        public void destroyPhysicsWorld(PhysicsWorld world)
        {
            mPhysicsWorlds.Remove(world);
        }
        
        public SphereShape createSphereShape(float radius)
        {
            var shape = new SphereShape(radius);
            mSphereShapes.Add(shape);
            return shape;
        }
        
        public void destroySphereShape(SphereShape sphereShape) {
            mSphereShapes.Remove(sphereShape);
        }


        public BoxShape createBoxShape(PVector3 halfExtents)
        {
            var shape = new BoxShape(halfExtents,this);
            mBoxShapes.Add(shape);
            return shape;
        }

        public void destroyBoxShape(BoxShape boxShape)
        {
            mBoxShapes.Remove(boxShape);
        }

        public CapsuleShape createCapsuleShape(float radius, float height)
        {
            CapsuleShape shape = new CapsuleShape(radius, height);
            mCapsuleShapes.Add(shape);
            return shape;
        }
        
        public void destroyCapsuleShape(CapsuleShape capsuleShape) {
            mCapsuleShapes.Remove(capsuleShape);
        }
        
        
        
        // Initialize the static half-edge structure of a BoxShape
        void initBoxShapeHalfEdgeStructure() {

            // Vertices
            mBoxShapeHalfEdgeStructure.addVertex(0);
            mBoxShapeHalfEdgeStructure.addVertex(1);
            mBoxShapeHalfEdgeStructure.addVertex(2);
            mBoxShapeHalfEdgeStructure.addVertex(3);
            mBoxShapeHalfEdgeStructure.addVertex(4);
            mBoxShapeHalfEdgeStructure.addVertex(5);
            mBoxShapeHalfEdgeStructure.addVertex(6);
            mBoxShapeHalfEdgeStructure.addVertex(7);


            // Faces
            var face0 = new List<int>{0,1,2,3};
            var face1 = new List<int>{1,5,6,2};
            var face2 = new List<int>{4,7,6,5};
            var face3 = new List<int>{4,0,3,7};
            var face4 = new List<int>{4,5,1,0};
            var face5 = new List<int>{2,6,7,3};

            mBoxShapeHalfEdgeStructure.addFace(face0);
            mBoxShapeHalfEdgeStructure.addFace(face1);
            mBoxShapeHalfEdgeStructure.addFace(face2);
            mBoxShapeHalfEdgeStructure.addFace(face3);
            mBoxShapeHalfEdgeStructure.addFace(face4);
            mBoxShapeHalfEdgeStructure.addFace(face5);

            mBoxShapeHalfEdgeStructure.init();
        }
        
        
        // Initialize the static half-edge structure of a TriangleShape
        void initTriangleShapeHalfEdgeStructure() {

            // Vertices
            mTriangleShapeHalfEdgeStructure.addVertex(0);
            mTriangleShapeHalfEdgeStructure.addVertex(1);
            mTriangleShapeHalfEdgeStructure.addVertex(2);

            // Faces
            var face0 = new List<int>(3){0,1,2};
            var face1 = new List<int>(3){0,2,1};

            mTriangleShapeHalfEdgeStructure.addFace(face0);
            mTriangleShapeHalfEdgeStructure.addFace(face1);

            mTriangleShapeHalfEdgeStructure.init();
        }

    }
}