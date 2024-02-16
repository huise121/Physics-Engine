using RP3D;
using UnityEngine;

public class CapsuleBoxSphereScene : MonoBehaviour
{
    [Header("Floor")]
    public GameObject floorPrefab;

    [Space(10)] [Header("Box")]
    public GameObject boxPrefab;
    public int MAXCUBE = 1;

    [Space(10)] [Header("Sphere")]
    public GameObject spherePrefab;
    public int MAXSphere = 1;
    
    [Space(10)] [Header("Capsule")]
    public GameObject capsulePrefab;
    public int MAXCapsule = 1;


    private void Start()
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
        var radius = 2.0f;
        var j = 0;
        
        Instantiate(floorPrefab);
        
        for (var i = 0; i < MAXCUBE; i++)
        {
            var angle = j * 30.0f;
            var position = new Vector3(radius * Mathf.Cos(angle), 10 + j * (2 + 0.3f), 0);
            Instantiate(boxPrefab, position, Quaternion.identity);
            j++;
        }

        for (var i = 0; i < MAXSphere; i++)
        {
            var angle = j * 30.0f;
            var position = new Vector3(radius * Mathf.Cos(angle), 10 + j * (2 + 0.3f), 0);
            Instantiate(spherePrefab, position, Quaternion.identity);
            j++;
        }

        for (var i = 0; i < MAXCapsule; i++)
        {
            var angle = j * 30.0f;
            var position = new Vector3(radius * Mathf.Cos(angle), 10 + j * (2 + 0.3f), 0);
            Instantiate(capsulePrefab, position, Quaternion.identity);
            j++;
        }
    }
}