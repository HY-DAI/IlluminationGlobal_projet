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

        public static string map = "everything";

        public static float RayonRendering = 7000;
        public static V3 eyeLocation = new V3(largeurEcran / 2, -largeurEcran, hauteurEcran / 2);


        public static Couleur diffuseLeveLUp(List<MyGeometry> geometriesList, MyGeometry geometry, V3 point, int nRays)
        {
            Couleur diffus = Couleur.Cyan;
            Couleur interm;

            V3 normal = geometry.GetNormalOfPoint(point);
            List<V3> randDir = V3.GetRandomDirectionsAlongVector(nRays, normal);
            foreach (V3 dir in randDir)
            {
                if (!FindNearestIntersectionAndGeometry(geometriesList, point, dir, out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
                    continue;/*
                if (Object.Equals(NearestIntersection, geometry))
                    continue;
*/
                IntersectedGeometry.CalculateUV(NearestIntersection, out float u, out float v);
                if (IntersectedGeometry.IsGeometryLight)
                    interm = IntersectedGeometry.Material.ColorMap.LireCouleur(u, v);
                else
                    interm = IntersectedGeometry.Material.ColorMap.LireCouleur(u, v) * IntersectedGeometry.Material.LightMap.LireCouleur(u, v);

                diffus += (normal * dir) * interm;
            }
            diffus /= nRays;
            return diffus;
        }


        public static Bitmap reSetBitmapPixels(List<MyLight> lightsList, List<MyGeometry> geometriesList, Point coordZone, int LargZonePix)
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
                    Couleur calculatedColor = Couleur.Blue;
                    V3 RayonDirection = eyeLocation.NormalizedDirectionToVec(new V3(px, 0, pz));
                    if (FindNearestIntersectionAndGeometry(geometriesList, eyeLocation, RayonDirection, out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
                    {
                        calculatedColor = diffuseLeveLUp(geometriesList, IntersectedGeometry, NearestIntersection, 10);
                    }
                    B.SetPixel(px - pxmin, pzmax - pz - 1, calculatedColor.Convertion());
                    //pzmax-pz-1 au lieu de pz-pzmin , car il semble avoir inversion de repère /z dans canvas...
                }
            }
            return B;
        }


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
                    //pzmax-pz-1 au lieu de pz-pzmin , car il semble avoir inversion de repère /z dans canvas...
                }
            }

            return B;
        }

        public static void CalculatePixelColor(List<MyLight> lightsList, List<MyGeometry> geometriesList, int px, int pz, out Couleur couleur)
        {
            couleur = Couleur.Black;
            V3 RayonDirection = eyeLocation.NormalizedDirectionToVec(new V3(px, 0, pz));
            if (FindNearestIntersectionAndGeometry(geometriesList, eyeLocation, RayonDirection, out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
            {
                Calcul_diffuse_speculaire_bumps(geometriesList, lightsList, eyeLocation, NearestIntersection, IntersectedGeometry, out Couleur NearestCouleur);
                couleur = NearestCouleur;
            }
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

            // calculate u, v, dmdu, dmdv :
            geometry.CalculateUV(uncoloredPoint, out float u, out float v);
            geometry.CalculateDifferentialUV(uncoloredPoint, out V3 dmdu, out V3 dmdv);
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
            if (map.Equals("everything"))
                couleur *= geometry.Material.LightMap.LireCouleur(u, v);
            else if (map.Equals("lightmaps"))
                couleur = geometry.Material.LightMap.LireCouleur(u, v);
            else if (map.Equals("colormaps"))
                return;
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


        // Pour traiter les VPL --------------------------------------------------

        public static void UpdateLightMapsWithVPL(List<MyLight> lightsList, List<MyGeometry> geometriesList, float pas, int RaysPerVPL)
        {
            MyRenderingManager.GenerateVirtualPointLights(lightsList, geometriesList, out List<MyLight> vpls, pas, RaysPerVPL);
            MyRenderingManager.CalculateLightMaps(vpls, geometriesList, 0.001f);
            MyLight.deleteLightsOfClass(lightsList, typeof(MyVirtualPointLight));
        }

        public static void GenerateVirtualPointLights(List<MyLight> lights, List<MyGeometry> geometriesList, out List<MyLight> vpls, float pas, int RaysPerVPL)
        {
            vpls = new List<MyLight>();
            int imax = ProjetEleve.lights.Count;
            for (int i = 0; i < imax; i++)
            {
                MyRenderingManager.GenerateVirtualPointLights(lights[i], geometriesList, out List<MyLight> vpl_, pas, RaysPerVPL);
                vpls.AddRange(vpl_.Cast<MyVirtualPointLight>());
            }
            Console.WriteLine($"nb of vpls : {vpls.Count()}");
        }

        public static void GenerateVirtualPointLights(MyLight light, List<MyGeometry> geometriesList, out List<MyLight> vpls, float pas, int RaysPerVPL)
        {
            vpls = new List<MyLight>();
            foreach (MyGeometry geometry in geometriesList)
            {
                for (float u = 0.05f; u < 0.95f; u += pas)
                {
                    for (float v = 0.05f; v < 0.95f; v += pas)
                    {
                        V3 vplPosition = geometry.Get3DPoint(u, v);
                        Couleur couleur = geometry.Material.ColorMap.LireCouleur(u, v);
                        if (light.CanIlluminatePoint(vplPosition)
                            && !ShadowsUnderLight(geometriesList, geometry, light, vplPosition, couleur, out Couleur Cocclusion))
                        {
                            float lightIntensity = light.GetIntensity()/2;
                            vpls.Add(new MyVirtualPointLight(vplPosition, couleur, lightIntensity,geometry.GetNormalOfPoint(vplPosition),RaysPerVPL));
                        }
                    }
                }
            }
        }

        public static void CalculateLightMaps(List<MyLight> lightsList, List<MyGeometry> geometriesList, float halfResolution)
        {
            foreach (MyLight light in lightsList)
            {
                if (!light.GetType().Equals(typeof(MyVirtualPointLight)))
                    continue;

                Couleur CAmb = light.Couleur;
                MyVirtualPointLight vpl = (MyVirtualPointLight)light;
                foreach (V3 lightDir in vpl.RandDirections)
                {
                    CAmb = light.Couleur;
                    if (FindNearestIntersectionAndGeometry(geometriesList, vpl.LightPosition, lightDir, out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
                        //&& IntersectedGeometry.GetNormalOfPoint(NearestIntersection)*lightDir>0)
                    {
                        IntersectedGeometry.CalculateUV(NearestIntersection, out float u, out float v);
                        Texture lightmap = IntersectedGeometry.Material.LightMap;
                        CAmb += lightmap.LireCouleur(u, v); CAmb.check();
                        IntersectedGeometry.Material.LightMap.SetColorByUVzone(u-halfResolution, u+halfResolution, v-halfResolution, v+halfResolution, CAmb);
                        //Console.WriteLine($"Dans CalculLM lightmap with u={u} , v={v} , Color = ({CAmb.R};{CAmb.V};{CAmb.B})");
                    }
                }
            }
        }


        // Pour le Draw en séquentiel --------------------------------------------------


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
