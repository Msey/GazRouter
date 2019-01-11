using GazRouter.Application.Helpers;
using GazRouter.Application.ModuleManagement;
using GazRouter.Repair.Agreement;
using GazRouter.Repair.Plan;
using GazRouter.Repair.RepWorks;
using Microsoft.Practices.Unity;

namespace GazRouter.Repair.Module
{
    public class RepairModule : SimpleAppModule
    {
        public RepairModule(IUnityContainer container)
            : base(container)
        {
        }

        public override string Id => "RepairModel";

        public override string Name => "Ремонты";

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterSingletonType<RepairMainViewModel>();
            //RegisterMainView<RepairMainView, RepairMainViewModel>();

            RegisterMainView<PlanView, PlanViewModel>();
            RegisterMainView<RequestView, RequestViewModel>();
            RegisterMainView<CurrentWorksView, CurrentWorksViewModel>();
            RegisterMainView<ComplitedWorksView, ComplitedWorksViewModel>();
            RegisterMainView<UserAgreementsListView, UserAgreementsListViewModel>();
        }
    }
}