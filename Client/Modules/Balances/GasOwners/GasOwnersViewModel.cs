using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.GasOwners
{
    public class GasOwnersViewModel : LockableViewModel
    {

        private int _minSort;
        private int _maxSort;

        public GasOwnersViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.GasOwners);
            //
            RefreshCommand = new DelegateCommand(Refresh);

            AddCommand = new DelegateCommand(Add, () => editPermission);

            EditCommand = new DelegateCommand(Edit, () => SelectedOwner != null & editPermission);

            RemoveCommand = new DelegateCommand(Remove, () => SelectedOwner != null & editPermission);

            UpCommand = new DelegateCommand(Up, () => SelectedOwner != null && _minSort < SelectedOwner.SortOrder & editPermission);

            DownCommand = new DelegateCommand(Down, () => SelectedOwner != null && _maxSort > SelectedOwner.SortOrder & editPermission);
            
            Refresh();
        }


        
        public List<GasOwnerItem> OwnerList { get; set; }
        

        private GasOwnerItem _selectedOwner;

        public GasOwnerItem SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                if(SetProperty(ref _selectedOwner,  value))
                    RefreshCommands();
            }
        }

        

        public DelegateCommand RefreshCommand { get; private set; }

        
        public async void Refresh()
        {
            try
            {
                Behavior.TryLock();
                var list = await new BalancesServiceProxy().GetGasOwnerListAsync(null);
                OwnerList = list.Select(o => new GasOwnerItem(o)).ToList();
                OnPropertyChanged(() => OwnerList);

                _minSort = list.Min((p => p.SortOrder));
                _maxSort = list.Max((p => p.SortOrder));
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }
        
        
        public DelegateCommand AddCommand { get; }

        private void Add()
        {
            var vm = new AddEditGasOwnersViewModel(x => Refresh());
            var v = new AddEditGasOwnersView {DataContext = vm};
            v.ShowDialog();
        }
        

       public DelegateCommand EditCommand { get; }

        private void Edit()
        {
            var viewModel = new AddEditGasOwnersViewModel(x => Refresh(), SelectedOwner.Dto);
            var view = new AddEditGasOwnersView {DataContext = viewModel};
            view.ShowDialog();
        }
        

        
        public DelegateCommand RemoveCommand { get; }

        private void Remove()
        {
            var dp = new DialogParameters
            {
                Closed = async (s, e) =>
                {
                    if (e.DialogResult.HasValue && e.DialogResult.Value)
                    {
                        await new BalancesServiceProxy().DeleteGasOwnerAsync(SelectedOwner.Dto.Id);
                        Refresh();
                    }
                },
                Content = "Вы уверены что хотите удалить поставщика?",
                Header = "Удаление поставщика",
                OkButtonContent = "Да",
                CancelButtonContent = "Нет"
            };

            RadWindow.Confirm(dp);
        }
        
        

        
        public DelegateCommand UpCommand { get; }

        private async void Up()
        {
            await new BalancesServiceProxy().SetGasOwnerSortOrderAsync(
                new SetGasOwnerSortOrderParameterSet
                {
                    GasOwnerId = SelectedOwner.Dto.Id,
                    UpDown = UpOrDownSortOrder.Up
                });

            var owner =
                OwnerList.Where(p => p.SortOrder < SelectedOwner.Dto.SortOrder)
                    .OrderByDescending(p => p.SortOrder)
                    .FirstOrDefault();

            ChangeSortOrder(owner);
        }

        public DelegateCommand DownCommand { get; }

        private async void Down()
        {
            await new BalancesServiceProxy().SetGasOwnerSortOrderAsync(
                new SetGasOwnerSortOrderParameterSet
                {
                    GasOwnerId = SelectedOwner.Dto.Id,
                    UpDown = UpOrDownSortOrder.Down
                });

            var owner =
                OwnerList.Where(p => p.SortOrder > SelectedOwner.Dto.SortOrder)
                    .OrderBy(p => p.SortOrder)
                    .FirstOrDefault();

            ChangeSortOrder(owner);
        }

        private void ChangeSortOrder(GasOwnerItem owner)
        {
            if (owner == null) return;

            var buf = SelectedOwner.SortOrder;
            SelectedOwner.SortOrder = owner.SortOrder;
            owner.SortOrder = buf;
            buf = SelectedOwner.Dto.Id;
            var tmpLst = OwnerList.OrderBy(p => p.SortOrder).ToList();
            OwnerList = tmpLst;
            OnPropertyChanged(() => OwnerList);
            SelectedOwner = OwnerList.FirstOrDefault(p => p.Dto.Id == buf);
            
        }



        private void RefreshCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
            UpCommand.RaiseCanExecuteChanged();
            DownCommand.RaiseCanExecuteChanged();
            
        }
    }

    public class GasOwnerItem : PropertyChangedBase
    {
        public GasOwnerItem(GasOwnerDTO dto)
        {
            var clienCache = ServiceLocator.Current.GetInstance<IClientCache>();
            Dto = dto;
            SystemList =
                clienCache.DictionaryRepository.GasTransportSystems.Where(s => Dto.SystemList.Contains(s.Id))
                    .ToList();
        }

        
        public int SortOrder
        {
            get { return Dto.SortOrder; }
            set { Dto.SortOrder = value; }
        }

        public GasOwnerDTO Dto { get; set; }

        public List<GasTransportSystemDTO> SystemList { get; }

        public bool IsActive => SystemList.Any();
    }


    public class GasOwnerRowSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var obj = (GasOwnerItem)item;

            return obj.IsActive ? ActiveStyle : NonActiveStyle;
        }

        public Style ActiveStyle { get; set; }

        public Style NonActiveStyle { get; set; }
        
    }
}
