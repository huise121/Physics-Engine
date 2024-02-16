using System.Collections.Generic;

namespace RP3D
{
    public abstract class CollisionShape
    {
        public List<Collider> mColliders;
        protected int mId;
        protected CollisionShapeName mName;
        protected CollisionShapeType mType;

        public CollisionShape(CollisionShapeName name, CollisionShapeType type)
        {
            mName = name;
            mType = type;
            mColliders = new List<Collider>();
        }


        public CollisionShapeType GetType()
        {
            return mType;
        }

        public CollisionShapeName GetName()
        {
            return mName;
        }

        public int GetId()
        {
            return mId;
        }

        public void addCollider(Collider collider)
        {
            mColliders.Add(collider);
        }

        public void removeCollider(Collider collider)
        {
            mColliders.Remove(collider);
        }

        /// 如果碰撞形状是凸形，则返回true，如果是凹形则返回false
        public abstract bool IsConvex();

        /// 如果碰撞形状是多面体，则返回true
        public abstract bool IsPolyhedron();

        public abstract void GetLocalBounds(out PVector3 min, out PVector3 max);
        
        public abstract float getVolume();


        // 根据从形状局部空间到世界空间的变换，计算碰撞形状的世界空间轴对齐边界框（AABB）。
        // 该技术在Christer Ericson的书《Real-Time Collision Detection》中有描述。
        /**
         * @param[out] aabb 碰撞形状在世界坐标系中计算得到的轴对齐边界框（AABB）
         * @param transform 用于计算碰撞形状AABB的从形状局部空间到世界空间的变换
         */
        public virtual void ComputeAABB(out AABB aabb, PTransform pTransform)
        {
            aabb = new AABB();
            // Get the local bounds in x,y and z direction
            GetLocalBounds(out var minBounds, out var maxBounds);

            var translation = pTransform.GetPosition();
            var matrix = pTransform.GetOrientation().getMatrix();
            var resultMin = PVector3.Zero();
            var resultMax = PVector3.Zero();

            // For each of the three axis
            for (var i = 0; i < 3; i++)
            {
                // Add translation component
                resultMin[i] = translation[i];
                resultMax[i] = translation[i];

                for (var j = 0; j < 3; j++)
                {
                    var e = matrix[i][j] * minBounds[j];
                    var f = matrix[i][j] * maxBounds[j];

                    if (e < f)
                    {
                        resultMin[i] += e;
                        resultMax[i] += f;
                    }
                    else
                    {
                        resultMin[i] += f;
                        resultMax[i] += e;
                    }
                }
            }

            // Update the AABB with the new minimum and maximum coordinates
            aabb.SetMin(resultMin);
            aabb.SetMax(resultMax);
        }

        public abstract PVector3 getLocalInertiaTensor(float mass);

        public abstract bool Raycast(PRay pRay, out RaycastInfo raycastInfo, Collider collider);

        public abstract bool testPointInside(PVector3 localPoint, Collider collider);
    }
}