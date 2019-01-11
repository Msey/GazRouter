using System.Windows;
using System.Windows.Controls;

namespace GazRouter.Common.Ui.AttachedProperties
{
    public class FocusHelper
    {
        private static void OnEnsureFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var control = (Control)d ;
                control.Dispatcher.BeginInvoke(() => control.Focus());
            }
        }

        public static bool GetEnsureFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnsureFocusProperty);
        }

        public static void SetEnsureFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(EnsureFocusProperty, value);
        }

        public static readonly DependencyProperty EnsureFocusProperty =
            DependencyProperty.RegisterAttached(
            "EnsureFocus",
            typeof(bool),
            typeof(FocusHelper),
            new PropertyMetadata(OnEnsureFocusChanged));
    }
}