using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using Projet_IMA.Geometries;
using Projet_IMA.Geometries.GeometryComponents;
using Projet_IMA.Lights;
using Projet_IMA.Lights.GeometryLights;

namespace Projet_IMA
{
    static class ProjetEleve
    {
        public static void Go()
        {
            // Pour etre clean a chaque fois qu'on clique sur e bouton 
            MyLight.LightsList.Clear();
            MyGeometry.GeometriesList.Clear();
            //IMA.InitRand();

            // Voir MyRenderingManager
            int largeurEcran = MyRenderingManager.largeurEcran;
            int hauteurEcran = MyRenderingManager.hauteurEcran;

            //////////////////////////////////////////////////////////////////////////
            ///  Formes
            //////////////////////////////////////////////////////////////////////////

            int scalex = largeurEcran;
            int scaley = largeurEcran;
            int scalez = hauteurEcran;

            //résolution des lightmaps
            float step = 0.001f; 

            MyParallelogram ceiling = new MyParallelogram(new V3(0, 0, scalez-2), new V3(scalex, 0, 0), new V3(0, scaley, 0), step/8, new MyMaterial(new Texture(Couleur.Cyan), Texture.BumpMap3));
            MyParallelogram floor = new MyParallelogram(new V3(0, 0, 0), new V3(scalex, 0, 0), new V3(0, scaley, 0), step, MyMaterial.Wood);
            MyParallelogram wall_left = new MyParallelogram(new V3(0, 0, 0),  new V3(0, scaley, 0), new V3(0, 0, scalez), step, MyMaterial.Brick);
            MyParallelogram wall_front = new MyParallelogram(new V3(0, scaley-2, 0), new V3(scalex, 0, 0), new V3(0, 0, scalez), step, MyMaterial.Test);
            MyParallelogram wall_right = new MyParallelogram(new V3(scalex-2, 0, 0), new V3(0, 0, scalez), new V3(0, scaley, 0), step, MyMaterial.Gold);

            MyParallelogram wall_rand = new MyParallelogram(new V3(scalex / 3, scaley / 3, 0), new V3(0, 0, scalez / 3), new V3(0, scaley / 2, 0), step, new MyMaterial(new Texture(Couleur.Red), Texture.BumpMap));
            MyParallelogram wall_rand2 = new MyParallelogram(new V3(scalex *2/3 -60, -scaley / 2-40, scalez/3+20), new V3(0, scaley / 2, 0), new V3(0, 0, scalez / 3), step, new MyMaterial(new Texture(Couleur.Blue), Texture.BumpMap3));

            // Un peu de mise en scene de spheres...

            int radius = 130;
            V3 sphereCenter = new V3(largeurEcran/3-radius, radius*1f, hauteurEcran/2);
            V3 offset = new V3(largeurEcran / 3, 0, 0);
            V3 offsety = new V3(0, -largeurEcran / 3, 0);
            MySphere Sphere1 = new MySphere(sphereCenter, radius, step/2, new MyMaterial(Texture.TestMap, Texture.BumpMap2, 5f, 200));
            MySphere Sphere2 = new MySphere(sphereCenter+offset-offsety, radius, step, new MyMaterial(Texture.GoldMap, Texture.GoldBumpMap, 2.5f,50));
            MySphere Sphere3 = new MySphere(sphereCenter+2*offset, radius, step, new MyMaterial(Texture.LeadMap, Texture.LeadBumpMap, 2.5f,50));

            //Texture textureZemmour = new Texture("zemmour.jpg"); 
            Texture textureZemmour = new Texture(Couleur.White); 
            MySphere Sphere4 = new MySphere(sphereCenter + new V3(radius*2, -radius*3,radius/3), radius/2, step, new MyMaterial(textureZemmour, textureZemmour, 2.5f, 50));
            Sphere4.Material.ReflexionCoeff = 0.2f;
            Sphere4.Material.RefractionCoeff = 0.3f;
            //Sphere4.Material.FresnelIndex = 1.0f;

            Sphere2.Material.ReflexionCoeff = 1f;

            //////////////////////////////////////////////////////////////////////////
            ///  Lumières
            //////////////////////////////////////////////////////////////////////////

            MyLight AmbiantLight = new MyAmbiantLight(Couleur.White, 0.15f);

            //MyLight PLight1 = new MyPointLight(MyRenderingManager.eyeLocation + offset, Couleur.White, 0.15f);
            //MyLight PLight2 = new MyPointLight(MyRenderingManager.eyeLocation + offsety, Couleur.Cyan, 0.15f);

            MyLight Light1 = new MyDirectionalLight(new V3(-1, 1, -1), Couleur.White, 0.30f);
            MyLight Light2 = new MyDirectionalLight(new V3(1, 1f, -1), Couleur.Cyan, 0.20f);
            //MyLight Light3 = new MyDirectionalLight(new V3(1, 1f, 1), Couleur.Magenta, 0.15f);

            //MyRectLight RectLight = new MyRectLight(Couleur.Yellow, 0.20f, wall_rand);
            //MyConeLight ConeLight = new MyConeLight(new V3(scalex / 2, scaley / 3, scalez / 2), new V3(0, 0, 1), Couleur.Red, 0.1f, 0.65f);


            //////////////////////////////////////////////////////////////////////////
            ///  Dessin sur l'interface dans la version séquentielle
            //////////////////////////////////////////////////////////////////////////

            //MyRenderingManager.Draw(MyLight.LightsList, MyGeometry.GeometriesList);


            //////////////////////////////////////////////////////////////////////////
            ///  Prise en Compte des Virtual Point Lights
            //////////////////////////////////////////////////////////////////////////



            // Les Virtual Point Lights
            //MyRenderingManager.map = "lightmaps";
            //MyVPLsManager.UpdateLightMapsWithVPL(MyLight.LightsList, MyGeometry.GeometriesList, 0.01f, 100);
            MyVPLsManager.UpdateLightMapsWithVPL(MyLight.LightsList, MyGeometry.GeometriesList, 0.5f, 10);



            /*
                        // Fail 2 drole sur les Virtual Point Lights
                        MyRenderingManager.map = "lightmaps";
                        MyVPLsManager.GenerateVirtualPointLights(ProjetEleve.lights, ProjetEleve.geometries, out List<MyLight> vpls_all, 0.1f, 100);
            */

            //////////////////////////////////////////////////////////////////////////
            ///  Prise en Compte du PathTracing
            //////////////////////////////////////////////////////////////////////////

            //MyRenderingManager.map = "lightmaps";

        }

    }

}
