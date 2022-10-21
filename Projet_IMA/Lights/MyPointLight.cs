using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Lights

{
    class MyPointLight : MyLight
    {
        public V3 LightPosition;

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyPointLight(V3 lightpos, Couleur couleur, float intensity) :
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
    }
}
