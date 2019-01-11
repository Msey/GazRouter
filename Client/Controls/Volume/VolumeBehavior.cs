using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GazRouter.Controls.Volume
{
    public class VolumeBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var tb = (TextBox)sender;
            tb.Text = tb.Text.Replace('.', ',');
            tb.SelectionStart = tb.Text.Length;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= OnTextChanged;
        }

        
    }
}