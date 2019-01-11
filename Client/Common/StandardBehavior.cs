using System;
using System.Windows;

namespace GazRouter.Common
{
    public class StandardBehavior : BaseBehavior
    {
        public override void ProcessResult(Exception exception, Func<Exception, bool> processUiCallback)
        {
#if SILVERLIGHT
            Deployment.Current.Dispatcher.BeginInvoke(() => ProcessUICallback(exception, processUiCallback));
#else

            Application.Current.Dispatcher.BeginInvoke((Action)(() => ProcessUICallback(exception, processUiCallback)));
#endif
        }
    }
}