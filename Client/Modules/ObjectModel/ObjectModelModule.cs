using GazRouter.Application.ModuleManagement;
using GazRouter.ObjectModel.Model;
using GazRouter.ObjectModel.SystemVariables;
using GazRouter.ObjectModel.Views;
using Microsoft.Practices.Unity;
using ObjectModelEditorMainView = GazRouter.ObjectModel.Views.ObjectModelEditorMainView;

namespace GazRouter.ObjectModel
{
    public class ObjectModelModule : SimpleAppModule
    {
        public ObjectModelModule(IUnityContainer container) :
            base(container)
        {
        }

        public override string Id => "ObjectModel";

        public override string Name => "Объектная модель";


        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();


            Container.RegisterType<ObjectModelEditorMainViewModel>(new PerResolveLifetimeManager());
            RegisterMainView<ObjectModelEditorMainView, ObjectModelEditorMainViewModel>();

            Container.RegisterType<DeviceConfig.DeviceConfigViewModel>(new PerResolveLifetimeManager());
            RegisterMainView<DeviceConfig.DeviceConfigView, DeviceConfig.DeviceConfigViewModel>();

            //            Container.RegisterType<SystemVariablesViewModel>(new ContainerControlledLifetimeManager());
            //            RegisterMainView<SystemVariablesView, SystemVariablesViewModel>();


        }


    }
}