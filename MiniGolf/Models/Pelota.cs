using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGolf.Models
{
    namespace MiniGolf.Models
    {
        public class Pelota
        {
            public double X { get; set; }
            public double Y { get; set; }

            public double VelocidadX { get; set; }
            public double VelocidadY { get; set; }

            public int Golpes { get; set; }

            public bool EnHoyo { get; set; }

            public Pelota(double x, double y)
            {
                X = x;
                Y = y;

                VelocidadX = 0;
                VelocidadY = 0;

                Golpes = 0;
                EnHoyo = false;
            }

            public bool EstaMoviendose()
            {
                return Math.Abs(VelocidadX) > 0.05 ||
                       Math.Abs(VelocidadY) > 0.05;
            }
        }
    }
}
