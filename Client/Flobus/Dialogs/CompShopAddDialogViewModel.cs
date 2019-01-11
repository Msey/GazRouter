using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.ObjectModel.CompShops;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Flobus.Dialogs
{
    public class CompShopAddDialogViewModel : DialogViewModel<Action<CompShopDTO>>
    {
        public DelegateCommand AddCommand { get; }

        public CompShopAddDialogViewModel(Action<CompShopDTO> closeCallback)
            : base(closeCallback)
        {
            AddCommand = new DelegateCommand(() => { DialogResult = true; }, () => SelectedCompressorShop != null);
        }

        protected override void InvokeCallback(Action<CompShopDTO> closeCallback)
        {
            closeCallback(SelectedCompressorShop);
        }

        private CompShopDTO _selectedCompressorShop;
        public CompShopDTO SelectedCompressorShop
        {
            get { return _selectedCompressorShop; }
            set
            {
                _selectedCompressorShop = value;
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public List<CompShopDTO> CompShopList { get; set; }

    }
}
