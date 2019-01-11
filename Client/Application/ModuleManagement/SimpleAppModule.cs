using System.Windows.Controls;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
namespace GazRouter.Application.ModuleManagement
{
	public abstract class SimpleAppModule : PropertyChangedBase, IModule 
	{
	    protected SimpleAppModule(IUnityContainer container,
							      IRegionViewRegistry regionViewRegistry, 
                                  IRegionManager regionManager, 
                                  INavigationService navigationService)
		{
			Container = container;
			RegionViewRegistry = regionViewRegistry;
			RegionManager = regionManager;
		    NavigationService = navigationService;
		}
        protected SimpleAppModule(IUnityContainer container,
                                  IRegionViewRegistry regionViewRegistry, 
                                  IRegionManager regionManager)
            : this(container, regionViewRegistry, regionManager, container.Resolve<INavigationService>())
        {

           
           
        }
		protected SimpleAppModule(IUnityContainer container) 
		{
			Container = container;
			RegionManager = Container.Resolve<IRegionManager>();
            NavigationService = container.Resolve<INavigationService>();
		}
		protected IUnityContainer Container { get; }
		protected IRegionViewRegistry RegionViewRegistry { get; private set; }
		protected IRegionManager RegionManager { get; private set; }
        protected INavigationService NavigationService { get; private set; }
	    public virtual string Name
		{
			get { return Id; }
		}
		public abstract string Id { get; }
	    public  virtual void Initialize()
	    {
	        ConfigureContainer();
	    }
	    protected virtual void ConfigureContainer()
	    {
	    }
	    protected void RegisterMainView<TView, TViewModel>() where TView : UserControl
        {
            var injectionProperty = new InjectionProperty("DataContext", new ResolvedParameter<TViewModel>());
            Container.RegisterType<object, TView>(typeof(TView).FullName,new PerResolveLifetimeManager(), injectionProperty);
        }

        protected void RegisterMainView<TView, TViewModel>(string constParam) where TView : UserControl
        {
            var injectionProperty = new InjectionProperty("DataContext", new ResolvedParameter<TViewModel>(constParam));
            Container.RegisterType<object, TView>(typeof(TView).FullName + constParam, new PerResolveLifetimeManager(), injectionProperty);
        }
    }
}