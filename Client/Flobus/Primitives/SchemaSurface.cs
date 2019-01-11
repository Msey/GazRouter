using System.Windows;
using System.Windows.Controls;
using Telerik.Windows;
using Telerik.Windows.Diagrams.Core;
using PropertyMetadata = Telerik.Windows.PropertyMetadata;

namespace GazRouter.Flobus.Primitives
{
    public class SchemaSurface : Canvas
    {
        public static readonly DependencyProperty ItemVisibilityProperty =
            DependencyPropertyExtensions.RegisterAttached("ItemVisibility", typeof (ItemVisibility),
                typeof (SchemaSurface), new PropertyMetadata(ItemVisibility.Visible));


        public static ItemVisibility GetItemVisibility(DependencyObject obj
            )
        {
            return (ItemVisibility) obj.GetValue(ItemVisibilityProperty);
        }

        public static void SetItemVisibility(DependencyObject obj, ItemVisibility value)
        {
            obj.SetValue(ItemVisibilityProperty, value);
        }
    }
}