using System;
using GazRouter.Balances.Commercial.Fact;
using GazRouter.Balances.Commercial.Plan;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.Balances.Commercial.Common
{
    public class TableViewModel : ViewModelBase
    {

        private readonly Action<ItemBase> _selectedItemChanged;
        private readonly Action<BalanceItem?> _valueChanged;
        private readonly BalanceItem _balItem;
        private readonly Target _target;
        
        public TableViewModel(Target target, BalanceItem balItem, Action<ItemBase> selectedItemChanged, Action<BalanceItem?> valueChanged)
        {
            _target = target;
            _balItem = balItem;
            _selectedItemChanged = selectedItemChanged;
            _valueChanged = valueChanged;

            ValueFormat = "#,0.000";
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }


        public ItemBase RootItem { get; set; }


        private ItemBase _selectedItem;
        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    _selectedItemChanged?.Invoke(value);
            }
        }
        

        public void AddTree(ItemBase item)
        {
            if (item == null) return;
            RootItem = _target == Target.Plan ? (ItemBase)new PlanSummaryItem("ВСЕГО:", null) : new FactSummaryItem("ВСЕГО", null);
            RootItem.ValueChanged = type => _valueChanged?.Invoke(_balItem);

            RootItem.AddChild(item);

            OnPropertyChanged(() => RootItem);
            OnPropertyChanged(() => ItemColumnName);
            OnPropertyChanged(() => UnitsName);
        }


        public string ItemColumnName { get; set; } = "Точка приема|сдачи газа";


        private string _unitsName;
        public string UnitsName
        {
            get { return _unitsName; }
            set
            {
                SetProperty(ref _unitsName, value);
            }
        }

        public string ValueFormat { get; set; }

        public string DeltaFormat => $"+{ValueFormat};-{ValueFormat};#";
    }
}
