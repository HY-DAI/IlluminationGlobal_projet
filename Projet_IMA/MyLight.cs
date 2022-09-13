using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyLight
    {
        public float LightIntensity;
        public Couleur Couleur;
        public V3 LightDir;

        public MyLight(V3 light_dir, Couleur couleur, float intensity)
        {
            LightDir = light_dir;
            Couleur = couleur;
            LightIntensity = intensity;
        }
        public MyLight(V3 light_dir, Couleur couleur) : this(light_dir,couleur,1f)  { }
    }

}
