using Microsoft.Practices.Prism.Regions;
namespace GazRouter.Common.ViewModel
{
    public abstract class MainViewModelBase : LockableViewModel, INavigationAware
    {
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}