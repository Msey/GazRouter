using System;
using System.Windows.Input;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.DataExchange.Dialogs
{
    public class FindByExtIdViewModel : DialogViewModel
    {
        public FindByExtIdViewModel(Action<string> closeAction)
            : base(null)
        {
            SetValidationRules();
            FindCommand = new DelegateCommand(() =>
            {
                if (FindId + "" != "")
                {
                    DialogResult = true;
                    closeAction(FindId);
                }
                else
                {
                    _isError = true;
                    OnPropertyChanged(() => FindId);
                }
                    
            }, () => !string.IsNullOrEmpty(FindId));
        }

        private bool _isError;

        public DelegateCommand FindCommand { get; set; }

        public string Caption
        {
            get { return "Поиск по идентификатору"; }
        }

        private string _findId;

        public string FindId
        {
            get { return _findId; }
            set
            {
                _findId = value;
                OnPropertyChanged(() => FindId);
                FindCommand.RaiseCanExecuteChanged();
            }
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => FindId)
                .When(() => _isError)
                .Show("Неверный идентификатор! Попробуйте ввести другой.");
        }
    }
}
