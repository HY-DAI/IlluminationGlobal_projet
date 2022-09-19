using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    abstract class MyGeometry
    {
        public MyMaillage Maillage;
        //public Couleur Couleur;
        //public Texture Texture;
        public MyMaterial Material;

        public static List<MyGeometry> GeometriesList = new List<MyGeometry>();

        public MyGeometry()
        {
            MyGeometry.GeometriesList.Add(this);
        }


        public abstract V3 GetNormalOfPoint(V3 point);

        public abstract void CalculateDifferentialUV(V3 point, out float u, out float v, out V3 dmdu, out V3 dmdv);

        public abstract bool RaycastingIntersection(V3 RayonOrigine, V3 RayonDirection, out float u, out float v);

        public abstract V3 get3DPoint(float u, float v);

    }
}
