using System;
using System.Net.Sockets;

namespace G7NClient.Core
{
    internal class ScadaProcessor
    {
        private System.Timers.Timer _analogTimer;
        private System.Timers.Timer _digitalTimer;

        private TcpClient? _tcpClient;
        private readonly string _ipAddress = "127.0.0.1";
        private readonly int _port = 43676;

        public ScadaProcessor()
        {
            _analogTimer = new System.Timers.Timer(3000);
            _analogTimer.Elapsed += OnAnalogTimerElapsed;
            _analogTimer.AutoReset = true;

            _digitalTimer = new System.Timers.Timer(4000);
            _digitalTimer.Elapsed += OnDigitalTimerElapsed;
            _digitalTimer.AutoReset = true;
        }

        public void Start()
        {
            Connect();

            _analogTimer.Start();
            _digitalTimer.Start();
        }

        public void Stop()
        {
            _analogTimer.Stop();
            _digitalTimer.Stop();

            _tcpClient?.Close();
        }

        private void Connect()
        {
            try
            {
                Console.WriteLine($"Pokušavam povezivanje na {_ipAddress}:{_port}...");
                _tcpClient = new TcpClient();

                _tcpClient.Connect(_ipAddress, _port);

                Console.WriteLine("Uspešno povezano sa dCom simulatorom!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri povezivanju: {ex.Message}");
            }
        }

        private void OnAnalogTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_tcpClient == null || !_tcpClient.Connected) return;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]Očitavam analogne vrednosti (3s)...");
        }

        private void OnDigitalTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_tcpClient == null || !_tcpClient.Connected) return;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]Očitavam digitalne vrednosti (4s)...");
        }
    }
}
