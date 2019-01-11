using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.History;
using ItemsControl = Telerik.Windows.Controls.ItemsControl;
using PropertyMetadata = System.Windows.PropertyMetadata;
using SelectionChangedEventArgs = Telerik.Windows.Controls.SelectionChangedEventArgs;

namespace Common.Ui.AttachedProperties
{
    public sealed class RadTreeViewHelper
    {
        public static readonly DependencyProperty AutoExpandToSelectedItemProperty =
           DependencyProperty.RegisterAttached("AutoExpandToSelectedItem", typeof(bool), typeof(RadTreeViewHelper),
                                               new PropertyMetadata(OnAutoExpandToSelectedItemCallback));
        public static void SetAutoExpandToSelectedItem(RadTreeView menuItem, bool value)
        {
            menuItem.SetValue(AutoExpandToSelectedItemProperty, value);
        }


        public static bool GetAutoExpandToSelectedItem(RadTreeView menuItem)
        {
            return menuItem.GetValue(AutoExpandToSelectedItemProperty) is bool && (bool) menuItem.GetValue(AutoExpandToSelectedItemProperty);
        }


        private static void OnAutoExpandToSelectedItemCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                return;

            var radTreeView = d as RadTreeView;
            if (radTreeView == null)
                return;
            radTreeView.SelectionChanged+=RadTreeViewOnSelected;
        }

        private static void RadTreeViewOnSelected(object sender, SelectionChangedEventArgs args)
        {

            var tree = (RadTreeView) sender;
            var item = tree.SelectedItem;
            if (item != null)
                ExpandItem(tree, item);
         
        }

        private static bool ExpandItem(ItemsControl control, object value)
        {
            for (int i =0;i < control.Items.Count; i++)
            {
                RadTreeViewItem item = control.ItemContainerGenerator.ContainerFromIndex(i) as RadTreeViewItem;
                if (item == null)
                    continue;
                
                if (item.Item == value || ExpandItem(item, value))
                {
                    if (!item.IsExpanded)
                        item.IsExpanded = true;
                    return true;
                }

            }

            return false;
        }
    }
}