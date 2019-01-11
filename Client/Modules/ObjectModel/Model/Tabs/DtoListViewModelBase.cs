using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.Prism;

namespace GazRouter.ObjectModel.Model.Tabs
{
    public abstract class DtoListViewModelBase<TItem, TId> : LockableViewModel, ITabItem
        where TItem : class, IListItem<TId>
        where TId : struct
    {
        private bool _isDataLoaded;

        private CommonEntityDTO _parentEntity;
        private TItem _selectedItem;

        protected DtoListViewModelBase()
        {
            Items = new ObservableCollection<TItem>();
        }

        public ObservableCollection<TItem> Items { get; set; }

        public TItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                RaiseCommands();
                OnPropertyChanged(() => SelectedItem);
            }
        }

        #region ITabItem Members

        public virtual void Refresh()
        {
            ClearLoadFlag();
            ReloadData();
        }

        public CommonEntityDTO ParentEntity
        {
            get { return _parentEntity; }
            set { SetParentEntity(value); }
        }

        public virtual string Header
        {
            get { return string.Empty; }
        }

        #endregion

        protected abstract void GetList();

        protected virtual void RaiseCommands()
        {
        }

        protected virtual bool GetListCallback(IEnumerable<TItem> dtos, Exception ex)
        {
            if (ex == null)
            {
                _isDataLoaded = true;
                Items.AddRange(dtos);
                SetSelection();
               
            }
            return ex == null;
        }

        protected virtual void SetSelection()
        {
            if (SelectedItem != null)
            {
                SelectedItem = Items.FirstOrDefault(p => Equals(p.Id, SelectedItem.Id));
            }
        }

        private void ReloadData()
        {
            if (_isDataLoaded) return;
            Items.Clear();
            GetList();
        
        }


        public void ClearLoadFlag()
        {
            _isDataLoaded = false;
        }

        protected virtual void SetParentEntity(CommonEntityDTO value)
        {
            _parentEntity = value;
            OnPropertyChanged(() => ParentEntity);
        }
    }
}