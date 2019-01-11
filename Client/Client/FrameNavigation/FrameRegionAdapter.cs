using System;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.Client.FrameNavigation
{
    public class FrameRegionAdapter : RegionAdapterBase<Frame>
    {
        public FrameRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, Frame regionTarget)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException(nameof(regionTarget));
            }

            region.NavigationService = new FrameRegionNavigationService {Frame = regionTarget, Region = region};
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }

        protected override void AttachBehaviors(IRegion region, Frame regionTarget)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }
            base.AttachBehaviors(region, regionTarget);
            if (!region.Behaviors.ContainsKey(FrameRegionSyncBehavior.BehaviorKey))
            {
                region.Behaviors.Add(FrameRegionSyncBehavior.BehaviorKey,
                    new FrameRegionSyncBehavior {HostControl = regionTarget});
            }
        }
    }
}