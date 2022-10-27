using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA.Geometries.GeometryComponents
{
    class MyMaterial
    {
        public float BumpIntensity;
        public int SpecularPower;

        public Texture ColorMap;
        public Texture BumpMap;
        public Texture LightMap;
                                     

        public static MyMaterial Brick = new MyMaterial(Texture.BrickMap, Texture.BrickMap);
        public static MyMaterial Gold = new MyMaterial(Texture.GoldMap, Texture.GoldBumpMap);
        public static MyMaterial Fibre = new MyMaterial(Texture.FibreMap, Texture.BumpMap);
        public static MyMaterial Lead = new MyMaterial(Texture.LeadMap, Texture.LeadBumpMap);
        public static MyMaterial Rock = new MyMaterial(Texture.RockMap, Texture.BumpMap1);
        public static MyMaterial Stone = new MyMaterial(Texture.StoneMap, Texture.BumpMap2);
        public static MyMaterial Test = new MyMaterial(Texture.TestMap, Texture.TestMap);
        public static MyMaterial UVTest = new MyMaterial(Texture.UVTestMap, Texture.BumpMap3);
        public static MyMaterial Wood = new MyMaterial(Texture.WoodMap, Texture.WoodMap);

        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyMaterial(Texture colormap, Texture bumpmap, float bumpintensity, int specularpower)
        {
            ColorMap = colormap;
            BumpMap = bumpmap;
            BumpIntensity = bumpintensity/1000; //pour éviter d'écrire trop souvent la valeur 0.001
            SpecularPower = specularpower;
            LightMap = new Texture(Couleur.Black);
        }

        public MyMaterial(Texture colormap, Texture bumpmap, float bumpintensity) :
            this(colormap, bumpmap, bumpintensity,20)
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
        public MyMaterial(Couleur couleur, Texture bumpmap) :
            this(new Texture(couleur), bumpmap)
        { }
        public MyMaterial(Couleur couleur) :
            this(new Texture(couleur))
        { }


    }
}
