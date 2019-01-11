using GazRouter.Application.Helpers;
using GazRouter.Application.ModuleManagement;
using GazRouter.Balances.BalanceGroups;
using GazRouter.Balances.Commercial.Fact;
using GazRouter.Balances.Commercial.Plan;
using GazRouter.Balances.Commercial.SiteInput;
using GazRouter.Balances.DayBalance;
using GazRouter.Balances.DistrNetworks;
using GazRouter.Balances.GasOwners;
using GazRouter.Balances.Routes;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using MainView = GazRouter.Balances.Commercial.Fact.MainView;
using PlanView = GazRouter.Balances.Commercial.Plan.PlanView;

namespace GazRouter.Balances
{
	public class BalancesModule : SimpleAppModule
	{
	
		public BalancesModule(IUnityContainer container) :
			base(container)
		{
			Container.Resolve<IRegionManager>();
		}

		public override string Id => "BalancesModelModel";


	    public override string Name => "Балансы";

	    protected override void ConfigureContainer()
		{
			base.ConfigureContainer();

            RegisterMainView<PlanView, PlanViewModel>();

            Container.RegisterSingletonType<DayBalanceViewModel>();
            RegisterMainView<DayBalanceView, DayBalanceViewModel>();
            RegisterMainView<GasOwnersView, GasOwnersViewModel>();
            RegisterMainView<RoutesView, RoutesViewModel>();
            RegisterMainView<BalanceGroupsView, BalanceGroupsViewModel>();
            RegisterMainView<MainView, FactViewModel>();
            RegisterMainView<SiteInputView, SiteInputViewModel>();
            RegisterMainView<DistrNetworksView, DistrNetworksViewModel>();

        }
	}
}
