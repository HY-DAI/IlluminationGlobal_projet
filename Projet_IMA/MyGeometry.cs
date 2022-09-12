using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projet_IMA
{
    abstract class MyGeometry
    {
        public Maillage Maillage;
        public Couleur Couleur;
        public Texture Texture;


        public static void calcul_diffuse(MyLight light, MyGeometry geometry)
        {
            foreach (var pointcolore in geometry.Maillage.Points.Zip(geometry.Maillage.Couleurs, Tuple.Create)) {
                V3 point = pointcolore.Item1;
                Couleur couleur = pointcolore.Item2;
            }
        }

        public static void calcul_speculaire(MyLight light, MyGeometry geometry)
        {
            foreach (var pointcolore in geometry.Maillage.Points.Zip(geometry.Maillage.Couleurs, Tuple.Create))
            {
                V3 point = pointcolore.Item1;
                Couleur couleur = pointcolore.Item2;
            }
        }

        public abstract void Draw(float pas);
    }
}
