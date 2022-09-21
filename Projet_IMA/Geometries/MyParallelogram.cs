using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyParallelogram : MyGeometry
    {
        public V3 Origine;
        public V3 Coté1;
        public V3 Coté2;

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyParallelogram(V3 origine, V3 cote1, V3 cote2, float pas, MyMaterial material) 
        {
            Origine = origine;
            Coté1 = cote1;
            Coté2 = cote2;
            Material = material;
            //Maillage = CreerMaillageColore(pas);
            //Calcul_normals_with_bump(this, out Maillage.Normals);
        }

        public MyParallelogram(V3 origine, V3 cote1, V3 cote2, float pas, Texture texture) :
            this(origine, cote1, cote2, pas, new MyMaterial(texture))
        { }

        public MyParallelogram(V3 origine, V3 cote1, V3 cote2, float pas, Couleur couleur) : 
            this(origine,cote1,cote2,pas, new Texture(couleur))
        { }


        //---------------------------------------
        // private méthodes :
        //---------------------------------------

        /*
                private MyMaillage CreerMaillageColore(float pas)
                {
                    List<V3> points = new List<V3>();
                    List<Couleur> couleurs = new List<Couleur>();

                    for (float u = 0; u < 1; u += pas)  // echantillonage fnt paramétrique
                        for (float v = 0; v < 1; v += pas)
                        {
                            V3 P3D = get3DPoint(u,v);

                            // projection orthographique => repère écran
                            int x_ecran = (int)(P3D.x);
                            int y_ecran = (int)(P3D.z);
                            Couleur couleur = Material.ColorMap.LireCouleur(u, v);
                            points.Add(new V3(x_ecran, y_ecran, 0));
                            couleurs.Add(couleur);
                        }
                    return new MyMaillage(points, couleurs);
                }
        */


        //---------------------------------------
        // public méthodes :
        //---------------------------------------

        public override V3 get3DPoint(float u, float v)
        {
            return Origine + u * Coté1 + v * Coté2;
        }

        public override V3 GetNormalOfPoint(V3 point)
        {
            V3 normal = (Coté1 ^ Coté2) / (Coté1 ^ Coté2).Norm();
            if (normal.y > 0)
                normal = -normal;
            return normal;
        }

        public override void CalculateDifferentialUV(V3 point, out float u, out float v, out V3 dmdu, out V3 dmdv)
        {
            //V3 n = point - Origine;
            V3 n = GetNormalOfPoint(Origine);
            u = (Coté2 ^ n) * (point - Origine) / (Coté1 ^ Coté2).Norm();
            v = (Coté1 ^ n) * (point - Origine) / (Coté2 ^ Coté1).Norm();

            dmdu = Coté1;
            dmdv = Coté2;
        }


        public override bool RaycastingIntersection(V3 RayonOrigine, V3 RayonDirection, out float u, out float v, out V3 intersection)
        {
            bool intersectionExists = false;

            V3 n = GetNormalOfPoint(Origine);
            float t = (Origine - RayonOrigine) * n / (RayonDirection * n);
            intersection = RayonOrigine + t * RayonDirection;
            u = (Coté2 ^ n) * (intersection - Origine) / (Coté1 ^ Coté2).Norm();
            v = -(Coté1 ^ n) * (intersection - Origine) / (Coté1 ^ Coté2).Norm();

            if ((0 <= u && u <= 1) && (0 <= v && v <= 1))
                intersectionExists = true;

            return intersectionExists;
        }
    }
}
