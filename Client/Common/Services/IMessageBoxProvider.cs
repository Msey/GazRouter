using System;

namespace GazRouter.Common.Services
{
    public interface IMessageBoxProvider
    {
        void Confirm(object content, Action<bool> closedAction, string header = "Подтверждение", string okButtonText = "Да", string cancelButtonText = "Нет");

        void Confirm(string msgText, Action<bool> closedAction, string header = "Подтверждение", string okButtonText = "Да", string cancelButtonText = "Нет");

        void Alert(string msgText, string header, string cancelButtonText = "Закрыть");
    }
}