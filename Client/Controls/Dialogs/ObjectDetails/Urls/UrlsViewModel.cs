using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Browser;
using System.Windows.Controls.Primitives;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Controls.Dialogs.ObjectDetails.Urls
{
    public class UrlsViewModel : ValidationViewModel
    {
        private Guid? _entityId;
        private bool _isActive;
        private bool _isReadOnly;
        private EntityUrlDTO _selectedItem;

        public UrlsViewModel()
        {
            GoToUrlCommand = new DelegateCommand<object>(GoToUrl);

            RefreshCommand = new DelegateCommand(Refresh);
            AddCommand = new DelegateCommand(Add, () => _entityId.HasValue);
            EditCommand = new DelegateCommand(Edit, () => _selectedItem != null);
            RemoveCommand = new DelegateCommand(Remove, () => _selectedItem != null);


            Refresh();
        }

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        public Guid? EntityId
        {
            get { return _entityId;}
            set
            {
                if(SetProperty(ref _entityId, value))
                {
                    Refresh();
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Если установлен в True, то при каждом изменении EntityId обновляется список 
        /// Это сделано для того, чтобы грузить данные только в том случае когда вкладка активна
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    Refresh();
                }
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }

        public List<EntityUrlDTO> ItemList { get; set; }

        public EntityUrlDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    RemoveCommand.RaiseCanExecuteChanged();
                    EditCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Существуют ли ссылки по данному объекту
        /// </summary>
        public bool HasUrls
        {
            get { return ItemList != null && ItemList.Any(); }
        }

        public DelegateCommand<object> GoToUrlCommand { get; set; }

        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand RemoveCommand { get; set; }


        private async void Refresh()
        {
            if (!_isActive) return;
            try
            {
                Behavior.TryLock();

                ItemList = _entityId.HasValue
                    ? await new ObjectModelServiceProxy().GetEntityUrlListAsync(_entityId)
                    : new List<EntityUrlDTO>();
                
                
                OnPropertyChanged(() => ItemList);
                OnPropertyChanged(() => HasUrls);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }

        private void GoToUrl(object x)
        {
            var url = x as string;
            HtmlPage.Window.Navigate(new Uri(url, UriKind.Absolute), "_blank");
        }

        private void Add()
        {
            if (!_entityId.HasValue) return;

            var vm = new AddEditUrlViewModel(Refresh, _entityId.Value);
            var v = new AddEditUrlView { DataContext = vm };
            v.ShowDialog();
        }

        private void Edit()
        {
            if (_selectedItem == null) return;

            var vm = new AddEditUrlViewModel(Refresh, _selectedItem);
            var v = new AddEditUrlView { DataContext = vm };
            v.ShowDialog();
        }


        private async void Remove()
        {
            if (_selectedItem == null) return;
            await new ObjectModelServiceProxy().RemoveEntityUrlAsync(_selectedItem.UrlId);
            Refresh();
        }
    }
}