﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projet_IMA
{

    class IMA
    {
        static public float DPI = (float) (Math.PI * 2);
        static public float PI  = (float) (Math.PI);
        static public float PI2 = (float) (Math.PI / 2);
        static public float PI4 = (float) (Math.PI / 4);

        static public float Cosf(float theta) { return (float)Math.Cos(theta); }
        static public float Sinf(float theta) { return (float)Math.Sin(theta); }
        static public float Acosf(float theta) { return (float)Math.Acos(theta); }
        static public float Asinf(float theta) { return (float)Math.Asin(theta); }
        static public float Sqrtf(float v)    { return (float) Math.Sqrt(v); }

        static public Random Ran;
        static public void InitRand() { Ran = new Random(); }
        static public float RandNP(float v) { return ((float) Ran.NextDouble()-0.5f)*2*v; }
        static public float RandP(float v)  { return ((float) Ran.NextDouble() ) * v; }

        static public float Abs(float x) { if (x < 0) return -x; return x; }

        static public void Invert_Coord_Spherique(V3 P3D, V3 sphereCenter, float r, out float u, out float v)
        {
            V3 P = P3D - sphereCenter;
            P = P / r;
            if (P.z >= 1) { u =(float) IMA.PI2 ; v = 0; }
            else if (P.z <= -1) { u = (float)-IMA.PI2 ; v = 0; }
            else
            {
                v = (float) Math.Asin(P.z);
                float t = (float) (P.x / IMA.Cosf(v));
                if (t <= -1) { u = (float) IMA.PI; }
                else if (t >= 1) { u = 0; }
                else
                {
                    if (P.y < 0) u = (float) ( 2 * IMA.PI - Math.Acos(t));
                    else u = (float) Math.Acos(t);
                }
            }
        }

        static public int pgcd(int a, int b)
        {
            int temp = a % b;
            if (temp == 0)
                return b;
            return pgcd(b, temp);
        }
    }


    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static float Rand(float v)
        {
            return (float) random.Value.NextDouble() * v;
        }

        public static int Rand()
        {
            return random.Value.Next();
        }
    }
}
