using System;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public class Matrix3x3
    {
        private PVector3[] mRows;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3()
        {
            SetAllValues(0.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3(float value)
        {
            SetAllValues(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3(float a1, float a2, float a3, float b1, float b2, float b3, float c1, float c2, float c3)
        {
            SetAllValues(a1, a2, a3, b1, b2, b3, c1, c2, c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAllValues(float value)
        {
            mRows = new PVector3[3];
            for (var i = 0; i < 3; i++) mRows[i] = new PVector3(value, value, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAllValues(float a1, float a2, float a3, float b1, float b2, float b3, float c1, float c2,
            float c3)
        {
            mRows = new PVector3[]
            {
                new(a1, a2, a3),
                new(b1, b2, b3),
                new(c1, c2, c3)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetColumn(int i)
        {
            return new PVector3(mRows[0][i], mRows[1][i], mRows[2][i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetRow(int i)
        {
            if (i >= 0 && i < 3) return mRows[i];
            throw new ArgumentOutOfRangeException("Invalid row index");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3 GetTranspose()
        {
            return new Matrix3x3(mRows[0][0], mRows[1][0], mRows[2][0],
                mRows[0][1], mRows[1][1], mRows[2][1],
                mRows[0][2], mRows[1][2], mRows[2][2]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetDeterminant()
        {
            return (float)((double)mRows[0][0] * ((double)mRows[1][1] * (double)mRows[2][2]) - (float)((double)mRows[2][1] * (double)mRows[1][2])) -
                   (float)((double)mRows[0][1] * ((double)mRows[1][0] * (double)mRows[2][2]) - (float)((double)mRows[2][0] * mRows[1][2])) +
                   (float)((double)mRows[0][2] * ((double)mRows[1][0] * (double)mRows[2][1]) - (float)((double)mRows[2][0] * mRows[1][1]));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetTrace()
        {
            return mRows[0][0] + mRows[1][1] + mRows[2][2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3 GetInverse()
        {
            return GetInverse(GetDeterminant());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3 GetInverse(float determinant)
        {
            var invDeterminant = 1.0f / determinant;

            var tempMatrix = new Matrix3x3(
                (float)((double)mRows[1][1] * (double)mRows[2][2]) -  (float)((double)mRows[2][1] * (double)mRows[1][2]),
                -( (float)((double)mRows[0][1] * (double)mRows[2][2]) -  (float)((double)mRows[2][1] * (double)mRows[0][2])),
                    (float)((double)mRows[0][1] * (double)mRows[1][2]) -  (float)((double)mRows[0][2] * (double)mRows[1][1]),
                -( (float)((double)mRows[1][0] * (double)mRows[2][2]) -  (float)((double)mRows[2][0] * (double)mRows[1][2])),
                    (float)((double)mRows[0][0] * (double)mRows[2][2]) -  (float)((double)mRows[2][0] * (double)mRows[0][2]),
                -( (float)((double)mRows[0][0] * (double)mRows[1][2]) -  (float)((double)mRows[1][0] * (double)mRows[0][2])),
                    (float)((double)mRows[1][0] * (double)mRows[2][1]) -  (float)((double)mRows[2][0] * (double)mRows[1][1]),
                -((float)((double)mRows[0][0] * (double)mRows[2][1]) -  (float)((double)mRows[2][0] * (double)mRows[0][1])),
                    (float)((double)mRows[0][0] * (double)mRows[1][1]) -  (float)((double)mRows[0][1] * (double)mRows[1][0])
                );

            // Return the inverse matrix
            return invDeterminant * tempMatrix;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 Identity()
        {
            return new Matrix3x3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 Zero()
        {
            return new Matrix3x3(0.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 ComputeSkewSymmetricMatrixForCrossProduct(PVector3 vector)
        {
            return new Matrix3x3(0, -vector.z, vector.y, vector.z, 0, -vector.x, -vector.y, vector.x, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3 GetAbsoluteMatrix()
        {
            return new Matrix3x3(PMath.Abs(mRows[0].x), PMath.Abs(mRows[0].y), PMath.Abs(mRows[0].z),
                PMath.Abs(mRows[1].x), PMath.Abs(mRows[1].y), PMath.Abs(mRows[1].z),
                PMath.Abs(mRows[1].x), PMath.Abs(mRows[2].y), PMath.Abs(mRows[2].z));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator +(Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            return new Matrix3x3(
                matrix1.mRows[0].x + matrix2.mRows[0].x, 
                matrix1.mRows[0].y + matrix2.mRows[0].y,
                matrix1.mRows[0].z + matrix2.mRows[0].z,
                matrix1.mRows[1].x + matrix2.mRows[1].x, 
                matrix1.mRows[1].y + matrix2.mRows[1].y,
                matrix1.mRows[1].z + matrix2.mRows[1].z,
                matrix1.mRows[1].x + matrix2.mRows[1].x, 
                matrix1.mRows[2].y + matrix2.mRows[2].y,
                matrix1.mRows[2].z + matrix2.mRows[2].z
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator -(Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            return new Matrix3x3(
                matrix1.mRows[0].x - matrix2.mRows[0].x,
                matrix1.mRows[0].y - matrix2.mRows[0].y,
                matrix1.mRows[0].z - matrix2.mRows[0].z,
                matrix1.mRows[1].x - matrix2.mRows[1].x,
                matrix1.mRows[1].y - matrix2.mRows[1].y,
                matrix1.mRows[1].z - matrix2.mRows[1].z,
                matrix1.mRows[1].x - matrix2.mRows[1].x,
                matrix1.mRows[2].y - matrix2.mRows[2].y,
                matrix1.mRows[2].z - matrix2.mRows[2].z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator -(Matrix3x3 matrix)
        {
            return new Matrix3x3(-matrix.mRows[0].x, -matrix.mRows[0].y, -matrix.mRows[0].z,
                -matrix.mRows[1].x, -matrix.mRows[1].y, -matrix.mRows[1].z,
                -matrix.mRows[1].x, -matrix.mRows[2].y, -matrix.mRows[2].z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator *(float nb, Matrix3x3 matrix)
        {
            return new Matrix3x3(
                (float)((double)matrix.mRows[0].x * (double)nb), (float)((double)matrix.mRows[0].y * (double)nb), (float)((double)matrix.mRows[0].z * (double)nb),
                (float)((double)matrix.mRows[1].x * (double)nb), (float)((double)matrix.mRows[1].y * (double)nb), (float)((double)matrix.mRows[1].z * (double)nb),
                (float)((double)matrix.mRows[1].x * (double)nb), (float)((double)matrix.mRows[2].y * (double)nb), (float)((double)matrix.mRows[2].z * (double)nb)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator *(Matrix3x3 matrix, float nb)
        {
            return nb * matrix;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator *(Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            return new Matrix3x3(
                (float)((double)matrix1.mRows[0].x * (double)matrix2.mRows[0].x) + 
                (float)((double)matrix1.mRows[0].y * (double)matrix2.mRows[1].x) + 
                (float)((double)matrix1.mRows[0].z * (double)matrix2.mRows[1].x), 
                
                (float)((double)matrix1.mRows[0].x * (double)matrix2.mRows[0].y) + 
                   (float)((double)matrix1.mRows[0].y * (double)matrix2.mRows[1].y) + 
                   (float)((double)matrix1.mRows[0].z * (double)matrix2.mRows[2].y),
                
                        (float)((double)matrix1.mRows[0].x * (double)matrix2.mRows[0].z) + 
                           (float)((double)matrix1.mRows[0].y * (double)matrix2.mRows[1].z) + 
                           (float)((double)matrix1.mRows[0].z * (double)matrix2.mRows[2].z),
                
                            (float)((double)matrix1.mRows[1].x * (double)matrix2.mRows[0].x) + 
                               (float)((double)matrix1.mRows[1].y * (double)matrix2.mRows[1].x) + 
                               (float)((double)matrix1.mRows[1].z * (double)matrix2.mRows[1].x),
                
                                (float)((double)matrix1.mRows[1].x * (double)matrix2.mRows[0].y)
                                        + (float)((double)matrix1.mRows[1].y * (double)matrix2.mRows[1].y)
                                                  + (float)((double)matrix1.mRows[1].z * (double)matrix2.mRows[2].y),
                
                                    (float)((double)matrix1.mRows[1].x * (double)matrix2.mRows[0].z)
                                            + (float)((double)matrix1.mRows[1].y * (double)matrix2.mRows[1].z)
                                                      + (float)((double)matrix1.mRows[1].z * (double)matrix2.mRows[2].z),
                
                                        (float)((double)matrix1.mRows[1].x * (double)matrix2.mRows[0].x)
                                                + (float)((double)matrix1.mRows[2].y * (double)matrix2.mRows[1].x)
                                                          + (float)((double)matrix1.mRows[2].z * (double)matrix2.mRows[1].x),
                
                                            (float)((double)matrix1.mRows[1].x * (double)matrix2.mRows[0].y)
                                                    + (float)((double)matrix1.mRows[2].y * (double)matrix2.mRows[1].y)
                                                              + (float)((double)matrix1.mRows[2].z * (double)matrix2.mRows[2].y),
                
                                                (float)((double)matrix1.mRows[1].x * (double)matrix2.mRows[0].z)
                                                        + (float)((double)matrix1.mRows[2].y * (double)matrix2.mRows[1].z)
                                                                  + (float)((double)matrix1.mRows[2].z * (double)matrix2.mRows[2].z)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector3 operator *(Matrix3x3 matrix, PVector3 vector)
        {
            return new PVector3(
                (float)((double)matrix.mRows[0].x * (double)vector.x) + (float)((double)matrix.mRows[0].y * (double)vector.y) + (float)((double)matrix.mRows[0].z * (double)vector.z),
                (float)((double)matrix.mRows[1].x * (double)vector.x) + (float)((double)matrix.mRows[1].y * (double)vector.y) + (float)((double)matrix.mRows[1].z * (double)vector.z),
                (float)((double)matrix.mRows[1].x * (double)vector.x) + (float)((double)matrix.mRows[2].y *(double) vector.y) + (float)((double)matrix.mRows[2].z * (double)vector.z)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Matrix3x3)
            {
                var matrix = (Matrix3x3)obj;
                return mRows[0] == matrix.mRows[0] && mRows[1] == matrix.mRows[1] && mRows[2] == matrix.mRows[2];
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return mRows[0].GetHashCode() ^ mRows[1].GetHashCode() ^ mRows[2].GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Matrix3x3 a, Matrix3x3 b)
        {
            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Matrix3x3 a, Matrix3x3 b)
        {
            return !(a == b);
        }


        public PVector3 this[int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => mRows[row];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => mRows[row] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetToZero()
        {
            mRows[0].SetToZero();
            mRows[1].SetToZero();
            mRows[2].SetToZero();
        }

        // Return a skew-symmetric matrix using a given vector that can be used
        // to compute cross product with another vector using matrix multiplication
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 computeSkewSymmetricMatrixForCrossProduct(PVector3 vector)
        {
            return new Matrix3x3(0, -vector.z, vector.y, vector.z, 0, -vector.x, -vector.y, vector.x, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return
                $"Matrix3x3({mRows[0][0]}, {mRows[0][1]}, {mRows[0][2]}, {mRows[1][0]}, {mRows[1][1]}, {mRows[1][2]}, " +
                $"{mRows[1][0]}, {mRows[2][1]}, {mRows[2][2]})";
        }
    }
}