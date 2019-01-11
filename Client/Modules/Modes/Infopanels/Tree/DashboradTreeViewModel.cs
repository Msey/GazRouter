using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GazRouter.Application;
using GazRouter.Common.Events;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Dashboards;
using GazRouter.DTO.Dashboards;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.Modes.ExcelReports;
using GazRouter.Modes.ProcessMonitoring.Dashboards.AddEditDashboard;
using GazRouter.Modes.ProcessMonitoring.Dashboards.AddEditFolder;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
namespace GazRouter.Modes.Infopanels.Tree
{
    /// <summary> как будут различаться объекты
    /// 
    /// todo: 1. xItem - добавить интерфейс по работе 
    /// с деревом и привязать к ним действия?
    /// todo: 2. сделать прокси?
    /// 
    /// Объекты дерева:
    /// 1. Папка пользователя
    /// 2. Инфопанель пользователя
    ///     расшаренная у владельца - изменяет цвет имени
    /// 3. Отчет пользователя
    ///     расшаренная у владельца - изменяет цвет имени
    /// -------------------------------------------------
    /// 4. Папка расшаренная 
    ///     запрещено: удалять папку
    ///                создавать в ней новые папки  
    ///                перемещать в нее другие папки
    ///                перемещать в нее другие отчеты не расшаренные
    ///     разрешено: перемещать в другие папки
    /// 5. Отчет расшаренный
    ///     имеет цвет
    /// 6. Инфопанель расшаренная
    ///     имеет цвет
    /// 
    /// 
    /// </summary>
    public class DashboardTreeViewModel : LockableViewModel
    {
#region constructor
        public DashboardTreeViewModel(PanelPermission permission,
                                      Action<ItemBase> dashboardSelectedAction)
        {
            _permission = permission;
            _builder = new DashboardTreeBuilder2(permission, this);
            _dashboardSelectedAction = dashboardSelectedAction;
            _treeStateDictionary = new Dictionary<int, bool>();
            //
            Items = new ObservableCollection<ItemBase>();
            InitCommands();
            Update();
        }
#endregion
#region variables
        private readonly PanelPermission _permission;
        public ObservableCollection<ItemBase> Items { get; set; }

        private readonly DashboardTreeBuilder2 _builder;
        private readonly Action<ItemBase> _dashboardSelectedAction;
        private readonly Dictionary<int, bool> _treeStateDictionary;
        private GridLength _storedTreeWidth;
#endregion
#region property
        private ItemBase _selectedItem;
        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (!SetProperty(ref _selectedItem, value)) return;
                //
                RefreshCommands();
                if (_selectedItem != null)
                    _dashboardSelectedAction?.Invoke(_selectedItem);
            }
        }
        private bool _treeVisibility;
        public bool TreeVisibility
        {
            get { return _treeVisibility; }
            set { SetProperty(ref _treeVisibility, value); }
        }

        private GridLength _treeWidth;
        public GridLength TreeWidth
        {
            get { return _treeWidth; }
            set { SetProperty(ref _treeWidth, value); }
        }
#endregion
#region events
        public void ChangeTreeVisibility(bool visibility)
        {
            TreeVisibility = visibility;
            if (visibility)
            {
                TreeWidth = _storedTreeWidth;
            }
            else
            {
                _storedTreeWidth = TreeWidth;
                TreeWidth = new GridLength(0);
            }
        }
#endregion
#region commands
        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand AddFolderCommand { get; private set; }
        public DelegateCommand AddDashboardCommand { get; private set; }
        public DelegateCommand AddReportCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        public DelegateCommand ShareCommand { get; private set; }
        private void RefreshCommands()
        {
            AddFolderCommand.RaiseCanExecuteChanged();
            AddDashboardCommand.RaiseCanExecuteChanged();
            AddReportCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            CopyCommand.RaiseCanExecuteChanged();
            ShareCommand.RaiseCanExecuteChanged();
        }
        private void InitCommands()
        {
            RefreshCommand = new DelegateCommand(Update, _permission.CanRefreshCommand);
            AddFolderCommand = new DelegateCommand(() =>
            {
                var folderId = (SelectedItem as FolderItem)?.Id;
                var viewModel = new AddEditFolderViewModel(folderId, FolderUpdate);
                var view = new AddEditFolderView { DataContext = viewModel };
                view.ShowDialog();
            }, () => _permission.CanAddCommand(InfopanelItemType.Folder, SelectedItem));
            AddDashboardCommand = new DelegateCommand(() =>
            {
                var folderId = (SelectedItem as FolderItem)?.Id;
                var viewModel = new AddEditDashboardViewModel(folderId, 
                                                              UserProfile.Current.Site.Id, 
                                                              DashboardUpdate);
                var view = new AddEditDashboardView { DataContext = viewModel };
                view.ShowDialog();
            }, () => _permission.CanAddCommand(InfopanelItemType.Dash, SelectedItem));
            AddReportCommand = new DelegateCommand(() =>
            {
                var folderId = (SelectedItem as FolderItem)?.Id;
                DialogHelper.AddReport(folderId, UserProfile.Current.Site.Id, DashboardUpdate);
            },  () => _permission.CanAddCommand(InfopanelItemType.Excel, SelectedItem));
            EditCommand = new DelegateCommand(() => SelectedItem?.Edit(),
                () => _permission.CanEditCommand(SelectedItem));
            DeleteCommand = new DelegateCommand(() => SelectedItem?.Delete(), 
                () => _permission.CanDeleteCommand(SelectedItem)); 
            CopyCommand = new DelegateCommand(() => SelectedItem?.Copy(),
                () => _permission.CanCopyCommand(SelectedItem));
            ShareCommand = new DelegateCommand(() => SelectedItem?.Share(),
                () => _permission.CanShareCommand(SelectedItem));
        }
        #endregion
        #region methods
        public bool IsDragAndDropEnable()
        {
            return _permission.GlobalEditPermission;
        }
        public ItemBase GetItem(int? id)
        {
            var items = new List<ItemBase>();
            _builder.Traversal(bases =>
            {
                if (bases.Id == id && bases.ContentType == InfopanelItemType.Folder)
                    items.Add(bases);
            });
            return items.Single();
        }
        private async Task LoadTreeWrapper()
        {
            Lock();
            SaveTreeState(_builder.RootItem);
            await _builder.Build();
            Items.Clear();
            Items.AddRange(_builder.RootItem.Childs);
            RestoreTreeState(_builder.RootItem);
            Unlock();
        }
        private async void FolderUpdate(int folderId)
        {           
            if (!_permission.IsReportAdminRole())
            {
                await new DashboardServiceProxy()
                    .AddFolderPermissionAsync(new DashboardPermissionParameterSet
                    {
                        Id         = folderId,
                        SiteId     = UserProfile.Current.Site.Id,                        
                        Permission = 2
                    });   
            }
            Update();
        }
        public async void DashboardUpdate(int dashId)
        {
            if (!_permission.IsReportAdminRole())
                await AddDashPermissionTask(dashId);
            Update();
        }
        private static Task AddDashPermissionTask(int dashId)
        {
            return new DashboardServiceProxy()
            .AddDashboardPermissionAsync(new DashboardPermissionParameterSet
            {
                Permission = 2,
                Id = dashId,
                SiteId = UserProfile.Current.Site.Id
            });
        }
        public async void Update()
        {
            await LoadTreeWrapper();
        }
        public void ClearSelection()
        {            
            SelectedItem = null;
        }
        private void SaveTreeState(ItemBase itemBase)
        {
            _treeStateDictionary.Clear();
            if (_builder.RootItem == null) return;
            //
            _builder.Traversal(itemBase, item =>
            {
                if (!(item is FolderItem)) return;
                var folder = (FolderItem)item;
                if (_treeStateDictionary.ContainsKey(folder.Id)) return;
                _treeStateDictionary.Add(folder.Id, folder.IsExpanded);
            });
        }
        private void RestoreTreeState(ItemBase itemBase)
        {
            if (_builder.RootItem == null) return;
            _builder.Traversal(itemBase, item =>
            {
                if (!(item is FolderItem)) return;
                var folder = (FolderItem) item;
                if (!_treeStateDictionary.ContainsKey(folder.Id)) return;
                //
                folder.IsExpanded = _treeStateDictionary[folder.Id];                
            });
        }
        private void Log(string s1, string s2)
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<AddLogEntryEvent>()
                .Publish(new Tuple<string, string>(s1, s2));
        }
#endregion
    }
}
#region trash
// Func<ItemBase, Action> dashboardSelectedAction)
// если пользователь юзер - то добавляем запись в v_dashboards_permissions
#endregion