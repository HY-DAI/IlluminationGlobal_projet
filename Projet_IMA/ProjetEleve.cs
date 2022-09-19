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

            //////////////////////////////////////////////////////////////////////////
            ///  Lumières
            //////////////////////////////////////////////////////////////////////////

            MyLight Light1 = new MyDirectionalLight(Couleur.White, 0.45f, new V3(-1, 1, -1));
            //MyLight Light2 = new MyDirectionalLight(Couleur.White, 0.25f, new V3(1, -1, 1));

            /*
                        MyLight Light1 = new MyDirectionalLight(Couleur.Yellow, 0.35f, new V3(-1, 1, -1));
                        MyLight Light2 = new MyDirectionalLight(Couleur.Cyan, 0.05f, new V3(1, 0f, -1));
                        MyLight Light3 = new MyDirectionalLight(Couleur.Magenta, 0.25f, new V3(1, -1f, 1));
            */

            //////////////////////////////////////////////////////////////////////////
            ///  Formes
            //////////////////////////////////////////////////////////////////////////

            //// sachant que : this.pictureBox1.Size = new System.Drawing.Size(1275, 700);
            int largeurEcran = 960;
            int hauteurEcran = 570;


            int radius = 130;
            float step = 0.007f;
            V3 sphereCenter = new V3(largeurEcran/3-radius, radius*1f, hauteurEcran/2);
            V3 offset = new V3(largeurEcran/3, 0, 0);
            MySphere Sphere1 = new MySphere(sphereCenter, radius, step, new MyMaterial(Texture.TestMap, Texture.BumpMap2, 0.005f, 200));
            MySphere Sphere2 = new MySphere(sphereCenter+offset, radius, step, new MyMaterial(Texture.GoldMap, Texture.GoldBumpMap, 0.0025f,50));
            MySphere Sphere3 = new MySphere(sphereCenter+2*offset, radius, step, new MyMaterial(Texture.LeadMap, Texture.LeadBumpMap, 0.0025f,50));


            int scalex = largeurEcran; // 960;
            int scaley = largeurEcran;
            int scalez = hauteurEcran; // 570;

            MyParallelogram Plafond = new MyParallelogram(new V3(0, 0, scalez), new V3(scalex, 0, 0), new V3(0, scaley, 0), 0.01f, new MyMaterial(Texture.WoodMap, Texture.FibreMap, 0.001f));
            MyParallelogram Sol = new MyParallelogram(new V3(0, 0, 0), new V3(scalex, 0, 0), new V3(0, scaley, 0), 0.01f, Texture.RockMap);
            MyParallelogram Mur1 = new MyParallelogram(new V3(0, 0, 0), new V3(0, 0, scalez), new V3(0, scaley, 0), 0.01f, Couleur.Cyan);
            MyParallelogram Mur2 = new MyParallelogram(new V3(0, scaley, 0),  new V3(scalex, 0, 0), new V3(0, 0, scalez), 0.01f, Couleur.Yellow);
            MyParallelogram Mur3 = new MyParallelogram(new V3(scalex, 0, 0), new V3(0, 0, scalez), new V3(0, scaley, 0), 0.01f, Couleur.Magenta);



            //////////////////////////////////////////////////////////////////////////
            ///  Dessin sur l'interface
            //////////////////////////////////////////////////////////////////////////

            //V3 eyeLocation = new V3(largeurEcran / 2, -largeurEcran, hauteurEcran / 2);
            V3 eyeLocation = new V3(scalex/2, -scalex, scalez/2); 
            MyRF.Draw(MyLight.LightsList, MyGeometry.GeometriesList, eyeLocation);
        }
    }
}
