using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Application.ModuleManagement;
using GazRouter.Modes.Calculations;
using GazRouter.Modes.DispatcherTasks;
using GazRouter.Modes.EventLog;
using GazRouter.Modes.GasCosts;
using GazRouter.Modes.GasCosts2;
using GazRouter.Modes.Infopanels;
using GazRouter.Modes.ProcessMonitoring;
using GazRouter.Modes.ProcessMonitoring.Dashboards;
using GazRouter.Modes.ProcessMonitoring.Reports;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
namespace GazRouter.Modes.Module
{
    public class ModesModule : SimpleAppModule
    {
        #region C-tors

        public ModesModule(
            IUnityContainer container,
            IRegionViewRegistry regionViewRegistry,
            IRegionManager regionManager,
            INavigationService navigationService)
            : base(container, regionViewRegistry, regionManager, navigationService)
        {
        }

        public ModesModule(
            IUnityContainer container,
            IRegionViewRegistry regionViewRegistry,
            IRegionManager regionManager)
            : base(container, regionViewRegistry, regionManager)
        {
        }

        public ModesModule(IUnityContainer container)
            : base(container)
        {
        }

        #endregion

        public override string Id => "ModesModuleId";

        protected override void ConfigureContainer()
        {
            Container.RegisterSingletonType<EventLogMainViewModel>();
            RegisterMainView<EventLogMainView, EventLogMainViewModel>();

            Container.RegisterSingletonType<MainCalcViewModel>();
            RegisterMainView<MainCalcView, MainCalcViewModel>();

            RegisterMainView<TasksMainView, TasksMainViewModel>();

            RegisterMainView<GasCostsMainView, GasCostsMainViewModel>();

            RegisterMainView<GasCostsMainView2, GasCostsMainViewModel2>();

            RegisterMainView<FloViewControl, FloViewControlViewModel>();

            RegisterMainView<InfopanelsView, InfopanelsViewModel>();            

            RegisterMainView<ReportsView, ReportsViewModel>();
        }
    }
}