using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MySphere : MyGeometry
    {
        public V3 CentreSphere;
        public float Rayon;

        private void CreerMaillageColore(float pas,Texture texture)
        {
            List<V3> points = new List<V3>();
            List<Couleur> couleurs = new List<Couleur>();

            for (float u = 0; u < 2 * IMA.PI; u += pas)  // echantillonage fnt paramétrique
                for (float v = -IMA.PI / 2; v < IMA.PI / 2; v += pas)
                {
                    // calcul des coordoonées dans la scène 3D
                    float x3D = Rayon * IMA.Cosf(v) * IMA.Cosf(u) + CentreSphere.x;
                    float y3D = Rayon * IMA.Cosf(v) * IMA.Sinf(u) + CentreSphere.y;
                    float z3D = Rayon * IMA.Sinf(v) + CentreSphere.z;
                    Couleur couleur = Texture.LireCouleur(u, v);
                    points.Add(new V3(x3D, y3D, z3D));
                    couleurs.Add(couleur);
                }
            Maillage Maillage = new Maillage(points, couleurs); 
        }

        public MySphere(V3 centreSphere, float rayon, float pas, Texture texture)
        {
            CentreSphere = centreSphere;
            Rayon = rayon;
            Texture = texture;
            CreerMaillageColore(pas,texture);
        }

        public MySphere(V3 centreSphere, float rayon, float pas, Couleur couleur) : this(centreSphere, rayon, pas, new Texture(couleur))
        { }


        public override void Draw(float pas)
        {

        }
    }
}
