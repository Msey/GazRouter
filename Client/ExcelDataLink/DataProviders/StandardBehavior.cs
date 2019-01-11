using System;
using System.Threading;
using System.Windows.Threading;

namespace DataProviders
{
    public class StandardBehavior : BaseBehavior
    {
        public override void ProcessResult(Exception exception, Func<Exception, bool> processUiCallback)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                ProcessUICallback(exception, processUiCallback);
                TryUnlock();
            }));
        }
    }
}