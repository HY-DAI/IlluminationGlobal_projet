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


        public MySphere(V3 centreSphere, float rayon, float pas, MyMaterial material)
        {
            CentreSphere = centreSphere;
            Rayon = rayon;
            Material = material;
            Maillage = CreerMaillageColore(pas);
            MyRF.Calcul_normals_with_bump(this, out Maillage.Normals);
        }

        public MySphere(V3 centreSphere, float rayon, float pas, Texture texture) :
            this(centreSphere, rayon, pas, new MyMaterial(texture))
        { }

        public MySphere(V3 centreSphere, float rayon, float pas, Couleur couleur) : 
            this(centreSphere, rayon, pas, new Texture(couleur))
        { }

        private MyMaillage CreerMaillageColore(float pas)
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
                    float unormalized = u / (2 * IMA.PI);
                    float vnormalized = (v + IMA.PI / 2) / (IMA.PI);
                    V3 point = new V3(x3D, y3D, z3D);
                    Couleur couleur = Material.ColorMap.LireCouleur(unormalized, vnormalized);
                    points.Add(point);
                    couleurs.Add(couleur);
                }
            return new MyMaillage(points, couleurs);
        }

        public override V3 GetNormalOfPoint(V3 point)
        {
            V3 normal = point - CentreSphere;
            normal.Normalize();
            return normal ;
        }

        public override void CalculateDifferentialUV(V3 point,out float u, out float v, out V3 dmdu, out V3 dmdv)
        {
            IMA.Invert_Coord_Spherique(point, CentreSphere, Rayon, out u, out v);
            dmdu = -Rayon * (new V3(IMA.Sinf(u) * IMA.Cosf(v), -IMA.Cosf(u) * IMA.Cosf(v), 0));
            dmdv = -Rayon * (new V3(IMA.Cosf(u) * IMA.Sinf(v), IMA.Sinf(u) * IMA.Sinf(v), -IMA.Cosf(v)));

            // normalize u and v
            u = u / (2 * IMA.PI);
            v = (v + IMA.PI / 2) / (IMA.PI);
        }

    }
}
