namespace RP3D
{
    /// 包含有关射线命中的信息。
    public struct RaycastInfo
    {
        /// 世界坐标系中的命中点。
        public PVector3 worldPoint;

        ///  世界坐标系中命中点的表面法线。
        public PVector3 worldNormal;

        ///  命中点在点1和点2之间的分数距离。
        ///  命中点 "p" 满足 p = point1 + hitFraction * (point2 - point1)。
        public float hitFraction;

        ///  被命中的网格子部件索引（仅用于三角形网格，否则为-1）。
        public int meshSubpart;

        ///  命中的三角形索引（仅用于三角形网格，否则为-1）。
        public int triangleIndex;

        /// 命中的碰撞体。
        public CollisionBody body;

        /// 命中的碰撞器。
        public Collider collider;


    }

    // 类 RaycastCallback
    /// <summary>
    ///     用于射线投射查询注册回调的类。
    /// </summary>
    public abstract class RaycastCallback
    {
        // -------------------- 方法 -------------------- //

        /// <summary>
        ///     将为射线命中的每个碰撞器调用此方法。
        /// </summary>
        /// <param name="raycastInfo">射线命中的信息。</param>
        /// <returns>在命中后射线继续的值。</returns>
        public abstract float NotifyRaycastHit(RaycastInfo info);
    }

    // 类 RaycastTest
    /// <summary>
    ///     对碰撞器执行射线投射测试。
    /// </summary>
    public class RaycastTest
    {
        ///用户回调类。
        private readonly RaycastCallback userCallback;



        public RaycastTest(RaycastCallback callback)
        {
            userCallback = callback;
        }

   
        /// <summary>
        /// 对碰撞体进行光线投射测试
        /// </summary>
        /// <param name="shape">要进行光线投射测试的碰撞体</param>
        /// <param name="ray">光线</param>
        /// <returns>如果光线与碰撞体相交，则返回用户定义的碰撞信息中的光线击中分数，否则返回光线的最大分数</returns>
        public float raycastAgainstShape(Collider shape, PRay ray)
        {
            // 进行与碰撞体的光线投射测试
            RaycastInfo raycastInfo;
            bool isHit = shape.Raycast(ray, out raycastInfo);

            // 如果光线击中了碰撞体
            if (isHit)
            {
                // 报告击中信息给用户，并返回用户定义的击中分数
                return userCallback.NotifyRaycastHit(raycastInfo);
            }
    
            // 如果未击中碰撞体，则返回光线的最大分数
            return ray.maxFraction;
        }


    }
}