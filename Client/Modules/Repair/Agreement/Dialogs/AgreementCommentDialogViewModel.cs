using System;
using System.Windows.Controls;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Attachments;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Repair.Agreement.Dialogs
{
    public class AgreementCommentDialogViewModel : DialogViewModel<Action<string>>
    {
        private readonly string _header;
        public string header => _header;
        private string _comment;
        public string comment
        {
            get
            {
                return _comment;
            }
            set
            {
                SetProperty(ref _comment, value);
            }
        }

        private readonly DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand => _SaveCommand;

        public AgreementCommentDialogViewModel(string headerText, Action<string> closeCallback) : base(closeCallback)
        {
            _header = headerText;
            _SaveCommand = new DelegateCommand(() => DialogResult = true);
        }

        protected override void InvokeCallback(Action<string> closeCallback)
        {
            if(closeCallback != null)
                closeCallback(comment);
        }
    }
}
