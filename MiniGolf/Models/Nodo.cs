using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGolf.Models
{
    public class Nodo
    {
        public int X { get; set; }
        public int Y { get; set; }

        // Costo desde el inicio hasta este nodo
        public double G { get; set; }

        // Distancia estimada desde este nodo hasta el objetivo
        public double H { get; set; }

        // Costo total
        public double F
        {
            get { return G + H; }
        }

        // Indica si A* puede atravesar esta casilla
        public bool EsObstaculo { get; set; }

        // Nodo anterior utilizado para reconstruir la ruta
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
