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

            V3 lampOrientation = new V3(-1, 1, -1);
            lampOrientation.Normalize();
            MyLight light = new MyDirectionalLight(Couleur.White, 0.45f,lampOrientation);

            Texture colormap = new Texture("uvtest.jpg");
            Texture bumpmap = new Texture("bump38.jpg");
            MySphere Sphere = new MySphere(new V3(400, 0, 300), 75, 0.01f, new MyMaterial(colormap, bumpmap, 0.9f));
            MySphere Sphere2 = new MySphere(new V3(200, 0, 300), 75, 0.01f, new MyMaterial(Couleur.Cyan, bumpmap, 0.8f));

            MyRF.calcul_diffuse_speculaire(light, Sphere, eyeLocation);
            MyRF.Draw(Sphere, 0.01f);
            MyRF.calcul_diffuse_speculaire(light, Sphere2, eyeLocation);
            MyRF.Draw(Sphere2, 0.01f);
        }
    }
}
