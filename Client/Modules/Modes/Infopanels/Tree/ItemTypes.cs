using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Dashboards;
using GazRouter.DataProviders.ExcelReports;
using GazRouter.DTO.Dashboards;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Modes.ExcelReports;
using GazRouter.Modes.Infopanels.ShareDashboard;
using GazRouter.Modes.ProcessMonitoring.Dashboards.AddEditDashboard;
using GazRouter.Modes.ProcessMonitoring.Dashboards.AddEditFolder;
using Telerik.Windows.Controls;
using ShareDashboardView2 = GazRouter.Modes.Infopanels.ShareDashboard.ShareDashboardView2;

namespace GazRouter.Modes.Infopanels.Tree
{
#region typres
    public enum DashPermissionType
    {
        Denied = 0,
        Read   = 1,
        Edit   = 2,
    }
    public interface IItemTreeActions
    {
        void Delete();
        void Edit();
        void Share();
        void Copy();
        void Drop(object target, DropPosition dropPosition);
    }
    #endregion
#region items
    /// <summary>
    /// _________________________________
    /// action         | Clear selection
    /// _______________|_________________
    /// update         | 1
    /// delete folder  | 1
    /// insert dash    | 0
    /// insert report  | 0
    /// delete         | 1
    /// edit           | 0
    /// copy           | 0
    /// share          | 0
    /// 
    /// </summary>
    public class ItemBase : PropertyChangedBase, IItemTreeActions
    {
#region constructor

        public ItemBase()
        {
            Childs            = new ObservableCollection<ItemBase>();
            Permission        = DashPermissionType.Denied;
            IsChanged = () => false;
        }
#endregion
#region variables
        private DashPermissionType _permission;
        public DashPermissionType Permission
        {
            get { return _permission; }
            set { SetProperty(ref _permission, value); }
        }
        public InfopanelItemType ContentType { get; set; }
        public virtual ImageSource Image { get; }
        public virtual int Id { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual string Name { get; set; }
        public virtual int? SortOrder { get; set; }
        public DashboardTreeViewModel Tree { get; set; }

        public ObservableCollection<ItemBase> Childs { get; set; }
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged(() => IsExpanded);
            }
        }
        public Func<bool> IsChanged { get; set; }
        public Func<Task> Save { get; set; }
#endregion
#region virtual
        public virtual void Add(int? folderId, Action action) { }
        public virtual void Delete(){}
        public virtual void Edit(){}
        public virtual Task EditItem(int? parentId)
        {
            return new Task(() => {});
        }
        public virtual void Share(){}
        public virtual void Copy(){}
        /// <summary> drop </summary>
        /// <param name="target"> FolderItem </param>
        /// <param name="dropPosition"></param>
        public async void Drop(object target, DropPosition dropPosition)
        {
            var targetItem = target as ItemBase;
            if (targetItem == null) return;
            //
            if (dropPosition != DropPosition.Inside)
            {
                targetItem = targetItem.ParentId == null ?
                    null :
                    Tree.GetItem((int)targetItem.ParentId);
            }
            await EditItem(targetItem?.Id); 
            await SetSortOrder(targetItem);
        }
        protected async Task SetSortOrder(ItemBase targetItem)
        {
            var list = targetItem == null ?
                Tree.Items.Select((e, i) => new DashSortOrderDTO
                            {
                                Id = e.Id,
                                Type = e.ContentType,
                                SortOrder = i
                            }) :
                targetItem.Childs.Select((e, i) => new DashSortOrderDTO
                            {
                                Id = e.Id,
                                Type = e.ContentType,
                                SortOrder = i
                            });
            await new DashboardServiceProxy().SetSortOrderAsync(list.ToList());
        }
#endregion
    }
    public class ItemContent : ItemBase
    {
        public override string Name => Dto.DashboardName;
        public override int? ParentId => Dto.FolderId;
        public override int Id => Dto.Id;
        public override int? SortOrder => Dto.SortOrder;

        public DashboardDTO Dto { get; set; }
        public override async Task EditItem(int? parentId)
        {
            await new DashboardServiceProxy()
                .EditDashboardAsync(new EditDashboardParameterSet
                {
                    DashboardId = Id,
                    FolderId = parentId,
                    DashboardName = Dto.DashboardName,
                    PeriodTypeId = Dto.PeriodTypeId,
                });
        }
        public override void Share()
        {
            var viewModel = new ShareDashboardViewModel2(new ShareDashboard(Id), id => {
                Tree.Update();
            });
            var view = new ShareDashboardView2 { DataContext = viewModel };
            view.ShowDialog();
        }
    }
    public class FolderItem : ItemBase
    {
        public FolderItem(FolderDTO dto)
        {
            Dto = dto;
        }
#region variables
        public FolderDTO Dto { get; set; }
        public override string Name => Dto.Name;
        public override int Id => Dto.Id;
        public override int? ParentId => Dto.ParentId;
        public override int? SortOrder => Dto.SortOrder;
        public bool IsEmpty => Childs.Count == 0;
        public override ImageSource Image => (ImageSource)
                    new ImageSourceConverter()
                        .ConvertFromString(@"/Common;component/Images/16x16/folder.png");
#endregion
#region overrides     
        public override async void Delete()
        {                 
            var isFolderContain = await new DashboardServiceProxy().IsFolderContainPanelsAsync(Id);
            if (!IsEmpty)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Header = "Удаление невозможно",
                    Content = "Невозможно удалить папку, т.к. папка содержит объекты.",
                    OkButtonContent = "Закрыть"
                });
                return;
            }
            if (isFolderContain)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Header = "Удаление невозможно",
                    Content = "Невозможно удалить папку, т.к. папка содержит скрытые объекты.",
                    OkButtonContent = "Закрыть"
                });
                return;
            }
            RadWindow.Confirm(new DialogParameters
            {
                Header              = "Удаление",
                Content             = $"Удалить {"папку"} '{Name}'?",
                OkButtonContent     = "Удалить",
                CancelButtonContent = "Отмена",
                Closed = async (s, e) =>
                {
                    if (e.DialogResult != true) return;
                    //
                    await new DashboardServiceProxy().DeleteFolderAsync(Id);
                    Tree.Update();
                    Tree.ClearSelection();
                }
            });
        }
        public override void Edit()
        {
            var viewModel = new AddEditFolderViewModel(Dto, id => Tree.Update());
            var view = new AddEditFolderView { DataContext = viewModel };
            view.ShowDialog();
        }
        public override async Task EditItem(int? parentId)
        {
            await new DashboardServiceProxy()
                .EditFolderAsync(new EditFolderParameterSet
                {
                    FolderId = Id,
                    Name = Name,
                    ParentId = parentId,
                    SortOrder = 0
                });
        }
        public override void Share()
        {
            var viewModel = new ShareDashboardViewModel2(new ShareFolder(Id), id => {
                Tree.Update();
            });
            var view = new ShareDashboardView2 { DataContext = viewModel };
            view.ShowDialog();
        }
#endregion
    }
    public class DashboardItem : ItemContent
    {
#region variables
        public override ImageSource Image => (ImageSource)new ImageSourceConverter()
            .ConvertFromString(@"/Common;component/Images/16x16/dashboard2.png");
#endregion
#region overrides        
        public override void Delete()
        {            
            RadWindow.Confirm(new DialogParameters
            {
                Header              = "Удаление",
                Content             = $"Удалить {"инфопанель"} '{Name}'?",
                OkButtonContent     = "Удалить",
                CancelButtonContent = "Отмена",
                Closed = async (s, e) =>
                {
                    if (e.DialogResult != true) return;
                    //
                    await new DashboardServiceProxy().SoftDeleteDashboardAsync(new EditDashboardParameterSet
                    {
                        DashboardId = Id,
                        FolderId = ParentId,
                        PeriodTypeId = Dto.PeriodTypeId,
                        SortOrder = SortOrder,
                        DashboardName = Name,
                        IsDeleted = 1
                    });
                    Tree.ClearSelection();
                    Tree.Update();
                }
            }); 
        }
        public override void Edit()
        {
            var viewModel = new AddEditDashboardViewModel(Dto, id => Tree.Update());
            var view = new AddEditDashboardView { DataContext = viewModel };
            view.ShowDialog();
        }
        public override async void Copy()
        {
            Tree.Lock();
            var newId = await new DashboardServiceProxy().AddDashboardWithPermissionAsync(
                new AddDashboardPermissionParameterSet
                {
                    FolderId = Dto.FolderId,
                    DashboardName = Name + " (копия)",
                    SortOrder = SortOrder,
                    PeriodTypeId = Dto.PeriodTypeId,
                    Site = UserProfile.Current.Site.Id
                });
            var content = await new DashboardServiceProxy().GetDashboardContentAsync(Id);
            await new DashboardServiceProxy().UpdateDashboardContentAsync(
                new DashboardContentDTO
                {
                    DashboardId = newId,
                    Content = content.Content
                });
            // 
            Tree.DashboardUpdate(newId);
            Tree.Unlock();
        }
#endregion
    }
    public class ReportItem : ItemContent
    {
#region variables
        public override ImageSource Image => (ImageSource)new ImageSourceConverter()
            .ConvertFromString(@"/Common;component/Images/16x16/excel.png");
#endregion
#region overrides
        public override void Delete()
        {
            RadWindow.Confirm(new DialogParameters
            {
                Header = "Удаление",
                Content = $"Удалить {"инфопанель"} '{Name}'?",
                OkButtonContent = "Удалить",
                CancelButtonContent = "Отмена",
                Closed = async (s, e) =>
                {
                    if (e.DialogResult != true) return;
                    // 
                    await new DashboardServiceProxy().SoftDeleteDashboardAsync(new EditDashboardParameterSet
                    {
                        DashboardId = Id,
                        FolderId = ParentId,
                        PeriodTypeId = Dto.PeriodTypeId,
                        SortOrder = SortOrder,
                        DashboardName = Name,
                        IsDeleted = 1
                    });
                    Tree.ClearSelection();
                    Tree.Update();
                }
            });
        }
        public override void Edit()
        {            
            DialogHelper.EditReport(new DashboardDTO
            {
                Id = Id,
                FolderId = Dto.FolderId,
                DashboardName = Name, 
                PeriodTypeId = Dto.PeriodTypeId

            }, id => Tree.Update());
        }
        public override async void Copy()
        {            
            Tree.Lock();
            var newId = await new ExcelReportServiceProxy().AddExcelWithPermissionAsync(
                new AddDashboardPermissionParameterSet
                {
                    FolderId = Dto.FolderId,
                    DashboardName = Name + " (копия)",
                    SortOrder = SortOrder,
                    PeriodTypeId = Dto.PeriodTypeId,
                    Site = UserProfile.Current.Site.Id
                });
            var content = await new ExcelReportServiceProxy().GetExcelReportContentAsync(Id);
            await new ExcelReportServiceProxy().UpdateExcelReportContentAsync(
                new ExcelReportContentDTO
                {
                    ReportId = newId,
                    Content = content.Content
                });
            Tree.DashboardUpdate(newId);
            Tree.Unlock();
        }
#endregion
    }
#endregion
#region share
    public abstract class ShareBase
    {
        protected ShareBase(int itemId)
        {
            _itemId = itemId;
        }

        private readonly int _itemId;

        public int GetItemId()
        {
            return _itemId;
        }
        protected static ObservableCollection<RolesPermissionWrapper> GetRoles(IEnumerable<SiteDTO> roleDtos, 
                                                                               IEnumerable<DashboardGrantDTO2> permissions)
        {
            var permissionWrappers = from a in roleDtos
                                     join b in permissions
                  on a.Id equals b.SiteId into gj
                                     from sub in gj.DefaultIfEmpty()
                                     select new RolesPermissionWrapper(a)
                                     {
                                         Permission = RegisterItem.GetPermissionType(sub?.Permission ?? 0)
                                     };
            return new ObservableCollection<RolesPermissionWrapper>(permissionWrappers);
        }

        public abstract Task<ObservableCollection<RolesPermissionWrapper>> GetRolesPermissions();
        public abstract Task Update(ObservableCollection<RolesPermissionWrapper> rolesPermissions);
    }
    public class ShareFolder : ShareBase
    {
        public ShareFolder(int id) : base(id) { }

        public override async Task<ObservableCollection<RolesPermissionWrapper>> GetRolesPermissions()
        {
            var roles = await new DashboardServiceProxy().GetEnterpriseSitesAsync();
            var permissions = await new DashboardServiceProxy().GetFolderPermissionListAsync();
            var dashboardPermissions = permissions.Where(e => e.ItemId == GetItemId());
            return GetRoles(roles, dashboardPermissions);
        }
        public override Task Update(ObservableCollection<RolesPermissionWrapper> rolesPermissions)
        {
            return Task.Factory.StartNew(() =>
            {
                return new DashboardServiceProxy().UpdateFolderPermissionsAsync(
                    new UpdateDashboardPermissionsParameterSet
                    {
                        DashboardId = GetItemId(),
                        ChangedPermissions = rolesPermissions.Select(e => new DashboardGrantDTO2
                        {
                            ItemId = GetItemId(),
                            SiteId = e.SiteId,
                            Permission = (int)e.Permission
                        }).ToList()
                    });
            });
        }
    }
    public class ShareDashboard : ShareBase
    {
        public ShareDashboard(int id) : base(id) { }
        public override async Task<ObservableCollection<RolesPermissionWrapper>> GetRolesPermissions()
        {
            var sites = await new DashboardServiceProxy().GetEnterpriseSitesAsync();
            var permissions = await new DashboardServiceProxy().GetDashboardPermissionListAsync();
            var dashboardPermissions = permissions.Where(e => e.ItemId == GetItemId());
            return GetRoles(sites, dashboardPermissions);
        }
        public override Task Update(ObservableCollection<RolesPermissionWrapper> rolesPermissions)
        {
            return Task.Factory.StartNew(() =>
            {
                return new DashboardServiceProxy().UpdateDashboardPermissionsAsync(
                    new UpdateDashboardPermissionsParameterSet
                    {
                        DashboardId = GetItemId(),
                        ChangedPermissions = rolesPermissions.Select(e => new DashboardGrantDTO2
                        {
                            ItemId = GetItemId(),
                            SiteId = e.SiteId,
                            Permission = (int)e.Permission
                        }).ToList()
                    });
            });
        }
    }
#endregion
}