using GazRouter.Common.Cache;
using GazRouter.Common.Services;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Common.ViewModel
{
    public abstract class ViewModelBase : AsyncViewModelBase
    {
        private bool _isBusyLoading;
        private string _busyMessage;

        public bool IsBusyLoading
        {
            get { return _isBusyLoading; }
            set { SetProperty(ref _isBusyLoading, value); }
        }

        public string BusyMessage
        {
            get { return _busyMessage; }
            set { SetProperty(ref _busyMessage, value); }
        }

        public IMessageBoxProvider MessageBoxProvider => ServiceLocator.Current.GetInstance<IMessageBoxProvider>();

        protected static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
    }
}