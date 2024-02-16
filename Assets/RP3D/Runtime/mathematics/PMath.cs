using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RP3D
{
    public static partial class PMath
    {
        public static float PI_RP3D = 3.141592653589f;

        public static float PI_TIMES_2 = 6.28318530f;

        public static float MACHINE_EPSILON = float.Epsilon;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            return a > b ? b : a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            return a > b ? b : a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Max(short a, short b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Min(short a, short b)
        {
            return a > b ? b : a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float a)
        {
            return Math.Abs(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float a)
        {
            return (float)Math.Sqrt(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float a)
        {
            return (float)Math.Acos(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float a)
        {
            return (float)Math.Sin(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float a)
        {
            return (float)Math.Cos(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float a, float b)
        {
            return (float)Math.Atan2(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float fmod(float a, float b)
        {
            return Mathf.Repeat(a, b);
        }


        /// Function to test if two real numbers are (almost) equal
        /// We test if two numbers a and b are such that (a-b) are in [-EPSILON; EPSILON]
        public static bool approxEqual(float a, float b, float epsilon = float.Epsilon)
        {
            return Abs(a - b) < epsilon;
        }

        // Function to test if two vectors are (almost) equal
        public static bool approxEqual(PVector3 vec1, PVector3 vec2, float epsilon = float.Epsilon)
        {
            return approxEqual(vec1.x, vec2.x, epsilon) && approxEqual(vec1.y, vec2.y, epsilon) &&
                   approxEqual(vec1.z, vec2.z, epsilon);
        }
    }
}