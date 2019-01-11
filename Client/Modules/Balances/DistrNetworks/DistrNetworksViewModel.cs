using System.Collections.Generic;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.DistrNetworks;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.DistrNetworks
{
    public class DistrNetworksViewModel : LockableViewModel
    {
        //private int _minSort;
        //private int _maxSort;

        public DistrNetworksViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.DistrNetworks);
            

            RefreshCommand = new DelegateCommand(Refresh);
            AddCommand = new DelegateCommand(Add, () => editPermission);
            EditCommand = new DelegateCommand(Edit, () => _selectedItem != null & editPermission);
            RemoveCommand = new DelegateCommand(Remove, () => _selectedItem != null & editPermission);
            //UpCommand = new DelegateCommand(Up, () => _selectedItem != null && _minSort < _selectedItem.SortOrder & editPermission);
            //DownCommand = new DelegateCommand(Down, () => _selectedItem != null && _maxSort > _selectedItem.SortOrder & editPermission);
            
            Refresh();
        }


        
        public List<DistrNetworkDTO> ItemList { get; set; }
        

        private DistrNetworkDTO _selectedItem;

        public DistrNetworkDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if(SetProperty(ref _selectedItem,  value))
                    RefreshCommands();
            }
        }
        

        public DelegateCommand RefreshCommand { get; private set; }

        
        public async void Refresh()
        {
            Lock();
            ItemList = await new BalancesServiceProxy().GetDistrNetworkListAsync();
            OnPropertyChanged(() => ItemList);

            //_minSort = list.Min((p => p.SortOrder));
            //_maxSort = list.Max((p => p.SortOrder));
            
            Unlock();
        }
        
        
        public DelegateCommand AddCommand { get; }

        private void Add()
        {
            var vm = new AddEditDistrNetworkViewModel(x => Refresh(), 0);
            var v = new AddEditDistrNetworkView { DataContext = vm };
            v.ShowDialog();
        }
        

       public DelegateCommand EditCommand { get; }

        private void Edit()
        {
            var vm = new AddEditDistrNetworkViewModel(x => Refresh(), SelectedItem);
            var v = new AddEditDistrNetworkView { DataContext = vm };
            v.ShowDialog();
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
                        await new BalancesServiceProxy().RemoveDistrNetworkAsync(SelectedItem.Id);
                        Refresh();
                    }
                },
                Content = "Удаляем ГРО. Нужно подтверждение.",
                Header = "Удаление ГРО",
                OkButtonContent = "Удалить",
                CancelButtonContent = "Отмена"
            };

            RadWindow.Confirm(dp);
        }



        #region SORT ORDER

        //public DelegateCommand UpCommand { get; }

        //private async void Up()
        //{
        //    await new BalancesServiceProxy().SetGasOwnerSortOrderAsync(
        //        new SetGasOwnerSortOrderParameterSet
        //        {
        //            GasOwnerId = SelectedOwner.Dto.Id,
        //            UpDown = UpOrDownSortOrder.Up
        //        });

        //    var owner =
        //        OwnerList.Where(p => p.SortOrder < SelectedOwner.Dto.SortOrder)
        //            .OrderByDescending(p => p.SortOrder)
        //            .FirstOrDefault();

        //    ChangeSortOrder(owner);
        //}

        //public DelegateCommand DownCommand { get; }

        //private async void Down()
        //{
        //    await new BalancesServiceProxy().SetGasOwnerSortOrderAsync(
        //        new SetGasOwnerSortOrderParameterSet
        //        {
        //            GasOwnerId = SelectedOwner.Dto.Id,
        //            UpDown = UpOrDownSortOrder.Down
        //        });

        //    var owner =
        //        OwnerList.Where(p => p.SortOrder > SelectedOwner.Dto.SortOrder)
        //            .OrderBy(p => p.SortOrder)
        //            .FirstOrDefault();

        //    ChangeSortOrder(owner);
        //}

        //private void ChangeSortOrder(GasOwnerItem owner)
        //{
        //    if (owner == null) return;

        //    var buf = SelectedOwner.SortOrder;
        //    SelectedOwner.SortOrder = owner.SortOrder;
        //    owner.SortOrder = buf;
        //    buf = SelectedOwner.Dto.Id;
        //    var tmpLst = OwnerList.OrderBy(p => p.SortOrder).ToList();
        //    OwnerList = tmpLst;
        //    OnPropertyChanged(() => OwnerList);
        //    SelectedOwner = OwnerList.FirstOrDefault(p => p.Dto.Id == buf);
            
        //}

        #endregion


        private void RefreshCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
            //UpCommand.RaiseCanExecuteChanged();
            //DownCommand.RaiseCanExecuteChanged();
            
        }
    }

    
}
