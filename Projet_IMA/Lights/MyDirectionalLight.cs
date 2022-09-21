using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyDirectionalLight : MyLight
    {


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyDirectionalLight(V3 lightdir, Couleur couleur, float intensity) :
            base(lightdir, couleur, intensity)
        {  }


        //---------------------------------------
        // autres méthodes :
        //---------------------------------------

        public override V3 getLightDirOnPoint(V3 point) { return LightDirection; }

        public override Couleur shadowsIfIntersection(List<MyGeometry> geometriesList, MyGeometry geometry, V3 point, Couleur couleur)
        {            
            V3 ip;

            float u, v;
            foreach (MyGeometry geom in geometriesList)
            {
                if ( geom.RaycastingIntersection(point, -LightDirection, out u, out v, out ip))
                {
                    if (!Object.ReferenceEquals(geometry, geom) && (ip - point)*LightDirection<0)
                        return couleur * (Couleur.White-this.Couleur*0.9f);
                }
            }
            return couleur;
        }
    }
}
