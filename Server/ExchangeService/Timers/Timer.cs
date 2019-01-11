﻿using System;
using System.Threading;

namespace ExchangeService.Timers
{
    public class Timer
    {
        private readonly TimeSpan _disposalTimeout;

        private readonly System.Threading.Timer _timer;

        private bool _disposeEnded;

        public Timer(TimerCallback onTimerElapsed, object state, TimeSpan dueTime, TimeSpan period) : this(onTimerElapsed, state, dueTime, period, TimeSpan.FromMinutes(3))
        {
        }
        public Timer(TimerCallback onTimerElapsed, object state, TimeSpan dueTime, TimeSpan period, TimeSpan disposalTimeout)
        {
            Elapsed += onTimerElapsed;
            _disposalTimeout = disposalTimeout;
            _timer = new System.Threading.Timer(HandleTimerElapsed, state, dueTime, period);
        }

        public event TimerCallback Elapsed;

        public void Change(TimeSpan time, TimeSpan span)
        {
            _timer.Change(time, span);
        }

        public void TriggerOnceIn(TimeSpan time)
        {
            try
            {
                _timer.Change(time, Timeout.InfiniteTimeSpan);
            }
            catch (ObjectDisposedException)
            {
                // race condition with Dispose can cause trigger to be called when underlying
                // timer is being disposed - and a change will fail in this case.
                // see 
                // https://msdn.microsoft.com/en-us/library/b97tkt95(v=vs.110).aspx#Anchor_2
                if (_disposeEnded)
                {
                    // we still want to throw the exception in case someone really tries
                    // to change the timer after disposal has finished
                    // of course there's a slight race condition here where we might not
                    // throw even though disposal is already done.
                    // since the offending code would most likely already be "failing"
                    // unreliably i personally can live with increasing the
                    // "unreliable failure" time-window slightly
                    throw;
                }
            }
        }

        private void HandleTimerElapsed(object state)
        {
            Elapsed?.Invoke(state);
        }

        public void Dispose()
        {
            using (var waitHandle = new ManualResetEvent(false))
            {
                // returns false on second dispose
                if (_timer.Dispose(waitHandle))
                {
                    if (!waitHandle.WaitOne(_disposalTimeout))
                    {
                        throw new TimeoutException(
                            "Timeout waiting for timer to stop. (...)");
                    }
                    _disposeEnded = true;
                }
            }
        }
    }

}
