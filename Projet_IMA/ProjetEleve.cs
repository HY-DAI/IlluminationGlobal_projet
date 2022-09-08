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

            Couleur CLampe = new Couleur(1f, 1f, 1f);
            //Couleur CLampe = new Couleur(1f, 0f, 0f);
            //Couleur CLampe = new Couleur(0f, 1f, 0f);
            //Couleur CLampe = new Couleur(0f, 0f, 1f);
            //Couleur CLampe = new Couleur(1f, 1f, 0f);
            //Couleur CLampe = new Couleur(0f, 1f, 1f);
            //Couleur CLampe = new Couleur(1f, 0f, 1f);

            int x = 500;
            int y = 150;
            int z = 300;

            int xoffset = 150;
            int zoffset = 150;

            MySphere whiteSphere = new MySphere(new V3(x, y, z), 50, new Couleur(1f, 1f, 1f));
            whiteSphere.Couleur *= CLampe;
            whiteSphere.Draw(0.01f);

            MySphere redSphere = new MySphere(new V3(x-xoffset,y,z-zoffset*1/2), 50, Couleur.Red);
            redSphere.Couleur *= CLampe;
            redSphere.Draw(0.01f);

            MySphere greenSphere = new MySphere(new V3(x+xoffset,y,z-zoffset*1/2), 50, Couleur.Green);
            greenSphere.Couleur *= CLampe;
            greenSphere.Draw(0.01f);

            MySphere blueSphere = new MySphere(new V3(x,y,z+zoffset), 50, Couleur.Blue);
            blueSphere.Couleur *= CLampe;
            blueSphere.Draw(0.01f);

            MySphere yellowSphere = new MySphere(new V3(x, y, z-zoffset), 50, new Couleur(1f, 1f, 0f));
            yellowSphere.Couleur *= CLampe;
            yellowSphere.Draw(0.01f);

            MySphere cyanSphere = new MySphere(new V3(x+xoffset, y, z+zoffset*1/2), 50, new Couleur(0f, 1f, 1f));
            cyanSphere.Couleur *= CLampe;
            cyanSphere.Draw(0.01f);

            MySphere magentaSphere = new MySphere(new V3(x-xoffset, y, z+zoffset*1/2), 50, new Couleur(1f, 0f, 1f));
            magentaSphere.Couleur *= CLampe;
            magentaSphere.Draw(0.01f);




            // Gestion des textures
            //Texture T1 = new Texture("brick01.jpg");
            //Couleur c = T1.LireCouleur(0.1f, 0.1f);

        }
    }
}
