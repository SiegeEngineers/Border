using System;
using System.Timers;
namespace Border.Helpers
{
    public class BorderTimer
    {
        private Timer timer;
        BorderTimer(double interval, double startTime = 0)
        {
            CurrentTime = startTime;
            timer = new Timer(interval);
            timer.Elapsed += Elapsed;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            var t = (StartDateTime - e.SignalTime).TotalMilliseconds;
            CurrentTime = t;
            OnInterval(t);
        }

        public void Start() { Reset(); Unpause(); }
        public void Unpause() { timer.Start(); }
        public void Pause() { timer.Stop(); }
        public void Reset() { StartDateTime = DateTime.UtcNow; }
        public void Set(double t) { StartDateTime = DateTime.UtcNow.AddMilliseconds(-t); }

        public DateTime StartDateTime
        {
            get; protected set;
        } = DateTime.UtcNow;

        public double CurrentTime
        {
            get; protected set;
        }

        public delegate void OnIntervalEventHandler(double time);
        public OnIntervalEventHandler OnInterval;
    }
}
