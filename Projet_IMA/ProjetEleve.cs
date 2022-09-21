using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projet_IMA
{
    static class ProjetEleve
    {
        public static void Go()
        {

            //// sachant que : this.pictureBox1.Size = new System.Drawing.Size(1275, 700);
            int largeurEcran = 960;
            int hauteurEcran = 570;

            V3 eyeLocation = new V3(largeurEcran / 2, -largeurEcran, hauteurEcran / 2);

            //////////////////////////////////////////////////////////////////////////
            ///  Formes
            //////////////////////////////////////////////////////////////////////////


            int scalex = largeurEcran;
            int scaley = largeurEcran;
            int scalez = hauteurEcran;

            MyParallelogram ceiling = new MyParallelogram(new V3(0, 0, scalez), new V3(scalex, 0, 0), new V3(0, scaley, 0), 0.01f, new MyMaterial(Texture.WoodMap, Texture.FibreMap));
            MyParallelogram floor = new MyParallelogram(new V3(0, 0, 0), new V3(scalex, 0, 0), new V3(0, scaley, 0), 0.01f, new MyMaterial(Texture.LeadMap, Texture.LeadBumpMap, 0.5f));
            MyParallelogram wall_left = new MyParallelogram(new V3(0, 0, 0), new V3(0, 0, scalez), new V3(0, scaley, 0), 0.01f, new MyMaterial(new Texture(Couleur.Cyan), Texture.BumpMap1));
            MyParallelogram wall_front = new MyParallelogram(new V3(0, scaley, 0), new V3(scalex, 0, 0), new V3(0, 0, scalez), 0.01f, new MyMaterial(new Texture(Couleur.Yellow), Texture.BumpMap2));
            MyParallelogram wall_right = new MyParallelogram(new V3(scalex, 0, 0), new V3(0, 0, scalez), new V3(0, scaley, 0), 0.01f, new MyMaterial(new Texture(Couleur.Magenta), Texture.BumpMap3));

            MyParallelogram wall_rand = new MyParallelogram(new V3(scalex / 3, scaley / 3, 0), new V3(0, 0, scalez / 3), new V3(0, scaley / 2, 0), 0.01f, new MyMaterial(new Texture(Couleur.Red), Texture.BumpMap3));
            MyParallelogram wall_rand2 = new MyParallelogram(new V3(scalex *2/3 -60, -scaley / 2-40, scalez/3+20), new V3(0, scaley / 2, 0), new V3(0, 0, scalez / 3), 0.01f, new MyMaterial(new Texture(Couleur.Blue), Texture.BumpMap3));

            //// Un peu de mise en scene de spheres...

            int radius = 130;
            float step = 0.007f;
            V3 sphereCenter = new V3(largeurEcran/3-radius, radius*1f, hauteurEcran/2);
            V3 offset = new V3(largeurEcran / 3, 0, 0);
            V3 offsety = new V3(0, -largeurEcran / 3, 0);
            MySphere Sphere1 = new MySphere(sphereCenter+offsety, radius, step, new MyMaterial(Texture.TestMap, Texture.BumpMap2, 5f, 200));
            MySphere Sphere2 = new MySphere(sphereCenter+offset-offsety, radius, step, new MyMaterial(Texture.GoldMap, Texture.GoldBumpMap, 2.5f,50));
            MySphere Sphere3 = new MySphere(sphereCenter+2*offset, radius, step, new MyMaterial(Texture.LeadMap, Texture.LeadBumpMap, 2.5f,50));

            MySphere Sphere4 = new MySphere(sphereCenter + new V3(radius*2, -radius*4,radius/3), radius/2, step, new MyMaterial(Texture.BrickMap, Texture.BrickMap, 2.5f, 50));


            //////////////////////////////////////////////////////////////////////////
            ///  Lumières
            //////////////////////////////////////////////////////////////////////////

            MyLight PLight1 = new MyPointLight(eyeLocation + offset, new V3(-1, 1, -1), Couleur.White, 0.35f);
            MyLight PLight2 = new MyPointLight(eyeLocation * 2 - offset, new V3(1, 0f, -1), Couleur.Cyan, 0.05f);

            /*
                        MyLight Light1 = new MyDirectionalLight(new V3(-1, 1, -1), Couleur.Yellow, 0.35f);
                        MyLight Light2 = new MyDirectionalLight(new V3(1, 0f, -1), Couleur.Cyan, 0.05f);
                        MyLight Light3 = new MyDirectionalLight(new V3(1, -1f, 1), Couleur.Magenta, 0.25f);
            */

            //////////////////////////////////////////////////////////////////////////
            ///  Dessin sur l'interface
            //////////////////////////////////////////////////////////////////////////

            //V3 eyeLocation = new V3(scalex / 2, -scalex, scalez / 2);
            MyRF.Draw(MyLight.LightsList, MyGeometry.GeometriesList, eyeLocation);
        }

    }
}
