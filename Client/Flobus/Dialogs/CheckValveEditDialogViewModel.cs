using GazRouter.Common.ViewModel;
using GazRouter.Flobus.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System;

namespace GazRouter.Flobus.Dialogs
{
    public class CheckValveEditDialogViewModel : DialogViewModel<Action<CheckValveEditDialogViewModel>>
    {
        private readonly ICheckValveWidget _model;
        private string _tooltip;
        public CheckValveEditDialogViewModel(ICheckValveWidget model) : base(vm => { })
        {
            _model = model;
            _tooltip = model.Tooltip;

            SaveCommand = new DelegateCommand(OnSave);
        }
        
        public DelegateCommand SaveCommand { get; private set; }

        public string Tooltip
        {
            get { return _tooltip; }
            set
            {
                _tooltip = value;
                OnPropertyChanged(() => Tooltip);
            }
        }               

        protected override void InvokeCallback(Action<CheckValveEditDialogViewModel> closeCallback)
        {
            closeCallback(this);
        }

        private void OnSave()
        {
            _model.Tooltip = _tooltip;
            DialogResult = true;
        }
    }
}
