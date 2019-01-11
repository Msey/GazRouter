using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace GazRouter.ObjectModel
{
    public class MessageBoxNotificationAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            if (args == null)
            {
                return;
            }

            var confirmation = (Confirmation) args.Context;
            MessageBoxResult result = MessageBox.Show(confirmation.Content.ToString(), confirmation.Title,
                MessageBoxButton.OKCancel);

            confirmation.Confirmed = result == MessageBoxResult.OK;
            args.Callback();
        }
    }
}