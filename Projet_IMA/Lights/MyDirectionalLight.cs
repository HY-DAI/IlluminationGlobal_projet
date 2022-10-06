using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyDirectionalLight : MyLight
    {
        V3 LightDirection;

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyDirectionalLight(V3 lightdir, Couleur couleur, float intensity) :
            base(couleur, intensity)
        {
            LightDirection = lightdir;
            LightDirection.Normalize();
        }


        //---------------------------------------
        // autres méthodes :
        //---------------------------------------

        public override V3 GetLightDirOnPoint(V3 point) { return LightDirection; }


        // true by default for non physical lights
        public override bool IlluminatedUnderPhysicalLight(V3 point) { return true; }

    }
}
