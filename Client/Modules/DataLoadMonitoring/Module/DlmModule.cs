using GazRouter.Application.Helpers;
using GazRouter.Application.ModuleManagement;
using GazRouter.DataLoadMonitoring.ViewModels;
using GazRouter.DataLoadMonitoring.Views;
using Microsoft.Practices.Unity;

namespace GazRouter.DataLoadMonitoring.Module
{
	public class DlmModule : SimpleAppModule
	{

		#region overrided properties

		public override string Id
		{
			get { return "DataLoadMonitoring"; }
		}

	   

	    public override string Name
		{
            get { return "DataLoadMonitoring"; }
		}

		#endregion properties

		#region overrided methods

	    protected override void ConfigureContainer()
	    {
	        base.ConfigureContainer();
	        Container.RegisterSingletonType<DlmMainViewModel>();
            RegisterMainView<DlmMainView, DlmMainViewModel>(null);
	    }

		

		#endregion overrides

		#region .Ctor

        public DlmModule(IUnityContainer container)
            : base(container)
		{
		}

		#endregion
	}
}
