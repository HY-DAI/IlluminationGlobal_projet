using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections.Concurrent;
using Projet_IMA.Lights;

namespace Projet_IMA
{
    public partial class Form1 : Form
    {

        // pour dessiner à l'écran
        Graphics canvas;

        // liste de tous les threads
        List<Thread> LThreads = new List<Thread>();

        // largeur de la zone carrée
        int LargZonePix = 50;

        // liste des zones carré à traiter
        ConcurrentBag<Point> JobList = new ConcurrentBag<Point>();


        public Form1()
        {
            InitializeComponent();
            //pictureBox1.Image = BitmapEcran.Init(pictureBox1.Width, pictureBox1.Height); //Séquentiel
            canvas = pictureBox1.CreateGraphics(); //Multithreading
        }

        public bool Checked()               { return checkBox1.Checked;   }
        public void PictureBoxInvalidate()  { pictureBox1.Invalidate(); }
        public void PictureBoxRefresh()     { pictureBox1.Refresh();    }

/*
        // Séquentiel -----------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            BitmapEcran.RefreshScreen(new Couleur(0.2f, 0.2f, 0.2f));
            ProjetEleve.Go();
            BitmapEcran.Show();          
        }
*/

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Multithreading -------------------------------------------------------

        // lorsque vous cliquez sur le bouton dans la fenêtre
        private void button1_Click(object sender, EventArgs e)
        {
            ProjetEleve.Go();

            // crée la liste des zones à afficher
            int LargAff = pictureBox1.Width;
            int HautAff = pictureBox1.Height;
            for (int x = 0; x < LargAff; x += LargZonePix)
                for (int y = 0; y < HautAff; y += LargZonePix)
                    JobList.Add(new Point(x, y));

            //JobList.Add(new Point(pictureBox1.Width/2, pictureBox1.Height/2));

            // crée et lance le pool de threads
            canvas.FillRectangle(Brushes.Gray, 0, 0, 2000, 2000);
            for (int i = 0; i < 1; i++)  // 4: nb de threads
            {
                int idThread = i; // capture correctement la valeur de i pour le délégué ci-dessous
                Thread T = new Thread(delegate () { FntThread(idThread); });
                LThreads.Add(T);
                T.Start();        // demarre le thread enfant
            }


        }

        delegate void MonDelegue(Point P, Bitmap B);

        // fonction appelée dans le thread principal suite à l'envoi d'un évènement 
        // par un thread enfant grâce à la méthode invoke

        private void DrawInMainThread(Point P, Bitmap B)
        { 
            // pour corriger l'inversion de repère dans canevas :
            P.Y = pictureBox1.Height-P.Y-LargZonePix;
            canvas.DrawImage(B, P);
        }

        // methode déclenchée par chaque thread
        // le code ci-dessous s'exécute dans les threads enfants


        private void FntThread(int idThread)
        {
            //Brush MaCouleur = LBrushes[idThread];
            //int MonTemps = Temps[idThread];

            Point CoordZone;

            // capture une zone dans la liste des zones à traiter
            while (JobList.TryTake(out CoordZone))
            {
                // crée un bitmap local correspondant à la zone à dessiner
                Bitmap B = MyRenderingManager.SetBitmapPixels(ProjetEleve.lights, ProjetEleve.geometries, CoordZone, LargZonePix);
                Graphics G = Graphics.FromImage(B);

                // renvoi les infos suffisantes dans un évènement pour que
                // le thread principal puisse dessiner la région au bon endroit
                var d = new MonDelegue(DrawInMainThread);
                pictureBox1.Invoke(d, new object[] { CoordZone, B });
            }
        }



    }
}
