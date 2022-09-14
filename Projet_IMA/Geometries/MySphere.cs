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

        private MyMaillage CreerMaillageColore(float pas)
        {
            List<V3> points = new List<V3>();
            List<V3> normals = new List<V3>();
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
                    float dhdu, dhdv;
                    V3 dmdu, dmdv;
                    Material.BumpMap.Bump(u, v, out dhdu, out dhdv);
                    dmdu = new V3(-IMA.Sinf(u) * IMA.Sinf(v), IMA.Cosf(u) * IMA.Sinf(v), 0);
                    dmdv = new V3(IMA.Cosf(u) * IMA.Cosf(v), IMA.Sinf(u) * IMA.Cosf(v), -IMA.Sinf(v));
                    V3 point = new V3(x3D, y3D, z3D);
                    Couleur couleur = Material.ColorMap.LireCouleur(unormalized, vnormalized);
                    V3 additionalBump = (dhdu * dmdv + dhdv * dmdu)*Material.BumpIntensity;
                    points.Add(point);
                    couleurs.Add(couleur);
                    normals.Add(CentreSphere.NormalizedDirectionToVec(point)+additionalBump);
                }
            return new MyMaillage(points, couleurs, normals); 
        }



        public MySphere(V3 centreSphere, float rayon, float pas, MyMaterial material)
        {
            CentreSphere = centreSphere;
            Rayon = rayon;
            Material = material;
            Maillage = CreerMaillageColore(pas);
        }

        public MySphere(V3 centreSphere, float rayon, float pas, Texture texture) :
            this(centreSphere, rayon, pas, new MyMaterial(texture))
        { }

        public MySphere(V3 centreSphere, float rayon, float pas, Couleur couleur) : 
            this(centreSphere, rayon, pas, new Texture(couleur))
        { }


        public override V3 GetNormalOfPoint(V3 point)
        {
            V3 normal = point - CentreSphere;
            normal.Normalize();
            return normal ;
        }

    }
}
