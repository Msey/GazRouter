using System;
using System.Windows.Threading;
using GazRouter.Common;

namespace GazRouter.Modes.DispatcherTasks.Common
{
    public class TaskAutoRefresher
    {
        private DispatcherTimer _timer;
        private readonly Action _action;
        private const int Interval = 60;

        public TaskAutoRefresher(Action action)
        {
            _action = action;
        }

        public void Update()
        {
            var isOn = IsolatedStorageManager.Get<bool?>("TasksAutoRefreshOn") ?? false;
            
            if (isOn)
            {
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Interval) };
                _timer.Tick += TimerTick;
                _timer.Start();
            }
            else
            {
                if (_timer == null) return;
                {
                    _timer.Tick -= TimerTick;
                    _timer.Stop();
                    _timer = null;
                }
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _action?.Invoke();
        }

        
    }
}
