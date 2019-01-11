using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using NavigationContext = Microsoft.Practices.Prism.Regions.NavigationContext;

namespace GazRouter.Client.FrameNavigation
{
    public class FrameContentLoader : INavigationContentLoader
    {
        private IRegion _region;
        private IRegionNavigationContentLoader _targetHandler;

        public IAsyncResult BeginLoad(Uri targetUri, Uri currentUri, AsyncCallback userCallback, object asyncState)
        {
            EnsureRegionIsSet();

            var view = _targetHandler.LoadContent(_region, new NavigationContext(null, targetUri));

            var effectiveView = GetFrameContent(view, _region);

            var result = new LoadAsyncResult {FrameContent = effectiveView, ActualView = view, AsyncState = asyncState};

            userCallback?.Invoke(result);
            return result;
        }

        public bool CanLoad(Uri targetUri, Uri currentUri)
        {
            return true;
        }

        public void CancelLoad(IAsyncResult asyncResult)
        {
        }

        public LoadResult EndLoad(IAsyncResult asyncResult)
        {
            var loadAsyncResult = (LoadAsyncResult) asyncResult;
            return new LoadResult(loadAsyncResult.FrameContent);
        }

        internal static DependencyObject GetFrameContent(object view, IRegion region)
        {
            var frameworkElement = view as FrameworkElement;
            var content = frameworkElement ??
                          new ContentControl
                          {
                              Content = view,
                              HorizontalAlignment = HorizontalAlignment.Stretch,
                              VerticalContentAlignment = VerticalAlignment.Stretch
                          };

            if (content is Page)
            {
                return content;
            }

            var pageWrapper = content.Parent ??
                              new FrameNavigationWrapperPage
                              {
                                  Content = content,
                                  FrameRegionNavigationService =
                                      region.NavigationService as FrameRegionNavigationService
                              };
            return pageWrapper;
        }

        private void EnsureRegionIsSet()
        {
            if (_region == null)
            {
                var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
                _region = regionManager.Regions["MainRegion"];
                _targetHandler = ServiceLocator.Current.GetInstance<IRegionNavigationContentLoader>();
            }
        }

        public class LoadAsyncResult : IAsyncResult
        {
            public bool IsCompleted => true;

            public WaitHandle AsyncWaitHandle { get; }
            public object AsyncState { get; set; }
            public bool CompletedSynchronously { get; }
            public object FrameContent { get; set; }
            public object ActualView { get; set; }
        }
    }
}