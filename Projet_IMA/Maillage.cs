using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    class Maillage
    {
        public List<V3> Points;
        public List<Couleur> Couleurs;

        public Maillage(List<V3> points, List<Couleur> couleurs)
        {
            Points = points;
            Couleurs = couleurs;
        }

    }
}
