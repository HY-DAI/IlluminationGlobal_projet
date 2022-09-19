using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    abstract class MyLight
    {
        public V3 LightPosition;
        public V3 LightDirection;
        public float LightIntensity;
        public Couleur Couleur;

        public static List<MyLight> LightsList = new List<MyLight>();


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyLight(V3 lightpos, V3 lightdir, Couleur couleur, float intensity)
        {
            LightPosition = lightpos;
            LightDirection = lightdir;
            Couleur = couleur*intensity;
            LightIntensity = intensity;
            check();
            MyLight.LightsList.Add(this);
        }

        public MyLight(V3 lightdir, Couleur couleur, float intensity) :
            this(new V3(0,0,0),lightdir, couleur, intensity)
        { }

        public MyLight(Couleur couleur, float intensity) :
            this(new V3(-1,1,-1), couleur, intensity)
        { }

        public MyLight(Couleur couleur) : this(couleur, 1f)  { }

        public MyLight() : this(Couleur.White) { }


        //---------------------------------------
        // Autres méthodes :
        //---------------------------------------

        public void setIntensity(float intensity)
        {
            Couleur /= LightIntensity;
            Couleur *= intensity;
        }

        private void check()
        {
            LightDirection.Normalize();
        }
    }

}
