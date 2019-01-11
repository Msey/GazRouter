using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.GasOwners;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Commercial.Dialogs.Swap
{
    public class SwapViewModel : DialogViewModel<Action<int, double>> 
    {
        private readonly double _totalVolume;
        private bool _swapWholeVolume;
        private double _volume;
        private GasOwnerDTO _selectedOwner;
        
        public SwapViewModel(List<GasOwnerDTO> ownerList, double totalVolume, Action<int, double> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            OwnerList = ownerList;
            _totalVolume = totalVolume;
            SwapCommand = new DelegateCommand(() => DialogResult = true, () => SelectedOwner != null && Volume != 0 );

            AddValidationFor(() => Volume)
                .When(() => Volume > _totalVolume)
                .Show($"Не может превышать {_totalVolume:0,0.###}");
        }
        
        public List<GasOwnerDTO> OwnerList { get; set; }

        public GasOwnerDTO SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                if (SetProperty(ref _selectedOwner, value))
                {
                    SwapCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                if (SetProperty(ref _volume, value))
                {
                    SwapCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool SwapWholeVolume
        {
            get { return _swapWholeVolume; }
            set
            {
                if (SetProperty(ref _swapWholeVolume, value))
                {
                    Volume = value ? _totalVolume : 0;
                }
            }
        }


        public DelegateCommand SwapCommand { get; set; }

        protected override void InvokeCallback(Action<int, double> closeCallback)
        {
            closeCallback?.Invoke(SelectedOwner.Id, Volume);
        }
    }

    

    
}
