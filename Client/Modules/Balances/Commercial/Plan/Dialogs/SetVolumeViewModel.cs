using System;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.Commercial.Plan.Dialogs
{
    public class SetVolumeViewModel : DialogViewModel<Action<double>>
    {
        public SetVolumeViewModel(string caption, Action<double> saveValue)
            : base(saveValue)
        {
            Caption = caption;
            SaveCommand = new DelegateCommand(() => DialogResult = true);
        }


        public string Caption { get; }


        public double Value { get; set; }

        protected override void InvokeCallback(Action<double> closeCallback)
        {
            closeCallback(Value);
        }

        public DelegateCommand SaveCommand { get; set; }
    }
}
