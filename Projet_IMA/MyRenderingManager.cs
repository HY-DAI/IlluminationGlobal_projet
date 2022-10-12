using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Projet_IMA.Geometries;
using Projet_IMA.Geometries.GeometryComponents;
using Projet_IMA.Lights;
using Projet_IMA.Lights.GeometryLights;

namespace Projet_IMA
{
    class MyRenderingManager 
    {

        static int largeurEcran = 960;
        static int hauteurEcran = 570;

        public static V3 eyeLocation = new V3(largeurEcran / 2, -largeurEcran, hauteurEcran / 2);
        public static float RayonRendering = 7000;


        /*
                public static void DrawPoints(List<V3> points, List<Couleur> couleurs)
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
        */

        public static void Draw(List<MyLight> lightsList, List<MyGeometry> geometriesList, V3 eyePosition)
        {
            int nbpx = 1000; // 910;
            int nbpz = 600; // 540;

            V3 NearestIntersection;
            Couleur NearestCouleur;
            MyGeometry IntersectedGeometry;

            for (int px = 0; px < nbpx; px++)
            {
                for (int pz = 0; pz < nbpz; pz++)
                {
                    V3 RayonDirection = eyePosition.NormalizedDirectionToVec(new V3(px, 0, pz));
                    FindNearestIntersectionAndGeometry(geometriesList, eyePosition, RayonDirection, out NearestIntersection, out IntersectedGeometry);
                    Calcul_diffuse_speculaire_bumps(geometriesList, lightsList, eyePosition, NearestIntersection, IntersectedGeometry, out NearestCouleur);

                    BitmapEcran.DrawPixel(px,pz, NearestCouleur);
                }
            }

        }

        public static void FindNearestIntersectionAndGeometry(List<MyGeometry> geometriesList, V3 eyePosition, V3 rayonDirection, out V3 NearestIntersection, out MyGeometry IntersectedGeometry)
        {
            V3 intersection;
            NearestIntersection = eyePosition + RayonRendering * rayonDirection;
            IntersectedGeometry = geometriesList[0];
            foreach (MyGeometry geometry in geometriesList)
            {
                if (geometry.RaycastingIntersection(eyePosition, rayonDirection, out intersection))
                {
                    if ((intersection - eyePosition).Norm() < (NearestIntersection - eyePosition).Norm())
                    {
                        NearestIntersection = intersection;
                        IntersectedGeometry = geometry;
                    }
                }
            }
        }

        public static void Calcul_diffuse_speculaire_bumps(List<MyGeometry> geometriesList, List<MyLight> lightsList, V3 eyePosition, V3 uncoloredPoint, MyGeometry geometry, out Couleur couleur)
        {
            // Normalized vectors : L, R, N, D 
            V3 lightDirection;
            V3 reflectedLightDirection;
            V3 normal;
            V3 point2eyesDirection;

            float u, v;
            V3 dmdu, dmdv;
            // calculate u, v, dmdu, dmdv :
            geometry.CalculateUV(uncoloredPoint, out u, out v);
            geometry.CalculateDifferentialUV(uncoloredPoint, out dmdu, out dmdv);
            couleur = geometry.Material.ColorMap.LireCouleur(u, v); 
            // calculate normal with bumps :
            normal = geometry.GetNormalOfPoint(uncoloredPoint);
            normal = geometry.GetNormalWithBump(normal, u, v, dmdu, dmdv);
            // calculate invert ray direction :
            point2eyesDirection = uncoloredPoint.NormalizedDirectionToVec(eyePosition);


            Couleur Cinterm = Couleur.Black;
            foreach (MyLight light in lightsList)
            {
                Couleur CAmb = light.Couleur;
                lightDirection = light.GetLightDirOnPoint(uncoloredPoint);

                // particulierement pour une RectLight
                if (!light.IlluminatedUnderPhysicalLight(uncoloredPoint))
                    continue;

                // occlusion management :
                if (!ShadowsUnderLight(geometriesList, geometry, light, uncoloredPoint, couleur, out Couleur Cocclusion))
                {
                    // diffuse and specular management :
                    reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                    Couleur CDiffuse = couleur * CAmb * V3.prod_scal(normal, -lightDirection);
                    Couleur CSpeculaire = couleur * CAmb * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), geometry.Material.SpecularPower);

                    CDiffuse.check();
                    CSpeculaire.check();
                    Cinterm += CDiffuse + CSpeculaire;
                }

                // par défaut Cocclusion = couleur
                Cinterm += Cocclusion * CAmb;
            }
            couleur = Cinterm;
        }


        public static bool ShadowsUnderLight(List<MyGeometry> geometriesList, MyGeometry geometry, MyLight light, V3 point, Couleur couleur, out Couleur couleurout)
        {
            // the point in argument should be from from the raycasting with the eyeposition (nearest point)
            V3 RayonDirection = light.GetLightDirOnPoint(point);
            V3 intersection;

            couleurout = couleur;

            if (!light.CanShadow)
                return false;

            foreach (MyGeometry geom in geometriesList)
            {
                bool occlusion = geom.RaycastingIntersection(point, -RayonDirection, out intersection);
                occlusion = occlusion && (intersection - point) * RayonDirection < 0 ;
                bool ownhiddenface = geometry.GetNormalOfPoint(point) * RayonDirection > 0;

                if (ownhiddenface || !Object.ReferenceEquals(geometry, geom) && occlusion )
                {
                    // moins light intense , moins couleur intense : 
                    couleurout *= light.GetIntensity();
                    couleurout.check();
                    return true;
                }
            }
            return false;
        }

        public void CalculateLightBounces(List<MyGeometry> geometriesList, MyLight light) 
        {
            
        }

    }
}
