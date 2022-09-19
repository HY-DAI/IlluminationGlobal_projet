using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyRF // my rendering functions
    {

        public static void Calcul_normals_with_bump(MyGeometry geometry, out List<V3> normals)
        {
            List<V3> vnormals = new List<V3>();
            for (int i = 0; i < geometry.Maillage.Points.Count; i++)
            {
                V3 point = geometry.Maillage.Points[i];
                float unormalized, vnormalized;
                float dhdu, dhdv;
                V3 dmdu, dmdv;
                geometry.CalculateDifferentialUV(point,out unormalized, out vnormalized, out dmdu, out dmdv);
                geometry.Material.BumpMap.Bump(unormalized, vnormalized, out dhdu, out dhdv);
                V3 Nuv = (dmdu ^ dmdv) / (dmdu ^ dmdv).Norm();
                V3 T2 = dhdu * (Nuv ^ dmdv);
                V3 T3 = dhdv * (dmdu ^ Nuv);
                vnormals.Add(Nuv + (T2 + T3) * geometry.Material.BumpIntensity);
            }
            normals = vnormals;
        }
            
        public static void Calcul_diffuse_speculaire(List<MyLight> lightsList, MyGeometry geometry, V3 eyePosition, out List<V3> points, out List<Couleur> couleurs)
        {
            points = geometry.Maillage.Points;
            List<Couleur> vcouleurs = new List<Couleur>();

            // Normalized vectors : L, R, N, D 
            V3 lightDirection;
            V3 reflectedLightDirection;
            V3 normal;
            V3 point2eyesDirection;
            // Colors that will be added together
            Couleur CAmb;
            Couleur CDiffuse;
            Couleur CSpeculaire;

            for (int i = 0; i < points.Count; i++)
            {
                V3 point = geometry.Maillage.Points[i];
                Couleur couleur = geometry.Maillage.Couleurs[i];
                Couleur CFinale = Couleur.Black;
                foreach (MyLight light in lightsList)
                {
                    CAmb = light.Couleur;
                    lightDirection = light.LightDirection;

                    // normal and then normalwithbumps :
                    normal = geometry.Maillage.Normals[i];
                    point2eyesDirection = point.NormalizedDirectionToVec(eyePosition);

                    // to set R correctly, check if the light actually hits the surface
                    reflectedLightDirection = new V3(0,0,0); //ou lightDirection;
                    if (V3.prod_scal(normal, -lightDirection) > 0)
                    {
                        reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                    }
                    CDiffuse = couleur * CAmb * V3.prod_scal(normal, -lightDirection);
                    CSpeculaire = couleur * CAmb * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), geometry.Material.SpecularPower);
                    CDiffuse.check(); 
                    CSpeculaire.check();
                    CFinale += couleur * CAmb + CDiffuse + CSpeculaire;
                }
                vcouleurs.Add(CFinale);
            }
            couleurs = vcouleurs;
        }


        public static void Calcul_diffuse_speculaire(List<MyLight> lightsList, MyGeometry geometry, V3 eyePosition, V3 point, out Couleur couleur)
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
            geometry.CalculateDifferentialUV(point, out u, out v, out dmdu, out dmdv);
            couleur = geometry.Material.ColorMap.LireCouleur(u, v); ;
            /*
                        // projection orthographique => repère écran
                        int x_ecran = (int)(point.x);
                        int y_ecran = (int)(point.z);
                        BitmapEcran.DrawPixel(x_ecran, y_ecran, couleur);
            */
            Couleur Cinterm = Couleur.Black;
            foreach (MyLight light in lightsList)
            {
                CAmb = light.Couleur;
                lightDirection = light.LightDirection;
                point2eyesDirection = point.NormalizedDirectionToVec(eyePosition);

                // normal and then normalwithbumps :
                float dhdu, dhdv;
                geometry.Material.BumpMap.Bump(u, v, out dhdu, out dhdv);
                V3 Nuv = (dmdu ^ dmdv) / (dmdu ^ dmdv).Norm();
                V3 T2 = dhdu * (Nuv ^ dmdv);
                V3 T3 = dhdv * (dmdu ^ Nuv);
                normal = Nuv + (T2 + T3) * geometry.Material.BumpIntensity;


                // to set R correctly, check if the light actually hits the surface
                reflectedLightDirection = lightDirection;
                if (V3.prod_scal(normal, -lightDirection) > 0)
                {
                    reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                }
                CDiffuse = couleur * CAmb * V3.prod_scal(normal, -lightDirection);
                CSpeculaire = couleur * CAmb * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), geometry.Material.SpecularPower);
                CDiffuse.check();
                CSpeculaire.check();
                Cinterm += couleur * CAmb + CDiffuse + CSpeculaire;
            }
            couleur = Cinterm;
        }


        public static void DrawGeometry(List<V3> points, List<Couleur> couleurs)
        {
            foreach (var pointcolore in points.Zip(couleurs, Tuple.Create))
            {
                V3 point = pointcolore.Item1;
                Couleur couleur = pointcolore.Item2;

                // projection orthographique => repère écran
                int x_ecran = (int)(point.x);
                int y_ecran = (int)(point.z);
                BitmapEcran.DrawPixel(x_ecran, y_ecran, couleur);
            }
        }

        public static void Draw(List<MyLight> lightsList, List<MyGeometry> geometriesList, V3 eyePosition)
        {
            int nbpx = 1000; // 910;
            int nbpz = 600; // 540;
            float u, v;

            V3 NearestIntersection;
            Couleur NearestCouleur;

            for (int px = 0; px < nbpx; px++)
            {
                for (int pz = 0; pz < nbpz; pz++)
                {
//                    bool interactionExists = false;
                    V3 RayonDirection = eyePosition.NormalizedDirectionToVec(new V3(px, 0, pz));
                    NearestIntersection = eyePosition + 7000 * RayonDirection; //val arbi grande
                    NearestCouleur = Couleur.White * 0.1f;
                    foreach (MyGeometry geometry in geometriesList)
                    {

                        if (geometry.RaycastingIntersection(eyePosition, RayonDirection, out u, out v))
                        {
                            V3 P3D = geometry.get3DPoint(u, v);
                            if ((P3D - eyePosition).Norm() < (NearestIntersection - eyePosition).Norm())
                            {
                                NearestIntersection = P3D;
                                Calcul_diffuse_speculaire(lightsList, geometry, eyePosition, P3D, out NearestCouleur);
                            }
                        }


                        /*   
                           geometry.RaycastingIntersection(eyePosition, RayonDirection, out u, out v);
                           V3 P3D = geometry.get3DPoint(u, v);
                           if ((P3D - eyePosition).Norm() < (NearestIntersection - eyePosition).Norm())
                           {
                               NearestIntersection = P3D;
                               Calcul_diffuse_speculaire(lightsList, geometry, eyePosition, P3D, out NearestCouleur);
                           }
                        */

                    }
/*
                    // projection orthographique => repère écran
                    int x_ecran = (int)(NearestIntersection.x);
                    int y_ecran = (int)(NearestIntersection.z);
*/
                    BitmapEcran.DrawPixel(px,pz, NearestCouleur);

                }
            }
        }
    }
}
