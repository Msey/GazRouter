using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GazRouter.Application;
using GazRouter.Common.Services;
using Microsoft.Practices.Prism.Regions;
using NavigationContext = Microsoft.Practices.Prism.Regions.NavigationContext;
namespace GazRouter.Client.FrameNavigation
{
    public partial class FrameNavigationWrapperPage
    {
#region constructor
        public FrameNavigationWrapperPage()
        {
            InitializeComponent();
        }
#endregion

#region variables
        public FrameRegionNavigationService FrameRegionNavigationService { get; set; }
        private object View => (Content as ContentControl)?.Content ?? Content;
#endregion

#region events
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Execute<INavigationAware>(ina => { ina.OnNavigatedTo(CreateNavigationContext(e.Uri)); });
            var baseTitle = "Gazrouter";
            var view = View as IPageTitle;
            if (view != null && !string.IsNullOrWhiteSpace(view.PageTitle))
            {
                Title = $"{baseTitle}-{view.PageTitle}";
            }
            else
            {
                var fwe = View as FrameworkElement;
                var viewModel = fwe?.DataContext as IPageTitle;
                if (viewModel != null && !string.IsNullOrWhiteSpace(viewModel.PageTitle))
                    Title = $"{baseTitle}-{viewModel.PageTitle}";
                else Title = baseTitle;
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Execute<INavigationAware>(ina => ina.OnNavigatedFrom(CreateNavigationContext(e.Uri)));
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Execute<IConfirmNavigationRequest>(ina =>
            {
                bool? result = null;
                var ignore = false;
                ina.ConfirmNavigationRequest(CreateNavigationContext(e.Uri),
                    r => {
                             if (!ignore) result = r;
                    });
                ignore = true;
                if (result == false) e.Cancel = true; 
            });
        }
#endregion

#region methods
        private NavigationContext CreateNavigationContext(Uri uri)
        {
            return new NavigationContext(FrameRegionNavigationService,
                FrameRegionNavigationService != null ? FrameRegionNavigationService.MapUri(uri) : uri);
        }
        private void Execute<T>(Action<T> action) where T : class
        {
            var viewAsT = View as T;
            if (viewAsT != null) action(viewAsT);
            var viewAsFrameworkElement = View as FrameworkElement;
            var dataContext = viewAsFrameworkElement?.DataContext as T;
            if (dataContext != null) action(dataContext);
        }
#endregion
    }
}