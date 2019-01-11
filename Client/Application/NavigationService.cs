using System;
using System.Windows.Browser;
using System.Windows.Controls;
using GazRouter.Common;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace GazRouter.Application
{
    public class NavigationService : INavigationService
    {
        private readonly IUnityContainer _container;

        private const string DefaultTarget = "MainRegion";

        public NavigationService(IRegionManager regionManager, IUnityContainer container)
        {
            _container = container;
            RegionManager = regionManager;
        }

        private IRegionManager RegionManager { get; }

        public void Navigate(Uri uri)
        {
            if (_container.IsRegistered<object>(uri.OriginalString))
            {
                RegionManager.RequestNavigate(
                    DefaultTarget,
                    uri,
                    result =>
                    {
#if DEBUG
                        if (!(result.Result ?? true))
                        {
                            return;
                        }

                        IsolatedStorageManager.LastView = uri.OriginalString;
#endif
                    });
            }

        }
    }
}