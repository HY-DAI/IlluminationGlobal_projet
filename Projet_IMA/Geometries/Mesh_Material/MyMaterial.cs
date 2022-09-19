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


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyMaterial(Texture colormap, Texture bumpmap, float bumpintensity, int specularpower)
        {
            ColorMap = colormap;
            BumpMap = bumpmap;
            BumpIntensity = bumpintensity/1000; //pour éviter d'écrire trop souvent la valeur 0.001
            SpecularPower = specularpower;
        }
        public MyMaterial(Texture colormap, Texture bumpmap, float bumpintensity) :
            this(colormap, bumpmap, bumpintensity,50)
        { }

        public MyMaterial(Texture colormap, Texture bumpmap) :
            this(colormap,bumpmap,1f)
        { }

        public MyMaterial(Texture colormap) : 
            this(colormap, new Texture(Couleur.Black))
        { }
        public MyMaterial(Couleur couleur, Texture bumpmap, float bumpintensity) :
            this(new Texture(couleur), bumpmap, bumpintensity)
        { }
    }
}
