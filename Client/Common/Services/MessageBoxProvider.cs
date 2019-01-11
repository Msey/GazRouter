using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace GazRouter.Common.Services
{
    public sealed class MessageBoxProvider : IMessageBoxProvider
    {
        public void Confirm(object content, Action<bool> closedAction, string header, string okButtonText = "Да",
            string cancelButtonText = "Нет")
        {
            RadWindow.Confirm(
                new DialogParameters
                {
                    OkButtonContent = okButtonText,
                    CancelButtonContent = cancelButtonText,
                    Header = header,
                    Content = content,
                    Closed = (s, e) => closedAction(e.DialogResult ?? false),
                });
        }

        public void Confirm(string msgText, Action<bool> closedAction, string header, string okButtonText = "Да",
            string cancelButtonText = "Нет")
        {
            RadWindow.Confirm(
                new DialogParameters
                {
                    OkButtonContent = okButtonText,
                    CancelButtonContent = cancelButtonText,
                    Header = header,
                    Content = new TextBlock
                    {
                        Text = msgText,
                        Width = 300,
                        TextWrapping = TextWrapping.Wrap
                    },
                    Closed = (s, e) => closedAction(e.DialogResult ?? false),
                });
        }

        public void Alert(string msgText, string header, string cancelButtonText = "Закрыть")
        {
            
            RadWindow.Alert(
                new DialogParameters
                {
                    OkButtonContent = cancelButtonText,
                    Header = header,
                    Content = new TextBlock
                    {
                        Text = msgText,
                        Width = 300,
                        TextWrapping = TextWrapping.Wrap
                    }
                });
        }
    }
}