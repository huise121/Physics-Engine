
namespace RP3D {

    // 结构体 Ray
    /// <summary>
    /// 表示由两个点构成的3D射线。
    /// 射线从 point1 出发，经过 point1 + maxFraction * (point2 - point1) 处。
    /// 所有点在世界坐标系中指定。
    /// </summary>
    public struct PRay {

        // -------------------- 属性 -------------------- //

        /// <summary>
        /// 射线的起点，即第一个点，以世界坐标表示。
        /// </summary>
        public PVector3 point1;

        /// <summary>
        /// 射线的第二个点，以世界坐标表示。
        /// </summary>
        public PVector3 point2;

        /// <summary>
        /// 射线的最大分数值，用于确定射线的最远可达位置。
        /// </summary>
        public float maxFraction;

        // -------------------- 方法 -------------------- //

        /// <summary>
        /// 带参数的构造函数，用于创建射线对象。
        /// </summary>
        /// <param name="p1">射线的起点</param>
        /// <param name="p2">射线的第二个点</param>
        /// <param name="maxFrac">射线的最大分数值，默认为1.0f</param>
        public PRay(PVector3 p1, PVector3 p2, float maxFrac = 1.0f) {
            point1 = p1;
            point2 = p2;
            maxFraction = maxFrac;
        }
    }
}