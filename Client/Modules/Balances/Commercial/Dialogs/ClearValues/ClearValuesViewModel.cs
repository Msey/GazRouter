using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Commercial.Dialogs.ClearValues
{
    public class ClearValuesViewModel : DialogViewModel
    {
        private readonly int _contractId;
        private readonly Guid? _siteId;

        public ClearValuesViewModel(int contractId, Guid? siteId, List<GasOwnerDTO> owners, Action actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _contractId = contractId;
            _siteId = siteId;

            ClearCommand = new DelegateCommand(Clear);


            OwnerList = owners.Select(o => new OwnerSelector(o, () => OnPropertyChanged(() => IsAllOwnersSelected))).ToList();

            var siteItems = new List<BalanceItem>
            {
                BalanceItem.Intake,
                BalanceItem.Transit,
                BalanceItem.Consumers,
                BalanceItem.OperConsumers
            };

            var enterpriseItems = new List<BalanceItem>
            {
                BalanceItem.Intake,
                BalanceItem.Transit,
                BalanceItem.Consumers,
                BalanceItem.AuxCosts,
                BalanceItem.OperConsumers,
                BalanceItem.GasSupply
            };

            BalItemList =
                (siteId.HasValue ? siteItems : enterpriseItems).Select(
                    i => new BalItemSelector(i, () => OnPropertyChanged(() => IsAllBalItemsSelected))).ToList();
        }

        public List<OwnerSelector> OwnerList { get; set; }
        public List<BalItemSelector> BalItemList { get; set; }
        
        public DelegateCommand ClearCommand { get; set; }

        private async void Clear()
        {
            await new BalancesServiceProxy().ClearBalanceValuesAsync(
                new ClearBalanceValuesParameterSet
                {
                    ContractId = _contractId,
                    SiteId = _siteId,
                    OwnerIdList = OwnerList.Where(o => o.IsSelected).Select(o => o.Owner.Id).ToList(),
                    BalanceItemList = BalItemList.Where(i => i.IsSelected).Select(i => i.BalItem).ToList()
                });

            DialogResult = true;
        }


        public bool IsAllOwnersSelected
        {
            get { return OwnerList.All(o => o.IsSelected); }
            set { OwnerList.ForEach(o => o.IsSelected = value); }
        }

        public bool IsAllBalItemsSelected
        {
            get { return BalItemList.All(o => o.IsSelected); }
            set { BalItemList.ForEach(o => o.IsSelected = value); }
        }
    }

    public abstract class SelectorBase : PropertyChangedBase
    {
        private readonly Action _selectionChanged;
        protected SelectorBase(Action selectionChanged)
        {
            _selectionChanged = selectionChanged;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (SetProperty(ref _isSelected, value))
                    _selectionChanged?.Invoke();
            }
        }
    }

    public class OwnerSelector : SelectorBase
    {
        public OwnerSelector(GasOwnerDTO owner, Action selectionChanged)
            : base(selectionChanged)
        {
            Owner = owner;
        }
        public GasOwnerDTO Owner { get; }
    }

    public class BalItemSelector : SelectorBase
    {
        public BalItemSelector(BalanceItem balItem, Action selectionChanged)
            : base(selectionChanged)
        {
            BalItem = balItem;
        }

        public BalanceItem BalItem { get; }
    }
}
