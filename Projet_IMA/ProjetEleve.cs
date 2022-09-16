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
            ///
            ///     EX1 : Sphère en 3D et Lumière de lampe
            /// 
            //////////////////////////////////////////////////////////////////////////


            V3 eyeLocation = new V3(1000, 3000, 500);

            V3 lampOrientation = new V3(0, 1, 0);
            lampOrientation.Normalize();
            MyLight light = new MyDirectionalLight(Couleur.White, 0.45f,lampOrientation);

            V3 sphereCenter = new V3(180, 0, 300);
            V3 offset = new V3(290, 0, 0);
            int radius = 130;
            float step = 0.007f;
            MySphere Sphere1 = new MySphere(sphereCenter, radius, step, new MyMaterial(Texture.TestMap, Texture.BumpMap2, 0.005f, 100));
            MySphere Sphere2 = new MySphere(sphereCenter+offset, radius, step, new MyMaterial(Texture.GoldMap, Texture.GoldBumpMap, 0.005f));
            MySphere Sphere3 = new MySphere(sphereCenter+2*offset, radius, step, new MyMaterial(Texture.LeadMap, Texture.LeadBumpMap, 0.005f));


            MyRF.calcul_diffuse_speculaire(light, Sphere1, eyeLocation);
            MyRF.Draw(Sphere1, 0.01f);
            MyRF.calcul_diffuse_speculaire(light, Sphere2, eyeLocation);
            MyRF.Draw(Sphere2, 0.01f);
            MyRF.calcul_diffuse_speculaire(light, Sphere3, eyeLocation);
            MyRF.Draw(Sphere3, 0.01f);
        }
    }
}
