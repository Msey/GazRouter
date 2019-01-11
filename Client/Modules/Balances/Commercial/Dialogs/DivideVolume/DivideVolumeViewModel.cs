using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.MonthAlgorithms;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Commercial.Dialogs.DivideVolume
{
    public class DivideVolumeViewModel : DialogViewModel
    {
        private GasOwnerDTO _selectedOwner;
        private readonly Guid? _siteId;
        private readonly int _contractId;

        public DivideVolumeViewModel(List<GasOwnerDTO> ownerList, Guid? siteId, int contractId, Action actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _siteId = siteId;
            _contractId = contractId;
            OwnerList = ownerList;
            var defaultOwnerId = IsolatedStorageManager.Get<int?>("MakeBalanceDefaultOwner");
            _selectedOwner = OwnerList.SingleOrDefault(o => o.Id == defaultOwnerId);

            SelectCommand = new DelegateCommand(() => DialogResult = true, () => SelectedOwner != null );
        }
        
        public List<GasOwnerDTO> OwnerList { get; set; }

        public GasOwnerDTO SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                if(SetProperty(ref _selectedOwner, value))
                {
                    SelectCommand.RaiseCanExecuteChanged();
                    IsolatedStorageManager.Set("MakeBalanceDefaultOwner", value.Id);
                }
            }
        }

        public bool IntakeTransit { get; set; }

        public bool Consumers { get; set; }

        public bool OperConsumers { get; set; }



        public DelegateCommand SelectCommand { get; set; }
        
        protected override async void InvokeCallback(Action closeCallback)
        {
            Lock();
            await new BalancesServiceProxy().RunDivideVolumeAlgorithmAsync(
                new DivideVolumeAlgorithmParameterSet
                {
                    ContractId = _contractId,
                    DefaultOwnerId = SelectedOwner.Id,
                    SiteId = _siteId,
                    IntakeTransitFilter = IntakeTransit,
                    ConsumersFilter = Consumers,
                    OperConsumersFilter = OperConsumers
                });
            Unlock();

            closeCallback?.Invoke();
        }
    }

    

    
}
