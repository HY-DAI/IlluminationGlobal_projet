using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyRectangle : MyGeometry
    {
        public V3 Origine;
        public V3 Coté1;
        public V3 Coté2;

        public MyRectangle(V3 origine, V3 cote1, V3 cote2, float pas, Couleur couleur)
        {
            Origine = origine;
            Coté1 = cote1;
            Coté2 = cote2;
            Couleur = couleur;
        }
        public MyRectangle(V3 origine, V3 cote1, V3 cote2, float pas, Texture texture) 
        {
            Origine = origine;
            Coté1 = cote1;
            Coté2 = cote2;
            Texture = texture;
        }


        public override void Draw(float pas)
        {
            for (float u = 0; u < 1; u += pas)  // echantillonage fnt paramétrique
                for (float v = 0; v < 1; v += pas)
                {
                    V3 P3D = Origine + u * Coté1 + v * Coté2;

                    // projection orthographique => repère écran
                    int x_ecran = (int)(P3D.x);
                    int y_ecran = (int)(P3D.y);

                    /*for (int i = 0; i < 100; i++)  // pour ralentir et voir l'animation - devra être retiré*/
                    Texture = new Texture("uvtest.jpg");
                    Couleur c = Texture.LireCouleur(u, v);
                    BitmapEcran.DrawPixel(x_ecran, y_ecran, c);
                }
        }
    }
}
