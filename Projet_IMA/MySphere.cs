using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MySphere 
    {
        public V3 CentreSphere;
        public float Rayon;
        public Couleur Couleur;

        public MySphere(V3 centreSphere, float rayon, Couleur couleur) 
        {
            CentreSphere = centreSphere;
            Rayon = rayon;
            Couleur = couleur;
        }

        public void Draw(float pas)
        {
            for (float u = 0; u < 2 * IMA.PI; u += pas)  // echantillonage fnt paramétrique
                for (float v = -IMA.PI / 2; v < IMA.PI / 2; v += pas)
                {
                    // calcul des coordoonées dans la scène 3D
                    float x3D = Rayon * IMA.Cosf(v) * IMA.Cosf(u) + CentreSphere.x;
                    float y3D = Rayon * IMA.Cosf(v) * IMA.Sinf(u) + CentreSphere.y;
                    float z3D = Rayon * IMA.Sinf(v) + CentreSphere.z;

                    // projection orthographique => repère écran
                    int x_ecran = (int)(x3D);
                    int y_ecran = (int)(z3D);

                    /*for (int i = 0; i < 100; i++)  // pour ralentir et voir l'animation - devra être retiré*/
                    BitmapEcran.DrawPixel(x_ecran, y_ecran, Couleur);
                }
        }
    }
}
