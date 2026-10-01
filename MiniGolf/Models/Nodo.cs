using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGolf.Models
{
    public class Nodo
    {
        public int X { get; set; }
        public int Y { get; set; }

        public double G { get; set; }

        public double H { get; set; }

        public double F
        {
            get { return G + H; }
        }

        public bool EsObstaculo { get; set; }

        public Nodo? Padre { get; set; }

        public Nodo(int x, int y)
        {
            X = x;
            Y = y;

            G = double.MaxValue;
            H = 0;

            EsObstaculo = false;
            Padre = null;
        }
    }
}
