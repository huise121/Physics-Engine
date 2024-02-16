using System;
using System.Collections.Generic;

namespace RP3D
{
    public class EntityManager
    {
        private List<uint> mGenerations;
        private Queue<uint> mFreeIndices;

        public EntityManager()
        {
            mGenerations = new List<uint>();
            mFreeIndices = new Queue<uint>();
        }

        // 创建一个新实体
        public Entity CreateEntity()
        {
            uint index;
            // 如果已经有足够的空闲索引可供使用
            if (mFreeIndices.Count > Entity.MINIMUM_FREE_INDICES)
            {
                // 从空闲索引中回收一个索引
                index = mFreeIndices.Dequeue();
            }
            else
            {
                // 我们从第一代开始
                mGenerations.Add(0);
                // 创建一个新的索引
                index = (uint)mGenerations.Count - 1;
                if (index >= 1u << (int)Entity.ENTITY_INDEX_BITS)
                    throw new InvalidOperationException("索引超过了允许的最大值。");
            }

            // 返回一个新实体
            return new Entity(index, mGenerations[(int)index]);
        }

        // 销毁一个实体
        public void destroyEntity(Entity entity)
        {
            var index = entity.GetIndex();
            // 增加此索引的代数
            mGenerations[(int)index]++;
            // 将索引添加到空闲索引队列中
            mFreeIndices.Enqueue(index);
        }
        
    }
}