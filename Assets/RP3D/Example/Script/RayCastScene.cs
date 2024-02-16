using RP3D;
using UnityEngine;

public class RayCastScene : MonoBehaviour
{

    [Space(10)] [Header("Box")]
    public GameObject boxPrefab;

    [Space(10)] [Header("Sphere")]
    public GameObject spherePrefab;
    
    [Space(10)] [Header("Capsule")]
    public GameObject capsulePrefab;
    
    void Start()
    {
        var mWorldSettings = new WorldSettings();
        mWorldSettings.gravity = new PVector3(0, -9.81f, 0);
        mWorldSettings.persistentContactDistanceThreshold = 0.03f;
        mWorldSettings.defaultFrictionCoefficient = 0.3f;
        mWorldSettings.defaultBounciness = 0.5f;
        mWorldSettings.restitutionVelocityThreshold = 0.5f;
        mWorldSettings.isSleepingEnabled = true;
        mWorldSettings.defaultVelocitySolverNbIterations = 6;
        mWorldSettings.defaultPositionSolverNbIterations = 3;
        mWorldSettings.defaultTimeBeforeSleep = 1.0f;
        mWorldSettings.defaultSleepLinearVelocity = 0.02f;
        mWorldSettings.defaultSleepAngularVelocity = 3.0f * (PMath.PI_RP3D / 180.0f);
        mWorldSettings.cosAngleSimilarContactManifold = 0.95f;
        createPhysicsWorld();
    }

    private void createPhysicsWorld()
    {
        BaseCollider mBox = GameObject.Instantiate(boxPrefab,new Vector3(0,0,0),Quaternion.identity).GetComponent<BaseCollider>();
        mBox.createRigidBody = false;
        
        BaseCollider mSphere = GameObject.Instantiate(spherePrefab,new Vector3(3,0,0),Quaternion.identity).GetComponent<BaseCollider>();
        mSphere.createRigidBody = false;

        BaseCollider mCapsule = GameObject.Instantiate(capsulePrefab,new Vector3(-3.0f,0,0),Quaternion.identity).GetComponent<BaseCollider>();
        mCapsule.createRigidBody = false;


    }
}
