using System;
using System.Collections.Generic;

namespace RP3D
{
    //"Concave Shape" 的中文翻译是 "凹形"。
    public class ConcaveShape : CollisionShape
    {
        public PVector3 mScale;


        public ConcaveShape(CollisionShapeName name, CollisionShapeType type) : base(name, type)
        {
        }

        public PVector3 getScale()
        {
            return mScale;
        }

        public void setScale(PVector3 scale)
        {
            mScale = scale;
        }

        /**
         * 计算碰撞形状的惯性张量时使用的质量
         * @param mass 用于计算碰撞形状惯性张量的质量
         * @return 返回形状的局部惯性张量，对于凹三角网格，此处的默认惯性张量并不是非常真实。
         * 然而，在大多数情况下，它仅用于静态物体，因此惯性张量并未被使用。
         */
        public override PVector3 getLocalInertiaTensor(float mass)
        {
            // 默认的惯性张量
            // 注意，对于凹三角网格，这并不是非常真实的。
            // 然而，在大多数情况下，它只会用于静态物体，因此惯性张量并未被使用。
            return new PVector3(mass, mass, mass);
        }


        public override bool IsConvex()
        {
            return false;
        }

        public override bool IsPolyhedron()
        {
            return true;
        }

        public override void GetLocalBounds(out PVector3 min, out PVector3 max)
        {
            throw new NotImplementedException();
        }

        public override float getVolume()
        {
            throw new NotImplementedException();
        }

        public override bool Raycast(PRay pRay, out RaycastInfo raycastInfo, Collider collider)
        {
            throw new NotImplementedException();
        }

        public override bool testPointInside(PVector3 localPoint, Collider collider)
        {
            return false;
        }

        /// 在给定的轴对齐边界框内部的凹形状的所有三角形上使用回调方法。
        public  void computeOverlappingTriangles(AABB localAABB, List<PVector3> triangleVertices,
            List<PVector3> triangleVerticesNormals, List<int> shapeIds)
        {
            
        }
        
 


    }
}