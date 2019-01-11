using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.GasOwners;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Commercial.Dialogs.SelectOwner
{
    public class SelectOwnerViewModel : DialogViewModel<Action<int>> 
    {
        private GasOwnerDTO _selectedOwner;
        
        public SelectOwnerViewModel(List<GasOwnerDTO> ownerList, Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            OwnerList = ownerList;
            SelectCommand = new DelegateCommand(() => DialogResult = true, () => _selectedOwner != null);
        }
        
        public List<GasOwnerDTO> OwnerList { get; set; }

        public GasOwnerDTO SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                if (SetProperty(ref _selectedOwner, value))
                {
                    SelectCommand.RaiseCanExecuteChanged();
                }
            }
        }
        
        public DelegateCommand SelectCommand { get; set; }

        protected override void InvokeCallback(Action<int> closeCallback)
        {
            closeCallback?.Invoke(SelectedOwner.Id);
        }
    }

    

    
}
