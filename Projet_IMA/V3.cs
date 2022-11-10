using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    struct V3
    {
        public float x;		// coordonnées du vecteur
        public float y;
        public float z;

        public static V3 Vnull = new V3(0, 0, 0);

        public float Norm()		// retourne la norme du vecteur
        {
            return (float) IMA.Sqrtf(x * x + y * y + z * z);
        }

        public float Norme2()
        {
            return x * x + y * y + z * z;
        }

        public void Normalize()	// normalise le vecteur
        {
            float n = Norm();
            if (n == 0) return;
            x /= n;
            y /= n;
            z /= n;
        }

        public V3 NormalizedDirectionToVec(V3 B)
        {
            V3 dir = B - this;
            dir.Normalize();
            return dir;
        }

        public V3(V3 t)
        {
            x = t.x;
            y = t.y;
            z = t.z;
        }

        public V3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        V3(int _x, int _y, int _z)
        {
            x = (float)_x;
            y = (float)_y;
            z = (float)_z;
        }

        public static V3 operator +(V3 a, V3 b)
        {
            V3 t;
            t.x = a.x + b.x;
            t.y = a.y + b.y;
            t.z = a.z + b.z;
            return t;
        }

        public static V3 operator -(V3 a, V3 b)
        {
            V3 t;
            t.x = a.x - b.x;
            t.y = a.y - b.y;
            t.z = a.z - b.z;
            return t;
        }

        public static V3 operator -(V3 a)
        {
            V3 t;
            t.x = -a.x;
            t.y = -a.y;
            t.z = -a.z;
            return t;
        }

        public static V3 operator ^ (V3 a, V3 b)  // produit vectoriel
        {
            V3 t;
            t.x = a.y * b.z - a.z * b.y;
            t.y = a.z * b.x - a.x * b.z;
            t.z = a.x * b.y - a.y * b.x;
            return t;
        }

        public static float operator * (V3 a,V3 b)         // produit scalaire
        {
            return a.x*b.x+a.y*b.y+a.z*b.z;
        }

       

        public static V3 operator *(float a, V3 b)
        {
            V3 t;
            t.x = b.x*a;
            t.y = b.y*a;
            t.z=  b.z*a;
            return t;
        }

        public static V3 operator *(V3 b, float a)
        {
            V3 t;
            t.x = b.x*a;
            t.y = b.y*a;
            t.z=  b.z*a;
            return t;
        }

        public static V3 operator /(V3 b, float a)
        {
            V3 t;
            t.x = b.x/a;
            t.y = b.y/a;
            t.z=  b.z/a;
            return t;
        }

        public static float prod_scal(V3 u, V3 v)
        {
            return u.x * v.x + u.y * v.y + u.z * v.z;
        }

        public static bool colinear(V3 u, V3 v)
        {
            return u * v - u.Norm() * v.Norm() < 0.001;
        }
        public static bool orthogonal(V3 u, V3 v)
        {
            return u * v < 0.001;
        }

        public static float distance(V3 u, V3 v)
        {
            return (u - v).Norm();
        }


        public static V3 GetRandomDirection()
        {
            float x, y, z;
            float theta = 2 * IMA.PI * IMA.RandP(1.0f);
            float phi = IMA.Acosf(2 * IMA.RandP(1.0f) - 1.0f);
            x = IMA.Cosf(theta) * IMA.Sinf(phi);
            y = IMA.Sinf(theta) * IMA.Sinf(phi);
            z = IMA.Cosf(phi);
            return new V3(x, y, z);
        }

        public static List<V3> GetRandomDirections(int n)
        {
            List<V3> listofdir = new List<V3>();
            for (int i = 0; i < n; i++)
            {
                V3 dir = GetRandomDirection();
                dir.Normalize();
                listofdir.Add(dir);
            }
            return listofdir;
        }

        public static List<V3> GetRandomDirectionsAlongVector(int n, V3 vector)
        {
            List<V3> returnlist = new List<V3>();
            List<V3> listofdir = GetRandomDirections(n);
            foreach (V3 dir in listofdir)
            {
                if (dir * vector > 0)
                    returnlist.Add(dir);
                else
                    returnlist.Add(-dir);
            }
            //Console.WriteLine($"list size = {returnlist.Count()}");
            return returnlist;
        }

    }
}
