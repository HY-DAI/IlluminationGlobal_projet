using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyRenderingManager 
    {

        static int largeurEcran = 960;
        static int hauteurEcran = 570;

        public static V3 eyeLocation = new V3(largeurEcran / 2, -largeurEcran, hauteurEcran / 2);


        /*
                public static void DrawGeometry(List<V3> points, List<Couleur> couleurs)
                {
                    foreach (var pointcolore in points.Zip(couleurs, Tuple.Create))
                    {
                        V3 point = pointcolore.Item1;
                        Couleur couleur = pointcolore.Item2;

                        // projection orthographique => repère écran
                        int x_ecran = (int)(point.x);
                        int y_ecran = (int)(point.z);
                        BitmapEcran.DrawPixel(x_ecran, y_ecran, couleur);
                    }
                }
        */

        public static void Draw(List<MyLight> lightsList, List<MyGeometry> geometriesList, V3 eyePosition)
        {
            int nbpx = 1000; // 910;
            int nbpz = 600; // 540;
            float u, v;

            V3 NearestIntersection,ip;
            Couleur NearestCouleur;
            MyGeometry IntersectedGeometry=geometriesList[0];

            for (int px = 0; px < nbpx; px++)
            {
                for (int pz = 0; pz < nbpz; pz++)
                {
                    V3 RayonDirection = eyePosition.NormalizedDirectionToVec(new V3(px, 0, pz));
                    NearestIntersection = eyePosition + 7000 * RayonDirection; //val arbi grande
                    foreach (MyGeometry geometry in geometriesList)
                    {
                        if (geometry.RaycastingIntersection(eyePosition, RayonDirection, out u, out v, out ip))
                        {
                            V3 P3D = geometry.Get3DPoint(u, v);
                            if ((P3D - eyePosition).Norm() < (NearestIntersection - eyePosition).Norm())
                            {
                                NearestIntersection = P3D;
                                IntersectedGeometry = geometry;
                            }
                        }
                    }
                    IntersectedGeometry.Calcul_diffuse_speculaire_bumps(lightsList, eyePosition, NearestIntersection, out NearestCouleur);

                    BitmapEcran.DrawPixel(px,pz, NearestCouleur);
                }
            }
        }
    }
}
