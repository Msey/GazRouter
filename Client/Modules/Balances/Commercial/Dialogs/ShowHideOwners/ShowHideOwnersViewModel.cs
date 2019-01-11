using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.Commercial.Dialogs.ShowHideOwners
{
    public class ShowHideOwnersViewModel : DialogViewModel
    {
        private readonly Guid _entityId;
        private readonly int _systemId;
        private readonly BalanceItem _balItem;
        public ShowHideOwnersViewModel(Guid entityId, BalanceItem balItem, int systemId, Action actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _entityId = entityId;
            _systemId = systemId;
            _balItem = balItem;

            SaveCommand = new DelegateCommand(Save);

            LoadOwnerList();
        }

        public List<OwnerItem> Items { get; set; }

        private async void LoadOwnerList()
        {
            Lock();

            var owners = await new BalancesServiceProxy().GetGasOwnerListAsync(_systemId);

            Items = owners.Select(o => new OwnerItem(o, _entityId, _balItem, () => OnPropertyChanged(() => IsAllActive))).ToList();
            OnPropertyChanged(() => Items);
            OnPropertyChanged(() => IsAllActive);

            Unlock();
        }


        public bool IsAllActive
        {
            get { return Items?.All(i => i.IsActive) ?? false; }
            set { Items.ForEach(i => i.IsActive = value); }
        }



        public DelegateCommand SaveCommand { get; set; }


        private async void Save()
        {
            foreach (var item in Items)
            {
                await new BalancesServiceProxy().SetGasOwnerDisableAsync(
                    new SetGasOwnerDisableParameterSet
                    {
                        EntityId = _entityId,
                        GasOwnerId = item.Owner.Id,
                        BalanceItem = _balItem,
                        IsDisable = !item.IsActive
                    });
            }

            DialogResult = true;
        }
    }

    public class OwnerItem : PropertyChangedBase
    {
        private readonly Action _selectionChanged;

        public OwnerItem(GasOwnerDTO owner, Guid entityId, BalanceItem balItem, Action selectionChanged)
        {
            _selectionChanged = selectionChanged;
            Owner = owner;
            _isActive = !owner.DisableList.Any(d => d.EntityId == entityId && d.BalanceItem == balItem);
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                    _selectionChanged?.Invoke();
            }
        }
        
        public GasOwnerDTO Owner { get; set; }
    }
}
