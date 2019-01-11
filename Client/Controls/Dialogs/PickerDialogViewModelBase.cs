using System;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Data;

namespace GazRouter.Controls.Dialogs
{
    public abstract class PickerDialogViewModelBase<TItem> : DialogViewModel
        where TItem : class
    {
        private string _namePart;
        private VirtualQueryableCollectionView<TItem> _items;
        private TItem _selectedItem;
        private readonly SortDescriptorCollection _sortDescriptors = new SortDescriptorCollection();

        protected PickerDialogViewModelBase(Action closeCallback) : base(closeCallback) {}

        protected virtual void Init()
        {
            ApplyFilterCommand = new DelegateCommand(ApplyFilter);
            SelectCommand = new DelegateCommand(() => DialogResult = true, () => SelectedItem != null);
        }
        
        public string NamePart
        {
            get { return _namePart; }
            set
            {
                if (SetProperty(ref _namePart, value))
                {
                    ApplyFilter();
                }
            }
        }
        
        public string HighlightText => NamePart; 

        public DelegateCommand ApplyFilterCommand { get; protected set; }
        public DelegateCommand SelectCommand { get; protected set; }


        public VirtualQueryableCollectionView<TItem> Items
        {
            get { return _items; }
            protected set
            {
                _items = value;
                OnPropertyChanged(() => Items);
            }
        }

        
        public TItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    OnSelectedItemsChanges?.Invoke();
                    SelectCommand.RaiseCanExecuteChanged();
                }
            }
        }

    	public Action OnSelectedItemsChanges;

        

        protected void CreateItems()
        {
            var items = new VirtualQueryableCollectionView<TItem> { PageSize = PageSize, LoadSize = PageSize, VirtualItemCount = 100 };
            if (Items != null)
            {
                if (Items.SortDescriptors.Count > 0)
                {
                    _sortDescriptors.AddRange(Items.SortDescriptors);
                    Items.SortDescriptors.Clear();
                }
                Items.ItemsLoading -= OnItemsLoading;
            }
            items.ItemsLoading += OnItemsLoading;
            Items = items;
        }

        public virtual int PageSize => 21;

        protected virtual void OnItemsLoading(object sender, VirtualQueryableCollectionViewItemsLoadingEventArgs e)
        {
            if (_sortDescriptors.Count > 0)
            {
                Items.SortDescriptors.AddRange(_sortDescriptors);
                _sortDescriptors.Clear();
            }
        }

        protected abstract void ApplyFilter();
    }
}
