namespace RP3D
{
    public partial class PhysicsWorld
    {
        public void setBodyDisabled(Entity bodyEntity, bool isDisabled)
        {
            if (isDisabled == mCollisionBodyComponents.getIsEntityDisabled(bodyEntity)) return;
            mCollisionBodyComponents.setIsEntityDisabled(bodyEntity, isDisabled);
            mTransformComponents.setIsEntityDisabled(bodyEntity, isDisabled);
            mRigidBodyComponents.setIsEntityDisabled(bodyEntity, isDisabled);
            var collidersEntities = mCollisionBodyComponents.getColliders(bodyEntity);
            var nbColliderEntities = collidersEntities.Count;
            for (var i = 0; i < nbColliderEntities; i++)
                mCollidersComponents.setIsEntityDisabled(collidersEntities[i], isDisabled);
        }   
        
        /// <summary>
        /// 更新刚体的世界逆惯性张量（world inverse inertia tensors）。
        /// 逆惯性张量是描述刚体旋转惯性的参数，它是一个 3x3 的矩阵，描述了刚体在不同轴向的旋转惯性。
        /// 
        /// 具体而言，更新世界逆惯性张量通常涉及以下步骤：
        /// 迭代所有的刚体。
        /// 对于每个刚体，根据其当前的方向和局部逆惯性张量，计算出世界逆惯性张量。
        /// 将计算得到的世界逆惯性张量更新到刚体的属性中。
        /// 世界逆惯性张量的更新是物理引擎中的一个重要步骤，因为它直接影响了刚体在旋转运动中的行为。
        /// 通过更新世界逆惯性张量，可以确保刚体在旋转过程中的动能和角动量得到正确的处理，从而实现准确的旋转运动仿真。
        /// 总之，这段注释指示着对刚体的世界逆惯性张量进行更新，以确保在物理仿真中准确地模拟刚体的旋转行为。
        /// </summary>
        void updateBodiesInverseWorldInertiaTensors() {

            int nbComponents = mRigidBodyComponents.getNbEnabledComponents();
            for (int i=0; i < nbComponents; i++)
            {
                Entity entity = mRigidBodyComponents.mBodiesEntities[i];
                Matrix3x3 orientation = mTransformComponents.getTransform(entity).GetOrientation().getMatrix();
                //刚体的局部惯性张量的逆矩阵
                PVector3 mInverseInertiaTensorsLocal = mRigidBodyComponents.mInverseInertiaTensorsLocal[i];
                //计算在世界坐标系中的惯性张量的逆矩阵。
                computeWorldInertiaTensorInverse(orientation,mInverseInertiaTensorsLocal ,out var outInverseInertiaTensorWorld);
                mRigidBodyComponents.mInverseInertiaTensorsWorld[i] = outInverseInertiaTensorWorld;
            }
        }
        
        
        /// 计算在世界坐标系中的惯性张量的逆矩阵。
        public static void computeWorldInertiaTensorInverse(Matrix3x3 orientation, PVector3 inverseInertiaTensorLocal,
            out Matrix3x3 outInverseInertiaTensorWorld)
        {
            outInverseInertiaTensorWorld = new Matrix3x3();
            var pVector3 = outInverseInertiaTensorWorld[0];
            pVector3[0] = (float)((double)orientation[0][0] * (double)inverseInertiaTensorLocal.x);
            pVector3[1] = (float)((double)orientation[1][0] * (double)inverseInertiaTensorLocal.x);
            pVector3[2] = (float)((double)orientation[2][0] * (double)inverseInertiaTensorLocal.x);
            outInverseInertiaTensorWorld[0] = pVector3;

            var vector3 = outInverseInertiaTensorWorld[1];
            vector3[0] = (float)((double)orientation[0][1] * (double)inverseInertiaTensorLocal.y);
            vector3[1] = (float)((double)orientation[1][1] * (double)inverseInertiaTensorLocal.y);
            vector3[2] = (float)((double)orientation[2][1] * (double)inverseInertiaTensorLocal.y);
            outInverseInertiaTensorWorld[1] = vector3;

            var pVector4 = outInverseInertiaTensorWorld[2];
            pVector4[0] = (float)((double)orientation[0][2] * (double)inverseInertiaTensorLocal.z);
            pVector4[1] = (float)((double)orientation[1][2] * (double)inverseInertiaTensorLocal.z);
            pVector4[2] = (float)((double)orientation[2][2] * (double)inverseInertiaTensorLocal.z);
            outInverseInertiaTensorWorld[2] = pVector4;

            outInverseInertiaTensorWorld = orientation * outInverseInertiaTensorWorld;
        }
    }
}