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

        public MyRectangle(V3 origine, V3 cote1, V3 cote2, float pas, MyMaterial material) 
        {
            Origine = origine;
            Coté1 = cote1;
            Coté2 = cote2;
            Material = material;
            Maillage = CreerMaillageColore(pas);
            MyRF.Calcul_normals_with_bump(this, out Maillage.Normals);
        }

        public MyRectangle(V3 origine, V3 cote1, V3 cote2, float pas, Texture texture) :
            this(origine, cote1, cote2, pas, new MyMaterial(texture))
        { }

        public MyRectangle(V3 origine, V3 cote1, V3 cote2, float pas, Couleur couleur) : 
            this(origine,cote1,cote2,pas, new Texture(couleur))
        { }


        private MyMaillage CreerMaillageColore(float pas)
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
                    Couleur couleur = Material.ColorMap.LireCouleur(u, v);
                    points.Add(new V3(x_ecran, y_ecran, 0));
                    couleurs.Add(couleur);
                }
            return new MyMaillage(points, couleurs);
        }


        public override V3 GetNormalOfPoint(V3 point)
        {
            return (Coté1^Coté2)/ (Coté1^Coté2).Norm();
        }

        public override void CalculateDifferentialUV(V3 point, out float u, out float v, out V3 dmdu, out V3 dmdv)
        {
            V3 vec = point - Origine;
            u = V3.prod_scal(vec,Coté1);
            v = V3.prod_scal(vec, Coté2);
            dmdu = Coté1;
            dmdv = Coté2;
        }
    }
}
