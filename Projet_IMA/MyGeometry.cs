using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    abstract class MyGeometry
    {
        public Maillage Maillage;
        //public Couleur Couleur;
        public Texture Texture;
        public int SpecularPower = 50;


        public abstract V3 GetNormalOfPoint(V3 point);

        public static void calcul_diffuse_speculaire(MyLight light, MyGeometry geometry, V3 eyePosition)
        {
            // Normalized vectors
            V3 lightDirection = light.LightDir;
            V3 reflectedLightDirection;;
            V3 normal;
            V3 point2eyesDirection;
            // Colors that will be added together
            Couleur Cdiffuse;
            Couleur Cspeculaire;
            // local point and its color
            V3 point;
            Couleur couleur;

            for (int i=0; i< geometry.Maillage.Points.Count; i++) {
                point = geometry.Maillage.Points[i];
                couleur = geometry.Maillage.Couleurs[i];

                normal = geometry.GetNormalOfPoint(point);
                point2eyesDirection = point.NormalizedDirectionToVec(eyePosition); 

                reflectedLightDirection = lightDirection;
                // check if the light actually hits the surface
                if (V3.prod_scal(normal, -lightDirection)>0)
                {
                    reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                }
                Cdiffuse = couleur * light.Couleur * V3.prod_scal(normal, -lightDirection);
                Cspeculaire = light.Couleur * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), geometry.SpecularPower);
                geometry.Maillage.Couleurs[i] = /*couleur * light.Couleur +*/  Cdiffuse + Cspeculaire;
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
