using System.Collections.Generic;
using UnityEngine;

namespace RP3D
{
    public class Components
    {
        
        protected int INIT_NB_ALLOCATED_COMPONENTS = 10;


        // 组件数量
        protected int mNbComponents;

        // 已分配的组件数量
        private readonly int mNbAllocatedComponents;

        // 实体到组件索引的映射
        public Dictionary<Entity, int> mMapEntityToComponentIndex;

        // 禁用组件的起始索引
        protected int mDisabledStartIndex;

        // 构造函数
        public Components()
        {
            mNbComponents = 0;
            mNbAllocatedComponents = 0;
            mMapEntityToComponentIndex = new Dictionary<Entity, int>();
            mDisabledStartIndex = 0;
        }

        // 析构函数
        ~Components()
        {
            // 如果有分配的组件
            if (mNbAllocatedComponents > 0)
                // 销毁所有剩余的组件
                for (var i = 0; i < mNbComponents; i++)
                    destroyComponent(i);
        }

        // 计算插入新组件的索引
        public int prepareAddComponent(bool isSleeping)
        {
            int index;

            // 如果要添加的组件属于禁用实体或没有禁用实体
            if (isSleeping)
            {
                // 在数组末尾添加组件
                index = mNbComponents;
            }
            // 如果要添加的组件不属于禁用实体
            else
            {
                // 如果已经有禁用的组件
                if (mDisabledStartIndex != mNbComponents)
                    // 将第一个禁用的组件移动到数组末尾
                    moveComponentToIndex(mDisabledStartIndex, mNbComponents);

                index = mDisabledStartIndex;

                mDisabledStartIndex++;
            }

            return index;
        }

        // 销毁给定索引处的组件
        public virtual void destroyComponent(int index)
        {
            assert(index < mNbComponents);
            // 添加适当的销毁组件的代码
        }

        // 移除给定索引处的组件
        public void removeComponent(Entity entity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(entity));

            var index = mMapEntityToComponentIndex[entity];

            assert(index < mNbComponents);

            // 保持数组紧凑。因此，当组件被移除时，将其替换为数组的最后一个元素。
            // 但我们需要确保已启用和已禁用的组件保持分组在一起。

            // 销毁组件
            destroyComponent(index);

            // 如果要删除的组件被禁用
            if (index >= mDisabledStartIndex)
            {
                // 如果组件不是最后一个
                if (index != mNbComponents - 1)
                    // 用最后一个禁用的组件替换它
                    moveComponentToIndex(mNbComponents - 1, index);
            }
            else // 如果要删除的组件已启用
            {
                // 如果它不是最后一个已启用的组件
                if (index != mDisabledStartIndex - 1)
                    // 用最后一个已启用的组件替换它
                    moveComponentToIndex(mDisabledStartIndex - 1, index);

                // 如果末尾有禁用的组件
                if (mDisabledStartIndex != mNbComponents)
                    // 用最后一个禁用的组件替换最后一个已启用的组件
                    moveComponentToIndex(mNbComponents - 1, mDisabledStartIndex - 1);

                mDisabledStartIndex--;
            }

            mNbComponents--;
        }

        // 通知给定实体是否被禁用（休眠）或未禁用
        public void setIsEntityDisabled(Entity entity, bool isDisabled)
        {
            var index = mMapEntityToComponentIndex[entity];

            // 如果组件先前已禁用且现在未禁用
            if (!isDisabled && index >= mDisabledStartIndex)
            {
                assert(mDisabledStartIndex < mNbComponents);

                // 如果禁用的组件不是第一个禁用的组件
                if (mDisabledStartIndex != index)
                    // 交换第一个禁用的组件与需要启用的组件
                    swapComponents(index, mDisabledStartIndex);

                mDisabledStartIndex++;
            }
            // 如果组件先前已启用且现在需要禁用
            else if (isDisabled && index < mDisabledStartIndex)
            {
                assert(mDisabledStartIndex > 0);

                // 如果启用的组件不是唯一一个启用的组件
                if (index != mDisabledStartIndex - 1)
                    // 交换最后一个启用的组件与需要禁用的组件
                    swapComponents(index, mDisabledStartIndex - 1);

                mDisabledStartIndex--;
            }

            assert(mDisabledStartIndex <= mNbComponents);
        }


        // 添加适当的实现
        protected virtual void moveComponentToIndex(int sourceIndex, int destIndex)
        {
            // 添加适当的实现
        }

        // 添加适当的实现
        protected virtual void swapComponents(int index1, int index2)
        {
            // 添加适当的实现
        }

        // 添加适当的实现
        protected virtual void assert(bool condition)
        {
            // 添加适当的实现
        }

        public bool getIsEntityDisabled(Entity entity)
        {
            return mMapEntityToComponentIndex[entity] >= mDisabledStartIndex;
        }


        public int getNbComponents()
        {
            return mNbComponents;
        }

        public int getNbEnabledComponents()
        {
            return mDisabledStartIndex;
        }

        public int getEntityIndex(Entity entity)
        {
            Debug.Assert(hasComponent(entity));
            return mMapEntityToComponentIndex[entity];
        }

        public bool hasComponent(Entity entity)
        {
            return mMapEntityToComponentIndex.ContainsKey(entity);
        }

        public bool HasComponentGetIndex(Entity entity, out int entityIndex)
        {
            if (mMapEntityToComponentIndex.TryGetValue(entity, out entityIndex)) return true;
            return false;
        }
    }
}