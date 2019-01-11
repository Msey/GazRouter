using DataExchange.ASDU;
using DataExchange.Integro.ASSPOOTI;
using DataExchange.Integro.Summary;
using DataExchange.RestServices;
using DataExchange.Timers;
using DataExchange.Typical;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Application.ModuleManagement;
using GazRouter.Common;
using GazRouter.DataExchange.ASTRA;
using GazRouter.DataExchange.ASUTP;
using GazRouter.DataExchange.CustomSource;
using GazRouter.DataExchange.ExchangeLog;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;


namespace GazRouter.DataExchange.Module
{
    public class DataExchangeModule : SimpleAppModule
    {
        public DataExchangeModule(
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
            get
            {
                return "DataExchangeModule";
            }
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterSingletonType<CustomSourceViewModel>();
            RegisterMainView<CustomSourceView, CustomSourceViewModel>();

            Container.RegisterSingletonType<AstraViewModel>();
            RegisterMainView<AstraView, AstraViewModel>();

            Container.RegisterSingletonType<TypicalExchangeView>();
            RegisterMainView<TypicalExchangeView, TypicalExchangeViewModel>();

            Container.RegisterSingletonType<TimerSettingsViewModel>();
            RegisterMainView<TimerSettingsView, TimerSettingsViewModel>();

            Container.RegisterSingletonType<AsutpViewModel>();
            RegisterMainView<AsutpView, AsutpViewModel>();

            Container.RegisterSingletonType<RestServicesViewModel>();
            RegisterMainView<RestServicesView, RestServicesViewModel>();

            Container.RegisterSingletonType<AsduSourceViewModel>();
            RegisterMainView<AsduSourceView, AsduSourceViewModel>();

            Container.RegisterSingletonType<ExchangeLogViewModel>();
            RegisterMainView<ExchangeLogView, ExchangeLogViewModel>();

            //Container.RegisterSingletonType<MappingViewModel>();
            //RegisterMainView<MappingView, MappingViewModel>();

            Container.RegisterSingletonType<SummaryCatalogViewModel>(LinkType.AsduMapping.ToString());
            RegisterMainView<SummaryCatalog, SummaryCatalogViewModel>(LinkType.AsduMapping.ToString());

            Container.RegisterSingletonType<SummaryCatalogViewModel>(LinkType.AsspootiMapping.ToString());
            RegisterMainView<SummaryCatalog, SummaryCatalogViewModel>(LinkType.AsspootiMapping.ToString());

            Container.RegisterSingletonType<AsduDataImportViewModel>();
            RegisterMainView<AsduImportData, AsduDataImportViewModel>();

            Container.RegisterSingletonType<AsduMetadataViewModel>();
            RegisterMainView<AsduMetadataView, AsduMetadataViewModel>();

            Container.RegisterSingletonType<AsduDataViewModel>();
            RegisterMainView<AsduDataView, AsduDataViewModel>();
        }
    }
}