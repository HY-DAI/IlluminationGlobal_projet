using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Projet_IMA
{
    class Texture
    {
        int Hauteur;
        int Largeur;
        Couleur [,] C;

        // u,v compris entre 0 et 1

        public static Texture BrickMap = new Texture("brick01.jpg");
        public static Texture GoldMap = new Texture("gold.jpg");
        public static Texture FibreMap = new Texture("fibre.jpg");
        public static Texture LeadMap = new Texture("lead.jpg");
        public static Texture RockMap = new Texture("rock.jpg");
        public static Texture StoneMap = new Texture("stone2.jpg");
        public static Texture TestMap = new Texture("test.jpg");
        public static Texture UVTestMap = new Texture("uvtest.jpg");
        public static Texture WoodMap = new Texture("wood.jpg");

        public static Texture BumpMap = new Texture("bump.jpg");
        public static Texture BumpMap1 = new Texture("bump1.jpg");
        public static Texture BumpMap2 = new Texture("bump20.jpg");
        public static Texture BumpMap3 = new Texture("bump38.jpg");
        public static Texture GoldBumpMap = new Texture("gold_Bump.jpg");
        public static Texture LeadBumpMap = new Texture("lead_bump.jpg");


        //---------------------------------------
        // Constructeurs :
        //---------------------------------------

        public Texture(Couleur Couleur, int largeur, int hauteur)
        {
            Hauteur = hauteur;
            Largeur = largeur;
            C = new Couleur[Largeur, Hauteur];
            for (int x = 0; x < Largeur; x++)
                for (int y = 0; y < Hauteur; y++)
                    C[x, y] = Couleur;
        }

        public Texture(Couleur Couleur) :
            this(Couleur, 1, 1)
        {   }


        public Texture(string ff)
        {
            string s = System.IO.Path.GetFullPath("..\\..");
            string path = System.IO.Path.Combine(s,"textures",ff);
            Bitmap B = new Bitmap(path); 
            
            Hauteur = B.Height;
            Largeur = B.Width;
            BitmapData data = B.LockBits(new Rectangle(0, 0, B.Width, B.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = data.Stride;
             
            C = new Couleur[Largeur,Hauteur];

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                for (int x = 0; x < Largeur; x++)
                    for (int y = 0; y < Hauteur; y++)
                    {
                        byte RR, VV, BB;
                        BB = ptr[(x * 3) + y * stride];
                        VV = ptr[(x * 3) + y * stride + 1];
                        RR = ptr[(x * 3) + y * stride + 2];
                        C[x, y].From255(RR, VV, BB);
                    }
            }
            B.UnlockBits(data);
            B.Dispose();
        }





        //---------------------------------------
        // Public functions :
        //---------------------------------------

        public Couleur LireCouleur(float u, float v)
        {
            return Interpol(Largeur * u, Hauteur * v);
        }

        public void ConvertBW()
        {
            for (int l = 0; l < Largeur; l++)
                for (int h=0; h<Hauteur; h++)
                    C[l,h] = Couleur.White*C[l, h].GreyLevel();
        }

        public void Bump(float u, float v, out float dhdu, out float dhdv)
        {
            float x = u * Hauteur;
            float y = v * Largeur;

            float vv = Interpol(x, y).GreyLevel();
            float vx = Interpol(x + 1, y).GreyLevel();
            float vy = Interpol(x, y + 1).GreyLevel();

            dhdu = vx - vv;
            dhdv = vy - vv;
        }

        public void SetColorByUV(float u, float v, Couleur couleur)
        {
            UVintoXY(Largeur*u, Hauteur*v, out int x, out int y);
            C[x, y] = couleur;
        }

        //---------------------------------------
        // Private functions :
        //---------------------------------------

        private void UVintoXY(float u, float v, out int x, out int y)
        {
            x = (int)u; // plus grand entier <=
            y = (int)v;

            //  float cx = Lu - x; // reste
            //  float cy = Hv - y;

            x = x % Largeur;
            y = y % Hauteur;
            if (x < 0) x += Largeur;
            if (y < 0) y += Hauteur;
        }

        private Couleur Interpol(float Lu, float Hv)
        {
            UVintoXY(Lu, Hv, out int x, out int y);
            return C[x, y];

         /*
            int x = (int)Lu; // plus grand entier <=
            int y = (int)Hv;

            float cx = Lu - x; // reste
            float cy = Hv - y;

            x = x % Largeur;
            y = y % Hauteur;
            if (x < 0) x += Largeur;
            if (y < 0) y += Hauteur;

            int xpu = (x + 1) % Largeur;
            int ypu = (y + 1) % Hauteur;

            float ccx = cx * cx;
            float ccy = cy * cy;

            return
              C[x, y] * (1 - ccx) * (1 - ccy)
            + C[xpu, y] * ccx * (1 - ccy)
            + C[x, ypu] * (1 - ccx) * ccy
            + C[xpu, ypu] * ccx * ccy;
        */
        }
    }    
}
