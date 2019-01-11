using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.DispatcherTasks.Enterprise;
using GazRouter.Modes.DispatcherTasks.Site;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;

namespace GazRouter.Modes.DispatcherTasks
{
    public class TasksMainViewModel : LockableViewModel
    {
        public TasksMainViewModel()
        {
            if (UserProfile.Current.Site.IsEnterprise)
                ViewModel = new EnterpriseViewModel();
            else
                ViewModel = new SiteViewModel();
        }

        public ViewModelBase ViewModel { get; set; }

    }
}