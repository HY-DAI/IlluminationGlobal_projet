using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyRF // my rendering functions
    {

  /*
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
*/

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
                                geometry.Calcul_diffuse_speculaire_bumps(lightsList, eyePosition, P3D, out NearestCouleur);
                                foreach (MyLight light in lightsList)
                                {
                                    if (geometry.RaycastingIntersection(light.LightPosition, RayonDirection, out u, out v)) { }
                                }
                            }
                        }
                    }
                    BitmapEcran.DrawPixel(px,pz, NearestCouleur);
                }
            }
        }
    }
}
