using MiniGolf.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGolf.Algoritmos
{
    public class AStar
    {
        private int filas;
        private int columnas;
        private int tamanoCelda;

        private Nodo[,] mapa = null!;

        public AStar(
            int anchoCampo,
            int altoCampo,
            int tamanoCelda,
            List<Obstaculo> obstaculos)
        {
            this.tamanoCelda = tamanoCelda;

            columnas =
                anchoCampo / tamanoCelda;

            filas =
                altoCampo / tamanoCelda;

            CrearMapa(obstaculos);
        }

        private void CrearMapa(
            List<Obstaculo> obstaculos)
        {
            mapa =
                new Nodo[filas, columnas];

            for (int y = 0; y < filas; y++)
            {
                for (int x = 0; x < columnas; x++)
                {
                    mapa[y, x] =
                        new Nodo(x, y);

                    double centroX =
                        x * tamanoCelda +
                        tamanoCelda / 2.0;

                    double centroY =
                        y * tamanoCelda +
                        tamanoCelda / 2.0;

                    foreach (Obstaculo obstaculo in obstaculos)
                    {
                       bool bloqueado =
    centroX >= obstaculo.X &&
    centroX <= obstaculo.X + obstaculo.Ancho &&
    centroY >= obstaculo.Y &&
    centroY <= obstaculo.Y + obstaculo.Alto;

                        if (bloqueado)
                        {
                            mapa[y, x].EsObstaculo =
                                true;

                            break;
                        }
                    }
                }
            }
        }

        public Nodo? ObtenerSiguienteObjetivo(
            double pelotaX,
            double pelotaY,
            double objetivoX,
            double objetivoY)
        {
            int inicioX =
                (int)((pelotaX + 12.5) /
                tamanoCelda);

            int inicioY =
                (int)((pelotaY + 12.5) /
                tamanoCelda);

            int destinoX =
                (int)((objetivoX + 17.5) /
                tamanoCelda);

            int destinoY =
                (int)((objetivoY + 17.5) /
                tamanoCelda);

            inicioX =
                Math.Clamp(
                    inicioX,
                    0,
                    columnas - 1);

            inicioY =
                Math.Clamp(
                    inicioY,
                    0,
                    filas - 1);

            destinoX =
                Math.Clamp(
                    destinoX,
                    0,
                    columnas - 1);

            destinoY =
                Math.Clamp(
                    destinoY,
                    0,
                    filas - 1);

            Nodo inicio =
                mapa[inicioY, inicioX];

            Nodo objetivo =
                mapa[destinoY, destinoX];

            if (inicio.EsObstaculo)
            {
                inicio =
                    BuscarNodoLibreCercano(
                        inicioX,
                        inicioY);
            }

            if (objetivo.EsObstaculo)
            {
                objetivo =
                    BuscarNodoLibreCercano(
                        destinoX,
                        destinoY);
            }

            List<Nodo> ruta =
                BuscarRuta(
                    inicio,
                    objetivo);

            if (ruta.Count < 2)
                return null;

            return SeleccionarSiguienteObjetivo(
                ruta);
        }

        private Nodo BuscarNodoLibreCercano(
            int x,
            int y)
        {
            for (int radio = 1;
                 radio <= 4;
                 radio++)
            {
                for (int dy = -radio;
                     dy <= radio;
                     dy++)
                {
                    for (int dx = -radio;
                         dx <= radio;
                         dx++)
                    {
                        int nuevoX =
                            x + dx;

                        int nuevoY =
                            y + dy;

                        if (nuevoX < 0 ||
                            nuevoX >= columnas ||
                            nuevoY < 0 ||
                            nuevoY >= filas)
                        {
                            continue;
                        }

                        if (!mapa[nuevoY, nuevoX]
                            .EsObstaculo)
                        {
                            return mapa[
                                nuevoY,
                                nuevoX];
                        }
                    }
                }
            }

            return mapa[y, x];
        }

        private Nodo SeleccionarSiguienteObjetivo(
      List<Nodo> ruta)
        {
            if (ruta.Count <= 2)
                return ruta[ruta.Count - 1];

            int mejorIndice = 1;
            int maximo = Math.Min(12, ruta.Count - 1);

            for (int i = 2; i <= maximo; i++)
            {
                if (HayLineaLibre(
                    ruta[0],
                    ruta[i]))
                {
                    mejorIndice = i;
                }
                else
                {
                    break;
                }
            }

            return ruta[mejorIndice];
        }

        private bool HayLineaLibre(
            Nodo inicio,
            Nodo destino)
        {
            double x1 =
                inicio.X + 0.5;

            double y1 =
                inicio.Y + 0.5;

            double x2 =
                destino.X + 0.5;

            double y2 =
                destino.Y + 0.5;

            double diferenciaX =
                x2 - x1;

            double diferenciaY =
                y2 - y1;

            int pasos =
                (int)Math.Ceiling(
                    Math.Max(
                        Math.Abs(diferenciaX),
                        Math.Abs(diferenciaY)) *
                    4);

            if (pasos <= 0)
                return true;

            for (int i = 0;
                 i <= pasos;
                 i++)
            {
                double t =
                    (double)i / pasos;

                int x =
                    (int)Math.Floor(
                        x1 +
                        diferenciaX * t);

                int y =
                    (int)Math.Floor(
                        y1 +
                        diferenciaY * t);

                if (x < 0 ||
                    x >= columnas ||
                    y < 0 ||
                    y >= filas)
                {
                    return false;
                }

                if (mapa[y, x]
                    .EsObstaculo)
                {
                    return false;
                }
            }

            return true;
        }

        public double ObtenerDestinoX(
            Nodo nodo)
        {
            return nodo.X *
                   tamanoCelda +
                   tamanoCelda / 2.0;
        }

        public double ObtenerDestinoY(
            Nodo nodo)
        {
            return nodo.Y *
                   tamanoCelda +
                   tamanoCelda / 2.0;
        }

        private List<Nodo> BuscarRuta(
            Nodo inicio,
            Nodo objetivo)
        {
            List<Nodo> abiertos =
                new List<Nodo>();

            HashSet<Nodo> cerrados =
                new HashSet<Nodo>();

            ReiniciarMapa();

            inicio.G = 0;

            inicio.H =
                CalcularHeuristica(
                    inicio,
                    objetivo);

            abiertos.Add(inicio);

            while (abiertos.Count > 0)
            {
                Nodo actual =
                    abiertos
                    .OrderBy(n => n.F)
                    .ThenBy(n => n.H)
                    .First();

                if (actual.X == objetivo.X &&
                    actual.Y == objetivo.Y)
                {
                    return ReconstruirRuta(
                        actual);
                }

                abiertos.Remove(
                    actual);

                cerrados.Add(
                    actual);

                foreach (
                    Nodo vecino
                    in ObtenerVecinos(actual))
                {
                    if (vecino.EsObstaculo ||
                        cerrados.Contains(vecino))
                    {
                        continue;
                    }

                    int diferenciaX =
                        Math.Abs(
                            vecino.X -
                            actual.X);

                    int diferenciaY =
                        Math.Abs(
                            vecino.Y -
                            actual.Y);

                    bool diagonal =
                        diferenciaX == 1 &&
                        diferenciaY == 1;

                    if (diagonal &&
                        !DiagonalPermitida(
                            actual,
                            vecino))
                    {
                        continue;
                    }

                    double costo =
                        diagonal
                        ? Math.Sqrt(2)
                        : 1;

                    double nuevoG =
                        actual.G +
                        costo;

                    if (nuevoG <
                        vecino.G)
                    {
                        vecino.Padre =
                            actual;

                        vecino.G =
                            nuevoG;

                        vecino.H =
                            CalcularHeuristica(
                                vecino,
                                objetivo);

                        if (!abiertos.Contains(
                            vecino))
                        {
                            abiertos.Add(
                                vecino);
                        }
                    }
                }
            }

            return new List<Nodo>();
        }

        private List<Nodo> ObtenerVecinos(
            Nodo actual)
        {
            List<Nodo> vecinos =
                new List<Nodo>();

            for (int y = -1;
                 y <= 1;
                 y++)
            {
                for (int x = -1;
                     x <= 1;
                     x++)
                {
                    if (x == 0 &&
                        y == 0)
                    {
                        continue;
                    }

                    AgregarVecino(
                        vecinos,
                        actual.X + x,
                        actual.Y + y);
                }
            }

            return vecinos;
        }

        private void AgregarVecino(
            List<Nodo> vecinos,
            int x,
            int y)
        {
            if (x >= 0 &&
                x < columnas &&
                y >= 0 &&
                y < filas)
            {
                vecinos.Add(
                    mapa[y, x]);
            }
        }

        private bool DiagonalPermitida(
            Nodo actual,
            Nodo destino)
        {
            int diferenciaX =
                destino.X -
                actual.X;

            int diferenciaY =
                destino.Y -
                actual.Y;

            int xHorizontal =
                actual.X +
                diferenciaX;

            int yHorizontal =
                actual.Y;

            int xVertical =
                actual.X;

            int yVertical =
                actual.Y +
                diferenciaY;

            if (xHorizontal < 0 ||
                xHorizontal >= columnas ||
                yVertical < 0 ||
                yVertical >= filas)
            {
                return false;
            }

            return
                !mapa[
                    yHorizontal,
                    xHorizontal]
                    .EsObstaculo
                &&
                !mapa[
                    yVertical,
                    xVertical]
                    .EsObstaculo;
        }

        private double CalcularHeuristica(
            Nodo actual,
            Nodo objetivo)
        {
            int dx =
                Math.Abs(
                    actual.X -
                    objetivo.X);

            int dy =
                Math.Abs(
                    actual.Y -
                    objetivo.Y);

            int diagonal =
                Math.Min(
                    dx,
                    dy);

            int recto =
                Math.Max(
                    dx,
                    dy) -
                diagonal;

            return
                diagonal *
                Math.Sqrt(2) +
                recto;
        }

        private List<Nodo> ReconstruirRuta(
            Nodo objetivo)
        {
            List<Nodo> ruta =
                new List<Nodo>();

            Nodo? actual =
                objetivo;

            while (actual != null)
            {
                ruta.Add(
                    actual);

                actual =
                    actual.Padre;
            }

            ruta.Reverse();

            return ruta;
        }

        private void ReiniciarMapa()
        {
            for (int y = 0;
                 y < filas;
                 y++)
            {
                for (int x = 0;
                     x < columnas;
                     x++)
                {
                    mapa[y, x].G =
                        double.MaxValue;

                    mapa[y, x].H =
                        0;

                    mapa[y, x].Padre =
                        null;
                }
            }
        }
    }
}