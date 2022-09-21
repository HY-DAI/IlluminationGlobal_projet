using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyPointLight : MyLight
    {

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyPointLight(V3 lightpos, V3 lightdir, Couleur couleur, float intensity) :
            base(lightpos, lightdir, couleur, intensity)
        { }


        //---------------------------------------
        // autres méthodes :
        //---------------------------------------

        public override V3 getLightDirOnPoint(V3 point)
        {
            V3 lightdir = point - LightPosition;
            lightdir.Normalize();
            return lightdir;
        }

        public override Couleur shadowsIfIntersection(List<MyGeometry> geometriesList, MyGeometry geometry, V3 point, Couleur couleur)
        {
            // the point in argument should be from from the raycasting with the eyeposition (nearest point)
            V3 RayonDirection = (point - LightPosition) / (point - LightPosition).Norm();
            V3 ip;
            MyGeometry ig;

            float u, v;
            foreach (MyGeometry geom in geometriesList)
            {
                if ( geom.RaycastingIntersection(point, -RayonDirection, out u, out v, out ip))
                {
                    if (!Object.ReferenceEquals(geometry, geom) && (ip - LightPosition).Norm() < (point - LightPosition).Norm())
                        return couleur * (Couleur.White - this.Couleur * 0.9f);
                }
            }
            return couleur;
        }
    }
}
