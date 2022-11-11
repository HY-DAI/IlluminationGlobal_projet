using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Projet_IMA.Geometries;
using Projet_IMA.Lights;
using System.Drawing;
using System.Threading;

namespace Projet_IMA
{

    class MyRenderingManager
    {

        public static int largeurEcran = 960;
        public static int hauteurEcran = 570;
        public static V3 eyeLocation = new V3(largeurEcran / 2, -largeurEcran, hauteurEcran / 2);

        public static string map = "everything";
        public static float RenderingMaxDist = 7000;
        public static int pathtracingLevel = 2;
        public static int pathtracingRays = 0;

        public static int raytracingMaxSteps = 2;

        public static Bitmap SetBitmapPixels(List<MyLight> lightsList, List<MyGeometry> geometriesList, Point coordZone, int LargZonePix)
        {
            Bitmap B = new Bitmap(LargZonePix, LargZonePix);
            int pxmin = coordZone.X;
            int pzmin = coordZone.Y;
            int pxmax = coordZone.X + LargZonePix;
            int pzmax = coordZone.Y + LargZonePix;

            for (int px = pxmin; px < pxmax; px++)
            {
                for (int pz = pzmin; pz < pzmax; pz++)
                {
                    CalculatePixelColorNiv2(lightsList, geometriesList, px, pz, out Couleur calculatedColor);
                    B.SetPixel(px - pxmin, pzmax - pz - 1, calculatedColor.Convertion());
                    //pzmax-pz-1 au lieu de pz-pzmin , car il semble avoir inversion de repère /z dans canvas...
                }
            }
            return B;
        }

        public static void CalculatePixelColorNiv2(List<MyLight> lightsList, List<MyGeometry> geometriesList, int px, int pz, out Couleur couleur)
        {
            if (pathtracingLevel == 1)
                pathtracingRays = 0;

            couleur = Couleur.Black;
            Couleur CDiffusNiv2 = Couleur.Black;

            V3 RayonDirection = eyeLocation.NormalizedDirectionToVec(new V3(px, 0, pz));
            if (FindNearestIntersectionAndGeometry(geometriesList, eyeLocation, RayonDirection,
                out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
            {

                V3 normal = IntersectedGeometry.GetNormalOfPoint(NearestIntersection);
                List<V3> randDir = V3.GetRandomDirectionsAlongVector(pathtracingRays, normal);
                foreach (V3 dir in randDir)
                {
                    if (!FindNearestIntersectionAndGeometry(geometriesList, NearestIntersection, dir,
                        out V3 IntersectionOfRaycast2, out MyGeometry GeometryOfRaycast2)) {
                        continue;
                    }
                    ColorDependingOfMap(geometriesList, lightsList, NearestIntersection,
                        IntersectionOfRaycast2, GeometryOfRaycast2, out Couleur CDiffSpecNiv1,0);

                    CDiffusNiv2 += (normal * dir) * CDiffSpecNiv1;
                }
                if (pathtracingRays!=0)  CDiffusNiv2 /= pathtracingRays;

                ColorDependingOfMap(geometriesList, lightsList, eyeLocation,
                NearestIntersection, IntersectedGeometry, out Couleur CAmbDiffSpec,0);

                couleur = CAmbDiffSpec + CDiffusNiv2;
            }
        }

        public static bool FindNearestIntersectionAndGeometry(List<MyGeometry> geometriesList,
            V3 rayonOrigine, V3 rayonDirection, out V3 NearestIntersection, out MyGeometry IntersectedGeometry)
        {
            bool NearIntersectionExists = false;
            NearestIntersection = rayonOrigine + RenderingMaxDist * rayonDirection;
            IntersectedGeometry = geometriesList[0];
            foreach (MyGeometry geometry in geometriesList)
            {
                if (geometry.RaycastingIntersection(rayonOrigine, rayonDirection, out V3 intersection))
                {
                    if ((intersection - rayonOrigine).Norm() < (NearestIntersection - rayonOrigine).Norm())
                    {
                        NearestIntersection = intersection;
                        IntersectedGeometry = geometry;
                    }
                    NearIntersectionExists = true;
                }
            }
            return NearIntersectionExists;
        }

        public static void ColorDependingOfMap(List<MyGeometry> geometriesList, List<MyLight> lightsList,
            V3 eyeLocation, V3 uncoloredPoint, MyGeometry geometry, out Couleur couleurFinale, int recursionLevel)
        {
            // calculate u, v, dmdu, dmdv, colorbyUV :
            geometry.CalculateUV(uncoloredPoint, out float u, out float v);
            geometry.CalculateDifferentialUV(uncoloredPoint, out V3 dmdu, out V3 dmdv);
            Couleur CouleurUV = geometry.Material.ColorMap.LireCouleur(u, v);
            Couleur CDiffus = Couleur.Black;
            Couleur CSpeculaire = Couleur.Black;
            // si on veut que la couleur, ou si l'objet est source lumineuse, c bon :
            couleurFinale = CouleurUV;
            if (map.Equals("colormaps")) return;
            if (geometry.IsGeometryLight) return;

            // calculate normal with bumps :
            V3 normalWithBump = geometry.GetNormalOfPoint(uncoloredPoint);
            normalWithBump = geometry.GetNormalWithBump(normalWithBump, u, v, dmdu, dmdv);
            // calculate invert ray direction :
            V3 point2eyesDirection = uncoloredPoint.NormalizedDirectionToVec(eyeLocation);

            // Tous les calculs de couleur... :
            Calcul_diffuse_speculaire(geometriesList, lightsList, geometry, uncoloredPoint, normalWithBump,
                point2eyesDirection, out Couleur CAmbient, out CDiffus, out CSpeculaire);
            Couleur Cinterm = CAmbient + CDiffus + CSpeculaire;

            // test avec des lightmaps pour accelerer les calculs avec vpl
            Cinterm += geometry.Material.LightMap.LireCouleur(u, v);
            if (MyRenderingManager.pathtracingLevel < 2)
                geometry.Material.LightMap.SetColorByUV(u, v, Cinterm);

            // Raytracing
            CalculRaytracingColors(geometriesList, lightsList, geometry, uncoloredPoint, -point2eyesDirection, 
                out Couleur CReflexion, out Couleur CRefraction, recursionLevel);
            float rho1 = geometry.Material.ReflexionCoeff;
            float rho2 = geometry.Material.RefractionCoeff;
            Cinterm += (-rho1 - rho2) * Cinterm;
            Cinterm += rho1*CReflexion + rho2*CRefraction;

            // comme si la somme des couleurs avait été factorisée :
            if (map.Equals("everything"))
                couleurFinale *= Cinterm;
            else if (map.Equals("lightmaps"))
                couleurFinale = Cinterm;
        }

        private static void Calcul_diffuse_speculaire(List<MyGeometry> geometriesList, List<MyLight> lightsList,
            MyGeometry geometry, V3 uncoloredPoint, V3 normalOfPointWithBump, V3 point2eyesDirection,
            out Couleur CAmbient,out Couleur CDiffus, out Couleur CSpeculaire)
        {
            CAmbient = Couleur.Black;
            CDiffus = Couleur.Black; 
            CSpeculaire = Couleur.Black;
            foreach (MyLight light in lightsList)
            {
                // particulierement pour une RectLight
                if (!light.CanIlluminatePoint(uncoloredPoint))
                    continue;

                Couleur CAmb = light.Couleur; //produit avec couleurObj vers la fin

                // si lampe ambiante, on prend seulement sa couleur
                if (light.GetType().Equals(typeof(MyAmbiantLight)))
                {
                    CAmbient += CAmb;
                    continue;
                }
                // couleur diffus et spec avec management des ombres :
                if (!PointShadowedUnderLight(geometriesList, geometry, light, uncoloredPoint))
                {
                    V3 lightDirection = light.GetLightDirOnPoint(uncoloredPoint);
                    V3 reflectedLightDirection = lightDirection + 2 * normalOfPointWithBump * V3.prod_scal(normalOfPointWithBump, -lightDirection);

                    // diffuse and specular management : //produit avec couleur vers la fin
                    float RpD = V3.prod_scal(reflectedLightDirection, point2eyesDirection);
                    CDiffus += CAmb * V3.prod_scal(normalOfPointWithBump, -lightDirection);
                    CSpeculaire += CAmb * (float)Math.Pow(RpD, geometry.Material.SpecularPower);

                    CDiffus.check();
                    CSpeculaire.check();
                }
            }
        }


        public static bool PointShadowedUnderLight(List<MyGeometry> geometriesList, MyGeometry geometry, MyLight light,V3 point)
        {
            if (!light.CanShadow)
                return false;

            // the point in argument should be from the raycasting 
            V3 RayonDirection = light.GetLightDirOnPoint(point);
            foreach (MyGeometry geom in geometriesList)
            {
                bool occlusion = geom.RaycastingIntersection(point, -RayonDirection, out V3 intersection);
                occlusion = occlusion && (intersection - point) * RayonDirection < 0;
                bool ownhiddenface = geometry.GetNormalOfPoint(point) * RayonDirection > 0;

                if (ownhiddenface || !Object.ReferenceEquals(geometry, geom) && occlusion)
                    return true;
            }
            return false;
        }


        public static void CalculRaytracingColors(List<MyGeometry> geometriesList, List<MyLight> lightsList,
            MyGeometry geometry, V3 uncoloredPoint, V3 lightDir, out Couleur CReflexion, out Couleur CRefraction, int recursionLevel)
        {
            CReflexion = Couleur.Black;
            CRefraction = Couleur.Black;

            if (recursionLevel == raytracingMaxSteps)
                return;

            V3 normal = geometry.GetNormalOfPoint(uncoloredPoint);

            // Calculer couleur reflexion si l'indice est non nul
            if (geometry.Material.ReflexionCoeff != 0) 
            {
                V3 ReflexionDir = lightDir - 2 * (lightDir*normal)*normal;
                ReflexionDir.Normalize();

                if (FindNearestIntersectionAndGeometry(geometriesList, uncoloredPoint,
                    ReflexionDir, out V3 IntersectionReflexion, out MyGeometry GeometryOfReflexion))
                {
                    ColorDependingOfMap(geometriesList, lightsList, uncoloredPoint,
                        IntersectionReflexion, GeometryOfReflexion, out CReflexion, recursionLevel+1);
                }
            }

            // Calculer couleur refraction si l'indice est non nul
            if (geometry.Material.RefractionCoeff != 0) 
            {

                float n = 1 / geometry.Material.FresnelIndex;
                float cosI = V3.prod_scal(normal, -lightDir);
                float sinT2 = n * n * (1.0f - cosI * cosI);
                float cosT = IMA.Sqrtf(1.0f - sinT2);
                V3 RefractionDir = n * lightDir + (n * cosI - cosT) * normal;

                if (FindNearestIntersectionAndGeometry(geometriesList, uncoloredPoint,
                    RefractionDir, out V3 IntersectionRefraction, out MyGeometry GeometryOfRefraction))
                {
                    ColorDependingOfMap(geometriesList, lightsList, uncoloredPoint,
                        IntersectionRefraction, GeometryOfRefraction, out CRefraction, recursionLevel+1);
                }
            }
        }

        /*

        // Souvenir du Calcul pour Diffus Niv 1 --------------------------------------------------


        public static void CalculatePixelColor(List<MyLight> lightsList, List<MyGeometry> geometriesList, int px, int pz, out Couleur couleur)
        {
            couleur = Couleur.Black;
            V3 RayonDirection = eyeLocation.NormalizedDirectionToVec(new V3(px, 0, pz));
            if (FindNearestIntersectionAndGeometry(geometriesList, eyeLocation, RayonDirection,
                out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
            {
                ColorDependingOfMap(geometriesList, lightsList, eyeLocation,
                NearestIntersection, IntersectedGeometry, out couleur);
            }
        }


        // Souvenir du Draw en séquentiel --------------------------------------------------


        public static void Draw(List<MyLight> lightsList, List<MyGeometry> geometriesList)
        {
            int nbpx = 1000; // 910; 
            int nbpz = 600; // 540;

            for (int px = 0; px < nbpx; px++)
            {
                for (int pz = 0; pz < nbpz; pz++)
                {
                    CalculatePixelColor(lightsList, geometriesList, px, pz, out Couleur calculatedColor);
                    BitmapEcran.DrawPixel(px, pz, calculatedColor);
                }
            }

        }

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
    }
}
