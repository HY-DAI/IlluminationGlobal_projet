using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Projet_IMA.Geometries;

namespace Projet_IMA.Lights.GeometryLights
{
    class MyRectLight : MyGeometryLight
    {
        public MyRectLight(Couleur couleur, float intensity, MyParallelogram parallelogram) :
            base(couleur,intensity, parallelogram)
        {  }


        public override V3 GetLightDirOnPoint(V3 point)
        {
            return Geometry.GetNormalOfPoint(point);
        }

        public override bool CanIlluminatePoint(V3 pointofgeom)
        {
            V3 intersection;
            V3 lightToPointDir = Geometry.GetBarycenter().NormalizedDirectionToVec(pointofgeom);
            V3 rectLightDir = GetLightDirOnPoint(Geometry.GetBarycenter());
            float cosOfAngle = lightToPointDir * rectLightDir;
            bool inLightedZone = (cosOfAngle >= 0); 
            return inLightedZone && Geometry.RaycastingIntersection(pointofgeom, -rectLightDir, out intersection);
        }



    }
}
