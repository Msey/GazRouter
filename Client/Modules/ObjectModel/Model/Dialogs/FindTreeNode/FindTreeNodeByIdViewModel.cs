using System;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;

namespace GazRouter.ObjectModel.Model.Dialogs.FindTreeNode
{
    public class FindTreeNodeByIdViewModel : DialogViewModel
    {
        public FindTreeNodeByIdViewModel(Action<Guid?> closeCallback)
            : base(null)
        {
            SetValidationRules();
            FindCommand = new DelegateCommand(() =>
            {
                Guid guid;
                if (Guid.TryParse(FindId, out guid))
                {
                    var result = guid.Convert();
                    DialogResult = true;
                    closeCallback(result);
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
