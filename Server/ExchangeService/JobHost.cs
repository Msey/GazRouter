using System;

namespace ExchangeService
{
    public class JobHost 
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;

        public JobHost()
        {
        }

        public void Stop()
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }
        }

        public void DoWork(Action work)
        {
            lock (_lock)
            {
                if (_shuttingDown)
                {
                    return;
                }
                work();
            }
        }

        public void Start()
        {
            lock (_lock)
            {
                _shuttingDown = false;
            }
        }
    }
}