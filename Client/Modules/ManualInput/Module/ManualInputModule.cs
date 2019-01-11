using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Application.ModuleManagement;
using GazRouter.ManualInput.ChemicalTests;
using GazRouter.ManualInput.CompUnits;
using GazRouter.ManualInput.CompUnitTests;
using GazRouter.ManualInput.Daily;
using GazRouter.ManualInput.Dashboard;
using GazRouter.ManualInput.Hourly;
using GazRouter.ManualInput.PipelineLimits;
using GazRouter.ManualInput.Settings;
using GazRouter.ManualInput.Valves;
using GazRouter.ManualInput.ContractPressures;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace GazRouter.ManualInput.Module
{
    public class ManualInputModule : SimpleAppModule
    {
        public ManualInputModule(
            IUnityContainer container,
            IRegionViewRegistry regionViewRegistry, 
            IRegionManager regionManager, 
            INavigationService navigationService) :
            base(container, regionViewRegistry, regionManager, navigationService)
        {
            Container.Resolve<IRegionManager>();
        }

        public override string Id
        {
            get { return "ManualInput"; }
        }

        public override string Name
        {
            get { return "Ручной ввод"; }
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterSingletonType<HourlyViewModel>();
            RegisterMainView<HourlyView, HourlyViewModel>();

            Container.RegisterSingletonType<ValveViewModel>();
            RegisterMainView<ValveView, ValveViewModel>();

            Container.RegisterSingletonType<DailyViewModel>();
            RegisterMainView<DailyView, DailyViewModel>();

            Container.RegisterSingletonType<ChemicalTestsViewModel>();
            RegisterMainView<ChemicalTestsView, ChemicalTestsViewModel>();

            Container.RegisterSingletonType<CompUnitViewModel>();
            RegisterMainView<CompUnitView, CompUnitViewModel>();

            Container.RegisterSingletonType<CompUnitTestsViewModel>();
            RegisterMainView<CompUnitTestsView, CompUnitTestsViewModel>();

            Container.RegisterSingletonType<PipelineLimitsViewModel>();
            RegisterMainView<PipelineLimitsView, PipelineLimitsViewModel>();

            Container.RegisterSingletonType<DashboardViewModel>();
            RegisterMainView<DashboardView, DashboardViewModel>();
            
            Container.RegisterSingletonType<SettingsViewModel>();
            RegisterMainView<SettingsView, SettingsViewModel>();

            Container.RegisterSingletonType<ContractPressuresViewModel>();
            RegisterMainView<ContractPressuresView, ContractPressuresViewModel>();
        }
    }
}