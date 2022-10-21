using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Projet_IMA.Geometries.GeometryComponents;

namespace Projet_IMA.Geometries
{
    abstract class MyGeometry
    {
        //public MyMaillage Maillage;
        public MyMaterial Material;
        public bool IsGeometryLight;

        public static List<MyGeometry> GeometriesList = new List<MyGeometry>();


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------
        public MyGeometry()
        {
            IsGeometryLight = false;
            MyGeometry.GeometriesList.Add(this);
        }


        //---------------------------------------
        // public méthodes :
        //---------------------------------------

        public abstract V3 GetBarycenter();

        public abstract V3 Get3DPoint(float u, float v);

        public abstract V3 GetNormalOfPoint(V3 point);
        
        public abstract void CalculateUV(V3 point, out float u, out float v);

        public abstract void CalculateDifferentialUV(V3 point, out V3 dmdu, out V3 dmdv);
        

        public abstract bool RaycastingIntersection(V3 RayonOrigine, V3 RayonDirection, out V3 intersection);

        public V3 GetNormalWithBump(V3 normal, float u, float v, V3 dmdu, V3 dmdv)
        {
            float dhdu, dhdv;
            Material.BumpMap.Bump(u, v, out dhdu, out dhdv);
            V3 Nuv = (dmdu ^ dmdv) / (dmdu ^ dmdv).Norm();
            if (Nuv * normal < 0)
                Nuv = -Nuv;
            V3 T2 = dhdu * (Nuv ^ dmdv);
            V3 T3 = dhdv * (dmdu ^ Nuv);
            return Nuv + (T2 + T3) * Material.BumpIntensity;
        }


       
    }
}
