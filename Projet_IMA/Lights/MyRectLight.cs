using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
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

        public override bool IlluminatedUnderPhysicalLight(V3 pointofgeom)
        {
            V3 lightToPointDir = Geometry.GetBarycenter().NormalizedDirectionToVec(pointofgeom);
            V3 rectLightDir = GetLightDirOnPoint(Geometry.GetBarycenter());
            float cosOfAngle = lightToPointDir * rectLightDir;
            bool inLightedZone = (cosOfAngle >= 0); 
            return inLightedZone && Geometry.RaycastingIntersection(pointofgeom, -rectLightDir);
        }



    }
}
