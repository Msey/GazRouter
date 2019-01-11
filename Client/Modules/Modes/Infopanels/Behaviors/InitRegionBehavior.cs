using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using GazRouter.Modes.ExcelReports;
using GazRouter.Modes.ProcessMonitoring.Dashboards;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
namespace GazRouter.Modes.Infopanels.Behaviors
{
    public class InitRegionBehavior : Behavior<UIElement>
    {
        public InitRegionBehavior()
        {
            _emptyView = new EmptyView();
            _busyView = new BusyView();
        }

        private InfopanelsView _infopanelsView;
        private readonly EmptyView _emptyView;
        private readonly BusyView _busyView;

        protected override void OnAttached()
        {
            base.OnAttached();
            _infopanelsView = AssociatedObject as InfopanelsView;
            _infopanelsView.Loaded += InfopanelsViewOnLoaded;
        }

        private void InfopanelsViewOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var regionManager = (RegionManager)ServiceLocator.Current.GetInstance<IRegionManager>();
            var viewsCollection = regionManager.Regions[InfopanelsViewModel.DashboardRegionConst].Views;
            var regionNames = viewsCollection.Select(e => e.GetType().Name).Distinct().ToDictionary(k=>k);
            if (!regionNames.ContainsKey(nameof(EmptyView)))
                regionManager.Regions[InfopanelsViewModel.DashboardRegionConst].Add(_emptyView, nameof(EmptyView));
            if (!regionNames.ContainsKey(nameof(BusyView)))
                regionManager.Regions[InfopanelsViewModel.DashboardRegionConst].Add(_busyView, nameof(BusyView));
            //
            regionManager.Regions[InfopanelsViewModel.DashboardRegionConst].Activate(_busyView);
            regionManager.Regions[InfopanelsViewModel.DashboardRegionConst].Activate(_emptyView);
            //
            var infopanelsViewModel = _infopanelsView.DataContext as InfopanelsViewModel;
            infopanelsViewModel.InitReportView();
        }

        protected override void OnDetaching()
        {
            _infopanelsView.Loaded -= InfopanelsViewOnLoaded;
        }
    }
}
