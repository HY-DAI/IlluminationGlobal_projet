using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Lights
{
    class MyAmbiantLight : MyLight
    {

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyAmbiantLight(Couleur couleur, float intensity) :
            base(couleur, intensity)
        {
            CanShadow = false;
        }


        //---------------------------------------
        // autres méthodes :
        //---------------------------------------

        public override V3 GetLightDirOnPoint(V3 point)
        {
            return V3.Vnull;
        }

        // true by default for non physical lights
        public override bool CanIlluminatePoint(V3 point) { return true; }
    }
}
