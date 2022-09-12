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

            //Couleur CRouge = new Couleur(1f, 0f, 0f);
            //Couleur CVert = new Couleur(0f, 1f, 0f);
            //Couleur CBleu = new Couleur(0f, 0f, 1f);
            //Couleur CLampe = new Couleur(1f, 1f, 0f);
            //Couleur CLampe = new Couleur(0f, 1f, 1f);
            //Couleur CLampe = new Couleur(1f, 0f, 1f);
            Couleur CBlanc = new Couleur(1f, 1f, 1f);
            Couleur CLampe = CBlanc;

            V3 lampOrientation = new V3(1, -1, 1);
            V3 lampLocation = 200*lampOrientation;

            V3 eyeLocation = new V3(3000, 3000, 3000);

            int x = 500;
            int y = 150;
            int z = 300;

            int xoffset = 150;
            int zoffset = 150;

            MySphere whiteSphere = new MySphere(new V3(x, y, z), 100, 0.05f, CBlanc);

            Texture texture = new Texture("uvtest.jpg");

            // projection orthographique => repère écran
            int x_ecran = (int)(x3D);
            int y_ecran = (int)(z3D);
            BitmapEcran.DrawPixel(x_ecran, y_ecran, c);

            /*
                        // Gestion des textures
                        Texture T1 = new Texture("brick01.jpg");
                        Couleur c = T1.LireCouleur(0.1f, 0.1f);
            */
        }
    }
}
