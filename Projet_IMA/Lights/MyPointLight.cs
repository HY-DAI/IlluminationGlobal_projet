using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Lights
{
    class MyPointLight : MyLight
    {

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyPointLight(V3 lightpos, V3 lightdir, Couleur couleur, float intensity) :
            base(lightpos, lightdir, couleur, intensity)
        { }
    
    }
}
