using System;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.DataLoadMonitoring.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class DlmMainViewModel : ViewModelBase, IConfirmNavigationRequest
	{

		public LoadMonViewModel DataLoadMonitoringViewModel { get; private set; }



		public DlmMainViewModel()
		{
            DataLoadMonitoringViewModel = new LoadMonViewModel(SeriesType.oddSeries);
		}

	    public void Refresh()
	    {
            //DataLoadMonitoringManagerViewModel.Refresh();
	    }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }
	}
}
