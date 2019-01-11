using GazRouter.Common.ViewModel;

namespace GazRouter.Modes.EventLog
{
    public abstract class EventLogTabBase : LockableViewModel
    {

        public abstract string Header { get; }
        //private bool _loaded;
        private bool _isActive;

        public virtual bool IsActive
        {
            set
            {
                if (_isActive == value)
                    return;
                _isActive = value;
                //if (!_loaded)
                //{
                //    _loaded = true;
                //    Refresh();
                //}
                Refresh();
            }
           
        }

        public abstract void Refresh();
    }
}