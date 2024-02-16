using System.Collections.Generic;

namespace RP3D
{
    public partial class PhysicsWorld
    {
        //使用潜在接触和关节计算岛屿
        /// 我们在这里创建实际联系人之前计算岛屿，因为我们想要所有,同一个岛的接触歧管和接触点
        /// 将其封装在一起，形成歧管和触点的线性阵列，以实现更好的缓存。孤岛是一组具有约束（关节或接触）的孤立刚体
        /// 彼此之间。此方法计算每个时间步长的孤岛，如下所示：对于每个清醒的刚体，我们通过该刚体的约束图运行深度优先搜索（DFS）
        /// （图中节点是实体，边是实体之间的约束）到查找与其连接的所有实体（与共享关节或接触的实体它然后，我们用这组相连的物体创建一个岛。
        private void createIslands()
        {
            // 重置所有刚体和关节的 isAlreadyInIsland 变量
            var nbRigidBodyComponents = mRigidBodyComponents.getNbComponents();
            for (var b = 0; b < nbRigidBodyComponents; b++) mRigidBodyComponents.mIsAlreadyInIsland[b] = false;

            // 创建用于深度优先搜索期间要访问的物体堆栈
            var bodyEntitiesToVisit = new Stack<Entity>();

            // 用于当前岛中添加的静态刚体的数组（用于重置静态刚体的 isAlreadyInIsland 变量）
            var staticBodiesAddedToIsland = new List<Entity>(16);

            var nbTotalManifolds = 0;

            // 对于每个刚体组件
            for (var b = 0; b < mRigidBodyComponents.getNbEnabledComponents(); b++)
            {
                // 如果该刚体已经添加到岛中，则继续下一个刚体
                if (mRigidBodyComponents.mIsAlreadyInIsland[b]) continue;

                // 如果该刚体是静态的，则继续下一个刚体
                if (mRigidBodyComponents.mBodyTypes[b] == BodyType.STATIC) continue;

                // 重置要访问的物体堆栈
                bodyEntitiesToVisit.Clear();

                // 将该刚体添加到要访问的物体堆栈中
                mRigidBodyComponents.mIsAlreadyInIsland[b] = true;
                bodyEntitiesToVisit.Push(mRigidBodyComponents.mBodiesEntities[b]);

                // 创建新的岛
                var islandIndex = mIslands.addIsland(nbTotalManifolds);

                // 当堆栈中仍有物体要访问时
                while (bodyEntitiesToVisit.Count > 0)
                {
                    // 获取要访问的物体实体
                    var bodyToVisitEntity = bodyEntitiesToVisit.Pop();

                    // 将物体添加到岛中
                    mIslands.addBodyToIsland(bodyToVisitEntity);

                    var rigidBodyToVisit = mRigidBodyComponents.GetRigidBody(bodyToVisitEntity);

                    // 如果物体处于睡眠状态，则唤醒它（注意，这个调用可能会改变 mRigidBodyComponents 数组中的物体索引）
                    rigidBodyToVisit.setIsSleeping(false);

                    // 计算数组中的物体索引（注意，由于前面对 rigidBodyToVisit->setIsSleeping(false) 的调用，索引可能已经改变）
                    var bodyToVisitIndex = mRigidBodyComponents.getEntityIndex(bodyToVisitEntity);

                    // 如果当前物体是静态的，则我们不想在该物体上执行 DFS 搜索
                    if (mRigidBodyComponents.mBodyTypes[bodyToVisitIndex] == BodyType.STATIC)
                    {
                        staticBodiesAddedToIsland.Add(bodyToVisitEntity);

                        // 继续下一个物体
                        continue;
                    }

                    // 如果物体与其他物体存在接触
                    // 对于当前物体涉及的每个接触对
                    var nbBodyContactPairs = mRigidBodyComponents.mContactPairs[bodyToVisitIndex].Count;
                    for (var p = 0; p < nbBodyContactPairs; p++)
                    {
                        var contactPairIndex = mRigidBodyComponents.mContactPairs[bodyToVisitIndex][p];
                        var pair = mCollisionDetection.mCurrentContactPairs[contactPairIndex];

                        // 检查当前接触对是否已经添加到岛中
                        if (pair.IsAlreadyInIsland) continue;

                        var otherBodyEntity =
                            pair.Body1Entity == bodyToVisitEntity ? pair.Body2Entity : pair.Body1Entity;

                        // 如果发生碰撞的物体是刚体（而不是碰撞体）且不是触发器
                        int otherBodyIndex;
                        if (mRigidBodyComponents.HasComponentGetIndex(otherBodyEntity, out otherBodyIndex)
                            && !mCollidersComponents.getIsTrigger(pair.Collider1Entity) &&
                            !mCollidersComponents.getIsTrigger(pair.Collider2Entity))
                        {
                            mProcessContactPairsOrderIslands.Add(contactPairIndex);

                            nbTotalManifolds += pair.NbPotentialContactManifolds;

                            // 将接触曲面添加到岛中
                            mIslands.nbContactManifolds[islandIndex] += pair.NbPotentialContactManifolds;
                            pair.IsAlreadyInIsland = true;

                            // 检查其他物体是否已经添加到岛中
                            if (mRigidBodyComponents.mIsAlreadyInIsland[otherBodyIndex]) continue;

                            // 将其他物体插入要访问的物体堆栈中
                            bodyEntitiesToVisit.Push(otherBodyEntity);
                            mRigidBodyComponents.mIsAlreadyInIsland[otherBodyIndex] = true;
                        }
                        else
                        {
                            // 将接触对索引添加到不属于岛的接触对数组中
                            pair.IsAlreadyInIsland = true;
                        }
                    }
                }

                // 重置静态刚体的 isAlreadyIsland 变量，以便它们也可以包含在其他岛中
                var nbStaticBodiesAddedToIsland = staticBodiesAddedToIsland.Count;
                for (var j = 0; j < nbStaticBodiesAddedToIsland; j++)
                    mRigidBodyComponents.SetIsAlreadyInIsland(staticBodiesAddedToIsland[j], false);

                staticBodiesAddedToIsland.Clear();
            }

            // 清除刚体的关联接触对
            var nbRigidBodyEnabledComponents = mRigidBodyComponents.getNbEnabledComponents();
            for (var b = 0; b < nbRigidBodyEnabledComponents; b++) mRigidBodyComponents.mContactPairs[b].Clear();
        }
    }
}