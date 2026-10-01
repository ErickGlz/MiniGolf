using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGolf.Models
{
        public class Obstaculo
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Ancho { get; set; }
            public double Alto { get; set; }

            public Obstaculo(
                double x,
                double y,
                double ancho,
                double alto)
            {
                X = x;
                Y = y;
                Ancho = ancho;
                Alto = alto;
            }
        }
    }

