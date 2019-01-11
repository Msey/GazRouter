using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace GazRouter.Common.Ui.AttachedProperties
{
    public static class KeepSelectedItemInViewBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
    DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(KeepSelectedItemInViewBehavior),
        new PropertyMetadata(OnKeepSelectedItemInViewPropertyChanged));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private static void OnKeepSelectedItemInViewPropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs
                dependencyPropertyChangedEventArgs)
        {
            var radGridView = dependencyObject as GridViewDataControl;
            if (radGridView != null)
            {
                radGridView.SelectionChanged += RadGridViewSelectionChanged;
            }
        }

        private static void RadGridViewSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var radGridView = sender as GridViewDataControl;

            if (radGridView?.SelectedItem == null)
            {
                return;
            }

            radGridView.ScrollIntoView(radGridView.SelectedItem);
        }
    }
}