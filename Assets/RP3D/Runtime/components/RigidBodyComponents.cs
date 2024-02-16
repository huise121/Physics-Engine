using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public enum BodyType
    {
        STATIC,
        KINEMATIC,
        DYNAMIC
    }

    public struct RigidBodyComponent
    {
        public RigidBody body;
        public BodyType bodyType;
        public PVector3 worldPosition;
    }

    public partial class RigidBodyComponents : Components
    {
        public RigidBodyComponents() : base()
        {
            mBodiesEntities = new List<Entity>();
            mRigidBodies = new List<RigidBody>();
            mIsAllowedToSleep = new List<bool>();
            mIsSleeping = new List<bool>();
            mSleepTimes = new List<float>();
            mBodyTypes = new List<BodyType>();
            mLinearVelocities = new List<PVector3>();
            mAngularVelocities = new List<PVector3>();
            mExternalForces = new List<PVector3>();
            mExternalTorques = new List<PVector3>();
            mLinearDampings = new List<float>();
            mAngularDampings = new List<float>();
            mMasses = new List<float>();
            mInverseMasses = new List<float>();
            mLocalInertiaTensors = new List<PVector3>();
            mInverseInertiaTensorsLocal = new List<PVector3>();
            mInverseInertiaTensorsWorld = new List<Matrix3x3>();
            mConstrainedLinearVelocities = new List<PVector3>();
            mConstrainedAngularVelocities = new List<PVector3>();
            mSplitLinearVelocities = new List<PVector3>();
            mSplitAngularVelocities = new List<PVector3>();
            mConstrainedPositions = new List<PVector3>();
            mConstrainedOrientations = new List<PQuaternion>();
            mCentersOfMassLocal = new List<PVector3>();
            mCentersOfMassWorld = new List<PVector3>();
            mIsGravityEnabled = new List<bool>();
            mIsAlreadyInIsland = new List<bool>();
            mJoints = new List<List<Entity>>();
            mContactPairs = new List<List<int>>();
            mLinearLockAxisFactors = new List<PVector3>();
            mAngularLockAxisFactors = new List<PVector3>();
        }

        public void addComponent(Entity bodyEntity, bool isSleeping, RigidBodyComponent component)
        {
            var index = prepareAddComponent(isSleeping);
            mBodiesEntities.Add(bodyEntity);
            mRigidBodies.Add(component.body);
            mIsAllowedToSleep.Add(true);
            mIsSleeping.Add(false);
            mSleepTimes.Add(0);
            mBodyTypes.Add(component.bodyType);
            mLinearVelocities.Add(new PVector3(0, 0, 0));
            mAngularVelocities.Add(new PVector3(0, 0, 0));
            mExternalForces.Add(new PVector3(0, 0, 0));
            mExternalTorques.Add(new PVector3(0, 0, 0));

            mLinearDampings.Add(0.0f);
            mAngularDampings.Add(0.0f);
            mMasses.Add(1.0f);
            mInverseMasses.Add(1.0f);

            mLocalInertiaTensors.Add(new PVector3(1.0f, 1.0f, 1.0f));
            mInverseInertiaTensorsLocal.Add(new PVector3(1.0f, 1.0f, 1.0f));

            mInverseInertiaTensorsWorld.Add(new Matrix3x3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f));
            mConstrainedLinearVelocities.Add(new PVector3(0, 0, 0));
            mConstrainedAngularVelocities.Add(new PVector3(0, 0, 0));
            mSplitLinearVelocities.Add(new PVector3(0, 0, 0));
            mSplitAngularVelocities.Add(new PVector3(0, 0, 0));
            mConstrainedPositions.Add(new PVector3(0, 0, 0));
            mConstrainedOrientations.Add(new PQuaternion(0, 0, 0, 1));
            mCentersOfMassLocal.Add(new PVector3(0, 0, 0));
            mCentersOfMassWorld.Add(component.worldPosition);

            mIsGravityEnabled.Add(true);
            mIsAlreadyInIsland.Add(false);

            mJoints.Add(new List<Entity>());
            mContactPairs.Add(new List<int>());

            mLinearLockAxisFactors.Add(new PVector3(1.0f, 1.0f, 1.0f));
            mAngularLockAxisFactors.Add(new PVector3(1.0f, 1.0f, 1.0f));


            mMapEntityToComponentIndex.Add(bodyEntity, index);
            mNbComponents++;
        }

        protected override void moveComponentToIndex(int srcIndex, int destIndex)
        {
            var entity = mBodiesEntities[srcIndex];


            // Copy the data of the source component to the destination location
            mBodiesEntities[destIndex] = mBodiesEntities[srcIndex];
            mRigidBodies[destIndex] = mRigidBodies[srcIndex];
            mIsAllowedToSleep[destIndex] = mIsAllowedToSleep[srcIndex];
            mIsSleeping[destIndex] = mIsSleeping[srcIndex];
            mSleepTimes[destIndex] = mSleepTimes[srcIndex];
            mBodyTypes[destIndex] = mBodyTypes[srcIndex];
            mLinearVelocities[destIndex] = mLinearVelocities[srcIndex];
            mAngularVelocities[destIndex] = mAngularVelocities[srcIndex];
            mExternalForces[destIndex] = mExternalForces[srcIndex];
            mExternalTorques[destIndex] = mExternalTorques[srcIndex];
            mLinearDampings[destIndex] = mLinearDampings[srcIndex];
            mAngularDampings[destIndex] = mAngularDampings[srcIndex];
            mMasses[destIndex] = mMasses[srcIndex];
            mInverseMasses[destIndex] = mInverseMasses[srcIndex];
            mLocalInertiaTensors[destIndex] = mLocalInertiaTensors[srcIndex];
            mInverseInertiaTensorsLocal[destIndex] = mInverseInertiaTensorsLocal[srcIndex];
            mInverseInertiaTensorsWorld[destIndex] = mInverseInertiaTensorsWorld[srcIndex];
            mConstrainedLinearVelocities[destIndex] = mConstrainedLinearVelocities[srcIndex];
            mConstrainedAngularVelocities[destIndex] = mConstrainedAngularVelocities[srcIndex];
            mSplitLinearVelocities[destIndex] = mSplitLinearVelocities[srcIndex];
            mSplitAngularVelocities[destIndex] = mSplitAngularVelocities[srcIndex];
            mConstrainedPositions[destIndex] = mConstrainedPositions[srcIndex];
            mConstrainedOrientations[destIndex] = mConstrainedOrientations[srcIndex];
            mCentersOfMassLocal[destIndex] = mCentersOfMassLocal[srcIndex];
            mCentersOfMassWorld[destIndex] = mCentersOfMassWorld[srcIndex];
            mIsGravityEnabled[destIndex] = mIsGravityEnabled[srcIndex];
            mIsAlreadyInIsland[destIndex] = mIsAlreadyInIsland[srcIndex];
            mJoints[destIndex] = mJoints[srcIndex];
            mContactPairs[destIndex] = mContactPairs[srcIndex];
            mLinearLockAxisFactors[destIndex] = mLinearLockAxisFactors[srcIndex];
            mAngularLockAxisFactors[destIndex] = mAngularLockAxisFactors[srcIndex];

            // Destroy the source component
            destroyComponent(srcIndex);

            assert(!mMapEntityToComponentIndex.ContainsKey(entity));

            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(entity, destIndex);

            assert(mMapEntityToComponentIndex[mBodiesEntities[destIndex]] == destIndex);
        }

        protected override void swapComponents(int index1, int index2)
        {
            // Copy component 1 data
            var entity1 = mBodiesEntities[index1];
            var body1 = mRigidBodies[index1];
            var isAllowedToSleep1 = mIsAllowedToSleep[index1];
            var isSleeping1 = mIsSleeping[index1];
            var sleepTime1 = mSleepTimes[index1];
            var bodyType1 = mBodyTypes[index1];
            var linearVelocity1 = mLinearVelocities[index1];
            var angularVelocity1 = mAngularVelocities[index1];
            var externalForce1 = mExternalForces[index1];
            var externalTorque1 = mExternalTorques[index1];
            var linearDamping1 = mLinearDampings[index1];
            var angularDamping1 = mAngularDampings[index1];
            var mass1 = mMasses[index1];
            var inverseMass1 = mInverseMasses[index1];
            var inertiaTensorLocal1 = mLocalInertiaTensors[index1];
            var inertiaTensorLocalInverse1 = mInverseInertiaTensorsLocal[index1];
            var inertiaTensorWorldInverse1 = mInverseInertiaTensorsWorld[index1];
            var constrainedLinearVelocity1 = mConstrainedLinearVelocities[index1];
            var constrainedAngularVelocity1 = mConstrainedAngularVelocities[index1];
            var splitLinearVelocity1 = mSplitLinearVelocities[index1];
            var splitAngularVelocity1 = mSplitAngularVelocities[index1];
            var constrainedPosition1 = mConstrainedPositions[index1];
            var constrainedOrientation1 = mConstrainedOrientations[index1];
            var centerOfMassLocal1 = mCentersOfMassLocal[index1];
            var centerOfMassWorld1 = mCentersOfMassWorld[index1];
            var isGravityEnabled1 = mIsGravityEnabled[index1];
            var isAlreadyInIsland1 = mIsAlreadyInIsland[index1];
            var joints1 = mJoints[index1];
            var contactPairs1 = mContactPairs[index1];
            var linearLockAxisFactor1 = mLinearLockAxisFactors[index1];
            var angularLockAxisFactor1 = mAngularLockAxisFactors[index1];

            // Destroy component 1
            destroyComponent(index1);

            moveComponentToIndex(index2, index1);

            // Reconstruct component 1 at component 2 location
            mBodiesEntities[index2] = entity1;
            mRigidBodies[index2] = body1;
            mIsAllowedToSleep[index2] = isAllowedToSleep1;
            mIsSleeping[index2] = isSleeping1;
            mSleepTimes[index2] = sleepTime1;
            mBodyTypes[index2] = bodyType1;
            mLinearVelocities[index2] = linearVelocity1;
            mAngularVelocities[index2] = angularVelocity1;
            mExternalForces[index2] = externalForce1;
            mExternalTorques[index2] = externalTorque1;
            mLinearDampings[index2] = linearDamping1;
            mAngularDampings[index2] = angularDamping1;
            mMasses[index2] = mass1;
            mInverseMasses[index2] = inverseMass1;
            mLocalInertiaTensors[index2] = inertiaTensorLocal1;
            mInverseInertiaTensorsLocal[index2] = inertiaTensorLocalInverse1;
            mInverseInertiaTensorsWorld[index2] = inertiaTensorWorldInverse1;
            mConstrainedLinearVelocities[index2] = constrainedLinearVelocity1;
            mConstrainedAngularVelocities[index2] = constrainedAngularVelocity1;
            mSplitLinearVelocities[index2] = splitLinearVelocity1;
            mSplitAngularVelocities[index2] = splitAngularVelocity1;
            mConstrainedPositions[index2] = constrainedPosition1;
            mConstrainedOrientations[index2] = constrainedOrientation1;
            mCentersOfMassLocal[index2] = centerOfMassLocal1;
            mCentersOfMassWorld[index2] = centerOfMassWorld1;
            mIsGravityEnabled[index2] = isGravityEnabled1;
            mIsAlreadyInIsland[index2] = isAlreadyInIsland1;
            mJoints[index2] = joints1;
            mContactPairs[index2] = contactPairs1;
            mLinearLockAxisFactors[index2] = linearLockAxisFactor1;
            mAngularLockAxisFactors[index2] = angularLockAxisFactor1;

            // Update the entity to component index mapping
            mMapEntityToComponentIndex.Add(entity1, index2);
        }

        public override void destroyComponent(int index)
        {
            base.destroyComponent(index);

            assert(mMapEntityToComponentIndex[mBodiesEntities[index]] == index);

            mMapEntityToComponentIndex.Remove(mBodiesEntities[index]);

            mBodiesEntities[index] = null;
            mRigidBodies[index] = null;
            // mLinearVelocities[index] = null;
            // mAngularVelocities[index] = null;
            // mExternalForces[index] = null;
            // mExternalTorques[index] = null;
            // mLocalInertiaTensors[index] = null;
            // mInverseInertiaTensorsLocal[index] = null;
            // mInverseInertiaTensorsWorld[index] = null;
            // mConstrainedLinearVelocities[index] = null;
            // mConstrainedAngularVelocities[index] = null;
            // mSplitLinearVelocities[index] = null;
            // mSplitAngularVelocities[index] = null;
            // mConstrainedPositions[index] = null;
            // mConstrainedOrientations[index] = null;
            // mCentersOfMassLocal[index] = null;
            // mCentersOfMassWorld[index] = null;
            mJoints[index] = null;
            mContactPairs[index] = null;
            // mLinearLockAxisFactors[index] = null;
            // mAngularLockAxisFactors[index] = null;
        }
    }
}