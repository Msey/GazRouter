using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Flobus.FloScheme;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Gets the enclosing bounds of the list of diagram items.
        /// </summary>
        /// <param name="items">The diagram items.</param>
        /// <returns></returns>
        public static Rect GetEnclosingBounds(this IEnumerable<ISchemaItem> items)
        {
            var enclosingBounds = Rect.Empty;

            foreach (var child in items.Where(s => s.IsVisible()))
            {
                var bounds = child.Bounds;
                if (bounds.X == 0d && bounds.Y == 0d && bounds.Width == 0d && bounds.Height == 0)
                    continue;
                enclosingBounds.Union(bounds);
            }

            return enclosingBounds;
        }

        /// <summary>
        /// Returns whether according to the virtualization the given item is visible.
        /// </summary>
        /// <param name="item">A diagram item.</param>
        /// <param name="isGraphVirtualized">If set to <c>true</c> the item's <see cref="ISupportVirtualization.VirtualizationState"/> will be queried.</param>
        /// <returns>
        ///   Returns <c>true</c> if the specified item is visible; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsVisible(this ISchemaItem item)
        {
            return item.VirtualizationState != ItemVisibility.Collapsed;
        }


        /// <summary>
        /// Sets the location in the element in Canvas.
        /// 
        /// </summary>
        /// <param name="element">The element.</param><param name="x">The x.</param><param name="y">The y.</param>
        public static void SetLocation(this UIElement element, double x, double y)
        {
            if (!x.IsNanOrInfinity())
                Canvas.SetLeft(element, x);
            if (!y.IsNanOrInfinity())
                Canvas.SetTop(element, y);
        }

        /// <summary>
        /// Sets the location in the element in Canvas.
        /// 
        /// </summary>
        /// <param name="element">The element.</param><param name="point">The point.</param>
        public static void SetLocation(this UIElement element, Point point)
        {
            SetLocation(element, Math.Round(point.X, 0), Math.Round(point.Y, 0));

        }

    }
}
