using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Lights
{
    class MyConeLight : MyLight
    {
        V3 LightPosition;
        V3 LightDirection;
        float Angle;

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyConeLight(V3 lightpos, V3 lightdir, Couleur couleur, float intensity, float angle) :
            base(couleur, intensity)
        {
            LightPosition = lightpos;
            LightDirection = lightdir;
            Angle = angle;
        }


        //---------------------------------------
        // autres méthodes :
        //---------------------------------------

        public override V3 GetLightDirOnPoint(V3 point)
        {
            return LightDirection;
        }

        public override bool IlluminatedUnderPhysicalLight(V3 pointofgeom)
        {
            V3 lightToPointDir = LightPosition.NormalizedDirectionToVec(pointofgeom);
            float cosOfAngle = lightToPointDir * LightDirection;
            bool inLightedZone = (cosOfAngle >= IMA.Cosf(Angle));
            return inLightedZone;
        }
    }
}
