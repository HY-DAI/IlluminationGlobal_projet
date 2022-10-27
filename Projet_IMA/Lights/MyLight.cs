using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Lights
{
    abstract class MyLight
    {
        float LightIntensity;
        public Couleur Couleur;
        // contingents :
        public bool CanShadow;

        public static List<MyLight> LightsList = new List<MyLight>();


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyLight(Couleur couleur, float intensity)
        {
            Couleur = couleur * intensity;
            LightIntensity = intensity;
            CanShadow = true;
            MyLight.LightsList.Add(this);
        }

        public MyLight(Couleur couleur) : this(couleur, 1f)  { }

        public MyLight() : this(Couleur.White) { }


        //---------------------------------------
        // Autres méthodes :
        //---------------------------------------

        public float GetIntensity()
        {
            return LightIntensity;
        }

        public void SetIntensity(float intensity)
        {
            Couleur /= LightIntensity;
            Couleur *= intensity;
        }

        public abstract V3 GetLightDirOnPoint(V3 point);

        public abstract bool CanIlluminatePoint(V3 point);


        public static void splitLightsByClass(List<MyLight> lightlist, Type type, out List<MyLight> returnlistYes, out List<MyLight> returnlistNo)
        {
            returnlistYes = new List<MyLight>();
            returnlistNo = new List<MyLight>();
            foreach (MyLight light in lightlist)
            {
                if (light.GetType().Equals(type))
                    returnlistYes.Add(light);
                else returnlistNo.Add(light);
            }
        }

        public static List<MyLight> getLightsOfClass(List<MyLight> lightlist, Type type)
        {
            splitLightsByClass(lightlist, type, out List<MyLight> returnlistYes, out List<MyLight> returnlistNo);
            return returnlistYes;
        }

        public static List<MyLight> deleteLightsOfClass(List<MyLight> lightlist, Type type)
        {
            splitLightsByClass(lightlist, type, out List<MyLight> returnlistYes, out List<MyLight> returnlistNo);
            foreach (MyLight light in returnlistYes)
            {
                if (MyLight.LightsList.Contains(light))
                    LightsList.Remove(light);
            }
            return returnlistNo;
        }

    }

}
