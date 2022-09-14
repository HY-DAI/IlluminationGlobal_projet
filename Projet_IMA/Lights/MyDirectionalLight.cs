using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyDirectionalLight : MyLight
    {


        public MyDirectionalLight(Couleur couleur, float intensity, V3 lightdir) :
            base(couleur, intensity, lightdir)
        {  }
    }
}
