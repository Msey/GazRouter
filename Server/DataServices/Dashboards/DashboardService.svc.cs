using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataServices.Authorization;
using GazRouter.DataServices.ExcelReports;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Dashboards.DashboardContent;
using GazRouter.DAL.Dashboards.DashboardGrants;
using GazRouter.DAL.Dashboards.Dashboards;
using GazRouter.DAL.Dashboards.Folders;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Authorization.Role;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.ExcelReport;
using GazRouter.DAL.ExcelReport.Folders;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.Dashboards;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dashboards.DashboardFolder;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Sites;
using AddDashboardToFolderCommand = GazRouter.DAL.Dashboards.DashboardFolder.AddDashboardToFolderCommand;
using DeleteDashboardCommand = GazRouter.DAL.Dashboards.Dashboards.DeleteDashboardCommand;
using DeleteDashboardFromFolderCommand = GazRouter.DAL.Dashboards.DashboardFolder.DeleteDashboardFromFolderCommand;
using DeleteFolderCommand = GazRouter.DAL.Dashboards.Folders.DeleteFolderCommand;
using EditDashboardCommand = GazRouter.DAL.Dashboards.Dashboards.EditDashboardCommand;
using EditDashboardGrantCommand = GazRouter.DAL.Dashboards.DashboardGrants.EditDashboardGrantCommand;
using EditFolderCommand = GazRouter.DAL.Dashboards.Folders.EditFolderCommand;
using GetDashboardGrantListQuery = GazRouter.DAL.Dashboards.DashboardGrants.GetDashboardGrantListQuery;
namespace GazRouter.DataServices.Dashboards
{
    [ErrorHandlerLogger("mainLogger")][Authorization]
	public class DashboardService : ServiceBase, IDashboardService
    {
#region CONTENT
        public DashboardContentDTO GetDashboardContent(int parameters)
		{
            return ExecuteRead<GetDashboardContentQuery, DashboardContentDTO, int>(parameters);
		}
        public void UpdateDashboardContent(DashboardContentDTO parameters)
        {
            ExecuteNonQuery<UpdateDashboardContentCommand, DashboardContentDTO>(parameters);
        }
#endregion
#region FOLDER
        public bool IsFolderContainPanels(int folderId)
        {
            using (var context = OpenDbContext())
            {
                var dashDtos = new GetAllDashboardListQuery(context).Execute().Where(e => e.FolderId == folderId);                                          
                var excelDtos = new GetAllExcelReportListQuery(context).Execute().Where(e => e.FolderId == folderId);
                var dashFoldersDtos = new GetAllFolderListQuery(context).Execute().Where(e => e.ParentId == folderId);              
                var excelFoldersDtos = new GetAllExcelReportFolderListQuery(context).Execute().Where(e => e.ParentId == folderId);  
                // 
                if (dashDtos.Any() || excelDtos.Any() || dashFoldersDtos.Any() || excelFoldersDtos.Any()) return true;
            }
            return false;
        }
        public int AddFolder(AddFolderParameterSet parameters)
		{
			return ExecuteRead<AddFolderCommand, int, AddFolderParameterSet>(parameters);
		}
		public void DeleteFolder(int parameters)
		{
			ExecuteNonQuery<DeleteFolderCommand, int>(parameters);
		}
		public void EditFolder(EditFolderParameterSet parameters)
		{
			ExecuteNonQuery<EditFolderCommand, EditFolderParameterSet>(parameters);
		}
        public List<FolderDTO> GetFolderList()
        {
            return ExecuteRead<GetFolderListQuery, List<FolderDTO>, int>(Session.User.Id);
        }
#endregion
#region GRANTS
#region folders
        public List<DashboardGrantDTO2> GetFolderPermissionList()
        {
            return ExecuteRead<GetFolderPermissionListQuery, List<DashboardGrantDTO2>>();
        }
        public void AddFolderPermission(DashboardPermissionParameterSet parameters)
        {
            ExecuteNonQuery<AddFolderPermissionCommand, DashboardPermissionParameterSet>(parameters);
        }
        public void UpdateFolderPermissions(UpdateDashboardPermissionsParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var allPermissions = new GetFolderPermissionListQuery(context).Execute();
                var dashboardPermissions = allPermissions.Where(e => e.ItemId == parameters.DashboardId).ToArray();
                var dashboardPermissionsRoleIds = dashboardPermissions.Select(e => e.SiteId);
                parameters.ChangedPermissions.ForEach(permission =>
                {
                    // удаление запрещающего просмотр
                    if (permission.Permission == 0)
                    {
                        if (dashboardPermissionsRoleIds.Contains(permission.SiteId))
                        {
                            new DeleteFolderPermissionCommand(context).Execute(
                            new DeleteDashboardPermissionParameterSet
                            {
                                Id = parameters.DashboardId,
                                SiteId = permission.SiteId
                            });
                        }
                    }
                    else
                    {
                        // редактирование существующих
                        if (dashboardPermissions.Any(e => e.SiteId == permission.SiteId))
                        {
                            new EditFolderPermissionCommand(context).Execute(
                                new DashboardPermissionParameterSet
                                {
                                    Id = parameters.DashboardId,
                                    SiteId = permission.SiteId,
                                    Permission = permission.Permission,
                                });
                        }
                        else
                        {
                            // добавление новых
                            new AddFolderPermissionCommand(context).Execute(
                                new DashboardPermissionParameterSet
                                {
                                    Id = parameters.DashboardId,
                                    SiteId = permission.SiteId,
                                    Permission = permission.Permission,
                                });
                        }
                    }
                });
            }
        }
#endregion
        public List<DashboardGrantDTO2> GetDashboardPermissionList()
        {
            return ExecuteRead<GetDashboardPermissionListQuery, List<DashboardGrantDTO2>>();
        }
        /// <summary></summary>
        /// <param name="parameters"></param>
        public void UpdateDashboardPermissions(UpdateDashboardPermissionsParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var allPermissions = new GetDashboardPermissionListQuery(context).Execute();
                var dashboardPermissions = allPermissions.Where(e=>e.ItemId == parameters.DashboardId).ToArray();
                var dashboardPermissionsRoleIds = dashboardPermissions.Select(e=>e.SiteId);
                parameters.ChangedPermissions.ForEach(permission =>
                {
                    // удаление запрещающего просмотр
                    if (permission.Permission == 0)
                    {
                        if (dashboardPermissionsRoleIds.Contains(permission.SiteId))
                        {
                            new DeleteDashboardPermissionCommand(context).Execute(
                            new DeleteDashboardPermissionParameterSet
                            {
                                Id = parameters.DashboardId,
                                SiteId      = permission.SiteId
                            });
                        }
                    } else {
                        // редактирование существующих
                        if (dashboardPermissions.Any(e => e.SiteId == permission.SiteId)) 
                        {
                            new EditDashboardPermissionCommand(context).Execute(
                                new DashboardPermissionParameterSet
                                {
                                    Id = parameters.DashboardId,
                                    SiteId = permission.SiteId,
                                    Permission  = permission.Permission,
                                });
                        }
                        else {
                            // добавление новых
                            new AddDashboardPermissionCommand(context).Execute(
                                new DashboardPermissionParameterSet
                                {
                                    Id = parameters.DashboardId,
                                    SiteId = permission.SiteId,
                                    Permission  = permission.Permission,
                                });
                        }
                    }
                });
            }
        }
        public List<DashboardDTO> GetDashboardUsersSharedList(int parameters)
        {
            return ExecuteRead<GetDashboardUsersSharedListQuery, List<DashboardDTO>, int>(Session.User.Id);
        }
        public List<DashboardDTO> GetDashboardUserSharedList(int parameters)
        {
            return ExecuteRead<GetDashboardUserSharedListQuery, List<DashboardDTO>, int>(Session.User.Id);
        }
        public List<int> GetDashboardSharedList(int parameters)
        {
            return ExecuteRead<GetDashboardSharedListQuery, List<int>, int>(Session.User.Id);
        }
        public List<DashboardGrantDTO> GetDashboardGrantList(int parameters)
		{
			return ExecuteRead<GetDashboardGrantListQuery, List<DashboardGrantDTO>, int>(parameters);
		}
        public void UpdateDashboardGrant(UpdateDashboardGrantParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var currentGrants = new GetDashboardGrantListQuery(context).Execute(parameters.DashboardId).Where(g => g.IsVisible);

                foreach (var grant in parameters.ChangedGrants)
                {
                    // Удаление гранта
                    if (!grant.IsVisible)
                    {
                        new DeleteDashboardGrantCommand(context).Execute(
                            new DeleteDashboardGrantParameterSet
                            {
                                DashboardId = parameters.DashboardId,
                                UserId = grant.UserId
                            });
                        continue;
                    }

                    // Добавление новых грантов
                    if (currentGrants.All(g => g.UserId != grant.UserId))
                    {
                        new AddDashboardGrantCommand(context).Execute(
                            new ShareDashboardParameterSet
                            {
                                DashboardId = parameters.DashboardId,
                                UserId = grant.UserId,
                                IsEditable = grant.IsEditable,
                                IsGrantable = grant.IsGrantable
                            });
                        continue;
                    }

                    new EditDashboardGrantCommand(context).Execute(
                        new DashboardGrantParameterSet
                        {
                            DashboardId = parameters.DashboardId,
                            UserId = grant.UserId,
                            IsEditable = grant.IsEditable,
                            IsGrantable = grant.IsGrantable
                        });
                }
            }
        }
#endregion
#region DASHBOARD
        public int AddDashboard(AddDashboardParameterSet parameters)
		{
			return ExecuteRead<AddDashboardCommand, int, AddDashboardParameterSet>(parameters);
		}
        /// <summary>
        /// 
        /// 1. добавление пермишина
        /// 2. добавление в папку
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int AddDashboardWithPermission(AddDashboardPermissionParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var dashId = new AddDashboardCommand(context).Execute(
                    new AddDashboardParameterSet
                    {
                        SortOrder = parameters.SortOrder,
                        PeriodTypeId = parameters.PeriodTypeId,
                        DashboardName = parameters.DashboardName,
                        FolderId = parameters.FolderId
                    });                
                return dashId;
            }
        }
        public void AddDashboardPermission(DashboardPermissionParameterSet parameters)
        {
            ExecuteNonQuery<AddDashboardPermissionCommand, DashboardPermissionParameterSet>(parameters);
        }
        public void DeleteDashboard(int parameters)
		{
			ExecuteNonQuery<DeleteDashboardCommand, int>(parameters);
		}
        public void SoftDeleteDashboard(EditDashboardParameterSet parameters)
        {
            ExecuteNonQuery<EditDashboardCommand, EditDashboardParameterSet>(parameters);
        }
        public void EditDashboard(EditDashboardParameterSet parameters)
		{
			ExecuteNonQuery<EditDashboardCommand, EditDashboardParameterSet>(parameters);
		}
		public List<DashboardDTO> GetDashboardList()
		{
			return ExecuteRead<GetDashboardListQuery, List<DashboardDTO>, int>(Session.User.Id);
		}
        public List<DashboardDTO> GetAllDashboardList()
        {
            return ExecuteRead<GetAllDashboardListQuery, List<DashboardDTO>>();
        }
#endregion
#region NEW
        public List<SiteDTO> GetEnterpriseSites()
        {
            var parameters = new GetSiteListParameterSet { EnterpriseId = AppSettingsManager.CurrentEnterpriseId };
            return ExecuteRead<GetSiteListQuery, List<SiteDTO>, GetSiteListParameterSet>(parameters); 
        }
        public DashboardDataDTO GetDashboardData(DashboardDataParameterSets parameters)
        {
            using (var context = OpenDbContext())
            {
                var dashPermissions   = new GetDashboardPermissionListQuery(context).Execute();                                    /* new DashboardService().GetDashboardPermissionList()*/
                var folderPermissions = new GetFolderPermissionListQuery(context).Execute();                                       /* new DashboardService().GetFolderPermissionList();*/
                var userRoleIds = new GetRolesByUserIdQuery(context)
                                        .Execute(parameters.UserId)
                                        .Select(e => e.Id).ToArray();
                var dashFoldersDtos  = new GetAllFolderListQuery(context).Execute();                                              //DashboardService().GetAllFolderList();
                var excelFoldersDtos = new GetAllExcelReportFolderListQuery(context).Execute();                                   //ExcelReportService().GetAllExcelReportFolderList();
                var dashDtos         = new GetAllDashboardListQuery(context).Execute();                                           // 
                var excelDtos        = new GetAllExcelReportListQuery(context).Execute();                                         //ExcelReportService().GetAllExcelReportList();
                // 
                var foldersUnionDtos = dashFoldersDtos.Union(excelFoldersDtos, new FolderDtoComparer());
                // 
                var panelIdMaxPermission       = new Dictionary<int, int>();
                var folderPanelIdMaxPermission = new Dictionary<int, int>();
                if (parameters.Filter)
                {
#region dashAndFolders
                    var folderIdPermissions = folderPermissions.Where(e => e.SiteId == parameters.SiteId).ToDictionary(e => e.ItemId, k => k.Permission);
                    var panelIdPermission   = dashPermissions.Where(e => e.SiteId == parameters.SiteId).ToDictionary(e => e.ItemId, k => k.Permission);

                    foldersUnionDtos = foldersUnionDtos.Where(e => folderIdPermissions.ContainsKey(e.Id)).ToList();//
                    var folderDictDtos = foldersUnionDtos.Select(e => e.Id);
                    var hashSet = new HashSet<int>(folderDictDtos);
                    dashDtos = dashDtos.Where(e => panelIdPermission.ContainsKey(e.Id) &&
                                                   (e.FolderId != null && hashSet.Contains((int)e.FolderId) ||
                                                   e.FolderId == null)).ToList();
                    excelDtos = excelDtos.Where(e => panelIdPermission.ContainsKey(e.Id) &&
                                                  (e.FolderId != null && hashSet.Contains((int)e.FolderId) ||
                                                    e.FolderId == null)).ToList();
                    panelIdMaxPermission       = panelIdPermission;
                    folderPanelIdMaxPermission = folderIdPermissions;
#endregion
                }
                var foldersUnionDtosList = foldersUnionDtos.ToList();
                var data = new DashboardDataDTO
                {
                    UserRoleIds = userRoleIds,
                    DashDtos = dashDtos,
                    ExcelDtos = excelDtos,
                    MaxRolesPermissions = panelIdMaxPermission,
                    FolderPanelIdMaxPermission = folderPanelIdMaxPermission,
                    FolderUnionDtos = foldersUnionDtosList.ToList()
                }; 
                return data;
            }
        }
        public void SetSortOrder(List<DashSortOrderDTO> orders)
        {
            using (var context = OpenDbContext())
            {
                var dashFoldersDtos  = new GetAllFolderListQuery(context).Execute();  
                var excelFoldersDtos = new GetAllExcelReportFolderListQuery(context).Execute();
                var dashsDtos        = new GetAllDashboardListQuery(context).Execute();
                var excelsDtos       = new GetAllExcelReportListQuery(context).Execute();
                var foldersUnion     = dashFoldersDtos.Concat(excelFoldersDtos).ToDictionary(k => k.Id);
                var dashsUnion       = dashsDtos.Concat(excelsDtos).ToDictionary(k => k.Id);
                orders.ForEach(item =>
                {
                    switch (item.Type)
                    {
                        case InfopanelItemType.Folder:
                            if (foldersUnion.ContainsKey(item.Id))
                            {
                                var folderDto = foldersUnion[item.Id];
                                new EditFolderCommand(context).Execute(new EditFolderParameterSet
                                {
                                    FolderId  = folderDto.Id,
                                    Name      = folderDto.Name,
                                    ParentId  = folderDto.ParentId,
                                    SortOrder = item.SortOrder
                                });
                            }
                            break;
                        case InfopanelItemType.Excel:
                        case InfopanelItemType.Dash:
                            if (dashsUnion.ContainsKey(item.Id))
                            {
                                var dashsDto = dashsUnion[item.Id];
                                new EditDashboardCommand(context).Execute(new EditDashboardParameterSet
                                {
                                    DashboardId = dashsDto.Id,
                                    FolderId = dashsDto.FolderId,
                                    DashboardName = dashsDto.DashboardName,
                                    PeriodTypeId = dashsDto.PeriodTypeId,
                                    SortOrder = item.SortOrder
                                });
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
        }
        public void MoveDashboard(DashboardFolderParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var dashboard = new GetDashboardListQuery(context).Execute(Session.User.Id).Single(d => d.Id == parameters.DashboardId);
                if (dashboard.FolderId.HasValue)
                    new DeleteDashboardFromFolderCommand(context).Execute(new DashboardFolderParameterSet
                    {
                        DashboardId = dashboard.Id, FolderId = dashboard.FolderId.Value
                    });
                new AddDashboardToFolderCommand(context).Execute(new DashboardFolderParameterSet
                {
                    DashboardId = dashboard.Id, FolderId = parameters.FolderId
                });
            }
        }
        public void MoveSharedDashboard(DashboardFolderGenericParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                if (parameters.CurrentFolderId.HasValue)
                    new DeleteDashboardFromFolderCommand(context).Execute(new DashboardFolderParameterSet
                    {
                        DashboardId = parameters.DashboardId, FolderId = (int) parameters.CurrentFolderId
                    });
                new AddDashboardToFolderCommand(context).Execute(new DashboardFolderParameterSet
                {
                    DashboardId = parameters.DashboardId, FolderId = parameters.TargetFolderId
                });
            }
        }
        public void TrashDashboard(int parameters)
        {
            using (var context = OpenDbContext())
            {
                var dashboard = new GetAllDashboardListQuery(context).Execute().First(d => d.Id == parameters); // Single
                if (dashboard.FolderId.HasValue)
                    new DeleteDashboardFromFolderCommand(context).Execute(new DashboardFolderParameterSet
                    {
                        DashboardId = dashboard.Id, FolderId = dashboard.FolderId.Value
                    });
                new DeleteDashboardCommand(context).Execute(parameters);
            }
        }
        /// <summary> todo: => сделать такой метод и в ReportService.svc </summary>
        /// <param name="parameters"> </param>
        public void TrashDashboard2(DashboardFolderParameterSet parameters)
        {
            // 1. проверка наличия удаляемого дашборда
//            var dash = GetAllDashboardList().Single(e => e.Id == parameters.DashboardId);
            using (var context = OpenDbContext())
            {
                // 2. проверка наличия связи папки из которой удаляется
                if (parameters.FolderId.HasValue)
                {
//                    var dashsFolder = new GetDashboardFolderListQuery(context).Execute()
//                                            .Single(e => e.DashboardId == parameters.DashboardId && 
//                                                         e.FolderId == parameters.FolderId);
                    new DeleteDashboardFromFolderCommand(context).Execute(new DashboardFolderParameterSet
                    {
                        DashboardId = parameters.DashboardId, FolderId = parameters.FolderId
                    });
                }
                new DeleteDashboardCommand(context).Execute(parameters.DashboardId);
            }
        }
        // DashboardFolderParameterSet
        //                    public void TrashDashboard(int parameters)
        //        {
        //            using (var context = OpenDbContext())
        //            {
        //                var dashboard =
        //                    new GetDashboardListQuery(context).Execute(Session.User.Id)
        //                        .Single(d => d.Id == parameters);
        //
        //                if (dashboard.FolderId.HasValue)
        //                    new DAL.Dashboards.DashboardFolder.DeleteDashboardFromFolderCommand(context).Execute(
        //                        new DashboardFolderParameterSet
        //                        {
        //                            DashboardId = dashboard.Id,
        //                            FolderId = dashboard.FolderId.Value
        //                        });
        //                new DeleteDashboardGrantCommand(context).Execute(
        //                    new DeleteDashboardGrantParameterSet
        //                    {
        //                        DashboardId = dashboard.Id,
        //                        UserId = Session.User.Id
        //                    });
        //            }
        //        }
        public void AddDashboardToFolder(DashboardFolderParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                new AddDashboardToFolderCommand(context).Execute(new DashboardFolderParameterSet
                {
                    DashboardId = parameters.DashboardId, FolderId = parameters.FolderId
                });
            }
        }
        public List<FolderDTO> GetAllFolderList()
        {
            return ExecuteRead<GetAllFolderListQuery, List<FolderDTO>>();
        }
        public List<DashboardFoldersDTO> GetDashboardFoldersList()
        {
            return ExecuteRead<GetDashboardFoldersQuery, List<DashboardFoldersDTO>, int>(Session.User.Id);
        }
#endregion
    }
    public class FolderDtoComparer : IEqualityComparer<FolderDTO>
    {
        public bool Equals(FolderDTO x, FolderDTO y)
        {
            return x.Id == y.Id;
        }
        public int GetHashCode(FolderDTO obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
#region trash
// var userRoleIds = new UserManagementService().GetUserRoles(parameters.UserId).Select(e => e.Id).ToArray();
// var enterpriseLpus   = sites.Select(e => e.Id).ToArray();

//                var sites = new GetSiteListQuery(context).Execute(new GetSiteListParameterSet
//                {
//                    EnterpriseId = AppSettingsManager.CurrentEnterpriseId
//                });
//              var userSite = sites.First(e => e.Id == parameters.SiteId);

//                    Sites = sites


//                    var rolesPermissions = dashPermissions.ToLookup(e => e.SiteId)
//                                                          .Where(e => enterpriseLpus.Contains(e.Key));

//                    panelIdMaxPermission = rolesPermissions.SelectMany(e => e)
//                                                           .ToLookup(e => e.ItemId)
//                                                           .ToDictionary(k => k.Key, v => v.Max(e => e.Permission));


//                    folderPanelIdMaxPermission = folderRolesPermissions
//                        .SelectMany(e => e)
//                        .ToLookup(e => e.ItemId)
//                        .ToDictionary(k => k.Key, v => v.Max(e => e.Permission));


//        public DashboardDataDTO GetDashboardDataFilter(int userId)
//        {
//            var permissions = new DashboardService().GetDashboardPermissionList();
//            var userRoleIds = new UserManagementService().GetUserRoles(userId).Select(e => e.Id).ToArray();
//            // 
//            var rolesPermissions = permissions.ToLookup(e => e.RoleId).Where(e => userRoleIds.Contains(e.Key));
//            var panelIdMaxPermission = rolesPermissions.SelectMany(e => e)
//                                                       .ToLookup(e => e.PanelId)
//                                                       .ToDictionary(k => k.Key, v => v.Max(e => e.Permission));
//
//            var data = new DashboardDataDTO
//            {
//                UserRoleIds = userRoleIds,
//                DashFoldersDtos = new DashboardService().GetAllFolderList(),
//                ExcelFoldersDtos = new ExcelReportService().GetAllExcelReportFolderList(),
//                // 
//                DashDtos = new DashboardService().GetAllDashboardList().Where(e => panelIdMaxPermission[e.Id] > 1).ToList(),
//                ExcelDtos = new ExcelReportService().GetAllExcelReportList().Where(e => panelIdMaxPermission[e.Id] > 1).ToList(),
//                MaxRolesPermissions = panelIdMaxPermission
//            };
//            return data;
//        }

// 
//                Permissions = permissions,
//                Permissions      = permissions,

//            RoleIds          = new UserManagementService().GetRoles().Select(e => e.Id).ToArray(),
//            RoleIds = new UserManagementService().GetRoles().Select(e => e.Id).ToArray(),

//            var roleIds = (await new UserManagementServiceProxy().GetRolesAsync()).Select(e => e.Id).ToArray(); // v_roles
//            var userRoleIds = (await new UserManagementServiceProxy().GetUserRolesAsync(UserProfile.Current.Id)).Select(e => e.Id).ToArray();// v_roles
//            var dashFoldersDtos = await new DashboardServiceProxy().GetAllFolderListAsync();                    // v_folders 
//            var excelFoldersDtos = await new ExcelReportServiceProxy().GetAllExcelReportFolderListAsync();      // v_folder_reports
//            var dashDtos = await new DashboardServiceProxy().GetAllDashboardListAsync();                        // v_dashboards
//            var excelDtos = await new ExcelReportServiceProxy().GetAllExcelReportListAsync();                   // v_dashboard_reports
//            var permissions = await new DashboardServiceProxy().GetDashboardPermissionListAsync();              // v_dashboards_permissions 
#endregion

