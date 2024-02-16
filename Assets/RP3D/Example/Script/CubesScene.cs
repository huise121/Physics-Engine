using RP3D;
using UnityEngine;

public class CubesScene : MonoBehaviour
{
    public int MAXNUM = 1;

    public GameObject floorPrefab;
    public GameObject shapePrefab;

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
        Instantiate(floorPrefab);
        var radius = 2.0f;
        for (var i = 0; i < MAXNUM; i++)
        {
            var angle = i * 30.0f;
            var position = new Vector3(radius * Mathf.Cos(angle), 10 + i * (2 + 0.3f),0);
            Instantiate(shapePrefab,position,Quaternion.identity);
        }
    }
}