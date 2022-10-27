using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Lights
{
    class MyVirtualPointLight : MyLight
    {
        public V3 LightPosition;
        public V3 LightDirection;
        public int RayNumber;
        public List<V3> RandDirections;


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyVirtualPointLight(V3 lightpos, Couleur couleur, float intensity, V3 direction, int nrayon) :
            base(couleur, intensity)
        {
            LightPosition = lightpos;
            LightDirection = direction;
            RayNumber = nrayon;
            RandDirections = V3.GetRandomDirectionsAlongVector(nrayon,direction);
        }
        public MyVirtualPointLight(V3 lightpos, Couleur couleur, float intensity, int nrayon) :
            base(couleur, intensity)
        {
            LightPosition = lightpos;
            LightDirection = V3.Vnull;
            RayNumber = nrayon;
            RandDirections = V3.GetRandomDirections(nrayon);
        }

        //---------------------------------------
        // autres méthodes :
        //---------------------------------------

        public override V3 GetLightDirOnPoint(V3 point)
        {
            return LightPosition.NormalizedDirectionToVec(point);
        }

        // true by default for non physical lights 
        public override bool CanIlluminatePoint(V3 point)
        {
            V3 dir = GetLightDirOnPoint(point);
            foreach (V3 vdir in RandDirections)
                if (0.999f < dir*vdir && dir*vdir <1)
                    if (IMA.RandP(1.0f)>0.5)
                        return true;
            return false;
        }


    }
}
