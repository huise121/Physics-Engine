using System.Collections.Generic;

namespace RP3D
{
    public abstract class CollisionCallback
    {
        private RP3D.ContactPair mContactPair;

        /// 在 mContactPairs 数组中是接触事件的索引的数组
        private List<int> mContactPairsIndices;

        private List<ContactPoint> mContactPoints;


        /// 如果这是一个失去的接触对（在上一帧中碰撞对发生碰撞，但在当前帧中未发生碰撞）
        private bool mIsLostContactPair;


        private PhysicsWorld mWorld;

        // Abstract method
        public abstract void onContact(CallbackData callbackData);


        // Return a pointer to the first body in contact
        public CollisionBody getBody1()
        {
            return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body1Entity);
        }

        public CollisionBody getBody2()
        {
            return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body2Entity);
        }

        public Collider getCollider1()
        {
            return mWorld.mCollidersComponents.getCollider(mContactPair.Collider1Entity);
        }

        public Collider getCollider2()
        {
            return mWorld.mCollidersComponents.getCollider(mContactPair.Collider2Entity);
        }

        public EventType getEventType()
        {
            if (mIsLostContactPair) return EventType.ContactExit;
            if (mContactPair.CollidingInPreviousFrame) return EventType.ContactStay;
            return EventType.ContactStart;
        }

        public ContactPoint getContactPoint(int index)
        {
            return mContactPoints[mContactPair.ContactPointsIndex + index];
        }

        public class ContactPoint
        {
            private readonly RP3D.ContactPoint mContactPoint;

            /// <summary>
            ///     构造函数
            /// </summary>
            /// <param name="contactPoint">接触点</param>
            public ContactPoint(RP3D.ContactPoint contactPoint)
            {
                mContactPoint = contactPoint;
            }

            /// <summary>
            ///     获取穿透深度
            /// </summary>
            /// <returns>两个碰撞体在此接触点处的穿透深度</returns>
            public float GetPenetrationDepth()
            {
                return mContactPoint.GetPenetrationDepth();
            }

            /// <summary>
            ///     获取世界空间接触法线
            /// </summary>
            /// <returns>世界空间接触法线</returns>
            public PVector3 GetWorldNormal()
            {
                return mContactPoint.GetNormal();
            }

            /// <summary>
            ///     获取第一个碰撞体上的接触点（在第一个碰撞体的局部空间中）
            /// </summary>
            /// <returns>第一个碰撞体上的局部空间接触点</returns>
            public PVector3 GetLocalPointOnCollider1()
            {
                return mContactPoint.GetLocalPointOnShape1();
            }

            /// <summary>
            ///     获取第二个碰撞体上的接触点（在第二个碰撞体的局部空间中）
            /// </summary>
            /// <returns>第二个碰撞体上的局部空间接触点</returns>
            public PVector3 GetLocalPointOnCollider2()
            {
                return mContactPoint.GetLocalPointOnShape2();
            }
        }

        public class ContactPair
        {
            /// <summary>
            ///     事件类型枚举
            /// </summary>
            public enum EventType
            {
                /// <summary>
                ///     此接触是两个碰撞体之间的新接触（在上一帧中，碰撞体之间没有接触）
                /// </summary>
                ContactStart,

                /// <summary>
                ///     两个碰撞体在上一帧中已经接触，这是一个新的或更新的接触
                /// </summary>
                ContactStay,

                /// <summary>
                ///     两个碰撞体在上一帧中接触，但在当前帧中没有接触
                /// </summary>
                ContactExit
            }

            // -------------------- 属性 -------------------- //

            private readonly RP3D.ContactPair mContactPair;

            /// <summary>
            ///     接触点列表的引用
            /// </summary>
            private readonly List<RP3D.ContactPoint> mContactPoints;

            /// <summary>
            ///     如果这是一个丢失的接触对（在上一帧中碰撞但在当前帧中没有）
            /// </summary>
            private readonly bool mIsLostContactPair;

            /// <summary>
            ///     物理世界的引用
            /// </summary>
            private readonly PhysicsWorld mWorld;

            // -------------------- 方法 -------------------- //

            // ContactPair构造函数
            public ContactPair(RP3D.ContactPair contactPair,
                List<RP3D.ContactPoint> contactPoints, PhysicsWorld world, bool isLostContactPair)
            {
                mContactPair = contactPair;
                mContactPoints = contactPoints;
                mWorld = world;
                mIsLostContactPair = isLostContactPair;
            }

            /// <summary>
            ///     返回接触对中接触点的数量
            /// </summary>
            /// <returns>接触对中接触点的数量</returns>
            public int GetNbContactPoints()
            {
                return mContactPair.NbTotalContactPoints;
            }

            /// <summary>
            ///     返回给定索引处的接触点
            /// </summary>
            /// <param name="index">要检索的接触点的索引</param>
            /// <returns>接触点对象</returns>
            public ContactPoint GetContactPoint(int index)
            {
                return new ContactPoint(mContactPoints[mContactPair.ContactPointsIndex + index]);
            }

            /// <summary>
            ///     返回接触对中的第一个碰撞体的指针
            /// </summary>
            /// <returns>接触对中第一个碰撞体的指针</returns>
            public CollisionBody GetBody1()
            {
                return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body1Entity);
            }

            /// <summary>
            ///     返回接触对中的第二个碰撞体的指针
            /// </summary>
            /// <returns>接触对中第二个碰撞体的指针</returns>
            public CollisionBody GetBody2()
            {
                return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body2Entity);
            }

            /// <summary>
            ///     返回接触对中的第一个碰撞体的碰撞器（在第一个碰撞体中）
            /// </summary>
            /// <returns>接触对中的第一个碰撞器</returns>
            public Collider GetCollider1()
            {
                return mWorld.mCollidersComponents.getCollider(mContactPair.Collider1Entity);
            }

            /// <summary>
            ///     返回接触对中的第二个碰撞体的碰撞器（在第二个碰撞体中）
            /// </summary>
            /// <returns>接触对中的第二个碰撞器</returns>
            public Collider GetCollider2()
            {
                return mWorld.mCollidersComponents.getCollider(mContactPair.Collider2Entity);
            }

            /// <summary>
            ///     返回此接触对的相应事件类型
            /// </summary>
            /// <returns>此接触对的接触事件类型</returns>
            public EventType GetEventType()
            {
                if (mIsLostContactPair) return EventType.ContactExit;
                if (mContactPair.CollidingInPreviousFrame) return EventType.ContactStay;
                return EventType.ContactStart;
            }
        }

        public class CallbackData
        {
            /// <summary>
            ///     接触曲面数组的指针
            /// </summary>
            private List<ContactManifold> mContactManifolds;
            // -------------------- 属性 -------------------- //

            /// <summary>
            ///     接触对数组的指针（包含接触和触发事件）
            /// </summary>
            private readonly List<RP3D.ContactPair> mContactPairs;

            /// <summary>
            ///     mContactPairs数组中是接触事件（而不是重叠/触发事件）的索引数组
            /// </summary>
            private readonly List<int> mContactPairsIndices;

            /// <summary>
            ///     接触点数组的指针
            /// </summary>
            private readonly List<RP3D.ContactPoint> mContactPoints;

            /// <summary>
            ///     丢失的接触对数组的指针（包含接触和触发事件）
            /// </summary>
            private readonly List<RP3D.ContactPair> mLostContactPairs;

            /// <summary>
            ///     mLostContactPairs数组中是接触事件（而不是重叠/触发事件）的索引数组
            /// </summary>
            private readonly List<int> mLostContactPairsIndices;

            /// <summary>
            ///     物理世界的引用
            /// </summary>
            private readonly PhysicsWorld mWorld;

            // -------------------- 方法 -------------------- //

            /// <summary>
            ///     构造函数
            /// </summary>
            public CallbackData(List<RP3D.ContactPair> contactPairs, List<ContactManifold> manifolds,
                List<RP3D.ContactPoint> contactPoints, List<RP3D.ContactPair> lostContactPairs,
                PhysicsWorld world)
            {
                mContactPairs = contactPairs;
                mContactManifolds = manifolds;
                mContactPoints = contactPoints;
                mLostContactPairs = lostContactPairs;
                mContactPairsIndices = new List<int>(contactPairs.Count);
                mLostContactPairsIndices = new List<int>(lostContactPairs.Count);
                mWorld = world;
                // 过滤接触对，仅保留接触事件（不包括重叠/触发事件）
                var nbContactPairs = mContactPairs.Count;
                for (var i = 0; i < nbContactPairs; i++)
                    // 如果接触对包含接触（因此不是重叠/触发事件）
                    if (!mContactPairs[i].IsTrigger)
                        mContactPairsIndices.Add(i);
                // 过滤丢失的接触对，仅保留接触事件（不包括重叠/触发事件）
                var nbLostContactPairs = mLostContactPairs.Count;
                for (var i = 0; i < nbLostContactPairs; i++)
                    // 如果接触对包含接触（因此不是重叠/触发事件）
                    if (!mLostContactPairs[i].IsTrigger)
                        mLostContactPairsIndices.Add(i);
            }

            /// <summary>
            ///     返回接触对的数量
            /// </summary>
            /// <returns>接触对的数量</returns>
            public int GetNbContactPairs()
            {
                return mContactPairsIndices.Count + mLostContactPairsIndices.Count;
            }

            /// <summary>
            ///     返回给定索引处的接触对
            /// </summary>
            /// <param name="index">要检索的接触对的索引</param>
            /// <returns>接触对对象</returns>
            public ContactPair GetContactPair(int index)
            {
                if (index < mContactPairsIndices.Count)
                    // 返回一个接触对
                    return new ContactPair(mContactPairs[mContactPairsIndices[index]], mContactPoints, mWorld,
                        false);
                // 返回一个丢失的接触对
                return new ContactPair(
                    mLostContactPairs[mLostContactPairsIndices[index - mContactPairsIndices.Count]], mContactPoints,
                    mWorld,
                    true);
            }
        }
    }
}