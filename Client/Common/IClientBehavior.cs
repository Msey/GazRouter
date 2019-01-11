using System;

namespace GazRouter.Common
{
    public interface IClientBehavior
    {
        void OnBeforeQuery();
        void ProcessResult(Exception exception, Func<Exception, bool> processUiCallback);
        void TryLock(string lockMessage = null);
        void TryUnlock();
    }
}