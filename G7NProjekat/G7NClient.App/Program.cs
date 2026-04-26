using System;
using System.Collections.Generic;
using System.Text;
using G7NClient.Core;

namespace G7NServer.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ScadaProcessor processor = new ScadaProcessor();

            processor.Start();

            var lPoint = processor._points.FirstOrDefault(p => p.Address == 3100);

            while(true)
            {
                if(lPoint != null)
                {
                    lPoint.Value += 2;
                    if (lPoint.Value > 1000) lPoint.Value = 0;
                }

                Console.WriteLine($"[SIM] Trenutna vrednost L: {lPoint?.Value}");
                Thread.Sleep(2000);
            }

        }
    }
}
