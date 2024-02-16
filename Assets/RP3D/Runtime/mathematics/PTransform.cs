using System.Runtime.CompilerServices;

namespace RP3D
{
    // Class Transform
    /**
     * This class represents a position and an orientation in 3D. It can
     * also be seen as representing a translation and a rotation.
     */
    public class PTransform
    {
        // Attributes
        private PVector3 mPosition;
        private PQuaternion mOrientation;

        // Constructor
        public PTransform()
        {
            mPosition = new PVector3(0.0f, 0.0f, 0.0f);
            mOrientation = PQuaternion.identity();
        }

        // Constructor
        public PTransform(PVector3 position, Matrix3x3 orientation)
        {
            mPosition = position;
            mOrientation = new PQuaternion(orientation);
        }

        public PTransform(PVector3 position, PQuaternion orientation)
        {
            mPosition = position;
            mOrientation = orientation;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetPosition()
        {
            return mPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPosition(PVector3 position)
        {
            mPosition = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PQuaternion GetOrientation()
        {
            return mOrientation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOrientation(PQuaternion orientation)
        {
            mOrientation = orientation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetToIdentity()
        {
            mPosition = new PVector3(0.0f, 0.0f, 0.0f);
            mOrientation = PQuaternion.identity();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PTransform GetInverse()
        {
            var invPQuaternion = mOrientation.getInverse();
            return new PTransform(invPQuaternion * -mPosition, invPQuaternion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PTransform InterpolateTransforms(PTransform oldPTransform, PTransform newPTransform,
            float interpolationFactor)
        {
            var interPosition = oldPTransform.mPosition * (1.0f - interpolationFactor) +
                                newPTransform.mPosition * interpolationFactor;

            var interOrientation = PQuaternion.Slerp(oldPTransform.mOrientation,
                newPTransform.mOrientation,
                interpolationFactor);

            return new PTransform(interPosition, interOrientation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PTransform Identity()
        {
            return new PTransform(new PVector3(0f, 0f, 0f), Matrix3x3.Identity());
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator *(PTransform transform1, PVector3 vector)
        {
            return transform1.mOrientation * vector + transform1.mPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PTransform operator *(PTransform transform1, PTransform transform2)
        {
            var mOrientation = transform1.mOrientation;
            var mPosition = transform1.mPosition;

            var prodX = (float)((double)mOrientation.w * (double)transform2.mPosition.x) + 
                        (float)((double)mOrientation.y * (double)transform2.mPosition.z) - 
                        (float)((double)mOrientation.z * (double)transform2.mPosition.y);
            
            var prodY = (float)((double)mOrientation.w * (double)transform2.mPosition.y) + 
                        (float)((double)mOrientation.z * (double)transform2.mPosition.x) - 
                        (float)((double)mOrientation.x * (double)transform2.mPosition.z);
            
            var prodZ = (float)((double)mOrientation.w * (double)transform2.mPosition.z) + 
                        (float)((double)mOrientation.x * (double)transform2.mPosition.y) - 
                        (float)((double)mOrientation.y * (double)transform2.mPosition.x);
            
            var prodW = -(float)((double)mOrientation.x * (double)transform2.mPosition.x) - 
                        (float)((double)mOrientation.y * (double)transform2.mPosition.y) - 
                        (float)((double)mOrientation.z * (double)transform2.mPosition.z);

            return new PTransform(new PVector3(
                    mPosition.x + 
                    (float)((double)mOrientation.w * (double)prodX) - 
                    (float)((double)prodY * (double)mOrientation.z) + 
                    (float)((double)prodZ * (double)mOrientation.y) -
                    (float)((double)prodW * (double)mOrientation.x),
                    
                    mPosition.y + 
                    (float)((double)mOrientation.w * (double)prodY) - 
                    (float)((double)prodZ * (double)mOrientation.x) + 
                    (float)((double)prodX * (double)mOrientation.z) -
                    (float)((double)prodW * (double)mOrientation.y),
                    
                    mPosition.z + 
                    (float)((double)mOrientation.w * (double)prodZ) - 
                    (float)((double)prodX * (double)mOrientation.y) + 
                    (float)((double)prodY * (double)mOrientation.x) -
                    (float)((double)prodW * (double)mOrientation.z)
                    ),
                
                new PQuaternion(
                    
                    (float)((double)mOrientation.w * (double)transform2.mOrientation.x) + 
                    (float)((double)transform2.mOrientation.w * (double)mOrientation.x) + 
                            (float)((double)mOrientation.y * (double)transform2.mOrientation.z) -
                                    (float)((double)mOrientation.z * (double)transform2.mOrientation.y),
                    
                    (float)((double)mOrientation.w * (double)transform2.mOrientation.y) + 
                    (float)((double)transform2.mOrientation.w * (double)mOrientation.y) + 
                    (float)((double)mOrientation.z * (double)transform2.mOrientation.x) -
                    (float)((double)mOrientation.x * (double)transform2.mOrientation.z),
                    
            (float)((double)mOrientation.w * (double)transform2.mOrientation.z) + 
                (float)((double)transform2.mOrientation.w * mOrientation.z) + 
                (float)((double) mOrientation.x * (double)transform2.mOrientation.y) -
                (float)((double)mOrientation.y * (double)transform2.mOrientation.x),
                    
            (float)((double)mOrientation.w * (double)transform2.mOrientation.w) - 
                (float)((double)mOrientation.x * (double)transform2.mOrientation.x) - 
                (float)((double) mOrientation.y * (double)transform2.mOrientation.y) -
                (float)((double)mOrientation.z * (double)transform2.mOrientation.z))
                
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var otherPTransform = (PTransform)obj;
            return mPosition.Equals(otherPTransform.mPosition) && mOrientation.Equals(otherPTransform.mOrientation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return mPosition.GetHashCode() ^ mOrientation.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return "Transform(" + mPosition + "," + mOrientation + ")";
        }
    }
}