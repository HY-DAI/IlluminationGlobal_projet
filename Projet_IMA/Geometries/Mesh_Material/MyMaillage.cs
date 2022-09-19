using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class MyMaillage
    {
        public List<V3> Points;
        public List<V3> Normals;
        public List<Couleur> Couleurs;


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public MyMaillage(List<V3> points, List<Couleur> couleurs, List<V3> normals)
        {
            Points = points;
            Couleurs = couleurs;
            Normals = normals;
        }

        public MyMaillage(List<V3> points, List<Couleur> couleurs)
        {
            Points = points;
            Couleurs = couleurs;
        }

        public MyMaillage() : this(new List<V3> {new V3(0,0,0)}, new List<Couleur> {Couleur.Black})
        { }
    }
}
