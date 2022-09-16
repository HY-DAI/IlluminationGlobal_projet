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
                V3 dmdu, dmdv;
                geometry.CalculateDifferentialUV(point,out unormalized, out vnormalized, out dmdu, out dmdv);
                float dhdu, dhdv;
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
                    reflectedLightDirection = lightDirection;
                    if (V3.prod_scal(normal, -lightDirection) > 0)
                    {
                        reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                    }
                    CDiffuse = couleur * CAmb * V3.prod_scal(normal, -lightDirection);
                    CSpeculaire = couleur * CAmb * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), geometry.Material.SpecularPower);
                    CDiffuse.check(); CSpeculaire.check();
                    CFinale += couleur * CAmb + CDiffuse + CSpeculaire;
                }
                vcouleurs.Add(CFinale);
            }
            couleurs = vcouleurs;
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
            List<V3> points;
            List<Couleur> couleurs;

            foreach (MyGeometry geometry in geometriesList)
            {
                Calcul_diffuse_speculaire(lightsList, geometry, eyePosition, out points, out couleurs);
                DrawGeometry(points, couleurs);
            }
        }
    }
}
