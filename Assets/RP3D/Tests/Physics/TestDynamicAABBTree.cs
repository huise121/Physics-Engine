using System.Collections;
using System.Collections.Generic;
using RP3D;
using UnityEngine;


class  DynamicTreeRaycastCallback:DynamicAABBTreeRaycastCallback
{
   List<int> mHitNodes = new List<int>();
   
   
   // Called when the AABB of a leaf node is hit by a ray
   public override float raycastBroadPhaseShape(int nodeId, PRay ray)  {
       mHitNodes.Add(nodeId);
       return 1.0f;
   }

   public void reset() {
        mHitNodes.Clear();
    }
   public bool isHit(int nodeId)  {
        return mHitNodes.Contains(nodeId);
    }
}

public class TestDynamicAABBTree : MonoBehaviour
{
    DynamicTreeRaycastCallback mRaycastCallback;

    // Start is called before the first frame update
    void Start()
    {
        //testOverlapping();

        testRaycast();
    }


    void testBasicsMethods()
    {   
        DynamicAABBTree tree = new DynamicAABBTree(0.0f);
        
        RP3D.Collider object1Data = new RP3D.Collider(new Entity(56,1),null);
        RP3D.Collider object2Data = new RP3D.Collider(new Entity(23,1),null);
        RP3D.Collider object3Data = new RP3D.Collider(new Entity(13,1),null);
        RP3D.Collider object4Data = new RP3D.Collider(new Entity(7,1),null);
        
        
        AABB aabb1 = new  AABB(new PVector3(-6, 4, -3),new PVector3(4, 8, 3));
        int object1Id = tree.addObject(aabb1, object1Data);
        
        // Second object
        AABB aabb2 =new AABB(new PVector3(5, 2, -3),new PVector3(10, 7, 3));
        int object2Id = tree.addObject(aabb2, object2Data);

        // Third object
        AABB aabb3 =new AABB(new PVector3(-5, 1, -3),new PVector3(-2, 3, 3));
        int object3Id = tree.addObject(aabb3, object3Data);

        // Fourth object
        AABB aabb4 =new AABB(new PVector3(0, -4, -3),new PVector3(3, -2, 3));
        int object4Id = tree.addObject(aabb4, object4Data);

        // ----------- Tests ----------- //

        // Test root AABB
        AABB rootAABB = tree.getRootAABB();
        UnityEngine.Debug.Log(rootAABB.GetMin().x == -6);
        UnityEngine.Debug.Log(rootAABB.GetMin().y == -4);
        UnityEngine.Debug.Log(rootAABB.GetMin().z == -3);
        UnityEngine.Debug.Log(rootAABB.GetMax().x == 10);
        UnityEngine.Debug.Log(rootAABB.GetMax().y == 8);
        UnityEngine.Debug.Log(rootAABB.GetMax().z == 3);

        // Test data stored at the nodes of the tree
        UnityEngine.Debug.Log(tree.getNodeDataPointer(object1Id) == object1Data);
        UnityEngine.Debug.Log(tree.getNodeDataPointer(object2Id) == object2Data);
        UnityEngine.Debug.Log(tree.getNodeDataPointer(object3Id) == object3Data);
        UnityEngine.Debug.Log(tree.getNodeDataPointer(object4Id) == object4Data);
    }


    void testOverlapping()
    {
        DynamicAABBTree tree = new DynamicAABBTree(0.0f);

        RP3D.Collider object1Data = new RP3D.Collider(new Entity(56,1),null);
        RP3D.Collider object2Data = new RP3D.Collider(new Entity(23,1),null);
        RP3D.Collider object3Data = new RP3D.Collider(new Entity(13,1),null);
        RP3D.Collider object4Data = new RP3D.Collider(new Entity(7,1),null);
        
        // First object
        AABB aabb1 = new AABB(new PVector3(-6, 4, -3),new PVector3(4, 8, 3));
        int object1Id = tree.addObject(aabb1, object1Data);

        // Second object
        AABB aabb2 = new AABB(new PVector3(5, 2, -3),new PVector3(10, 7, 3));
        int object2Id = tree.addObject(aabb2, object2Data);

        // Third object
        AABB aabb3 = new AABB(new PVector3(-5, 1, -3),new PVector3(-2, 3, 3));
        int object3Id = tree.addObject(aabb3, object3Data);

        // Fourth object
        AABB aabb4 = new AABB(new PVector3(0, -4, -3),new PVector3(3, -2, 3));
        int object4Id = tree.addObject(aabb4, object4Data);


        List<int> overlappingNodes = new List<int>();
        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-10, 12, -4),new PVector3(10, 50, 4)),ref overlappingNodes);
        UnityEngine.Debug.Log(!IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object4Id, overlappingNodes));
        

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-15, -15, -4),new PVector3(15, 15, 4)),ref overlappingNodes);
        UnityEngine.Debug.Log(IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object4Id, overlappingNodes));

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-4, 2, -4),new PVector3(-1, 7, 4)),ref overlappingNodes);
        UnityEngine.Debug.Log(IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object4Id, overlappingNodes));

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-6, -5, -2),new PVector3(2, 2, 0)),ref overlappingNodes);
        UnityEngine.Debug.Log(!IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object4Id, overlappingNodes));

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(5, -10, -2),new PVector3(7, 10, 9)),ref overlappingNodes);
        UnityEngine.Debug.Log(!IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object4Id, overlappingNodes));
        
        
        tree.updateObject(object1Id, aabb1);
        tree.updateObject(object2Id, aabb2);
        tree.updateObject(object3Id, aabb3);
        tree.updateObject(object4Id, aabb4);

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-10, 12, -4),new PVector3(10, 50, 4)),ref overlappingNodes);
        UnityEngine.Debug.Log(!IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object4Id, overlappingNodes));

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-15, -15, -4),new PVector3(15, 15, 4)),ref overlappingNodes);
        UnityEngine.Debug.Log(IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object4Id, overlappingNodes));

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-4, 2, -4),new PVector3(-1, 7, 4)),ref overlappingNodes);
        UnityEngine.Debug.Log(IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object4Id, overlappingNodes));

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(-6, -5, -2),new PVector3(2, 2, 0)),ref overlappingNodes);
        UnityEngine.Debug.Log(!IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object4Id, overlappingNodes));

        overlappingNodes.Clear();
        tree.ReportAllShapesOverlappingWithAABB(new AABB(new PVector3(5, -10, -2),new PVector3(7, 10, 9)),ref overlappingNodes);
        UnityEngine.Debug.Log(!IsOverlapping(object1Id, overlappingNodes));
        UnityEngine.Debug.Log(IsOverlapping(object2Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object3Id, overlappingNodes));
        UnityEngine.Debug.Log(!IsOverlapping(object4Id, overlappingNodes));
        
  
        
        tree.updateObject(object1Id, aabb1);
        tree.updateObject(object2Id, aabb2);
        tree.updateObject(object3Id, aabb3);
        tree.updateObject(object4Id, aabb4);


    }

    void testRaycast()
    {
        mRaycastCallback = new DynamicTreeRaycastCallback();
        
        DynamicAABBTree tree = new DynamicAABBTree(0.0f);
        RP3D.Collider object1Data = new RP3D.Collider(new Entity(56,1),null);
        RP3D.Collider object2Data = new RP3D.Collider(new Entity(23,1),null);
        RP3D.Collider object3Data = new RP3D.Collider(new Entity(13,1),null);
        RP3D.Collider object4Data = new RP3D.Collider(new Entity(7,1),null);
        
            // First object
            AABB aabb1 = new AABB(new PVector3(-6, 4, -3),new PVector3(4, 8, 3));
            int object1Id = tree.addObject(aabb1, object1Data);

            // Second object
            AABB aabb2 = new AABB(new PVector3(5, 2, -3),new PVector3(10, 7, 3));
            int object2Id = tree.addObject(aabb2, object2Data);

            // Third object
            AABB aabb3 = new AABB(new PVector3(-5, 1, -3),new PVector3(-2, 3, 3));
            int object3Id = tree.addObject(aabb3, object3Data);

            // Fourth object
            AABB aabb4 = new AABB(new PVector3(0, -4, -3),new PVector3(3, -2, 3));
            int object4Id = tree.addObject(aabb4, object4Data);

            // ---------- Tests ---------- //

            // Ray with no hits
            mRaycastCallback.reset();
            PRay ray1 = new PRay(new PVector3(4.5f, -10, -5), new PVector3(4.5f, 10, -5));
            tree.raycast(ray1, mRaycastCallback);
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 1
            mRaycastCallback.reset();
            PRay ray2 = new PRay(new PVector3(-1, -20, -2), new PVector3(-1, 20, -2));
            tree.raycast(ray2, mRaycastCallback);
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 1 and 2
            mRaycastCallback.reset();
            PRay ray3 = new PRay(new PVector3(-7, 6, -2), new PVector3(8, 6, -2));
            tree.raycast(ray3, mRaycastCallback);
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 3
            mRaycastCallback.reset();
            PRay ray4 = new PRay(new PVector3(-7, 2, 0), new PVector3(-1, 2, 0));
            tree.raycast(ray4, mRaycastCallback);
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // ---- Update the object AABBs with the initial AABBs (no reinsertion) ----- //

            tree.updateObject(object1Id, aabb1);
            tree.updateObject(object2Id, aabb2);
            tree.updateObject(object3Id, aabb3);
            tree.updateObject(object4Id, aabb4);

            // Ray with no hits
            mRaycastCallback.reset();
            tree.raycast(ray1, mRaycastCallback);
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 1
            mRaycastCallback.reset();
            tree.raycast(ray2, mRaycastCallback);
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 1 and 2
            mRaycastCallback.reset();
            tree.raycast(ray3, mRaycastCallback);
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 3
            mRaycastCallback.reset();
            tree.raycast(ray4, mRaycastCallback);
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // ---- Update the object AABBs with the initial AABBs (with reinsertion) ----- //

            tree.updateObject(object1Id, aabb1);
            tree.updateObject(object2Id, aabb2);
            tree.updateObject(object3Id, aabb3);
            tree.updateObject(object4Id, aabb4);

            // Ray with no hits
            mRaycastCallback.reset();
            tree.raycast(ray1, mRaycastCallback);
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 1
            mRaycastCallback.reset();
            tree.raycast(ray2, mRaycastCallback);
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 1 and 2
            mRaycastCallback.reset();
            tree.raycast(ray3, mRaycastCallback);
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 3
            mRaycastCallback.reset();
            tree.raycast(ray4, mRaycastCallback);
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // ---- Move objects 2 and 3 ----- //

            AABB newAABB2 = new AABB(new PVector3(-7, 10, -3),new PVector3(1, 13, 3));
            tree.updateObject(object2Id, newAABB2);

            AABB newAABB3 = new AABB(new PVector3(7, -6, -3),new PVector3(9, 1, 3));
            tree.updateObject(object3Id, newAABB3);

            // Ray that hits object 1, 2
            PRay ray5 = new PRay(new PVector3(-4, -5, 0),new PVector3(-4, 12, 0));
            mRaycastCallback.reset();
            tree.raycast(ray5, mRaycastCallback);
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object4Id));

            // Ray that hits object 3 and 4
            PRay ray6 = new PRay(new PVector3(11, -3, 1), new PVector3(-2, -3, 1));
            mRaycastCallback.reset();
            tree.raycast(ray6, mRaycastCallback);
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object1Id));
            UnityEngine.Debug.Log(!mRaycastCallback.isHit(object2Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object3Id));
            UnityEngine.Debug.Log(mRaycastCallback.isHit(object4Id));
    }
    
    
     bool IsOverlapping(int nodeId, List<int> overlappingNodes)
    {
        return overlappingNodes.Contains(nodeId);
    }

}
