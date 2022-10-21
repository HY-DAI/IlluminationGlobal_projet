using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Projet_IMA.Geometries;
using Projet_IMA.Geometries.GeometryComponents;
using Projet_IMA.Lights;
using Projet_IMA.Lights.GeometryLights;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Drawing;

namespace Projet_IMA
{
    class MyRenderingManager 
    {

        static int largeurEcran = 960;
        static int hauteurEcran = 570;

        public static float RayonRendering = 7000;
        public static V3 eyeLocation = new V3(largeurEcran / 2, -largeurEcran, hauteurEcran / 2);

        public static Bitmap SetBitmapPixels(List<MyLight> lightsList, List<MyGeometry> geometriesList, Point coordZone, int LargZonePix)
        {
            Bitmap B = new Bitmap(LargZonePix, LargZonePix);
            int pxmin = coordZone.X;
            int pzmin = coordZone.Y;
            int pxmax = coordZone.X+LargZonePix;
            int pzmax = coordZone.Y+LargZonePix;

            for (int px = pxmin; px < pxmax; px++)
            {
                for (int pz = pzmin; pz < pzmax; pz++)
                {
                    CalculatePixelColor(lightsList, geometriesList, px, pz, out Couleur calculatedColor);
                    B.SetPixel(px-pxmin,pzmax-pz-1,calculatedColor.Convertion());
                    //pzmax-pz-1 au lieu de pz-pzmin , car il semble avoir inversion de repère /z dans canvas
                }
            }
            return B;
        }

        public static void CalculatePixelColor(List<MyLight> lightsList, List<MyGeometry> geometriesList, int px, int pz, out Couleur couleur)
        {
            V3 RayonDirection = eyeLocation.NormalizedDirectionToVec(new V3(px, 0, pz));
            FindNearestIntersectionAndGeometry(geometriesList, eyeLocation, RayonDirection, out V3 NearestIntersection, out MyGeometry IntersectedGeometry);
            Calcul_diffuse_speculaire_bumps(geometriesList, lightsList, eyeLocation, NearestIntersection, IntersectedGeometry, out Couleur NearestCouleur);
            couleur = NearestCouleur;
        }

        public static bool FindNearestIntersectionAndGeometry(List<MyGeometry> geometriesList, V3 eyePosition, V3 rayonDirection, out V3 NearestIntersection, out MyGeometry IntersectedGeometry)
        {
            bool NearIntersectionExists = false;
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
                    NearIntersectionExists = true;
                }
            }
            return NearIntersectionExists;
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

            // si l'objet est de une forme lumineuse, on s'en fiche
            if (geometry.IsGeometryLight)
                return;

            Couleur Cinterm = Couleur.Black;
            foreach (MyLight light in lightsList)
            {
                Couleur CAmb = light.Couleur;
                lightDirection = light.GetLightDirOnPoint(uncoloredPoint);

                // particulierement pour une RectLight
                if (!light.CanIlluminatePoint(uncoloredPoint))
                    continue;

                // occlusion management :
                if (!ShadowsUnderLight(geometriesList, geometry, light, uncoloredPoint, couleur, out Couleur Cocclusion))
                {
                    // diffuse and specular management :
                    reflectedLightDirection = lightDirection + 2 * normal * V3.prod_scal(normal, -lightDirection);
                    Couleur CDiffuse = CAmb * V3.prod_scal(normal, -lightDirection);
                    Couleur CSpeculaire = CAmb * (float)Math.Pow(V3.prod_scal(reflectedLightDirection, point2eyesDirection), geometry.Material.SpecularPower);

                    CDiffuse.check();
                    CSpeculaire.check();

                    Cinterm += CDiffuse + CSpeculaire;
                }

                Cinterm += CAmb;
            }
            Cinterm += geometry.Material.LightMap.LireCouleur(u, v);
            geometry.Material.LightMap.SetColorByUV(u, v, Cinterm);
            // comme si la somme des couleurs avait été factorisée :
            couleur *= geometry.Material.LightMap.LireCouleur(u, v);
            //couleur = geometry.Material.LightMap.LireCouleur(u, v);
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
                occlusion = occlusion && (intersection - point) * RayonDirection < 0;
                bool ownhiddenface = geometry.GetNormalOfPoint(point) * RayonDirection > 0;

                if (ownhiddenface || !Object.ReferenceEquals(geometry, geom) && occlusion)
                {
                    // moins light intense , moins couleur intense : 
                    couleurout *= light.GetIntensity();
                    couleurout.check();
                    return true;
                }
            }
            return false;
        }

        public static void GenerateVirtualPointLights(List<MyLight> lightsList, List<MyGeometry> geometriesList)
        {
            // Laissé en pause pour l'instant, 
            // Raytracing parait moins embetant
            float pas = 0.1f;
            MyLight mainLight = lightsList[0];

            foreach (MyGeometry geometry in geometriesList)
            {
                for (float u = pas; u < 1; u += pas)
                {
                    for (float v = pas; v < 1; v += pas)
                    {
                        V3 vplPosition = geometry.Get3DPoint(u, v);
                        Couleur couleur = geometry.Material.ColorMap.LireCouleur(u, v) * 2;
                        if (mainLight.CanIlluminatePoint(vplPosition)
                            && !ShadowsUnderLight(geometriesList, geometry, mainLight, vplPosition, couleur, out Couleur Cocclusion))
                        {
                            float lightIntensity = mainLight.GetIntensity() / 2;
                            MyVirtualPointLight vpl = new MyVirtualPointLight(vplPosition, couleur, lightIntensity);
                        }
                    }
                }

            }

        }

        public static void CalculateLightMaps(List<MyLight> lightsList, List<MyGeometry> geometriesList)
        {
            int nrays = 10000;

            foreach (MyLight light in lightsList)
            {

                if (light.GetType() != typeof(MyVirtualPointLight))
                    continue;

                Couleur CAmb = light.Couleur;
                MyVirtualPointLight vpl = (MyVirtualPointLight)light;
                List<V3> listofdir = vpl.GetRandomDirections(nrays);
                foreach (V3 lightDir in listofdir)
                {
                    if (FindNearestIntersectionAndGeometry(geometriesList, vpl.LightPosition, lightDir, out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
                    {
                        IntersectedGeometry.CalculateUV(NearestIntersection, out float u, out float v);
                        IntersectedGeometry.Material.LightMap.SetColorByUV(u, v, light.Couleur);
                    }
                }
            }
        }

        public static List<MyLight> RemoveVirtualPointLights(List<MyLight> lightsList)
        {
            List<MyLight> lightListWithoutVPL = new List<MyLight>();

            foreach (MyLight light in lightsList)
            {
                if (light.GetType() != typeof(MyVirtualPointLight))
                {
                    lightListWithoutVPL.Add(light);
                    continue;
                }
            }
            return lightListWithoutVPL;
        }



        // Pour le Draw en séquentiel --------------------------------------------------

        public static void Draw(List<MyLight> lightsList, List<MyGeometry> geometriesList)
        {
            int nbpx = 1000; // 910; 
            int nbpz = 600; // 540;

            /*
                        // Test infructueux avec les VPL
                        MyRenderingManager.GenerateVirtualPointLights(lightsList, geometriesList);
                        MyRenderingManager.CalculateLightMaps(lightsList, geometriesList);
                        lightsList = MyRenderingManager.RemoveVirtualPointLights(lightsList);
            */

            for (int px = 0; px < nbpx; px++)
            {
                for (int pz = 0; pz < nbpz; pz++)
                {
                    CalculatePixelColor(lightsList, geometriesList, px, pz, out Couleur calculatedColor);
                    BitmapEcran.DrawPixel(px, pz, calculatedColor);
                }
            }

        }

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
    }
                }
