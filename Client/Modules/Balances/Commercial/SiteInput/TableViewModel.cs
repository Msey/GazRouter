using System;
using System.Collections.Generic;
using GazRouter.Common;
using GazRouter.Common.ViewModel;

namespace GazRouter.Balances.Commercial.SiteInput
{
    public class TableViewModel : ViewModelBase
    {
        private Action<ItemBase> _selectedItemChanged;
        private ItemBase _selectedItem;

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }
        public TableViewModel(Action<ItemBase> selectedItemChanged)
        {
            Items = new List<ItemBase>();
            _selectedItemChanged = selectedItemChanged;

            ValueFormat = "#,0.000";
        }

        public List<ItemBase> Items { get; set; }

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    _selectedItemChanged(value);
            }
        }

        public void UpdateItems()
        {
            OnPropertyChanged(() => Items);
        }

        public string ValueFormat { get; set; }

        public string DeltaFormat => $"+{ValueFormat};-{ValueFormat};#";
    }
}
