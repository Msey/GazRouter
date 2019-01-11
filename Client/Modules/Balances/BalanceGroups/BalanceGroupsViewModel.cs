using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.BalanceGroups
{
    public class BalanceGroupsViewModel : LockableViewModel
    {
        private int _minSort;
        private int _maxSort;

        public BalanceGroupsViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.BalanceGroups);

            RefreshCommand = new DelegateCommand(Refresh);
            AddCommand = new DelegateCommand(Add, () => editPermission);
            EditCommand = new DelegateCommand(Edit, () => SelectedGroup != null & editPermission);
            RemoveCommand = new DelegateCommand(Remove, () => SelectedGroup != null & editPermission);
            UpCommand = new DelegateCommand(Up,
                () => SelectedGroup != null && SelectedGroup.SortOrder > _minSort & editPermission);
            DownCommand = new DelegateCommand(Down,
                () => SelectedGroup != null && SelectedGroup.SortOrder < _maxSort & editPermission);

            SelectedSystem = SystemList.First();
        }

        public List<GasTransportSystemDTO> SystemList => ClientCache.DictionaryRepository.GasTransportSystems;

        private GasTransportSystemDTO _selectedSystem;

        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (SetProperty(ref _selectedSystem, value))
                    Refresh();
            }
        }


        public List<BalanceGroupDTO> GroupList { get; set; }


        private BalanceGroupDTO _selectedGroup;

        public BalanceGroupDTO SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if (SetProperty(ref _selectedGroup, value))
                    RefreshCommands();
            }
        }


        public DelegateCommand RefreshCommand { get; private set; }


        public async void Refresh()
        {
            Lock();
            GroupList = await new BalancesServiceProxy().GetBalanceGroupListAsync(_selectedSystem.Id);
            OnPropertyChanged(() => GroupList);

            if (GroupList.Count > 0)
            {
                _minSort = GroupList.Min((g => g.SortOrder));
                _maxSort = GroupList.Max((g => g.SortOrder));
            }
            Unlock();
        }


        public DelegateCommand AddCommand { get; }

        private void Add()
        {
            var vm = new AddEditBalanceGroupViewModel(x => Refresh(), _selectedSystem.Id, _maxSort + 1);
            var v = new AddEditBalanceGroupView {DataContext = vm};
            v.ShowDialog();
        }


        public DelegateCommand EditCommand { get; }

        private void Edit()
        {
            var vm = new AddEditBalanceGroupViewModel(x => Refresh(), SelectedGroup);
            var v = new AddEditBalanceGroupView {DataContext = vm};
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
                        await new BalancesServiceProxy().RemoveBalanceGroupAsync(SelectedGroup.Id);
                        Refresh();
                    }
                },
                Content = "Вы уверены что хотите удалить группу?",
                Header = "Удаление группы",
                OkButtonContent = "Да",
                CancelButtonContent = "Нет"
            };

            RadWindow.Confirm(dp);
        }


        public DelegateCommand UpCommand { get; }

        private void Up()
        {
            ChangeSortOrder(SelectedGroup, SortDirection.Up);
        }

        public DelegateCommand DownCommand { get; }

        private void Down()
        {
            ChangeSortOrder(SelectedGroup, SortDirection.Down);
        }

        private enum SortDirection
        {
            Up = 1,
            Down = 2    
        }

        private async void ChangeSortOrder(BalanceGroupDTO group, SortDirection direction)
        {
            BalanceGroupDTO otherGroup = null;

            switch (direction)
            {
                case SortDirection.Up:
                    otherGroup =
                        GroupList.Where(g => g.SortOrder < SelectedGroup.SortOrder)
                            .OrderByDescending(g => g.SortOrder)
                            .FirstOrDefault();
                    break;
                case SortDirection.Down:
                    otherGroup =
                        GroupList.Where(g => g.SortOrder > SelectedGroup.SortOrder)
                            .OrderBy(g => g.SortOrder)
                            .FirstOrDefault();
                    break;
            }

            if (otherGroup == null) return;

            var buf = otherGroup.SortOrder;
            otherGroup.SortOrder = group.SortOrder;
            group.SortOrder = buf;

            await new BalancesServiceProxy().EditBalanceGroupAsync(
                new EditBalanceGroupParameterSet
                {
                    Id = group.Id,
                    SystemId = group.SystemId,
                    Name = group.Name,
                    SortOrder = group.SortOrder
                });

            await new BalancesServiceProxy().EditBalanceGroupAsync(
                new EditBalanceGroupParameterSet
                {
                    Id = otherGroup.Id,
                    SystemId = otherGroup.SystemId,
                    Name = otherGroup.Name,
                    SortOrder = otherGroup.SortOrder
                });

            GroupList = GroupList.OrderBy(g => g.SortOrder).ToList();
            OnPropertyChanged(() => GroupList);
            SelectedGroup = group;
        }


        private void RefreshCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
            UpCommand.RaiseCanExecuteChanged();
            DownCommand.RaiseCanExecuteChanged();
        }
    }
}