using System;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public struct PQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PQuaternion(float newW, PVector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = newW;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PQuaternion(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion FromEulerAngles(float angleX, float angleY, float angleZ)
        {
            var pQuaternion = new PQuaternion();
            pQuaternion.InitWithEulerAngles(angleX, angleY, angleZ);
            return pQuaternion;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion FromEulerAngles(PVector3 eulerAngles)
        {
            var pQuaternion = new PQuaternion();
            pQuaternion.InitWithEulerAngles(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            return pQuaternion;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PQuaternion(Matrix3x3 matrix)
        {
            var trace = matrix.GetTrace();
            float r;
            float s;

            if (trace < 0.0f)
            {
                if (matrix[1][1] > matrix[0][0])
                {
                    if (matrix[2][2] > matrix[1][1])
                    {
                        r = PMath.Sqrt(matrix[2][2] - matrix[0][0] - matrix[1][1] + 1.0f);
                        s = 1.0f / (r * 2f);

                        x = (matrix[2][0] + matrix[0][2]) * s;
                        y = (matrix[1][2] + matrix[2][1]) * s;
                        z = r * 0.5f;
                        w = (matrix[1][0] - matrix[0][1]) * s;
                    }
                    else
                    {
                        r = PMath.Sqrt(matrix[1][1] - matrix[2][2] - matrix[0][0] + 1.0f);
                        s = 1.0f / (r * 2f);

                        x = (matrix[0][1] + matrix[1][0]) * s;
                        y = r * 0.5f;
                        z = (matrix[1][2] + matrix[2][1]) * s;
                        w = (matrix[0][2] - matrix[2][0]) * s;
                    }
                }
                else if (matrix[2][2] > matrix[0][0])
                {
                    r = PMath.Sqrt(matrix[2][2] - matrix[0][0] - matrix[1][1] + 1.0f);
                    s = 1.0f / (r * 2f);

                    x = (matrix[2][0] + matrix[0][2]) * s;
                    y = (matrix[1][2] + matrix[2][1]) * s;
                    z = (float)(r * 0.5f);
                    w = (matrix[1][0] - matrix[0][1]) * s;
                }
                else
                {
                    r = PMath.Sqrt(matrix[0][0] - matrix[1][1] - matrix[2][2] + 1.0f);
                    s = 1.0f / (r * 2f);

                    x = (float)(r * 0.5f);
                    y = (matrix[0][1] + matrix[1][0]) * s;
                    z = (matrix[2][0] + matrix[0][2]) * s;
                    w = (matrix[2][1] - matrix[1][2]) * s;
                }
            }
            else
            {
                r = PMath.Sqrt(trace + 1.0f);
                s = 1.0f / (r * 2f);

                x = (matrix[2][1] - matrix[1][2]) * s;
                y = (matrix[0][2] - matrix[2][0]) * s;
                z = (matrix[1][0] - matrix[0][1]) * s;
                w = (float)(r * 0.5f);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetRotationAngleAxis(out float angle, out PVector3 axis)
        {
            angle = (float)(PMath.Acos(w) * 2.0);

            var rotationAxis = new PVector3(x, y, z).GetUnit();
            axis = new PVector3(rotationAxis.x, rotationAxis.y, rotationAxis.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3 GetMatrix()
        {
            var nQ = x * x + y * y + z * z + w * w;
            var s = 0.0f;

            if (nQ > 0.0f) s = 2.0f / nQ;

            var xs = x * s;
            var ys = y * s;
            var zs = z * s;
            var wxs = w * xs;
            var wys = w * ys;
            var wzs = w * zs;
            var xxs = x * xs;
            var xys = x * ys;
            var xzs = x * zs;
            var yys = y * ys;
            var yzs = y * zs;
            var zzs = z * zs;

            return new Matrix3x3(1.0f - (yys + zzs), xys - wzs, xzs + wys,
                xys + wzs, 1.0f - (xxs + zzs), yzs - wxs,
                xzs - wys, yzs + wxs, 1.0f - (xxs + yys));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion Slerp(PQuaternion quaternion1, PQuaternion quaternion2, float t)
        {
            if (t < 0.0f || t > 1.0f)
                throw new ArgumentException("t must be between 0 and 1, inclusive.");

            var invert = 1.0f;
            var cosineTheta = quaternion1.Dot(quaternion2);

            if (cosineTheta < 0.0f)
            {
                cosineTheta = -cosineTheta;
                invert = -1.0f;
            }

            var epsilon = 0.00001f;

            if (1 - cosineTheta < epsilon) return quaternion1 * (1.0f - t) + quaternion2 * (t * invert);

            var theta = PMath.Acos(cosineTheta);
            var sineTheta = PMath.Sin(theta);

            var coeff1 = PMath.Sin((1.0f - t) * theta) / sineTheta;
            var coeff2 = PMath.Sin(t * theta) / sineTheta * invert;

            return quaternion1 * coeff1 + quaternion2 * coeff2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitWithEulerAngles(float angleX, float angleY, float angleZ)
        {
            var angle = (float)(angleX * 0.5f);
            var sinX = PMath.Sin(angle);
            var cosX = PMath.Cos(angle);

            angle = (float)(angleY * 0.5f);
            var sinY = PMath.Sin(angle);
            var cosY = PMath.Cos(angle);

            angle = (float)(angleZ * 0.5f);
            var sinZ = PMath.Sin(angle);
            var cosZ = PMath.Cos(angle);

            var cosYcosZ = (float)((double)cosY * (double)cosZ);
            var sinYcosZ = (float)((double)sinY * (double)cosZ);
            var cosYsinZ = (float)((double)cosY * (double)sinZ);
            var sinYsinZ = (float)((double)sinY * (double)sinZ);

            x = (float)((double)sinX * (double)cosYcosZ) - (float)((double)cosX * (double)sinYsinZ);
            y = (float)((double)cosX * (double)sinYcosZ) + (float)((double)sinX * (double)cosYsinZ);
            z = (float)((double)cosX * (double)cosYsinZ) - (float)((double)sinX * (double)sinYcosZ);
            w = (float)((double)cosX * (double)cosYcosZ) + (float)((double)sinX * (double)sinYsinZ);

            Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            var length = Length();
            if (length < float.Epsilon) return;

            x /= length;
            y /= length;
            z /= length;
            w /= length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length()
        {
            return PMath.Sqrt((float)((double)x * (double)x) + (float)((double)y * (double)y) + (float)((double)z * (double)z) + (float)((double)w * (double)w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(PQuaternion pQuaternion)
        {
            return (float)((double)x * (double)pQuaternion.x) + (float)((double)y * (double)pQuaternion.y) + (float)((double)z * (double)pQuaternion.z)
                   + (float)((double)w * (double)pQuaternion.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion operator *(PQuaternion pQuaternion, float number)
        {
            return new PQuaternion((float)((double)pQuaternion.x * (double)number),
                (float)((double)pQuaternion.y * (double)number), 
                    (float)((double)pQuaternion.z * (double)number),
                        (float)((double)pQuaternion.w * (double)number));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion operator +(PQuaternion quaternion1, PQuaternion quaternion2)
        {
            return new PQuaternion(quaternion1.x + quaternion2.x, quaternion1.y + quaternion2.y,
                quaternion1.z + quaternion2.z, quaternion1.w + quaternion2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion operator *(PQuaternion q1, PQuaternion q2)
        {
            return new PQuaternion(
                (float)((double)q1.w * (double)q2.x) + 
                        (float)((double)q1.x * (double)q2.w) + 
                                (float)((double)q1.y * (double)q2.z) - 
                                        (float)((double) q1.z * (double)q2.y),
                                            (float)((double)q1.w * (double)q2.y) + 
                                                    (float)((double)q1.y * (double)q2.w) + 
                                                            (float)((double)q1.z * (double)q2.x) - 
                                                                    (float)((double)q1.x * (double)q2.z),
                                                                        (float)((double)q1.w * (double)q2.z) + 
                                                                            (float)((double)q1.z * (double)q2.w) + 
                                                                                (float)((double)q1.x * (double)q2.y) - 
                                                                                    (float)((double)q1.y * (double)q2.x),
                                                                                        (float)((double)q1.w * (double)q2.w) - 
                                                                                            (float)((double)q1.x * (double)q2.x) - 
                                                                                                (float)((double)q1.y * (double)q2.y) - 
                                                                                                    (float)((double)q1.z * (double)q2.z)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion operator -(PQuaternion pQuaternion)
        {
            return new PQuaternion(-pQuaternion.x, -pQuaternion.y, -pQuaternion.z, -pQuaternion.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PQuaternion operator *(float number, PQuaternion pQuaternion)
        {
            return pQuaternion * number;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PQuaternion quaternion1, PQuaternion quaternion2)
        {
            return quaternion1.Equals(quaternion2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PQuaternion quaternion1, PQuaternion quaternion2)
        {
            return !quaternion1.Equals(quaternion2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is PQuaternion)) return false;

            var other = (PQuaternion)obj;
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"Quaternion({x}, {y}, {z}, {w})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAllValues(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setToZero()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 0;
        }


        public void setToIdentity()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 1;
        }

        public PVector3 getVectorV()
        {
            return new PVector3(x, y, z);
        }

        public float length()
        {
            return MathF.Sqrt((float)((double)x * (double)x) + (float)((double)y * (double)y) + (float)((double)z * (double)z) + (float)((double)w * (double)w));
        }


        public void inverse()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        public static PQuaternion identity()
        {
            return new PQuaternion(0.0f, 0.0f, 0.0f, 1.0f);
        }

        public PQuaternion getConjugate()
        {
            return new PQuaternion(-x, -y, -z, w);
        }

        public PQuaternion getInverse()
        {
            return new PQuaternion(-x, -y, -z, w);
        }


        public static PVector3 operator *(PQuaternion quaternion1, PVector3 point)
        {
            /* The following code is equivalent to this
             * Quaternion p(point.x, point.y, point.z, 0.0);
             * return (((*this) * p) * getConjugate()).getVectorV();
             */
            var x = quaternion1.x;
            var y = quaternion1.y;
            var z = quaternion1.z;
            var w = quaternion1.w;

            var prodX = (double)w * (double)point.x + (double)y * (double)point.z - (double)z * (double)point.y;
            var prodY = (double)w * (double)point.y + (double)z * (double)point.x - (double)x * (double)point.z;
            var prodZ = (double)w * (double)point.z + (double)x * (double)point.y - (double)y * (double)point.x;
            var prodW = -(double)x * (double)point.x - (double)y * (double)point.y - (double)z * (double)point.z;
            return new PVector3((float)(w * prodX - prodY * z + prodZ * y - prodW * x),
                (float)(w * prodY - prodZ * x + prodX * z - prodW * y),
                (float)(w * prodZ - prodX * y + prodY * x - prodW * z));
        }


        public Matrix3x3 getMatrix()
        {
            var nQ = (float)((double)x * (double)x) + (float)((double)y * (double)y) + (float)((double)z * (double)z) +
                     (float)((double)w * (double)w);
            var s = 0.0f;

            if (nQ > 0.0f) s = 2.0f / nQ;

            // Computations used for optimization (less multiplications)
            var xs = (float)((double)x * (double)s);
            var ys = (float)((double)y * (double)s);
            var zs = (float)((double)z * (double)s);
            var wxs = (float)((double)w * (double)xs);
            var wys = (float)((double)w * (double)ys);
            var wzs = (float)((double)w * (double)zs);
            var xxs = (float)((double)x * (double)xs);
            var xys = (float)((double)x * (double)ys);
            var xzs = (float)((double)x * (double)zs);
            var yys = (float)((double)y * (double)ys);
            var yzs = (float)((double)y * (double)zs);
            var zzs = (float)((double)z * (double)zs);

            // Create the matrix corresponding to the quaternion
            return new Matrix3x3(1.0f - yys - zzs, xys - wzs, xzs + wys,
                xys + wzs, 1.0f - xxs - zzs, yzs - wxs,
                xzs - wys, yzs + wxs, 1.0f - xxs - yys);
        }


        public PQuaternion getUnit()
        {
            var lengthQuaternion = length();

            // Compute and return the unit quaternion
            return new PQuaternion((float)((double)x / (double)lengthQuaternion), (float)((double)y / (double)lengthQuaternion),
                (float)((double)z / (double)lengthQuaternion), (float)((double)w / (double)lengthQuaternion));
        }
    }
}