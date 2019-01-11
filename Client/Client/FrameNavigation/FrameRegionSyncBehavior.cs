using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;

namespace GazRouter.Client.FrameNavigation
{
    public class FrameRegionSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        Frame _hostControl;
        bool _isNavigating = false;


        public const string BehaviorKey = nameof(FrameRegionSyncBehavior);

        public DependencyObject HostControl
        {
            get { return _hostControl; }
            set
            {
                var newValue = value as Frame;
                if (newValue == null) throw new ArgumentNullException(nameof(newValue));

                if (IsAttached)
                {
                    throw new InvalidOperationException("Already attached");
                }

                _hostControl = newValue;
            }
        }

        protected override void OnAttach()
        {
            if (_hostControl == null)
                throw  new InvalidOperationException();

            _hostControl.Navigating += OnFrameNavigating;
            _hostControl.NavigationFailed += OnFrameNavigationFailed;
            _hostControl.NavigationStopped+= OnFrameNavigationStopped;
            _hostControl.Navigated += OnFrameNavigated;
            Region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
        }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            _isNavigating = true;
        }


        private void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            _isNavigating = false;
        }

        private void OnFrameNavigationStopped(object sender, NavigationEventArgs navigationEventArgs)
        {
            _isNavigating = false;
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            var view = GetView(_hostControl.Content);
            if (view != null)
                Region.Activate(view);

            _isNavigating = false;

        }

        private object GetView(object content)
        {
            var wrappedPage = content as FrameNavigationWrapperPage;
            if (wrappedPage == null)
                return null;

            var contentControl = wrappedPage.Content as ContentControl;
            return  contentControl?.Content ?? wrappedPage.Content;
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (!_isNavigating)
            {
                var view = Region.ActiveViews.FirstOrDefault();
                _hostControl.Content = view != null ? FrameContentLoader.GetFrameContent(view, Region) : null;
            };
        }

   
      


     
    }
}