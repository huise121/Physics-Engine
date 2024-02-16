using System;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public struct PVector2
    {
        public float x;
        public float y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector2(float newX, float newY)
        {
            x = newX;
            y = newY;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAllValues(float newX, float newY)
        {
            x = newX;
            y = newY;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetToZero()
        {
            x = 0;
            y = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length()
        {
            return PMath.Sqrt(x * x + y * y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquare()
        {
            return (float)((double)x * (double)x) + (float)((double)y * (double)y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector2 GetUnit()
        {
            float length = Length();
            if (length < float.Epsilon)
            {
                return PVector2.Zero();
            }
            return new PVector2((float)((double)x / (double)length), (float)((double)y / (double)length));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector2 GetOneUnitOrthogonalVector()
        {
            return new PVector2(-y, x).GetUnit();
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
                   !float.IsNaN(y) && !float.IsInfinity(y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero()
        {
            return PMath.approxEqual(LengthSquare(), 0.0f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(PVector2 vector)
        {
            return x * vector.x + y * vector.y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            float length = Length();
            if (length < float.Epsilon)
            {
                return;
            }
            x /= length;
            y /= length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector2 GetAbsoluteVector()
        {
            return new PVector2(PMath.Abs(x), PMath.Abs(y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMinAxis()
        {
            return x < y ? 0 : 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxAxis()
        {
            return x < y ? 1 : 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is PVector2))
            {
                return false;
            }
            PVector2 other = (PVector2)obj;
            return x == other.x && y == other.y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PVector2 left, PVector2 right)
        {
            return left.Equals(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PVector2 left, PVector2 right)
        {
            return !left.Equals(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator +(PVector2 a, PVector2 b)
        {
            return new PVector2(a.x + b.x, a.y + b.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator -(PVector2 a, PVector2 b)
        {
            return new PVector2(a.x - b.x, a.y - b.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator -(PVector2 vector)
        {
            return new PVector2(-vector.x, -vector.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator *(PVector2 vector, float number)
        {
            return new PVector2((float)((double)vector.x * (double)number), (float)((double)vector.y * (double)number));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator *(float number, PVector2 vector)
        {
            return vector * number;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator *(PVector2 a, PVector2 b)
        {
            return new PVector2((float)((double)a.x * (double)b.x), (float)((double)a.y * (double)b.y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator /(PVector2 vector, float number)
        {
            if (number < float.Epsilon)
            {
                throw new ArgumentException("Division by zero or too small number.");
            }
            return new PVector2((float)((double)vector.x / (double)number), (float)((double)vector.y / (double)number));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator /(PVector2 a, PVector2 b)
        {
            if (b.x < float.Epsilon || b.y < float.Epsilon)
            {
                throw new ArgumentException("Division by zero or too small number.");
            }
            return new PVector2((float)((double)a.x / (double)b.x), (float)((double)a.y / (double)b.y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 Min(PVector2 a, PVector2 b)
        {
            return new PVector2(PMath.Min(a.x, b.x), PMath.Min(a.y, b.y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 Max(PVector2 a, PVector2 b)
        {
            return new PVector2(PMath.Max(a.x, b.x), PMath.Max(a.y, b.y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"Vector2({x}, {y})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 Zero()
        {
            return new PVector2(0, 0);
        }


        public float this[int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => row == 0 ? x : y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (row)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                }
            }
        }
    }
}
