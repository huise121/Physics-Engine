using UnityEngine;

namespace RP3D
{

    public class Entity
    {
        // 静态成员初始化
        public static uint ENTITY_INDEX_BITS = 24;
        public static uint ENTITY_INDEX_MASK = (1u << (int)ENTITY_INDEX_BITS) - 1;
        public static uint ENTITY_GENERATION_BITS = 8;
        public static uint ENTITY_GENERATION_MASK = (1u << (int)ENTITY_GENERATION_BITS) - 1;
        public static uint MINIMUM_FREE_INDICES = 1024;


        // 构造函数
        public Entity(uint index, uint generation)
        {
            Id = (index & ENTITY_INDEX_MASK) | ((generation & ENTITY_GENERATION_MASK) << (int)ENTITY_INDEX_BITS);
            Debug.Assert(GetIndex() == index);
            Debug.Assert(GetGeneration() == generation);
        }

        // 属性
        public uint Id { get; }

        // 方法
        public uint GetIndex()
        {
            return Id & ENTITY_INDEX_MASK;
        }

        public uint GetGeneration()
        {
            return (Id >> (int)ENTITY_INDEX_BITS) & ENTITY_GENERATION_MASK;
        }

        public override string ToString()
        {
            return $"id = {Id}";
        }
    }
}