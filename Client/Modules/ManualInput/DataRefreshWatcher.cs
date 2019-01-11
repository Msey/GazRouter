using System;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GazRouter.ManualInput
{
    public class DataRefreshWatcher
    {

        private DispatcherTimer _timer;
        private double _seconds;
        private int _totalTicks;
        private int _currentTick;
        private bool _paused;
        private const double _segmentInterval = 0.01;
        private bool _RunOnce;
        public void Run(double seconds = 5, bool runOnce=false)
        {
            if (_timer != null)
            {
                Stop();
            }
            //    throw new Exception("Таймер уже запущен");
            _RunOnce = runOnce;
            _seconds = seconds;
            _totalTicks = (int)(_seconds / _segmentInterval);
            _currentTick = 0;
            _paused = false;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_segmentInterval) };
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        public void Stop()
        {
            if (_timer == null) return;
            _timer.Tick -= TimerTick;
            _timer.Stop();
            _timer = null;
        }

        public void Pause()
        {
            _paused = true;
        }

        public void Unpause()
        {
            _paused = false;
        }

        private void TimerTick(object sender, EventArgs e)
        {
           // if(_currentTick%4==0)System.Diagnostics.Debug.WriteLine(_currentTick/4);

            if(!_paused && _currentTick++ == _totalTicks)
            {
                _currentTick = 0;
                TimerElapsed(sender, e);
            }
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            TimeToRefresh?.Invoke(sender, e);
            if (_RunOnce) Stop();
        }

        public event EventHandler TimeToRefresh;
    }
}
