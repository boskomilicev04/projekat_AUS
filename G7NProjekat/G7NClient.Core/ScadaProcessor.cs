using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace G7NClient.Core
{
    public class ScadaProcessor
    {
        private System.Timers.Timer _analogTimer;
        private System.Timers.Timer _digitalTimer;

        private readonly int _port = 43676;
        private bool _isRunning = false;

        public List<ScadaPoint> _points = new List<ScadaPoint>
        {
            new ScadaPoint { Name = "L", Address = 3100, Type = PointType.AnalogInput, Value = 0 },
            new ScadaPoint { Name = "STOP", Address = 3400, Type = PointType.DigitalOutput, Value = 0 },
            new ScadaPoint { Name = "Ventil V1", Address = 3402, Type = PointType.DigitalOutput, Value = 0 },
            new ScadaPoint { Name = "P1", Address = 3405, Type = PointType.DigitalOutput, Value = 0 },
            new ScadaPoint { Name = "P2", Address = 3406, Type = PointType.DigitalOutput, Value = 0 },
            new ScadaPoint { Name = "N1", Address = 1500, Type = PointType.AnalogOutput, Value = 0 }
        };

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
            _isRunning = true;
            Listen();

            _analogTimer.Start();
            _digitalTimer.Start();
            Console.WriteLine(">>> SCADA Sistem pokrenut...");
        }

        public void Stop()
        {
            _isRunning = false;
            _analogTimer.Stop();
            _digitalTimer.Stop();
        }

        private void Listen()
        {
            Task.Run(() =>
            {
                TcpListener? server = null;
                try
                {
                    server = new TcpListener(IPAddress.Any, _port);
                    server.Start();
                    Console.WriteLine($">>> Server sluša na portu {_port} (Slave ID: 158)..."); // [cite: 5, 6]

                    while (_isRunning)
                    {
                        using (TcpClient client = server.AcceptTcpClient())
                        {
                            Console.WriteLine("\n[VEZA] dComm simulator se povezao!");
                            using (NetworkStream stream = client.GetStream())
                            {
                                byte[] buffer = new byte[2048];
                                while (client.Connected && _isRunning)
                                {
                                    if (stream.DataAvailable)
                                    {
                                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                                        if (bytesRead > 0)
                                        {
                                            ProcessModbusRequest(stream, buffer, bytesRead);
                                        }
                                    }
                                    Thread.Sleep(20);
                                }
                            }
                            Console.WriteLine("[VEZA] dComm je prekinuo vezu.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">>> Greška servera: {ex.Message}");
                }
                finally
                {
                    server?.Stop();
                }
            });
        }

        private void ProcessModbusRequest(NetworkStream stream, byte[] request, int length)
        {
            if (length < 8) return;

            byte unitId = request[6];
            byte functionCode = request[7];

            if (unitId != 158) return;

            ushort address = (ushort)((request[8] << 8) | request[9]);

            var point = _points.FirstOrDefault(p => p.Address == address);

            if (point != null)
            {
                if (functionCode == 0x05 || functionCode == 0x06)
                {
                    if (functionCode == 0x05)
                    {
                        point.Value = request[10] == 0xFF ? 1 : 0;
                    }
                    else
                    {
                        point.Value = (ushort)((request[10] << 8) | request[11]);
                    }

                    stream.Write(request, 0, length);
                    Console.WriteLine($"[WRITE] Adresa {address} setovana na: {point.Value}");
                }

                else if (functionCode == 0x01 || functionCode == 0x03 || functionCode == 0x04)
                {
                    byte[] response = new byte[256];

                    Array.Copy(request, 0, response, 0, 6);

                    byte[] data = point.GetValueBytes();

                    response[4] = 0;
                    response[5] = (byte)(3 + data.Length);

                    response[6] = unitId;
                    response[7] = functionCode;
                    response[8] = (byte)data.Length;

                    Array.Copy(data, 0, response, 9, data.Length);

                    stream.Write(response, 0, 9 + data.Length);

                    string typeLabel = (data.Length == 2) ? "Analog" : "Digital";
                    Console.WriteLine($"[READ] {typeLabel} adresa {address}: {point.Value}");
                }
            }
        }

        private void OnAnalogTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var lPoint = _points.FirstOrDefault(p => p.Name == "L");
            var n1Point = _points.FirstOrDefault(p => p.Name == "N1");

            Console.WriteLine($"[{DateTime.Now:T}] UI Analog: L={lPoint?.Value}L, N1={n1Point?.Value}");
        }

        private void OnDigitalTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var stop = _points.FirstOrDefault(p => p.Name == "STOP")?.Value;
            Console.WriteLine($"[{DateTime.Now:T}] UI Digital: STOP status je {(stop > 0 ? "ON" : "OFF")}");
        }
    }
}
