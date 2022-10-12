using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Projet_IMA.Geometries.GeometryComponents;

namespace Projet_IMA.Geometries
{
    class MySphere : MyGeometry
    {
        public V3 CentreSphere;
        public float Rayon;


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MySphere(V3 centreSphere, float rayon, float pas, MyMaterial material) 
        {
            CentreSphere = centreSphere;
            Rayon = rayon;
            Material = material;
            Material.LightMap = new Texture(Couleur.Black, (int)(1 / pas), (int)(1 / pas));
            //Maillage = CreerMaillageColore(pas);
            //Calcul_normals_with_bump(this, out Maillage.Normals);
        }

        public MySphere(V3 centreSphere, float rayon, float pas, Texture texture) :
            this(centreSphere, rayon, pas, new MyMaterial(texture))
        { }

        public MySphere(V3 centreSphere, float rayon, float pas, Couleur couleur) : 
            this(centreSphere, rayon, pas, new Texture(couleur))
        { }


        //---------------------------------------
        // private méthodes :
        //---------------------------------------

        /*
                private MyMaillage CreerMaillageColore(float pas)
                {
                    List<V3> points = new List<V3>();
                    List<Couleur> couleurs = new List<Couleur>();

                    for (float u = 0; u < 2 * IMA.PI; u += pas)  // echantillonage fnt paramétrique
                        for (float v = -IMA.PI / 2; v < IMA.PI / 2; v += pas)
                        {
                            // calcul des coordoonées dans la scène 3D
                            V3 P3D = get3DPoint(u,v);
                            float unormalized = u / (2 * IMA.PI);
                            float vnormalized = (v + IMA.PI / 2) / (IMA.PI);
                            Couleur couleur = Material.ColorMap.LireCouleur(unormalized, vnormalized);
                            points.Add(P3D);
                            couleurs.Add(couleur);
                        }
                    return new MyMaillage(points, couleurs);
                }
        */

        //---------------------------------------
        // public méthodes :
        //---------------------------------------

        public override V3 GetBarycenter()
        {
            return CentreSphere;
        }

        public override V3 Get3DPoint(float u, float v)
        {
            float x3D = Rayon * IMA.Cosf(v) * IMA.Cosf(u) + CentreSphere.x;
            float y3D = Rayon * IMA.Cosf(v) * IMA.Sinf(u) + CentreSphere.y;
            float z3D = Rayon * IMA.Sinf(v) + CentreSphere.z;
            return new V3(x3D, y3D, z3D);
        }



        public override V3 GetNormalOfPoint(V3 point)
        {
            V3 normal = point - CentreSphere;
            normal.Normalize();
            return normal ;
        }


        //private void normalizeUV(float out u, float out v)
       
        public override void CalculateUV(V3 point, out float u, out float v)
        {
            IMA.Invert_Coord_Spherique(point, CentreSphere, Rayon, out u, out v);
            // normalize u and v
            u = u / (2 * IMA.PI);
            v = (v + IMA.PI / 2) / (IMA.PI);
        }

        public override void CalculateDifferentialUV(V3 point, out V3 dmdu, out V3 dmdv)
        {
            float u, v;
            IMA.Invert_Coord_Spherique(point, CentreSphere, Rayon, out u, out v);
            dmdu = -Rayon * (new V3(IMA.Sinf(u) * IMA.Cosf(v), -IMA.Cosf(u) * IMA.Cosf(v), 0));
            dmdv = -Rayon * (new V3(IMA.Cosf(u) * IMA.Sinf(v), IMA.Sinf(u) * IMA.Sinf(v), -IMA.Cosf(v)));
        }


        public override bool RaycastingIntersection(V3 RayonOrigine, V3 RayonDirection, out V3 intersection)
        {
            intersection = RayonOrigine;
            bool intersectionExists = false;

            float A = RayonDirection * RayonDirection;
            float B = 2 * (RayonOrigine - CentreSphere) * RayonDirection;
            float C = (RayonOrigine - CentreSphere) * (RayonOrigine - CentreSphere) - Rayon * Rayon;

            float D = B * B - 4 * A * C;

            if (D > 0)
            {
                float t1 = (-B - IMA.Sqrtf(D)) / (2 * A);
                float t2 = (-B + IMA.Sqrtf(D)) / (2 * A);
                if (t1 > 0 && t2 > 0)
                {
                    intersection = RayonOrigine + t1 * RayonDirection;
                    intersectionExists = true;
                }
                else if (t1 < 0 || t2 > 0)
                {
                    intersection = RayonOrigine + t2 * RayonDirection;
                    intersectionExists = true;
                }
            }
            else if (D == 0) {
                float t = -B / (2 * A);
                intersection = RayonOrigine + t * RayonDirection;
                intersectionExists = true;
            }

            return intersectionExists;
        }
    }
}
