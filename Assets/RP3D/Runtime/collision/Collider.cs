using System.Runtime.CompilerServices;

namespace RP3D
{
    public class Collider
    {
        public Entity mEntity;
        public CollisionBody mBody;

        public Collider(Entity entity, CollisionBody body)
        {
            mEntity = entity;
            mBody = body;
        }
        
        public AABB GetWorldAABB()
        {
            var collisionShape = mBody.mWorld.mCollidersComponents.getCollisionShape(mEntity);
            collisionShape.ComputeAABB(out var aabb, GetLocalToWorldTransform());
            return aabb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CollisionShape getCollisionShape()
        {
            return mBody.mWorld.mCollidersComponents.getCollisionShape(mEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int getBroadPhaseId()
        {
            return mBody.mWorld.mCollidersComponents.getBroadPhaseId(mEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PTransform getLocalToBodyTransform()
        {
            return mBody.mWorld.mCollidersComponents.getLocalToBodyTransform(mEntity);
        }

        /// <summary>
        /// 使用射线对碰撞体进行光线投射，返回是否有击中和击中信息。
        /// </summary>
        /// <param name="pRay">进行光线投射的射线</param>
        /// <param name="raycastInfo">光线投射的结果信息</param>
        /// <returns>如果有击中则返回true，否则返回false</returns>
        public bool Raycast(PRay pRay, out RaycastInfo raycastInfo)
        {
            // 初始化光线投射信息
            raycastInfo = new RaycastInfo();

            // 如果相应的刚体不活跃，它不能被光线击中
            if (!mBody.isActive())
                return false;

            // 获取碰撞体

            // 获取局部到世界变换
            var localToWorldTransform = mBody.mWorld.mCollidersComponents.getLocalToWorldTransform(mEntity);

            // 将射线转换为碰撞体的局部空间
            var worldToLocalTransform = localToWorldTransform.GetInverse();
            var rayLocal = new PRay(worldToLocalTransform * pRay.point1, worldToLocalTransform * pRay.point2, pRay.maxFraction);

            CollisionShape collisionShape = mBody.mWorld.mCollidersComponents.getCollisionShape(mEntity);

            // 进行碰撞体的光线投射
            var isHit = collisionShape.Raycast(rayLocal, out raycastInfo, this);

            if (isHit)
            {
                // 将光线投射信息转换回世界空间
                raycastInfo.worldPoint = localToWorldTransform * raycastInfo.worldPoint;
                raycastInfo.worldNormal = localToWorldTransform.GetOrientation() * raycastInfo.worldNormal;
                raycastInfo.worldNormal.Normalize();
            }

            return isHit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool testAABBOverlap(AABB worldAABB)
        {
            return worldAABB.TestCollision(GetWorldAABB());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool testPointInside(PVector3 worldPoint) {
             PTransform localToWorld = mBody.mWorld.mTransformComponents.getTransform(mBody.getEntity()) *
                                           mBody.mWorld.mCollidersComponents.getLocalToBodyTransform(mEntity);
             PVector3 localPoint = localToWorld.GetInverse() * worldPoint;
             CollisionShape collisionShape = mBody.mWorld.mCollidersComponents.getCollisionShape(mEntity);
            return collisionShape.testPointInside(localPoint, this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity GetEntity()
        {
            return mEntity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CollisionBody GetBody()
        {
            return mBody;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PTransform GetLocalToWorldTransform()
        {
            return mBody.mWorld.mCollidersComponents.getLocalToWorldTransform(mEntity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool getIsTrigger()  {
            return mBody.mWorld.mCollidersComponents.getIsTrigger(mEntity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setIsTrigger(bool isTrigger)  {
            mBody.mWorld.mCollidersComponents.setIsTrigger(mEntity, isTrigger);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short getCollisionCategoryBits()  {
            return mBody.mWorld.mCollidersComponents.getCollisionCategoryBits(mEntity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setCollisionCategoryBits( short collisionCategoryBits) {
            mBody.mWorld.mCollidersComponents.setCollisionCategoryBits(mEntity, collisionCategoryBits);
            int broadPhaseId = mBody.mWorld.mCollidersComponents.getBroadPhaseId(mEntity);
            // Ask the broad-phase collision detection to test this collider next frame
            mBody.mWorld.mCollisionDetection.askForBroadPhaseCollisionCheck(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short getCollideWithMaskBits()  {
            return mBody.mWorld.mCollidersComponents.getCollideWithMaskBits(mEntity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setCollideWithMaskBits( short collideWithMaskBits) {
            mBody.mWorld.mCollidersComponents.setCollideWithMaskBits(mEntity, collideWithMaskBits);
            int broadPhaseId = mBody.mWorld.mCollidersComponents.getBroadPhaseId(mEntity);
            // Ask the broad-phase collision detection to test this collider next frame
            mBody.mWorld.mCollisionDetection.askForBroadPhaseCollisionCheck(this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Material getMaterial() {
            return mBody.mWorld.mCollidersComponents.getMaterial(mEntity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setMaterial(Material material) {
            mBody.mWorld.mCollidersComponents.setMaterial(mEntity, material);
        }
        
    }
}