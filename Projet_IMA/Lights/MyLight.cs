using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    abstract class MyLight
    {
        float LightIntensity;
        public Couleur Couleur;
        // si on souhaite controler les calculs d'ombre d'une light :
        public bool CanShadow = true;

        public static List<MyLight> LightsList = new List<MyLight>();


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyLight(Couleur couleur, float intensity)
        {
            Couleur = couleur * intensity;
            LightIntensity = intensity;
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

        public abstract bool IlluminatedUnderPhysicalLight(V3 point);

        public bool ShadowsIfIntersection(List<MyGeometry> geometriesList, MyGeometry geometry, V3 point, Couleur couleur, out Couleur couleurout)
        {
            // the point in argument should be from from the raycasting with the eyeposition (nearest point)
            V3 RayonDirection = GetLightDirOnPoint(point);
            V3 ip;
            float u, v;

            couleurout = couleur;

            if (!CanShadow)
                return false;

            foreach (MyGeometry geom in geometriesList)
            {
                if (!Object.ReferenceEquals(geometry, geom)
                    && geom.RaycastingIntersection(point, -RayonDirection, out u, out v, out ip)
                    && (ip - point) * GetLightDirOnPoint(point) < 0)
                {
                    //couleurout = couleur * (Couleur.White - this.Couleur * 0.9f);
                    couleurout *= 0.5f;
                    return true;
                }
            }
            return false;
        }
    }

}
