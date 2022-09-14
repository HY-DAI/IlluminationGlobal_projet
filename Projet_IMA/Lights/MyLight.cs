using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    abstract class MyLight
    {
        public Couleur Couleur;
        public float LightIntensity;
        public V3 LightDirection;


        public MyLight(Couleur couleur, float intensity, V3 lightdir)
        {
            Couleur = couleur;
            LightIntensity = intensity;
            LightDirection = lightdir;
        }

        public MyLight(Couleur couleur, float intensity) :
            this(couleur,intensity,new V3(1,-1,1))
        { }

        public MyLight(Couleur couleur) : this(couleur,1f)  { }
    }

}
