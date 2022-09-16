using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyMaterial
    {
        public Texture ColorMap;
        public Texture BumpMap;
        public float BumpIntensity;
        public int SpecularPower;

        public MyMaterial(Texture colormap, Texture bumpmap, float bumpintensity, int specularpower)
        {
            ColorMap = colormap;
            BumpMap = bumpmap;
            BumpIntensity = bumpintensity;
            SpecularPower = specularpower;
        }
        public MyMaterial(Texture colormap, Texture bumpmap, float bumpintensity) :
            this(colormap, bumpmap, bumpintensity,50)
        { }

        public MyMaterial(Texture colormap, Texture bumpmap) :
            this(colormap,bumpmap,2f)
        { }

        public MyMaterial(Texture colormap) : 
            this(colormap, new Texture(Couleur.Black))
        { }
        public MyMaterial(Couleur couleur, Texture bumpmap, float bumpintensity) :
            this(new Texture(couleur), bumpmap, bumpintensity)
        { }
    }
}
