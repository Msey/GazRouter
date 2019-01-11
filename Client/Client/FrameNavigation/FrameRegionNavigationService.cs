using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Practices.Prism.Regions;
using NavigationContext = Microsoft.Practices.Prism.Regions.NavigationContext;
namespace GazRouter.Client.FrameNavigation
{
    public class FrameRegionNavigationService : IRegionNavigationService
    {
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            new FrameNavigationRequestAdapter(this, target, navigationCallback).RequestNavigate();
        }
        private void RaiseNavigated(NavigationContext context)
        {
            Navigated?.Invoke(this,new RegionNavigationEventArgs(context));
        }
        private void RaiseNavigating(NavigationContext context)
        {
            Navigating?.Invoke(this, new RegionNavigationEventArgs(context));
        }
        private void RaiseNavigationFailed(NavigationContext context, Exception exception)
        {
            NavigationFailed?.Invoke(this, new RegionNavigationFailedEventArgs(context, exception));
        }
        internal Uri MapUri(Uri uri)
        {
            var uriMapper = Frame?.UriMapper;
            return uriMapper != null ? uriMapper.MapUri(uri) : uri;
        }
        public Frame Frame { get; set; }
        public IRegion Region { get; set; }
        public IRegionNavigationJournal Journal { get; }
        public event EventHandler<RegionNavigationEventArgs> Navigating;
        public event EventHandler<RegionNavigationEventArgs> Navigated;
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
        
        public class FrameNavigationRequestAdapter
        {
            private readonly FrameRegionNavigationService _service;
            private readonly Uri _mappedSource;
            private readonly Action<NavigationResult> _navigationCallback;
            private Uri _source;
            private Frame _frame;
            public FrameNavigationRequestAdapter(FrameRegionNavigationService service, Uri source, Action<NavigationResult> navigationCallback)
            {
                _service = service;
                _mappedSource = source;
                _navigationCallback = navigationCallback;
                _source = service.MapUri(source);
                _frame = service.Frame;

                _frame.Navigated += OnNavigated;
                _frame.NavigationFailed += OnNavigationFailed;
                _frame.NavigationStopped += OnNavigationStopped;
            }
            private void UnhookFromFrame()
            {
                _frame.Navigated -= OnNavigated;
                _frame.NavigationFailed -= OnNavigationFailed;
                _frame.NavigationStopped -= OnNavigationStopped;
            }
            private void OnNavigated(object sender, NavigationEventArgs navigationEventArgs)
            {
                UnhookFromFrame();
                var context = new NavigationContext(_service, _source);
                _service.RaiseNavigating(context);
                _navigationCallback(new NavigationResult(context, true));
                _service.RaiseNavigated(context);


            }
            private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
            {
                UnhookFromFrame();
                e.Handled = true;
                var context = new NavigationContext(_service, _source);
                var navigationResult = new NavigationResult(context, e.Exception);
                _navigationCallback(navigationResult);
                _service.RaiseNavigationFailed(context, e.Exception);
            }
            private void OnNavigationStopped(object sender, NavigationEventArgs navigationEventArgs)
            {
               UnhookFromFrame();
            }
            public void RequestNavigate()
            {
                _frame.Navigate(_mappedSource);
            }
        }
    }
}