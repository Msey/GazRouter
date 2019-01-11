using GazRouter.Application.ModuleManagement;
using GazRouter.GasLeaks.ViewModels;
using GazRouter.GasLeaks.Views;
using Microsoft.Practices.Unity;

namespace GazRouter.GasLeaks.Module
{
    public class GasLeaksModule : SimpleAppModule
    {
        #region private fields

        #endregion

        #region overrided properties

        public override string Id
        {
            get { return "GasLeaksId"; }
        }

        public override string Name
        {
            get { return "Оперативная сводка мероприятий по устранению утечек газа"; }
        }

        #endregion properties

        #region overrided methods

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<MainGasLeaksViewModel>(new ContainerControlledLifetimeManager());
            RegisterMainView<MainGasLeaksView, MainGasLeaksViewModel>();
        }


        #endregion overrides

        #region .Ctor

        public GasLeaksModule(IUnityContainer container) : base(container)
        {
        }

        #endregion
    }
}