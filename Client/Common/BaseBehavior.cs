using System;

namespace GazRouter.Common
{
    public abstract class BaseBehavior : IClientBehavior
    {

        public virtual void TryLock(string lockMessage = null)
        {
        }

        public virtual void TryUnlock()
        {
        }

        public virtual void OnBeforeQuery()
        {
        }

        public virtual void ProcessResult(Exception exception, Func<Exception, bool> processUiCallback)
        { }

        protected static void ProcessUICallback(Exception exception, Func<Exception, bool> processUICallback)
        {
            if (!processUICallback(exception)/* && exception != null*/)
            {
                throw new ServerException(exception);
            }
        }
    }
}