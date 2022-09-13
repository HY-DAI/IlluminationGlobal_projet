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

        private Maillage CreerMaillageColore(float pas, Texture texture)
        {
            List<V3> points = new List<V3>();
            List<Couleur> couleurs = new List<Couleur>();

            for (float u = 0; u < 1; u += pas)  // echantillonage fnt paramétrique
                for (float v = 0; v < 1; v += pas)
                {
                    V3 P3D = Origine + u * Coté1 + v * Coté2;

                    // projection orthographique => repère écran
                    int x_ecran = (int)(P3D.x);
                    int y_ecran = (int)(P3D.z);
                    Couleur couleur = Texture.LireCouleur(u, v);
                    points.Add(new V3(x_ecran, y_ecran,0));
                    couleurs.Add(couleur);
                }
            return new Maillage(points, couleurs);
        }

        public MyRectangle(V3 origine, V3 cote1, V3 cote2, float pas, Texture texture) 
        {
            Origine = origine;
            Coté1 = cote1;
            Coté2 = cote2;
            Texture = texture;
            Maillage = CreerMaillageColore(pas, texture);
        }
        public MyRectangle(V3 origine, V3 cote1, V3 cote2, float pas, Couleur couleur) : this(origine,cote1,cote2,pas, new Texture(couleur))
        { }


        public override V3 GetNormalOfPoint(V3 point)
        {
            return new V3(0, 0, 1);
        }

    }
}
