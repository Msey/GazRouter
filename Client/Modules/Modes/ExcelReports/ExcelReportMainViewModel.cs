using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.Ui.Behaviors;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ExcelReports;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.Infrastructure.Faults;
using GazRouter.DTO.ObjectModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using CollectionExtensions = Microsoft.Practices.Prism.CollectionExtensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using MoveDashboardFolderParameterSet = GazRouter.DTO.ExcelReports.MoveDashboardFolderParameterSet;
//using SetSortOrderParameterSet = GazRouter.DTO.ExcelReports.SetSortOrderParameterSet;
namespace GazRouter.Modes.ExcelReports
{
    public class ExcelReportMainViewModel : LockableViewModel, ITabItem
    {
#region constructor                 
        public ExcelReportMainViewModel()
        {
            IsEditPermission = Authorization.Instance.IsAuthorized(GetType()) == Permissions.Write;
            //
            Items = new ObservableCollection<TreeItemVeiwModelBase>();
#region commands_tree
            RefreshCommand = new DelegateCommand(() => Refresh());
            EditCommand = new DelegateCommand(Edit, () => SelectedItem != null && IsEditPermission);
            ShareCommand = new DelegateCommand(Share, () => SelectedItem is ExcelReportItemViewModel && IsEditPermission);
            DeleteCommand = new DelegateCommand(Delete, ()=> CanDelete() && IsEditPermission);
            CopyCommand = new DelegateCommand(CopyEntity, () => SelectedItem is ExcelReportItemViewModel && 
                                              SelectedItem.Parent != null && IsEditPermission);
            UpSortOrderCommand = new DelegateCommand(UpSetOrder, ()=> CanUpSetOrder() && IsEditPermission);
            DownSortOrderCommand = new DelegateCommand(DownSetOrder, () => CanDownSetOrder() && IsEditPermission);
            TreeViewDragEndedCommand = new Telerik.Windows.Controls.DelegateCommand(DragEnded);
#endregion
#region commands_excel
            UpdateContentCommand = new DelegateCommand(UpdateContent, 
                () => SelectedItem is ExcelReportItemViewModel && Content != null && IsEditMode && IsEditPermission);
#endregion
            // Получение даты последнего сеанса данных 
            KeyDate = SeriesHelper.GetCurrentSession();
            Refresh();
        }
#endregion
        private bool _isEditPermission;
        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }
#region tree
        private TreeItemVeiwModelBase _selectedItem;
        private int? _newItem;
        public ObservableCollection<TreeItemVeiwModelBase> Items { get; }
        public List<MenuItemCommandWithChild> ContextMenuItems
        {
            get
            {
                var res = new List<MenuItemCommandWithChild>();
                if (!(SelectedItem is ExcelReportItemViewModel))
                    res.Add(new MenuItemCommandWithChild(() => {}, "Добавить"){ ToolTip = "Добавить", Children = AddEnititiesCommands });                
                res.Add(new MenuItemCommandWithChild(Edit, "Изменить") { ToolTip = "Изменить" });
                if (SelectedItem is ExcelReportItemViewModel)
                {
                    res.Add(new MenuItemCommandWithChild(Share, "Поделиться с...") { ToolTip = "Поделиться с..." });
                    res.Add(new MenuItemCommandWithChild(CopyEntity, () => SelectedItem?.Parent != null, "Создание копии") { ToolTip = "Создание копии" });
                }
                res.Add(new MenuItemCommandWithChild(Delete, "Удалить") { ToolTip = "Удалить" });
                return res;
            }
        }
        public IEnumerable<MenuItemCommandWithChild> AddEnititiesCommands
        {
            get
            {
                yield return new MenuItemCommandWithChild(AddRootFolder, "Корневая папка") { ToolTip = "Добавить корневую папку" };
                if (SelectedItem == null || SelectedItem is ExcelReportItemViewModel) yield break;
                yield return new MenuItemCommandWithChild(AddFolder, "Папка") { ToolTip = "Добавить папку" };
                yield return new MenuItemCommandWithChild(AddReport, "Отчет");
            }
        }
        private async void MoveFolder(MoveDashboardFolderParameterSet param)
        {
            await new ExcelReportServiceProxy().MoveExcelReportFolderAsync(param);
            Refresh();
        }
        private void DragEnded(object obj)
        {
            var e = (RadTreeViewDragEndedEventArgs)obj;
            var temp = e.TargetDropItem.Item as FolderItemViewModel;
            if (temp != null && e.DraggedItems.Count == 1)
            {
                var dashboard = e.DraggedItems[0] as ExcelReportItemViewModel;
                if (dashboard == null) return;
                var param = new MoveDashboardFolderParameterSet{ DashboardId = dashboard.Id, FolderId = temp.Id };
                if (dashboard.Parent != null)
                {
                    param.OldFolderId = dashboard.Parent.Id;
                    if (param.OldFolderId == param.FolderId) return;
                }
                MoveFolder(param);
            }
        }
        private bool CanUpSetOrder()
        {
            if (SelectedItem == null) return false;
            var items = (SelectedItem.Parent?.Children ?? Items).Where(p => p.GetType() == SelectedItem.GetType()).ToList();
            return items.IndexOf(SelectedItem) > 0;
        }
        private bool CanDownSetOrder()
        {
            if (SelectedItem == null) return false;
            var items = (SelectedItem.Parent?.Children ?? Items).Where(p => p.GetType() == SelectedItem.GetType()).ToList();

            return items.IndexOf(SelectedItem) < items.Count - 1;
        }
        private bool CanDelete()
        {
            return SelectedItem != null && (SelectedItem is ExcelReportItemViewModel || 
                SelectedItem.Children.Count == 0);
        }
        private void Share()
        {
            DialogHelper.ShareReport(SelectedItem.Id);
        }
        private async void CopyEntity()
        {
           var result = await new ExcelReportServiceProxy().AddExcelReportAsync(
           new AddDashboardParameterSet
           {
               FolderId = SelectedItem.Parent.Id,
               DashboardName = "Копия " + SelectedItem.Name,
               PeriodTypeId = ((ExcelReportItemViewModel)SelectedItem).PeriodTypeId,
               SortOrderParam = new DTO.Dashboards.Folders.SetSortOrderParameterSet
               {
                   FolderId = SelectedItem.Parent.Id,
                   IsFolder = false,
                   SortOrder = GetSortOrder()
               }
           });
            CopyCallBack(result, null);
        }
        private void CopyCallBack(int arg1, Exception arg2)
        {
            GetReportContent(SelectedItem.Id, dto =>
            {
                if (_newDash != -1)
                {
                    UpdateReportContent(dto, () =>
                    {
                        Refresh(_newDash);
                        _newDash = -1;
                    });
                }
            });
            _newDash = arg1;
        }
        private async void GetReportContent(int reportId, Action<ExcelReportContentDTO> callback)
        {
            ExcelReportContentDTO dto;
            Behavior.TryLock();

            try
            {
                dto = await new ExcelReportServiceProxy().GetExcelReportContentAsync(reportId);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            callback(dto);
        }
        private async void UpdateReportContent(ExcelReportContentDTO contentDTO, Action callback)
        {
            Behavior.TryLock();
            try
            {
                await new ExcelReportServiceProxy().UpdateExcelReportContentAsync(
                    new ExcelReportContentDTO
                    {
                        ReportId = _newDash,
                        Content = contentDTO.Content
                    });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            callback();
        }
        private void Edit()
        {
            bool isFolder = SelectedItem is FolderItemViewModel;
            if (isFolder)
            {
                var folderItemViewModel = (FolderItemViewModel)SelectedItem;
                DialogHelper.EditFolder(
                    new FolderDTO
                    {
                        Id = folderItemViewModel.Id,
                        ParentId = folderItemViewModel.Parent?.Id,
                        Name = folderItemViewModel.Name
                    }, id => Refresh());
            }
            else
            {
                var itemViewModel = (ExcelReportItemViewModel)SelectedItem;
                DialogHelper.EditReport(
                    new DashboardDTO
                    {
                        Id = itemViewModel.Id,
                        FolderId = itemViewModel.Parent?.Id,
                        DashboardName = itemViewModel.Name
                    }, id => Refresh());
            }
        }
        private void Delete()
        {
            bool isFolder = SelectedItem is FolderItemViewModel;
            RadWindow.Confirm(new DialogParameters
            {
                Content = $"Удалить {(isFolder ? "папку" : "отчет")}?",
                Header = "Удаление объекта",
                CancelButtonContent = "Отмена",
                OkButtonContent = "Удалить",
                Closed = (s, e) =>
                {
                    if (e.DialogResult == true)
                    {
                        if (isFolder)
                            DeleteFolder(SelectedItem.Id);
                        else
                        {
                            DeleteDashboard(SelectedItem.Id);
                        }
                    }
                },
            });

        }
        private async void DeleteDashboard(int id)
        {
            try
            {
                await new ExcelReportServiceProxy().DeleteDashboardAsync(id);
                DeleteCallback(null);
            }
            catch (Exception e)
            {
                DeleteCallback(e);
            }
        }
        private async void DeleteFolder(int id)
        {
            try
            {
                await new ExcelReportServiceProxy().DeleteFolderAsync(id);
                DeleteCallback(null);
            }
            catch (Exception e)
            {
                DeleteCallback(e);
            }
        }
        private void DeleteCallback(Exception ex)
        {
            if (ex == null)
            {
                Refresh(SelectedItem?.Parent?.Id);
                return;
            }

            var faultException = ex as FaultException<FaultDetail>;
            if (faultException != null && faultException.Detail.FaultType == FaultType.IntegrityConstraint)
            {
                RadWindow.Alert(new DialogParameters { Header = "Ошибка", Content = faultException.Detail.Message });
            }
        }
        private async void Refresh(int? newid = null)
        {
            _newItem = newid ?? SelectedItem?.Id;
            Behavior.TryLock();
            try
            {
                var list = await new ExcelReportServiceProxy().GetExcelReportFolderListAsync(UserProfile.Current.Id);
                CreateFoldersTree(list);
            }
            finally
            {
                Behavior.TryUnlock();
                IsBusyLoading = false;
            }
        }
        private async void CreateFoldersTree(IEnumerable<FolderDTO> data)
        {
            Items.Clear();
            var dataDict = data.ToDictionary(c => c.Id, c => new FolderItemViewModel(c));
            foreach (var folder in dataDict.Values.OrderBy(t => t.SortOrder))
            {
                if (folder.ParentId.HasValue)
                {
                    dataDict[folder.ParentId.Value].Children.Add(folder);
                    folder.Parent = dataDict[folder.ParentId.Value];
                    if (folder.Id != _newItem) continue;
                    folder.IsSelected = true;
                    folder.IsExpanded = true;
                    var parent = folder.Parent;
                    while (parent != null)
                    {
                        parent.IsExpanded = true;
                        parent = parent.Parent;

                    }
                    SelectedItem = folder;
                }
            }
            CollectionExtensions.AddRange(Items, dataDict.Values.Where(c => c.Parent == null).OrderBy(t => t.SortOrder));
            List<DashboardDTO> list;
            try
            {
                Behavior.TryLock();
                list = await new ExcelReportServiceProxy().GetExcelReportListAsync(UserProfile.Current.Id);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            foreach (var dash in list.OrderBy(t => t.SortOrder))
            {
                var model = new ExcelReportItemViewModel(dash);
                if (dash.FolderId.HasValue)
                {
                    if (dataDict.ContainsKey(dash.FolderId.Value)) { 
                        dataDict[dash.FolderId.Value].Children.Add(model);
                        model.Parent = dataDict[dash.FolderId.Value];
                    }
                }
                else
                {
                    Items.Add(model);
                }
                if (model.Id != _newItem) continue;
                model.IsSelected = true;
                var parent = model.Parent;
                while (parent != null)
                {
                    parent.IsExpanded = true;
                    parent = parent.Parent;
                }
                SelectedItem = model;
            }
            OnPropertyChanged(() => Items);
        }
        private void AddRootFolder()
        {
            DialogHelper.AddFolder(null, id => Refresh(id), GetSortOrder());
        }
        private void AddReport()
        {
            IsBusyLoading = true;
            DialogHelper.AddReport(SelectedItem.Id, id => Refresh(id), new DTO.Dashboards.Folders.SetSortOrderParameterSet
            {
                FolderId = SelectedItem.Id,
                IsFolder = false,
                SortOrder = GetSortOrder()
            });
            IsBusyLoading = false;
        }
        private void AddFolder()
        {
            DialogHelper.AddFolder(SelectedItem.Id, id => Refresh(id), GetSortOrder());
            IsBusyLoading = false;
        }
        public void SetContent(byte[] value, bool withoutNotify = false)
        {
            if (_content == value) return;
            _content = value;
            if (!withoutNotify)
            {
                OnPropertyChanged(() => Content);
            }
            RaiseCommands();
        }
        private void RaiseCommands()
        {
            DeleteCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            ShareCommand.RaiseCanExecuteChanged();
            CopyCommand.RaiseCanExecuteChanged();
            UpSortOrderCommand.RaiseCanExecuteChanged();
            DownSortOrderCommand.RaiseCanExecuteChanged();
            UpdateContentCommand.RaiseCanExecuteChanged();
        }
#endregion
#region excel
        private int _newDash = -1;
        private async void UpdateContent()
        {
            var itemViewModel = SelectedItem as ExcelReportItemViewModel;
            if (itemViewModel == null) return;
            var reportId = itemViewModel.Id;
            var parameters = new ExcelReportContentDTO
            {
                ReportId = reportId,
                Content = Content
            };
            await new ExcelReportServiceProxy().UpdateExcelReportContentAsync(parameters);
        }
#endregion
#region commands_tree
        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand ShareCommand { get; }
        public DelegateCommand CopyCommand { get; }
        public DelegateCommand UpSortOrderCommand { get; private set; }
        public DelegateCommand DownSortOrderCommand { get; private set; }
#endregion
#region commands_excel
        public DelegateCommand UpdateContentCommand { get; set; }
#endregion
 
        public Action ReportChanged { get; set; }
        public TreeItemVeiwModelBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedItem.IsExpanded = true;
                    _selectedItem.IsSelected = true;
                    Load();
                }
                OnPropertyChanged(() => AddEnititiesCommands);
                OnPropertyChanged(() => SelectedItem);
                OnPropertyChanged(() => ContextMenuItems);
                RaiseCommands();                
            }
        }
        private byte[] _content;
        public byte[] Content => _content;
        public DashboardLayout Layout { get; set; }
        private DateTime? _keyDate;
        public DateTime? KeyDate
        {
            get { return _keyDate; }
            set
            {
                if (_keyDate == value) return;
                _keyDate = value;
                Load();
                OnPropertyChanged(() => KeyDate);
            }
        }
        public Telerik.Windows.Controls.DelegateCommand TreeViewDragEndedCommand { get; private set; }
        public DelegateCommand<int> AlignElementsCommand { get; private set; }
        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (_isEditMode == value) return;
                _isEditMode = value;
                Load();
                OnPropertyChanged(() => IsEditMode);
                RaiseCommands();
            }
        }
        private bool _isEditModeAllowed = true;
        public bool IsEditModeAllowed
        {
            get { return _isEditModeAllowed; }
            set
            {
                _isEditModeAllowed = value;
                OnPropertyChanged(() => IsEditModeAllowed);
            }
        }
        public Visibility DetailGridVisibility => SelectedItem is ExcelReportItemViewModel ? Visibility.Visible : Visibility.Collapsed;
        public int ActiveSheetIndex { get; set; }
        public static readonly DependencyProperty DetailGridVisibleProperty = 
            DependencyProperty.Register("DetailGridVisible", typeof (Visibility), typeof (ExcelReportMainViewModel), new PropertyMetadata(default(Visibility)));
        public static readonly DependencyProperty VisibilityProperty = 
            DependencyProperty.Register("Visibility", typeof (Visibility), typeof (ExcelReportMainViewModel), new PropertyMetadata(default(Visibility)));
        private async void Load()
        {
            if (!(_selectedItem is ExcelReportItemViewModel)) return;
            IsBusyLoading = true;
            Behavior.TryLock();
            try
            {
                ExcelReportContentDTO dto;
                if (!IsEditMode)
                {
                    dto = await
                            new ExcelReportServiceProxy().EvaluateExcelReportAsync(
                                new EvaluateExcelReportContentParameterSet
                                {
                                    KeyDate = (DateTime) KeyDate,
                                    ReportId = SelectedItem.Id
                                });
                    SetContent(dto.Content);
                }
                else
                {
                    dto = await new ExcelReportServiceProxy()
                        .GetExcelReportContentAsync(_selectedItem.Id);
                    SetContent(dto.Content);
                }
            }
            finally
            {
                IsBusyLoading = false;
                Behavior.TryUnlock();
                ReportChanged?.Invoke();
            }
        }
        private void UpSetOrder()
        {
            UpdateOrder(true);
        }
        private void UpdateOrder(bool up)
        {
            //if (SelectedItem.Parent == null) return;
            var source = SelectedItem;
            var fullparent = SelectedItem.Parent;
            var sourceItems = fullparent == null ? Items : fullparent.Children;
            var parent = up
                ? sourceItems
                    .Where(t => t.SortOrder < source.SortOrder)
                    .OrderByDescending(t => t.SortOrder)
                    .FirstOrDefault()
                : sourceItems
                    .Where(t => t.SortOrder > source.SortOrder)
                    .OrderBy(t => t.SortOrder)
                    .FirstOrDefault();

            if (parent == null) return;
            var sortorder = source.SortOrder;
            source.SortOrder = parent.SortOrder;
            parent.SortOrder = sortorder;

            new ExcelReportServiceProxy().SetSortOrderAsync(new DTO.Dashboards.Folders.SetSortOrderParameterSet
            {
                Id = source.Id,
                SortOrder = source.SortOrder,
                IsFolder = source is FolderItemViewModel,
                FolderId = (source as ExcelReportItemViewModel)?.Parent?.Id ?? 0
            });

            new ExcelReportServiceProxy().SetSortOrderAsync(new DTO.Dashboards.Folders.SetSortOrderParameterSet
            {
                Id = parent.Id,
                SortOrder = parent.SortOrder,
                IsFolder = source is FolderItemViewModel,
                FolderId = (source as ExcelReportItemViewModel)?.Parent?.Id ?? 0
            });
            var index = sourceItems.IndexOf(source);
            sourceItems.Remove(parent);
            sourceItems.Insert(index, parent);
            UpSortOrderCommand.RaiseCanExecuteChanged();
            DownSortOrderCommand.RaiseCanExecuteChanged();
        }
        private void DownSetOrder()
        {
            UpdateOrder(false);
        }
        private int GetSortOrder()
        {
            if (SelectedItem == null)
                return 1;

            var children = SelectedItem.Children ?? SelectedItem.Parent?.Children;
            if (children != null && children.Count != 0)
                return children.Max(t => t.SortOrder) + 1;
            return 1;
        }
        public void Activate()
        {
            Refresh();
        }
        public void Deactivate()
        {
        }
        public async Task<CommonEntityDTO> EvaluateStringAsync(string input)
        {
            return await new ExcelReportServiceProxy().EvaluateStringAsync(input);
        }
    }
}