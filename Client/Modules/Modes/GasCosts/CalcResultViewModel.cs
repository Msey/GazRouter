using System;
using GazRouter.Common.ViewModel;
namespace GazRouter.Modes.GasCosts
{
    public class CalcResultViewModel : PropertyChangedBase
    {
        public CalcResultViewModel(string message, Action closeAction)
        {
            Message = message;
            CloseWindowCommand = new Telerik.Windows.Controls.DelegateCommand(obj =>
            {
                closeAction.Invoke();
            },
            obj => true);
        }
        public string Message { get; set; }
        public Telerik.Windows.Controls.DelegateCommand CloseWindowCommand { get; set; }
    }
}
