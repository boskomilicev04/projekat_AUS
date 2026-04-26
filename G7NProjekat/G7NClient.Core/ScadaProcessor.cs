using System;

namespace G7NClient.Core
{
    internal class ScadaProcessor
    {
        private System.Timers.Timer _analogTimer;
        private System.Timers.Timer _digitalTimer;

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
            _analogTimer.Start();
            _digitalTimer.Start();
        }

        private void OnAnalogTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {

        }

        private void OnDigitalTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {

        }
    }
}
