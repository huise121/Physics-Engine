using System.Runtime.CompilerServices;

namespace RP3D
{
    public struct PVector3
    {
        public float x;
        public float y;
        public float z;

        public PVector3(PVector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public PVector3(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAllValues(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetToZero()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length()
        {
            return PMath.Sqrt((float)((double)x * (double)x) + (float)((double)y * (double)y) + (float)((double)z * (double)z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquare()
        {
            return (float)((double)x * (double)x) + (float)((double)y * (double)y) + (float)((double)z * (double)z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetUnit()
        {
            var length = Length();
            if (length < float.Epsilon) return Zero();
            return new PVector3((float)((double)x / (double)length), (float)((double)y / (double)length), (float)((double)z / (double)length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetOneUnitOrthogonalVector()
        {
            return new PVector3(-y, x, -z).GetUnit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsUnit()
        {
            return PMath.approxEqual(LengthSquare(), 1.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFinite()
        {
            return !float.IsNaN(x) && !float.IsInfinity(x) &&
                   !float.IsNaN(y) && !float.IsInfinity(y) &&
                   !float.IsNaN(z) && !float.IsInfinity(z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero()
        {
            return PMath.approxEqual(LengthSquare(), 0.0f);
        }

        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 cross(PVector3 vector)
        {
            return new PVector3((float)((double)y * (double)vector.z) - (float)((double)z * (double)vector.y),
                (float)((double)z * (double)vector.x) - (float)((double)x * (double)vector.z),
                (float)((double)x * (double)vector.y) - (float)((double)y * (double)vector.x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            var length = Length();
            if ((double)length > 9.999999747378752E-06)
            {
                x /= length;
                y /= length;
                z /= length;
            }
            else
            {
                x = 0;
                y = 0;
                z = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetAbsoluteVector()
        {
            return new PVector3(PMath.Abs(x), PMath.Abs(y), PMath.Abs(z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMinAxis()
        {
            return x < y ? x < z ? 0 : 2 : y < z ? 1 : 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxAxis()
        {
            return x < y ? y < z ? 2 : 1 : x < z ? 2 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMinValue()
        {
            return PMath.Min(PMath.Min(x, y), z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMaxValue()
        {
            return PMath.Max(PMath.Max(x, y), z);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is PVector3 other1 && this.Equals(other1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(PVector3 other) => (double) x == (double) other.x && (double) y == (double) other.y && (double) z == (double) other.z;

   
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PVector3 left, PVector3 right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PVector3 left, PVector3 right)
        {
            return !left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator +(PVector3 a, PVector3 b)
        {
            return new PVector3((float)((double)a.x + (double)b.x), (float)((double)a.y + (double)b.y), (float)((double)a.z + (double)b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator -(PVector3 a, PVector3 b)
        {
            return new PVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator -(PVector3 vector)
        {
            return new PVector3(-vector.x, -vector.y, -vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator *(PVector3 vector, float number)
        {
            return new PVector3((float)((double)vector.x * (double)number), (float)((double)vector.y * (double)number), (float)((double)vector.z * (double)number));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator /(PVector3 vector, float number)
        {
            return new PVector3((float)((double)vector.x / (double)number), (float)((double)vector.y / (double)number), (float)((double)vector.z / (double)number));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator /(PVector3 a, PVector3 b)
        {
            return new PVector3((float)((double)a.x / (double)b.x), (float)((double)a.y / (double)b.y), (float)((double)a.z / (double)b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator *(float number, PVector3 vector)
        {
            return vector * number;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator *(PVector3 a, PVector3 b)
        {
            return new PVector3((float)((double)a.x * (double)b.x), (float)((double)a.y * (double)b.y), (float)((double)a.z * (double)b.z));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 Zero()
        {
            return new PVector3(0, 0, 0);
        }


        public float this[int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (row == 0) return x;
                if (row == 1) return y;
                if (row == 2) return z;
                return 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (row == 0) x = value;
                else if (row == 1) y = value;
                else if (row == 2) z = value;
            }
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float dot(PVector3 vector)
        {
            return (float)((double)x * (double)vector.x + (double)y * (double)vector.y + (double)z * (double)vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"Vector3({x:F7}, {y:F7}, {z:F7})";
        }
    }
}