using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Flobus.Dialogs
{
    public class PipelineAddDialogViewModel : DialogViewModel<Action<PipelineDTO>>
    {
        public DelegateCommand AddCommand { get; }

        public PipelineAddDialogViewModel(Action<PipelineDTO> closeCallback)
            : base(closeCallback)
        {
            AddCommand = new DelegateCommand(() => { DialogResult = true; AddCommand.RaiseCanExecuteChanged(); }, () => SelectedPipeline != null && DialogResult == null);
        }

        protected override void InvokeCallback(Action<PipelineDTO> closeCallback)
        {
            closeCallback(SelectedPipeline);
        }

        private PipelineDTO _selectedPipeline;
        public PipelineDTO SelectedPipeline
        {
            get { return _selectedPipeline; }
            set
            {
                _selectedPipeline = value;
                AddCommand.RaiseCanExecuteChanged();
            } 
        }

        public List<PipelineDTO> PipelineList { get; set; }
    }
}
