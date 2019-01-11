using GazRouter.ActionsRolesUsers.ViewModels;
using GazRouter.ActionsRolesUsers.Views;
using GazRouter.Application;
using GazRouter.Application.ModuleManagement;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace GazRouter.ActionsRolesUsers.Module
{
    public class ActionsRolesUsersModule : SimpleAppModule
    {
        public ActionsRolesUsersModule(IUnityContainer container,
                                       IRegionViewRegistry regionViewRegistry, 
                                       IRegionManager regionManager,
                                       INavigationService navigationService) :
            base(container, regionViewRegistry, regionManager, navigationService)
        {
        }
        public override string Id
        {
            get { return "ActionsRolesUsers"; }
        }

        public override string Name
        {
            get { return "ActionsRolesUsers"; }
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterMainView<ActionsRolesUsersView, ActionsRolesUsersViewModel>();
        }
    }
}