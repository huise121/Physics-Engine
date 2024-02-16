using System.Collections.Generic;

namespace RP3D
{
    public class NarrowPhaseInfo
    {
        ///     与之进行碰撞检测的第一个碰撞体的实体
        public Entity colliderEntity1;
        ///     与之进行碰撞检测的第二个碰撞体的实体
        public Entity colliderEntity2;
        ///     指向进行碰撞检测的第一个碰撞体的指针
        public CollisionShape collisionShape1;
        ///     指向进行碰撞检测的第二个碰撞体的指针
        public CollisionShape collisionShape2;
        ///     在狭相交期间创建的接触点数组
        public ContactPointInfo[] contactPoints;
        ///     狭相交碰撞检测测试的结果
        public bool isColliding;
        ///     上一帧碰撞的信息
        public LastFrameCollisionInfo lastFrameCollisionInfo;
        ///     接触点的数量
        public byte nbContactPoints;
        ///     广相交重叠对的 ID
        public int overlappingPairId;
        ///     如果需要报告接触点（例如用于触发器的情况下为 false）
        public bool reportContacts;
        ///     球体1的局部到世界变换
        public PTransform shape1ToWorldTransform;
        ///     球体2的局部到世界变换
        public PTransform shape2ToWorldTransform;
        

        ///     构造函数
        public NarrowPhaseInfo(int pairId, Entity collider1, Entity collider2, LastFrameCollisionInfo lastFrameInfo,
            PTransform shape1ToWorldTransform, PTransform shape2ToWorldTransform, CollisionShape shape1,
            CollisionShape shape2, bool needToReportContacts)
        {
            contactPoints = new ContactPointInfo[16];
            overlappingPairId = pairId;
            colliderEntity1 = collider1;
            colliderEntity2 = collider2;
            lastFrameCollisionInfo = lastFrameInfo;
            this.shape1ToWorldTransform = shape1ToWorldTransform;
            this.shape2ToWorldTransform = shape2ToWorldTransform;
            collisionShape1 = shape1;
            collisionShape2 = shape2;
            reportContacts = needToReportContacts;
            isColliding = false;
            nbContactPoints = 0;
        }
    }


    /// <summary>
    ///     NarrowPhaseInfoBatch 是可能表示一个批处理（batch）的类，用于管理一组窄相交检测（Narrow Phase）的信息。
    ///     窄相交检测是物理引擎中用于检测两个具体形状之间是否发生碰撞的阶段。
    ///     一般而言，窄相交检测是碰撞检测的一个阶段，其目的是在广相交（Broad Phase）之后进一步检查可能发生碰撞的形状对。
    ///     这个阶段通常会检测两个形状之间的具体碰撞信息，例如碰撞点、碰撞法线、碰撞深度等。
    /// </summary>
    public class NarrowPhaseInfoBatch
    {

        //protected OverlappingPairs mOverlappingPairs;
        public List<NarrowPhaseInfo> narrowPhaseInfos;

        public NarrowPhaseInfoBatch(/*OverlappingPairs overlappingPairs*/)
        {
            narrowPhaseInfos = new List<NarrowPhaseInfo>();
            //mOverlappingPairs = overlappingPairs;
        }


        ///     返回批处理中的对象数
        public int GetNbObjects()
        {
            return narrowPhaseInfos.Count;
        }


        // 将要在狭相交碰撞检测期间测试的形状添加到批处理中
        public void AddNarrowPhaseInfo(int pairId, Entity collider1, Entity collider2, CollisionShape shape1,
            CollisionShape shape2, PTransform shape1Transform, PTransform shape2Transform,
            bool needToReportContacts, LastFrameCollisionInfo lastFrameInfo)
        {
            var narrowPhaseInfo = new NarrowPhaseInfo(pairId, collider1, collider2, lastFrameInfo,
                shape1Transform, shape2Transform, shape1,
                shape2,
                needToReportContacts);

            narrowPhaseInfos.Add(narrowPhaseInfo);
        }

        // 添加一个新的接触点
        public void AddContactPoint(int index, PVector3 contactNormal, float penDepth, PVector3 localPt1,
            PVector3 localPt2)
        {
            var narrowPhaseInfo = narrowPhaseInfos[index];
            if (narrowPhaseInfo.nbContactPoints < 16)
            {
                byte nbContactPoints = narrowPhaseInfo.nbContactPoints;
                // 将其添加到接触点数组中
                narrowPhaseInfo.contactPoints[nbContactPoints].normal = contactNormal;
                narrowPhaseInfo.contactPoints[nbContactPoints].penetrationDepth = penDepth;
                narrowPhaseInfo.contactPoints[nbContactPoints].localPoint1 = localPt1;
                narrowPhaseInfo.contactPoints[nbContactPoints].localPoint2 = localPt2;
                narrowPhaseInfo.nbContactPoints++;
            }
        }


        public void clear()
        {
            // 注意，我们清空了以下容器并释放了它们分配的内存。
            // 因此，如果内存分配器是单帧分配器，那么内存将被释放，
            // 并且可能会在下一帧重新分配到内存中的不同位置（请记住，单帧分配器分配的内存位置可能在两帧之间发生变化）
            narrowPhaseInfos.Clear();
        }

        // 重置剩余的接触点
        public void ResetContactPoints(int index)
        {
            var narrowPhaseInfo = narrowPhaseInfos[index];
            narrowPhaseInfo.nbContactPoints = 0;
        }

        public void reserveMemory()
        {
            narrowPhaseInfos.Clear();
        }
    }
}