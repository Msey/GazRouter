namespace GazRouter.Common.ViewModel
{
    public abstract class LockableViewModel : ViewModelBase, ILockable
    {
        protected LockableViewModel()
        {
            Behavior = new LockBehavior(this);
        }

        public virtual void Lock(string lockMessage = null)
        {
            IsBusyLoading = true;
            if (!string.IsNullOrEmpty(lockMessage))
            {
                BusyMessage = lockMessage;
            }
        }

        public virtual void Unlock()
        {
            IsBusyLoading = false;
            BusyMessage = null;
        }
    }
}