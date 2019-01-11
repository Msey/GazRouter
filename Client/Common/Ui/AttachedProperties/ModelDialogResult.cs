using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace GazRouter.Common.Ui.AttachedProperties
{
    public static class ModelDialogResult
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(ModelDialogResult),
                new PropertyMetadata(OnSetDialogResultCallback));

        public static void SetDialogResult(RadWindow childWindow, bool? dialogResult)
        {
            childWindow.SetValue(DialogResultProperty, dialogResult);
        }

        public static bool? GetDialogResult(RadWindow childWindow)
        {
            return childWindow.GetValue(DialogResultProperty) as bool?;
        }

        private static void OnSetDialogResultCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var childWindow = dependencyObject as RadWindow;
            if (childWindow == null)
            {
                return;
            }

            childWindow.DialogResult = e.NewValue as bool?;
            childWindow.Close();
        }
    }
}