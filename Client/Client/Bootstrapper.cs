using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataProviders;
using GazRouter.ActionsRolesUsers.Module;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Balances;
using GazRouter.Client.Cache;
using GazRouter.Client.FrameNavigation;
using GazRouter.Client.Views;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.Common.Services;
using GazRouter.DataExchange.Module;
using GazRouter.DataProviders.Authorization;
using GazRouter.GasLeaks.Module;
using GazRouter.ManualInput.Module;
using GazRouter.Modes.Module;
using GazRouter.ObjectModel;
using GazRouter.Repair.Module;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
namespace GazRouter.Client
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var resolve = Container.Resolve<Shell>();
            return resolve;
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterSingletonType<INavigationService, NavigationService>();
            Container.RegisterSingletonType<IMessageBoxProvider, MessageBoxProvider>();
            Container.RegisterInstance<IClientCache>(new ClientCache());
            Container.RegisterType<object, HomeView>(typeof (HomeView).FullName);

            RegisterDataProviders();
        }
        protected override async void InitializeShell()
        {
            base.InitializeShell();
            var repository = Container.Resolve<IClientCache>().DictionaryRepository;
            try
            {
                await repository.Load().ConfigureAwait(true);
                var profileInfo = await new UserManagementServiceProxy()
                    .GetProfileInfoAsync().ConfigureAwait(true);
                UserProfile.Current.SetProfile(profileInfo);
#region authorization
                var userRoles = await new UserManagementServiceProxy()
                    .GetUserRolesAsync(UserProfile.Current.Id).ConfigureAwait(true);
                var permissions = await new UserManagementServiceProxy()
                    .GetPermissionsAsync().ConfigureAwait(true);
                Authorization.Instance.Init(userRoles, permissions);
                //
                Authorization2.Inst.Init(userRoles, permissions);
                #endregion
                #region uri
                DataProviders.UriBuilder.GetSapBoUri2 = await new UserManagementServiceProxy().GetSapBoUriAsync();
#endregion
                var shell = (UserControl)Shell;
                System.Windows.Application.Current.RootVisual = shell;
                shell.DataContext = Container.Resolve<ShellViewModel>();
            }
            catch (Exception e)
            {                
                App.Current.RootVisual = new TextBlock();
                Controls.Dialogs.DialogHelper.ShowErrorWindow(new ApplicationUnhandledExceptionEventArgs(e, true));

                Console.WriteLine(e);
            }
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();

            catalog
                .AddModule(typeof (ObjectModelModule))
                .AddModule(typeof (GasLeaksModule))
                .AddModule(typeof (ModesModule))
                .AddModule(typeof (ActionsRolesUsersModule))
                .AddModule(typeof (RepairModule))
                .AddModule(typeof (BalancesModule))
                .AddModule(typeof (ManualInputModule))
                .AddModule(typeof (DataExchangeModule));

            return catalog;
        }
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();
            mappings.RegisterMapping(typeof (Frame),
                new FrameRegionAdapter(ServiceLocator.Current.GetInstance<IRegionBehaviorFactory>()));
            return mappings;
        }
        private void RegisterDataProviders()
        {
            var assembly = typeof(DataProvideSettings).Assembly;
            foreach (var t in assembly.GetTypes().Where(t => t.FullName.EndsWith("Proxy") && t.IsClass))
            {
                Container.RegisterType(t.GetInterface("I" + t.Name, false), t);
            }
        }
    }
}
//               ServiceLocator.Current.GetInstance<IEventAggregator>().
//               this.Logger.Log("1", Category.Debug, Priority.None);
//               var missions = await new UserManagementServiceProxy().GetAdUsersTreeAsync().ConfigureAwait(true);
//#region test
//#endregion