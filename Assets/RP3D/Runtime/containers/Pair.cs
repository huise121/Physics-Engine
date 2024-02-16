using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RP3D
{
    public class Pair<T1, T2>
    {
        // 属性
        /// <summary>
        /// 对组的第一个元素
        /// </summary>
        public T1 First { get; }

        /// <summary>
        /// 对组的第二个元素
        /// </summary>
        public T2 Second { get; }

        // 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public Pair(T1 item1, T2 item2)
        {
            First = item1;
            Second = item2;
        }

        // 方法
        /// <summary>
        /// 重载的相等运算符
        /// </summary>
        public bool Equals(Pair<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(First, other.First) &&
                   EqualityComparer<T2>.Default.Equals(Second, other.Second);
        }

        /// <summary>
        /// 重载的不相等运算符
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
        
            var other = (Pair<T1, T2>)obj;
            return Equals(other);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (First != null ? First.GetHashCode() : 0);
                hash = hash * 23 + (Second != null ? Second.GetHashCode() : 0);
                return hash;
            }
        }
    }

}