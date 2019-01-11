using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Services
{
    public class VirtualizationService
    {
        private readonly Schema _scheme;

        public VirtualizationService(Schema scheme)
        {
            _scheme = scheme;
        }

        public static void UpdateVirtualization(IEnumerable<IWidget> items, Rect viewport)
        {

            foreach (var item in items.ToList())
            {
                if (IsInViewport(item, viewport))
                {
                    RealizeItem(item);
                }
                else
                {
                    VirtualizeItem(item);
                }
            }
        }


      

        public static void ForceRealization(IEnumerable<IWidget> items)
        {
            foreach (var widget in items)
            {
               RealizeItem(widget); 
            }
        }

        internal static void VirtualizeItem(IWidget item)
        {
            if (item.VirtualizationState == ItemVisibility.Visible)
            {
                var itemBounds = item.Bounds;
                if (itemBounds.Width > 0 && itemBounds.Height > 0)
                {
                    item.VirtualizationState = ItemVisibility.Virtualized;
                }
            }
        }

        private static void RealizeItem(IWidget item)
        {

            if (item.VirtualizationState == ItemVisibility.Virtualized)
            {
                item.VirtualizationState = ItemVisibility.Visible;
            }
        }

        private static bool IsInViewport(IWidget item, Rect viewport)
        {
            return viewport.IntersectsWith(item.Bounds);
        }


        public void Virtualize(IEnumerable<PipelineWidget> items)
        {
            if (IsBlocked)
                return;

            var viewport = _scheme.Viewport;
            UpdateVirtualization(items, viewport);
        }

        public bool IsBlocked { get; set; }
    }
}