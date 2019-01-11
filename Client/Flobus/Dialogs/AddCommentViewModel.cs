using System;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Flobus.Dialogs
{
    public class AddCommentViewModel : DialogViewModel
    {
        private string _comment;

        public AddCommentViewModel(Action closeCallback) : base(closeCallback)
        {
            OkCommand = new DelegateCommand(OkCommandExecute, OkCommandCanExecute);
        }

        private bool OkCommandCanExecute()
        {
            return string.IsNullOrWhiteSpace(Comment);
        }

        private void OkCommandExecute()
        {
            DialogResult = true;
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
               if ( SetProperty(ref _comment, value))
                    OkCommand.RaiseCanExecuteChanged();

            }
        }

        public DelegateCommand OkCommand { get; set; }
        
    }
}