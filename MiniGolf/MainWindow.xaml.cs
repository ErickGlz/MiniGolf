using MiniGolf.Algoritmos;
using MiniGolf.Models;
using MiniGolf.Models.MiniGolf.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MiniGolf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Pelota jugador;
        private Pelota ia;

        private bool apuntando = false;
        private bool turnoJugador = true;
        private bool jugadorYaGolpeo = false;
        private bool iaYaGolpeo = false;
        private bool partidaTerminada = false;

        private DispatcherTimer timer;

        private const double FRICCION = 0.985;
        private const double VELOCIDAD_MAXIMA = 16;
        private const int TAMANO_CELDA = 25;

        private const double HOYO_X = 1090;
        private const double HOYO_Y = 545;
        private const double TAMANO_HOYO = 42;

        private List<Obstaculo> obstaculos;
        private AStar aStar;

        public MainWindow()
        {
            InitializeComponent();

            jugador = new Pelota(70, 325);
            ia = new Pelota(70, 370);

            obstaculos = new List<Obstaculo>
{
    new Obstaculo(190, 400, 170, 100),
    new Obstaculo(380, 205, 200, 65),
    new Obstaculo(480, 440, 170, 100),
    new Obstaculo(625, 70, 180, 95),
    new Obstaculo(690, 300, 190, 70),
    new Obstaculo(760, 470, 180, 100),
    new Obstaculo(900, 150, 170, 95),
    new Obstaculo(955, 380, 150, 65)
};

            aStar = new AStar(
                (int)Campo.Width,
                (int)Campo.Height,
                TAMANO_CELDA,
                obstaculos);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Timer_Tick;
            timer.Start();

            ActualizarVisuales();
        }

        private void PelotaJugador_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (!turnoJugador ||
                jugador.EstaMoviendose() ||
                jugador.EnHoyo ||
                partidaTerminada)
            {
                return;
            }

            apuntando = true;

            Campo.CaptureMouse();

            LineaDireccion.Visibility =
                Visibility.Visible;

            e.Handled = true;
        }

        private void Campo_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (!apuntando)
                return;

            Point mouse =
                e.GetPosition(Campo);

            double centroX =
                jugador.X + 12.5;

            double centroY =
                jugador.Y + 12.5;

            LineaDireccion.X1 =
                centroX;

            LineaDireccion.Y1 =
                centroY;

            LineaDireccion.X2 =
                mouse.X;

            LineaDireccion.Y2 =
                mouse.Y;
        }

        private void Campo_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (!apuntando)
                return;

            apuntando = false;

            Point mouse =
                e.GetPosition(Campo);

            double centroX =
                jugador.X + 12.5;

            double centroY =
                jugador.Y + 12.5;

            double direccionX =
                centroX - mouse.X;

            double direccionY =
                centroY - mouse.Y;

            double distancia =
                Math.Sqrt(
                    direccionX * direccionX +
                    direccionY * direccionY);

            if (distancia > 5)
            {
                direccionX /= distancia;
                direccionY /= distancia;

                double potencia =
                    distancia / 12.0;

                potencia =
                    Math.Min(
                        potencia,
                        VELOCIDAD_MAXIMA);

                jugador.VelocidadX =
                    direccionX * potencia;

                jugador.VelocidadY =
                    direccionY * potencia;

                jugador.Golpes++;

                txtGolpesJugador.Text =
                    $"Golpes: {jugador.Golpes}";

                jugadorYaGolpeo = true;
            }

            LineaDireccion.Visibility =
                Visibility.Hidden;

            Campo.ReleaseMouseCapture();
        }

        private void Timer_Tick(
            object? sender,
            EventArgs e)
        {
            if (partidaTerminada)
                return;

            MoverPelota(jugador);
            MoverPelota(ia);

            if (jugador.EnHoyo &&
                ia.EnHoyo)
            {
                TerminarPartida();
                ActualizarVisuales();
                return;
            }

            if (turnoJugador)
            {
                if (jugador.EnHoyo)
                {
                    turnoJugador = false;

                    if (!ia.EstaMoviendose())
                    {
                        GolpearIA();
                    }
                }
                else if (
                    jugadorYaGolpeo &&
                    !jugador.EstaMoviendose())
                {
                    jugadorYaGolpeo = false;
                    turnoJugador = false;

                    GolpearIA();
                }
            }
            else
            {
                if (ia.EnHoyo)
                {
                    turnoJugador = true;
                }
                else if (
                    iaYaGolpeo &&
                    !ia.EstaMoviendose())
                {
                    iaYaGolpeo = false;

                    if (jugador.EnHoyo)
                    {
                        GolpearIA();
                    }
                    else
                    {
                        turnoJugador = true;
                    }
                }
            }

            ActualizarVisuales();
        }

        private void GolpearIA()
        {
            if (ia.EnHoyo ||
                partidaTerminada)
            {
                return;
            }

            Nodo? siguiente =
                aStar.ObtenerSiguienteObjetivo(
                    ia.X,
                    ia.Y,
                    HOYO_X,
                    HOYO_Y);

            if (siguiente == null)
            {
                if (!jugador.EnHoyo)
                {
                    turnoJugador = true;
                }

                return;
            }

            double destinoX =
                aStar.ObtenerDestinoX(
                    siguiente);

            double destinoY =
                aStar.ObtenerDestinoY(
                    siguiente);

            double centroIAX =
                ia.X + 12.5;

            double centroIAY =
                ia.Y + 12.5;

            double direccionX =
                destinoX - centroIAX;

            double direccionY =
                destinoY - centroIAY;

            double distancia =
                Math.Sqrt(
                    direccionX * direccionX +
                    direccionY * direccionY);

            if (distancia <= 0)
            {
                turnoJugador = true;
                return;
            }

            direccionX /= distancia;
            direccionY /= distancia;

            double potencia =
                distancia / 18.0;

            potencia =
                Math.Clamp(
                    potencia,
                    3.5,
                    9.5);

            ia.VelocidadX =
                direccionX * potencia;

            ia.VelocidadY =
                direccionY * potencia;

            ia.Golpes++;

            txtGolpesIA.Text =
                $"Golpes: {ia.Golpes}";

            iaYaGolpeo = true;
        }

        private void MoverPelota(
            Pelota pelota)
        {
            if (pelota.EnHoyo)
                return;

            if (!pelota.EstaMoviendose())
            {
                pelota.VelocidadX = 0;
                pelota.VelocidadY = 0;
                return;
            }

            pelota.X +=
                pelota.VelocidadX;

            pelota.Y +=
                pelota.VelocidadY;

            DetectarColisiones(
                pelota);

            if (DetectarHoyo(
                pelota))
            {
                return;
            }

            if (pelota.X <= 0)
            {
                pelota.X = 0;

                pelota.VelocidadX *=
                    -0.8;
            }

            if (pelota.X + 25 >=
                Campo.Width)
            {
                pelota.X =
                    Campo.Width - 25;

                pelota.VelocidadX *=
                    -0.8;
            }

            if (pelota.Y <= 0)
            {
                pelota.Y = 0;

                pelota.VelocidadY *=
                    -0.8;
            }

            if (pelota.Y + 25 >=
                Campo.Height)
            {
                pelota.Y =
                    Campo.Height - 25;

                pelota.VelocidadY *=
                    -0.8;
            }

            pelota.VelocidadX *=
                FRICCION;

            pelota.VelocidadY *=
                FRICCION;

            if (Math.Abs(
                pelota.VelocidadX) < 0.05)
            {
                pelota.VelocidadX = 0;
            }

            if (Math.Abs(
                pelota.VelocidadY) < 0.05)
            {
                pelota.VelocidadY = 0;
            }
        }

        private void DetectarColisiones(
            Pelota pelota)
        {
            const double tamanoPelota = 25;

            foreach (
                Obstaculo obstaculo
                in obstaculos)
            {
                bool colision =
                    pelota.X <
                    obstaculo.X +
                    obstaculo.Ancho &&

                    pelota.X +
                    tamanoPelota >
                    obstaculo.X &&

                    pelota.Y <
                    obstaculo.Y +
                    obstaculo.Alto &&

                    pelota.Y +
                    tamanoPelota >
                    obstaculo.Y;

                if (!colision)
                    continue;

                double desdeIzquierda =
                    pelota.X +
                    tamanoPelota -
                    obstaculo.X;

                double desdeDerecha =
                    obstaculo.X +
                    obstaculo.Ancho -
                    pelota.X;

                double desdeArriba =
                    pelota.Y +
                    tamanoPelota -
                    obstaculo.Y;

                double desdeAbajo =
                    obstaculo.Y +
                    obstaculo.Alto -
                    pelota.Y;

                double minimoHorizontal =
                    Math.Min(
                        desdeIzquierda,
                        desdeDerecha);

                double minimoVertical =
                    Math.Min(
                        desdeArriba,
                        desdeAbajo);

                if (minimoHorizontal <
                    minimoVertical)
                {
                    if (desdeIzquierda <
                        desdeDerecha)
                    {
                        pelota.X =
                            obstaculo.X -
                            tamanoPelota;
                    }
                    else
                    {
                        pelota.X =
                            obstaculo.X +
                            obstaculo.Ancho;
                    }

                    pelota.VelocidadX *=
                        -0.75;
                }
                else
                {
                    if (desdeArriba <
                        desdeAbajo)
                    {
                        pelota.Y =
                            obstaculo.Y -
                            tamanoPelota;
                    }
                    else
                    {
                        pelota.Y =
                            obstaculo.Y +
                            obstaculo.Alto;
                    }

                    pelota.VelocidadY *=
                        -0.75;
                }
            }
        }

        private bool DetectarHoyo(
            Pelota pelota)
        {
            double centroPelotaX =
                pelota.X + 12.5;

            double centroPelotaY =
                pelota.Y + 12.5;

            double centroHoyoX =
                HOYO_X +
                TAMANO_HOYO / 2.0;

            double centroHoyoY =
                HOYO_Y +
                TAMANO_HOYO / 2.0;

            double diferenciaX =
                centroPelotaX -
                centroHoyoX;

            double diferenciaY =
                centroPelotaY -
                centroHoyoY;

            double distancia =
                Math.Sqrt(
                    diferenciaX *
                    diferenciaX +
                    diferenciaY *
                    diferenciaY);

            if (distancia <= 18 &&
                Math.Abs(
                    pelota.VelocidadX) < 5 &&
                Math.Abs(
                    pelota.VelocidadY) < 5)
            {
                pelota.EnHoyo = true;

                pelota.VelocidadX = 0;
                pelota.VelocidadY = 0;

                pelota.X =
                    centroHoyoX - 12.5;

                pelota.Y =
                    centroHoyoY - 12.5;

                return true;
            }

            return false;
        }

        private void TerminarPartida()
        {
            if (partidaTerminada)
                return;

            partidaTerminada = true;

            timer.Stop();

            string mensaje;

            if (jugador.Golpes <
                ia.Golpes)
            {
                mensaje =
                    $"¡Ganaste!\n\n" +
                    $"Jugador: {jugador.Golpes} golpes\n" +
                    $"IA: {ia.Golpes} golpes";
            }
            else if (
                ia.Golpes <
                jugador.Golpes)
            {
                mensaje =
                    $"Ganó la IA\n\n" +
                    $"Jugador: {jugador.Golpes} golpes\n" +
                    $"IA: {ia.Golpes} golpes";
            }
            else
            {
                mensaje =
                    $"¡Empate!\n\n" +
                    $"Jugador: {jugador.Golpes} golpes\n" +
                    $"IA: {ia.Golpes} golpes";
            }

            MessageBox.Show(
                mensaje,
                "Resultado",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ActualizarVisuales()
        {
            Canvas.SetLeft(
                PelotaJugadorVisual,
                jugador.X);

            Canvas.SetTop(
                PelotaJugadorVisual,
                jugador.Y);

            Canvas.SetLeft(
                PelotaIAVisual,
                ia.X);

            Canvas.SetTop(
                PelotaIAVisual,
                ia.Y);
        }
    }
}