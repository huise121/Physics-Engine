// 引入库

using System;
using System.Runtime.CompilerServices;

namespace RP3D
{
    /// <summary>
    ///     Matrix2x2 类表示一个2x2矩阵。
    /// </summary>
    public class Matrix2x2
    {
        // 矩阵的行
        private PVector2[] mRows;

        /// <summary>
        ///     构造函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2()
        {
            // 初始化矩阵的所有值为零
            SetAllValues(0.0f, 0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2(float value)
        {
            SetAllValues(value, value, value, value);
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2(float a1, float a2, float b1, float b2)
        {
            // 用给定的值初始化矩阵
            SetAllValues(a1, a2, b1, b2);
        }

        /// <summary>
        ///     设置矩阵的所有值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAllValues(float a1, float a2, float b1, float b2)
        {
            mRows = new PVector2[2];
            mRows[0] = new PVector2(a1, a2);
            mRows[1] = new PVector2(b1, b2);
        }

        /// <summary>
        ///     设置矩阵为零矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetToZero()
        {
            mRows[0] = new PVector2(0.0f, 0.0f);
            mRows[1] = new PVector2(0.0f, 0.0f);
        }

        /// <summary>
        ///     返回一列
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector2 GetColumn(int i)
        {
            return i switch
            {
                0 => new PVector2(mRows[0][0], mRows[1][1]),
                1 => new PVector2(mRows[0][1], mRows[1][1]),
                _ => throw new ArgumentOutOfRangeException("i", "索引超出范围")
            };
        }

        /// <summary>
        ///     返回一行
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector2 GetRow(int i)
        {
            if (i >= 0 && i < 2)
                return mRows[i];
            throw new ArgumentOutOfRangeException("i", "索引超出范围");
        }

        /// <summary>
        ///     返回转置矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2 GetTranspose()
        {
            return new Matrix2x2(mRows[0][0], mRows[1][0],
                mRows[0][1], mRows[1][1]);
        }

        /// <summary>
        ///     返回矩阵的行列式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetDeterminant()
        {
            return (float)((double)mRows[0][0] * (double)mRows[1][1]) - (float)((double)mRows[1][0] * (double)mRows[0][1]);
        }

        /// <summary>
        ///     返回矩阵的迹
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetTrace()
        {
            return mRows[0][0] + mRows[1][1];
        }

        /// <summary>
        ///     返回矩阵的逆矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2 GetInverse()
        {
            return GetInverse(GetDeterminant());
        }

        /// <summary>
        ///     获取逆矩阵
        /// </summary>
        /// <param name="determinant">矩阵的行列式</param>
        /// <returns>逆矩阵</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2 GetInverse(float determinant)
        {
            // 检查行列式是否等于零
            if (PMath.Abs(determinant) <= float.Epsilon)
                throw new InvalidOperationException("Matrix is singular, cannot compute inverse.");

            var invDeterminant = 1.0f / determinant;

            // 计算临时矩阵
            var tempMatrix = new Matrix2x2(mRows[1][1], -mRows[0][1], -mRows[1][0], mRows[0][0]);

            // 返回逆矩阵
            return invDeterminant * tempMatrix;
        }


        /// <summary>
        ///     返回具有绝对值的矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2 GetAbsoluteMatrix()
        {
            return new Matrix2x2(PMath.Abs(mRows[0][0]), PMath.Abs(mRows[0][1]),
                PMath.Abs(mRows[1][0]), PMath.Abs(mRows[1][1]));
        }

        /// <summary>
        ///     设置矩阵为单位矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetToIdentity()
        {
            mRows[0] = new PVector2(1.0f, 0.0f);
            mRows[1] = new PVector2(0.0f, 1.0f);
        }

        /// <summary>
        ///     返回2x2单位矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 Identity()
        {
            return new Matrix2x2(1.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///     返回2x2零矩阵
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 Zero()
        {
            return new Matrix2x2(0.0f, 0.0f, 0.0f, 0.0f);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator +(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            return new Matrix2x2(matrix1.mRows[0][0] + matrix2.mRows[0][0],
                matrix1.mRows[0][1] + matrix2.mRows[0][1],
                matrix1.mRows[1][0] + matrix2.mRows[1][0],
                matrix1.mRows[1][1] + matrix2.mRows[1][1]);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator -(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            return new Matrix2x2(matrix1.mRows[0][0] - matrix2.mRows[0][0],
                matrix1.mRows[0][1] - matrix2.mRows[0][1],
                matrix1.mRows[1][0] - matrix2.mRows[1][0],
                matrix1.mRows[1][1] - matrix2.mRows[1][1]);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator -(Matrix2x2 matrix)
        {
            return new Matrix2x2(-matrix.mRows[0][0], -matrix.mRows[0][1],
                -matrix.mRows[1][0], -matrix.mRows[1][1]);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator *(float nb, Matrix2x2 matrix)
        {
            return new Matrix2x2(
                (float)((double)matrix.mRows[0][0] * (double)nb), (float)((double)matrix.mRows[0][1] * (double)nb),
                (float)((double)matrix.mRows[1][0] * (double)nb), (float)((double)matrix.mRows[1][1] * (double)nb));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator *(Matrix2x2 matrix, float nb)
        {
            return nb * matrix;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator *(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            return new Matrix2x2(
                (float)((double)matrix1.mRows[0][0] * (double)matrix2.mRows[0][0]) + (float)((double)matrix1.mRows[0][1] * (double)matrix2.mRows[1][0]),
                (float)((double)matrix1.mRows[0][0] * (double)matrix2.mRows[0][1]) + (float)((double)matrix1.mRows[0][1] * (double)matrix2.mRows[1][1]),
                (float)((double)matrix1.mRows[1][0] * (double)matrix2.mRows[0][1]) + (float)((double)matrix1.mRows[1][1] * (double)matrix2.mRows[1][0]),
                (float)((double)matrix1.mRows[1][0] * (double)matrix2.mRows[0][1]) + (float)((double)matrix1.mRows[1][1] * (double)matrix2.mRows[1][1]));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PVector2 operator *(Matrix2x2 matrix, PVector2 vector)
        {
            return new PVector2(
                (float)((double)matrix.mRows[0][0] * (double)vector.x) + (float)((double)matrix.mRows[0][1] * (double)vector.y),
                (float)((double)matrix.mRows[1][0] * (double)vector.x) + (float)((double)matrix.mRows[1][1] * (double)vector.y)
                );
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            return !(matrix1 == matrix2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            return matrix1 == matrix2;
        }

        /// <summary>
        ///     与赋值相加的重载运算符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2 Add(Matrix2x2 matrix)
        {
            mRows[0][0] += matrix.mRows[0][0];
            mRows[0][1] += matrix.mRows[0][1];
            mRows[1][0] += matrix.mRows[1][0];
            mRows[1][1] += matrix.mRows[1][1];
            return this;
        }

        /// <summary>
        ///     与赋值相减的重载运算符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2 Subtract(Matrix2x2 matrix)
        {
            mRows[0][0] -= matrix.mRows[0][0];
            mRows[0][1] -= matrix.mRows[0][1];
            mRows[1][0] -= matrix.mRows[1][0];
            mRows[1][1] -= matrix.mRows[1][1];
            return this;
        }

        /// <summary>
        ///     与赋值相乘的重载运算符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2x2 Multiply(float nb)
        {
            mRows[0][0] *= nb;
            mRows[0][1] *= nb;
            mRows[1][0] *= nb;
            mRows[1][1] *= nb;
            return this;
        }

        /// <summary>
        ///     用于访问矩阵行的索引器
        /// </summary>

        public PVector2 this[int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => mRows[row];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => mRows[row] = value;
        }

        /// <summary>
        ///     获取字符串表示形式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return "Matrix2x2(" + mRows[0][0] + "," + mRows[0][1] + "," +
                   mRows[1][0] + "," + mRows[1][1] + ")";
        }
    }
}