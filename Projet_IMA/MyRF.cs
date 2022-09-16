using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyRF // my rendering functions
    {


        public static void calcul_diffuse_speculaire(MyLight light, MyGeometry geometry, V3 eyePosition)
        {
            // Normalized vectors : L, R, N, D 
            V3 lightDirection = light.LightDirection;
            V3 reflectedLightDirection; 
            V3 normal;
            V3 point2eyesDirection;
            // Colors that will be added together
            Couleur CAmb = light.Couleur * light.LightIntensity;
            Couleur CDiffuse;
            Couleur CSpeculaire;

            for (int i = 0; i < geometry.Maillage.Points.Count; i++)
            {
                V3 point = geometry.Maillage.Points[i];
                Couleur couleur = geometry.Maillage.Couleurs[i];

                // normal and then normalwithbumps :
                // normal = geometry.GetNormalOfPoint(point);
                normal = geometry.Maillage.Normals[i];
                point2eyesDirection = point.NormalizedDirectionToVec(eyePosition);

                reflectedLightDirection = lightDirection;
                // check if the light actually hits the surface
                if (V3.prod_scal(normal, -lightDirection) > 0)
                {
                    reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                }
                CDiffuse = couleur * light.Couleur * V3.prod_scal(normal, -lightDirection);
                CSpeculaire = light.Couleur * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), geometry.Material.SpecularPower);
                CDiffuse.check();
                CSpeculaire.check();
                geometry.Maillage.Couleurs[i] = couleur * CAmb + CDiffuse + CSpeculaire;
            }
        }


        public static void Draw(MyGeometry geometry, float pas)
        {
            foreach (var pointcolore in geometry.Maillage.Points.Zip(geometry.Maillage.Couleurs, Tuple.Create))
            {
                V3 point = pointcolore.Item1;
                Couleur couleur = pointcolore.Item2;

                // projection orthographique => repère écran
                int x_ecran = (int)(point.x);
                int y_ecran = (int)(point.z);
                BitmapEcran.DrawPixel(x_ecran, y_ecran, couleur);
            }
        }
    }
}
