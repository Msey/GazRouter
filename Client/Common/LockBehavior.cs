using System;
using System.Windows;

namespace GazRouter.Common
{
    public class LockBehavior : BaseBehavior
    {
        private readonly object _locker = new object();
        private readonly ILockable _obj;
        private int _counter;

        public LockBehavior(ILockable obj)
        {
            _obj = obj;
        }

        public override void OnBeforeQuery()
        {
            TryLock();
        }

        public override void TryLock(string lockMessage = null)
        {
            lock (_locker)
            {
                if (_counter > 0)
                {
                    _counter++;
                }
                else
                {
                    _obj.Lock(lockMessage);
                    _counter = 1;
                }
            }
        }

        public override void TryUnlock()
        {
            lock (_locker)
            {
                _counter--;
                if (_counter == 0)
                {
                    _obj.Unlock();
                }
            }
        }

        public override void ProcessResult(Exception exception, Func<Exception, bool> processUiCallback)
        {
#if SILVERLIGHT
            Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    ProcessUICallback(exception, processUiCallback);
                    TryUnlock();
                });
#else
            Application.Current.Dispatcher.BeginInvoke(
            (Action) (() =>
             {
                 ProcessUICallback(exception, processUiCallback);
                 TryUnlock();
             }));
#endif
        }

    }
}
