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
        public int SpecularPower = 50;
/*
        public MyGeometry(Maillage maillage, MyMaterial material)
        {
            Maillage = maillage;
            Material = material;
        }
*/

        public abstract V3 GetNormalOfPoint(V3 point);


    }
}
