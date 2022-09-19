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

        public abstract V3 GetNormalOfPoint(V3 point);

        public abstract void CalculateDifferentialUV(V3 point, out float u, out float v, out V3 dmdu, out V3 dmdv);

        public abstract bool RaycastingIntersection(V3 RayonOrigine, V3 RayonDirection, out float u, out float v);

        public abstract V3 get3DPoint(float u, float v);

/*
        public void Calcul_normals_with_bump(MyGeometry geometry, out List<V3> normals)
        {
            List<V3> vnormals = new List<V3>();
            for (int i = 0; i < geometry.Maillage.Points.Count; i++)
            {
                V3 point = geometry.Maillage.Points[i];
                float unormalized, vnormalized;
                float dhdu, dhdv;
                V3 dmdu, dmdv;
                this.CalculateDifferentialUV(point, out unormalized, out vnormalized, out dmdu, out dmdv);
                this.Material.BumpMap.Bump(unormalized, vnormalized, out dhdu, out dhdv);
                V3 Nuv = (dmdu ^ dmdv) / (dmdu ^ dmdv).Norm();
                V3 T2 = dhdu * (Nuv ^ dmdv);
                V3 T3 = dhdv * (dmdu ^ Nuv);
                vnormals.Add(Nuv + (T2 + T3) * geometry.Material.BumpIntensity);
            }
            normals = vnormals;
        }
*/


        public void Calcul_diffuse_speculaire_bumps(List<MyLight> lightsList, V3 eyePosition, V3 point, out Couleur couleur)
        {
            // Normalized vectors : L, R, N, D 
            V3 lightDirection;
            V3 reflectedLightDirection;
            V3 normal;
            V3 point2eyesDirection;
            // Colors that will be added together
            Couleur CAmb;
            Couleur CDiffuse;
            Couleur CSpeculaire;

            float u, v;
            V3 dmdu, dmdv;
            CalculateDifferentialUV(point, out u, out v, out dmdu, out dmdv);
            couleur = Material.ColorMap.LireCouleur(u, v); ;

            Couleur Cinterm = Couleur.Black;
            foreach (MyLight light in lightsList)
            {
                CAmb = light.Couleur;
                lightDirection = light.LightDirection;
                point2eyesDirection = point.NormalizedDirectionToVec(eyePosition);

                // normal and then normal_with_bumps :
                float dhdu, dhdv;
                Material.BumpMap.Bump(u, v, out dhdu, out dhdv);
                V3 Nuv = (dmdu ^ dmdv) / (dmdu ^ dmdv).Norm();
                if (Nuv * point2eyesDirection < 0)
                    Nuv = -Nuv;
                V3 T2 = dhdu * (Nuv ^ dmdv);
                V3 T3 = dhdv * (dmdu ^ Nuv);
                normal = Nuv + (T2 + T3) * Material.BumpIntensity;

                // to set R correctly, check if the light actually hits the surface
                reflectedLightDirection = lightDirection;
                if (V3.prod_scal(normal, -lightDirection) > 0)
                {
                    reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                }
                CDiffuse = couleur * CAmb * V3.prod_scal(normal, -lightDirection);
                CSpeculaire = couleur * CAmb * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), Material.SpecularPower);
                CDiffuse.check();
                CSpeculaire.check();
                Cinterm += couleur * CAmb + CDiffuse + CSpeculaire;
            }
            couleur = Cinterm;
        }


    }
}
