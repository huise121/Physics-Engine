using NUnit.Framework;

public class PhysicsWorldTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestPointInside()
    {
        // PhysicsCommon mPhysicsCommon = new PhysicsCommon();
        // PhysicsWorld mWorld = mPhysicsCommon.createPhysicsWorld();
        //
        // BoxShape mBoxShape = mPhysicsCommon.createBoxShape(new PVector3(1, 1, 1));
        // CapsuleShape mCapsuleShape  = mPhysicsCommon.createCapsuleShape(1,2);
        // SphereShape mSphereShape  = mPhysicsCommon.createSphereShape(1);
        //
        // PVector3 p2 = new PVector3(0, 0, 0);
        // PVector3 p1 = new PVector3(5, 5, 5);
        // PRay pRay = new PRay(p1, p2, 1);
        //
        //
        // PTransform pTransform = new PTransform();
        //
        // RigidBody mRigidBody2Box = mWorld.createRigidBody(pTransform);
        // mRigidBody2Box.addCollider(mBoxShape, pTransform);
        // mRigidBody2Box.addCollider(mCapsuleShape, pTransform);
        // mRigidBody2Box.addCollider(mSphereShape, pTransform);
        //     
        // int conut = mRigidBody2Box.getNbColliders();
        //
        // Stopwatch stopwatch = new Stopwatch();
        // stopwatch.Start();
        //
        // for (var i = 0; i < conut; i++)
        // {
        //     Collider collider = mRigidBody2Box.getCollider(i);
        //     var ishit = collider.Raycast(pRay, out var raycastInfo);
        //     var isinside = collider.testPointInside(new PVector3(0.0f, 0.0f, 0.0f));
        //     Debug.Log("ishit:" + ishit +" "+ isinside);
        // }
        //
        // stopwatch.Stop();
        // Debug.Log($"程序执行时间: {stopwatch.Elapsed}");
    }

    /*// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        Debug.Log("NewTestScriptWithEnumeratorPasses");
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }*/
}