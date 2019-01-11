using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Telerik.Windows;
using Telerik.Windows.Controls;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace GazRouter.Common.Ui.AttachedProperties
{
    public class RadioMenuItem
    {
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.RegisterAttached("Group", typeof(string), typeof(RadioMenuItem),
                                                new PropertyMetadata(OnSetGroupCallback));


        private static void OnSetGroupCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var menuItem = dependencyObject as RadMenuItem;
            if (menuItem == null)
                return;
            menuItem.Click += MenuItem_Click;

        }


        static void MenuItem_Click(object sender, RadRoutedEventArgs e)
        {

            var currentItem = e.OriginalSource as RadMenuItem;
            if (currentItem != null && currentItem.IsCheckable && GetRadioGroup(currentItem) != null)
            {

                currentItem.IsCheckable = false;
                var siblingItems = GetSiblingGroupItems(currentItem);
                if (siblingItems == null)
                {
                    return;
                }

                foreach (var item in siblingItems.Where(item => item != currentItem))
                {
                    item.IsChecked = false;
                    item.IsCheckable = true;
                }


            }
        }

        public static void SetRadioGroup(RadMenuItem menuItem, string radioGroup)
        {
            menuItem.SetValue(GroupProperty, radioGroup);
        }

        public static string GetRadioGroup(RadMenuItem menuItem)
        {
            return menuItem.GetValue(GroupProperty) as string;
        }

        private static IEnumerable<RadMenuItem> GetSiblingGroupItems(RadMenuItem currentItem)
        {
            ItemsControl parentItem = (ItemsControl) currentItem.ParentOfType<RadMenuItem>() ?? currentItem.ParentOfType<RadContextMenu>();
            if (parentItem == null)
            {
                return null;
            }
            var items = new List<RadMenuItem>();
            foreach (var item in parentItem.Items)
            {
                var container = parentItem.ItemContainerGenerator.ContainerFromItem(item) as RadMenuItem;
                var radioGroup = GetRadioGroup(container);
                if (container == null || radioGroup == null)
                {
                    continue;
                }
                if (radioGroup.Equals(GetRadioGroup(currentItem)))
                {
                    items.Add(container);
                }
            }
            return items;
        }
    }
}