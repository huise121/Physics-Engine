using UnityEngine;

namespace RP3D
{
    public partial class PhysicsWorld
    {
        private void updateSleepingBodies(float timeStep)
        {
            var sleepLinearVelocitySquare = mSleepLinearVelocity * mSleepLinearVelocity;
            var sleepAngularVelocitySquare = mSleepAngularVelocity * mSleepAngularVelocity;

            // 对于世界中的每个岛屿
            var nbIslands = mIslands.getNbIslands();
            for (var i = 0; i < nbIslands; i++)
            {
                var minSleepTime = float.MaxValue;

                // 对于岛屿中的每个刚体
                for (var b = 0; b < mIslands.nbBodiesInIsland[i]; b++)
                {
                    var bodyEntity = mIslands.bodyEntities[mIslands.startBodyEntitiesIndex[i] + b];
                    var bodyIndex = mRigidBodyComponents.getEntityIndex(bodyEntity);

                    // 跳过静态刚体
                    if (mRigidBodyComponents.mBodyTypes[bodyIndex] == BodyType.STATIC) continue;

                    // 如果刚体速度足够大以保持清醒状态
                    if (mRigidBodyComponents.mLinearVelocities[bodyIndex].LengthSquare() > sleepLinearVelocitySquare ||
                        mRigidBodyComponents.mAngularVelocities[bodyIndex].LengthSquare() > sleepAngularVelocitySquare ||
                        !mRigidBodyComponents.mIsAllowedToSleep[bodyIndex])
                    {
                        // 重置刚体的睡眠时间
                        mRigidBodyComponents.mSleepTimes[bodyIndex] = 0.0f;
                        minSleepTime = 0.0f;
                    }
                    else
                    {
                        // 如果刚体速度低于睡眠速度阈值

                        // 增加睡眠时间
                        mRigidBodyComponents.mSleepTimes[bodyIndex] += timeStep;
                        if (mRigidBodyComponents.mSleepTimes[bodyIndex] < minSleepTime)
                            minSleepTime = mRigidBodyComponents.mSleepTimes[bodyIndex];
                    }
                }

                // 如果岛屿中所有刚体的速度都低于睡眠速度阈值一段时间，超过变为睡眠刚体所需的时间
                if (minSleepTime >= mTimeBeforeSleep)
                    // 将岛屿中的所有刚体置于睡眠状态
                    for (var b = 0; b < mIslands.nbBodiesInIsland[i]; b++)
                    {
                        var bodyEntity = mIslands.bodyEntities[mIslands.startBodyEntitiesIndex[i] + b];
                        var body = mRigidBodyComponents.GetRigidBody(bodyEntity);
                        body.setIsSleeping(true);
                    }
            }
        }


        public void enableSleeping(bool isSleepingEnabled)
        {
            mIsSleepingEnabled = isSleepingEnabled;
            if (!mIsSleepingEnabled)
                for (var i = 0; i < mRigidBodies.Count; i++)
                    mRigidBodies[i].setIsSleeping(false);
        }
    }
}