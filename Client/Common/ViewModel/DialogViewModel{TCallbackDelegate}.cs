using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Common.ViewModel
{
    public abstract class DialogViewModel<TCallBackDelegate> : ValidationViewModel where TCallBackDelegate : class
    {
        private bool? _dialogResult;

        protected DialogViewModel(TCallBackDelegate closeCallback)
        {
            CloseCallback = closeCallback;
            CancelCommand = new DelegateCommand(OnCancelCommandExecute);
        }

        public DelegateCommand CancelCommand { get; private set; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set
            {
                _dialogResult = null;
                if (!SetProperty(ref _dialogResult, value))
                {
                    return;
                }

                if (value == true && CloseCallback != null)
                {
                    InvokeCallback(CloseCallback);
                }
            }
        }

        private TCallBackDelegate CloseCallback { get; }

        protected sealed override bool ValidateExclude(string propertyName)
        {
            return propertyName == nameof(DialogResult) || base.ValidateExclude(propertyName);
        }

        protected virtual bool CallBackAfterRequestOnModifyData(Exception exception)
        {
            if (exception == null)
            {
                DialogResult = true;
                return true;
            }

            var faultException = exception as FaultException<FaultDetail>;
            if (faultException != null && faultException.Detail.FaultType == FaultType.IntegrityConstraint)
            {
                MessageBoxProvider.Alert(faultException.ToString(), "Ошибка");
                return true;
            }
            return false;
        }

        protected abstract void InvokeCallback(TCallBackDelegate closeCallback);

        private void OnCancelCommandExecute()
        {
            DialogResult = false;
        }
    }
}