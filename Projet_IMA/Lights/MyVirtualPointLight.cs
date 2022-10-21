using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Lights
{
    class MyVirtualPointLight : MyLight
    {
        public V3 LightPosition;

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyVirtualPointLight(V3 lightpos, Couleur couleur, float intensity) :
            base(couleur, intensity)
        {
            LightPosition = lightpos;
        }


        //---------------------------------------
        // autres méthodes :
        //---------------------------------------

        public override V3 GetLightDirOnPoint(V3 point)
        {
            V3 lightdir = point - LightPosition;
            lightdir.Normalize();
            return lightdir;
        }

        // true by default for non physical lights
        public override bool CanIlluminatePoint(V3 point) { return true; }


        private V3 GetRandomDirection()
        {
            float x, y, z;
            float theta = 2 * IMA.PI * IMA.RandP(1.0f);
            float phi = IMA.Cosf(2 * IMA.RandP(1.0f) - 1.0f);
            x = IMA.Cosf(theta) * IMA.Sinf(phi);
            y = IMA.Sinf(theta) * IMA.Sinf(phi);
            z = IMA.Cosf(phi);
            /*
            Console.WriteLine($"x : {x}");
            Console.WriteLine($"y : {y}");
            Console.WriteLine($"z : {z}");
            */
            return new V3(x, y, z);
        }
        public List<V3> GetRandomDirections(int n)
        {
            List<V3> listofdir = new List<V3>();
            for (int i = 0; i < n; i++)  {
                listofdir.Add(GetRandomDirection());
            }
            return listofdir;
        }

        public List<V3> GetRandomDirectionsAlongVector(int n, V3 vector)
        {
            int nb = 0;
            List<V3> returnlist = new List<V3>();
            List<V3> listofdir = GetRandomDirections(n*2);
            foreach (V3 dir in listofdir)
            {
                if (nb >= n)
                    break;

                if (dir * vector > 0)
                {
                    returnlist.Add(dir);
                    nb++;
                }
            }
            return returnlist;
        }
    }
}
