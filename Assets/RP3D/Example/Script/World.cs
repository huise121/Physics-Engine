using System.Diagnostics;
using RP3D;
using UnityEngine;

    public class World : MonoBehaviour
    {
        public bool drawBVH;
        
        private EventListener _listener;
        public PhysicsCommon mPhysicsCommon;

        private WorldSettings mSettings;
        public PhysicsWorld mWorld;

        public static World Instance { get; private set; }


        private void Awake()
        {
            mSettings = new WorldSettings();

            mSettings = new WorldSettings();
            mSettings.gravity = new PVector3(0, -9.81000041f, 0);
            mSettings.persistentContactDistanceThreshold = 0.03f;
            mSettings.defaultFrictionCoefficient = 0.3f;
            mSettings.defaultBounciness = 0.0f;
            mSettings.restitutionVelocityThreshold = 0.5f;
            mSettings.isSleepingEnabled = true;
            mSettings.defaultVelocitySolverNbIterations = 6;
            mSettings.defaultPositionSolverNbIterations = 3;
            mSettings.defaultTimeBeforeSleep = 1.0f;
            mSettings.defaultSleepLinearVelocity = 0.02f;
            mSettings.defaultSleepAngularVelocity = 3.0f * (PMath.PI_RP3D / 180.0f);
            mSettings.cosAngleSimilarContactManifold = 0.95f;

            mPhysicsCommon = new PhysicsCommon();
            mWorld = mPhysicsCommon.createPhysicsWorld(mSettings);


            _listener = new EventListener();
            mWorld.setEventListener(_listener);

            Instance = this;
        }

        private void FixedUpdate()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            mWorld.update(Time.fixedDeltaTime);
            stopwatch.Stop();
            double milliseconds = stopwatch.ElapsedMilliseconds;
            //Debug.Log($"程序执行时间: {milliseconds} 毫秒");
        }
        
        
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (World.Instance !=null && World.Instance.mWorld != null)
        {
            if (drawBVH)
                DynamicAABBTree();
        }
    }

    private void DynamicAABBTree()
    {
        DynamicAABBTree mDynamicAABBTree = World.Instance.mWorld.mCollisionDetection.mBroadPhaseSystem.mDynamicAABBTree;
        for (int i = 0; i < mDynamicAABBTree.mNbNodes; i++)
        {
            PVector3 center =  mDynamicAABBTree.GetAABB(i).GetCenter();
            PVector3 extent = mDynamicAABBTree.GetAABB(i).GetExtent();
            Gizmos.DrawWireCube(new Vector3(center.x,center.y,center.z), new Vector3(extent.x,extent.y,extent.z));
        }
    }
#endif
    }
