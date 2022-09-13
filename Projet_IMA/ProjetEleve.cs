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


            V3 lampOrientation = new V3(-1, 1, -1);
            //V3 lampOrientation = new V3(-10, 80, -10);
            //V3 lampOrientation = new V3(-1, 2, -1);
            lampOrientation.Normalize();
            MyLight light = new MyLight(lampOrientation, Couleur.White);

            V3 eyeLocation = new V3(1000, 3000, 500);

            int x = 400;
            int y = 0;
            int z = 300;

            int xoffset = 150;
            int zoffset = 150;

            Texture texture = new Texture("uvtest.jpg");
            MySphere Sphere = new MySphere(new V3(x, y, z), 75, 0.01f, texture);
            MyGeometry.calcul_diffuse_speculaire(light, Sphere, eyeLocation);
            MyGeometry.Draw(Sphere, 0.01f);
 

            /*
                        // Gestion des textures
                        Texture T1 = new Texture("brick01.jpg");
                        Couleur c = T1.LireCouleur(0.1f, 0.1f);
            */
        }
    }
}
