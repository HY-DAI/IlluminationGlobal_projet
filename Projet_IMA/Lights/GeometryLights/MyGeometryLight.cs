using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Projet_IMA.Geometries;

namespace Projet_IMA.Lights.GeometryLights
{
    abstract class MyGeometryLight : MyLight
    {
        public MyGeometry Geometry;

        public MyGeometryLight(Couleur couleur, float intensity, MyGeometry geometry) :
            base(couleur, intensity)
        {
            Geometry = geometry;
        }

        public MyGeometryLight(Couleur couleur, float intensity) :
            this(couleur, intensity, new MyParallelogram(V3.Vnull, V3.Vnull, V3.Vnull, 1f,Couleur.Black))
        { }



        //public V3 getLightPosition() { return Geometry.GetBarycenter(); }

    }
}
