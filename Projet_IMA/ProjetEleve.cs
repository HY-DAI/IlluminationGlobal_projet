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
            
            //MyLight Light = new MyDirectionalLight(Couleur.White, 0.35f, new V3(-1, 1, -1));
            MyLight Light1 = new MyDirectionalLight(Couleur.Yellow, 0.35f, new V3(-1, 1, -1));
            MyLight Light2 = new MyDirectionalLight(Couleur.Cyan, 0.05f, new V3(1, 0f, -1));
            MyLight Light3 = new MyDirectionalLight(Couleur.Magenta, 0.25f, new V3(1, -1f, 1));

            //////////////////////////////////////////////////////////////////////////
            ///  Formes
            //////////////////////////////////////////////////////////////////////////

            int radius = 130;
            float step = 0.007f;
            V3 sphereCenter = new V3(180, 0, 300);
            V3 offset = new V3(290, 0, 0);
            MySphere Sphere1 = new MySphere(sphereCenter, radius, step, new MyMaterial(Texture.TestMap, Texture.BumpMap2, 0.005f, 200));
            MySphere Sphere2 = new MySphere(sphereCenter+offset, radius, step, new MyMaterial(Texture.GoldMap, Texture.GoldBumpMap, 0.0025f,200));
            MySphere Sphere3 = new MySphere(sphereCenter+2*offset, radius, step, new MyMaterial(Texture.LeadMap, Texture.LeadBumpMap, 0.0025f,50));


            //////////////////////////////////////////////////////////////////////////
            ///  Dessin sur l'interface
            //////////////////////////////////////////////////////////////////////////

            V3 eyeLocation = new V3(1000, 3000, 500);
            MyRF.Draw(MyLight.LightsList, MyGeometry.GeometriesList, eyeLocation);
        }
    }
}
