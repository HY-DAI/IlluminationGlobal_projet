using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    abstract class MyGeometry
    {
        //public MyMaillage Maillage;
        public MyMaterial Material;

        public static List<MyGeometry> GeometriesList = new List<MyGeometry>();


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------
        public MyGeometry()
        {
            MyGeometry.GeometriesList.Add(this);
        }


        //---------------------------------------
        // public méthodes :
        //---------------------------------------

        public abstract V3 GetBarycenter();

        public abstract V3 Get3DPoint(float u, float v);

        public abstract V3 GetNormalOfPoint(V3 point);

        public abstract void CalculateDifferentialUV(V3 point, out float u, out float v, out V3 dmdu, out V3 dmdv);


        public abstract bool RaycastingIntersection(V3 RayonOrigine, V3 RayonDirection, out float u, out float v, out V3 intersection);

        public bool RaycastingIntersection(V3 RayonOrigine, V3 RayonDirection)
        {
            float u, v;
            V3 intersection;
            return RaycastingIntersection(RayonOrigine, RayonDirection, out u, out v, out intersection);
        }


        public V3 GetNormalWithBump(V3 normal, float u, float v, V3 dmdu, V3 dmdv)
        {
            float dhdu, dhdv;
            Material.BumpMap.Bump(u, v, out dhdu, out dhdv);
            V3 Nuv = (dmdu ^ dmdv) / (dmdu ^ dmdv).Norm();
            if (Nuv * normal < 0)
                Nuv = -Nuv;
            V3 T2 = dhdu * (Nuv ^ dmdv);
            V3 T3 = dhdv * (dmdu ^ Nuv);
            return Nuv + (T2 + T3) * Material.BumpIntensity;
        }


        public void Calcul_diffuse_speculaire_bumps(List<MyLight> lightsList, V3 eyePosition, V3 uncoloredPoint, out Couleur couleur)
        {
            // Normalized vectors : L, R, N, D 
            V3 lightDirection;
            V3 reflectedLightDirection;
            V3 normal;
            V3 point2eyesDirection;

            float u, v;
            V3 dmdu, dmdv;
            //V3 intersectionPointOcclusion;
            CalculateDifferentialUV(uncoloredPoint, out u, out v, out dmdu, out dmdv);
            couleur = Material.ColorMap.LireCouleur(u, v); ;

            // Colors that will be added together
            Couleur CAmb;
            Couleur CDiffuse;
            Couleur CSpeculaire;
            Couleur Cocclusion;
            Couleur Cinterm = Couleur.Black;

            foreach (MyLight light in lightsList)
            {
                CAmb = light.Couleur;
                normal = GetNormalOfPoint(uncoloredPoint);
                lightDirection = light.GetLightDirOnPoint(uncoloredPoint);
                point2eyesDirection = uncoloredPoint.NormalizedDirectionToVec(eyePosition);

                // particulierement pour une RectLight
                if (!light.IlluminatedUnderPhysicalLight(uncoloredPoint))
                    continue;

                // occlusion management :
                if (light.ShadowsIfIntersection(GeometriesList, this, uncoloredPoint, couleur, out Cocclusion))
                {
                    Cinterm += Cocclusion;// * CAmb;
                    continue;
                }

                // normal with bumps : les u,v,dmdu,dmdv are from the function CalculateDifferentialUV
                normal = GetNormalWithBump(normal, u, v, dmdu, dmdv);

                // si la face n'est pas éclairée dans le bon sens
                if (V3.prod_scal(normal, -lightDirection) < 0)
                    reflectedLightDirection = V3.Vnull;

                // diffuse and specular management :
                reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                CDiffuse = couleur * CAmb * V3.prod_scal(normal, -lightDirection);
                CSpeculaire = couleur * CAmb * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), Material.SpecularPower);
                
                CDiffuse.check();
                CSpeculaire.check();

                Cinterm += couleur*CAmb + CDiffuse + CSpeculaire;
            }
            couleur = Cinterm;
        }
    }
}
