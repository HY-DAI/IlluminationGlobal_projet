using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Projet_IMA.Geometries;
using Projet_IMA.Lights;

namespace Projet_IMA
{
    class MyVPLsManager
    {

        public static void UpdateLightMapsWithVPL(List<MyLight> lightsList, List<MyGeometry> geometriesList, float pas, int RaysPerVPL)
        {
            GenerateVirtualPointLights(lightsList, geometriesList, out List<MyLight> vpls, pas, RaysPerVPL);
            CalculateLightMaps(vpls, geometriesList, 0.001f);
            MyLight.deleteLightsOfClass(lightsList, typeof(MyVirtualPointLight));
        }

        public static void GenerateVirtualPointLights(List<MyLight> lights, List<MyGeometry> geometriesList,
            out List<MyLight> vpls, float pas, int RaysPerVPL)
        {
            vpls = new List<MyLight>();
            int imax = MyLight.LightsList.Count;
            for (int i = 0; i < imax; i++)
            {
                GenerateVirtualPointLights(lights[i], geometriesList, out List<MyLight> vpl_, pas, RaysPerVPL);
                vpls.AddRange(vpl_.Cast<MyVirtualPointLight>());
            }
            //Console.WriteLine($"nb of vpls : {vpls.Count()}");
        }

        public static void GenerateVirtualPointLights(MyLight light, List<MyGeometry> geometriesList,
            out List<MyLight> vpls, float pas, int RaysPerVPL)
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
                            && !MyRenderingManager.PointShadowedUnderLight(geometriesList, geometry, light, vplPosition))
                        {
                            float lightIntensity = light.GetIntensity() / 2;
                            vpls.Add(new MyVirtualPointLight(vplPosition, couleur, lightIntensity,
                                geometry.GetNormalOfPoint(vplPosition), RaysPerVPL));
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
                    if (MyRenderingManager.FindNearestIntersectionAndGeometry(geometriesList, vpl.LightPosition, lightDir,
                        out V3 NearestIntersection, out MyGeometry IntersectedGeometry))
                    //&& IntersectedGeometry.GetNormalOfPoint(NearestIntersection)*lightDir>0)
                    {
                        IntersectedGeometry.CalculateUV(NearestIntersection, out float u, out float v);
                        Texture lightmap = IntersectedGeometry.Material.LightMap;
                        CAmb += lightmap.LireCouleur(u, v); CAmb.check();
                        IntersectedGeometry.Material.LightMap.SetColorByUVzone(u - halfResolution, u + halfResolution, v - halfResolution, v + halfResolution, CAmb);
                        //Console.WriteLine($"Dans CalculLM lightmap with u={u} , v={v} , Color = ({CAmb.R};{CAmb.V};{CAmb.B})");
                    }
                }
            }
        }

    }
}
