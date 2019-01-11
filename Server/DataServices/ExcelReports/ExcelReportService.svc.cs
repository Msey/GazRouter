using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Dashboards.DashboardGrants;
using GazRouter.DAL.ExcelReport;
using GazRouter.DAL.ExcelReport.Folders;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.ObjectModel;
using GazRouter.Service.Exchange.Lib.Excel;
using DashboardFolderParameterSet = GazRouter.DTO.Dashboards.DashboardFolder.DashboardFolderParameterSet;
using DashboardGrantDTO = GazRouter.DTO.ExcelReports.DashboardGrantDTO;
using DeleteDashboardCommand = GazRouter.DAL.ExcelReport.DeleteDashboardCommand;
using EditDashboardCommand = GazRouter.DAL.ExcelReport.EditDashboardCommand;
using EditDashboardGrantCommand = GazRouter.DAL.ExcelReport.EditDashboardGrantCommand;
using GetDashboardGrantListQuery = GazRouter.DAL.ExcelReport.GetDashboardGrantListQuery;
using SetDashboardForderSortOrderCommand = GazRouter.DAL.ExcelReport.Folders.SetDashboardForderSortOrderCommand;
using UpdateDashboardGrantParameterSet = GazRouter.DTO.ExcelReports.UpdateDashboardGrantParameterSet;
namespace GazRouter.DataServices.ExcelReports
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class ExcelReportService : ServiceBase, IExcelReportService
    {
        public int AddExcelReport(AddDashboardParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                return new AddExcelReportCommand(context).Execute(parameters);
            }
        }
        public int AddExcelWithPermission(AddDashboardPermissionParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var dashId = new AddExcelReportCommand(context).Execute(
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
#region CONTENT
        public ExcelReportContentDTO GetExcelReportContent(int parameters)
        {
            return ExecuteRead<GetExcelReportContentQuery, ExcelReportContentDTO, int>(parameters);
        }
        public void UpdateExcelReportContent(ExcelReportContentDTO parameters)
        {
            ExecuteNonQuery<UpdateExcelReportContentCommand, ExcelReportContentDTO>(parameters);
        }
        #endregion
        #region DASHBOARD
        public void EditDashboard(EditDashboardParameterSet parameters)
        {
            ExecuteNonQuery<EditDashboardCommand, EditDashboardParameterSet>(parameters);
        }
        public void DeleteDashboard(int parameters)
        {
            ExecuteNonQuery<DeleteDashboardCommand, int>(parameters);
        }
        #endregion
        public List<DashboardDTO> GetAllExcelReportList()
        {
            return ExecuteRead<GetAllExcelReportListQuery, List<DashboardDTO>>();
        }
        public List<DashboardDTO> GetExcelReportList(int parameters)
        {
            return ExecuteRead<GetExcelReportListQuery, List<DashboardDTO>, int>(parameters);
        }
        public CommonEntityDTO EvaluateString(string parameters)
        {
            return new ExcelProcessor(DateTime.Now, PeriodType.Twohours).EvaluateString(parameters);
        }
        public ExcelReportContentDTO EvaluateExcelReport(EvaluateExcelReportContentParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var reportId = parameters.ReportId;
                if (!parameters.CellsToChange.Any()) return new ExcelReportContentDTO {ReportId = reportId};
                var values = ExcelHelper.GetTemplateValues(parameters, context);
                return new ExcelReportContentDTO {ChangedCells = values, ReportId = reportId};
            }
        }
        #region FOLDER
        public List<FolderDTO> GetAllExcelReportFolderList()
        {
            return ExecuteRead<GetAllExcelReportFolderListQuery, List<FolderDTO>>();
        }
        public List<FolderDTO> GetExcelReportFolderList(int parameters)
        {
            return ExecuteRead<GetExcelReportFolderListQuery, List<FolderDTO>, int>(parameters);
        }
        public int AddExcelReportFolder(AddFolderParameterSet parameters)
        {
            return ExecuteRead<AddExcelReportFolderCommand, int, AddFolderParameterSet>(parameters);
        }
        public void MoveExcelReportFolder(MoveDashboardFolderParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                if (parameters.OldFolderId.HasValue)
                    new DeleteDashboardFromFolderCommand(context).Execute(new DashboardFolderParameterSet
                    {
                        DashboardId = parameters.DashboardId,
                        FolderId = parameters.OldFolderId.Value
                    });
                new AddDashboardToFolderCommand(context).Execute(new DashboardFolderParameterSet
                {
                    DashboardId = parameters.DashboardId,
                    FolderId = parameters.FolderId
                });
            }
        }
        public void EditFolder(EditFolderParameterSet parameters)
        {
            ExecuteNonQuery<EditFolderCommand, EditFolderParameterSet>(parameters);
        }
        public void DeleteFolder(int parameters)
        {
            ExecuteNonQuery<DeleteFolderCommand, int>(parameters);
        }
        #endregion
        public void SetSortOrder(DTO.Dashboards.Folders.SetSortOrderParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                if (parameters.IsFolder)
                    new SetForderSortOrderCommand(context).Execute(parameters);
                else
                    new SetDashboardForderSortOrderCommand(context).Execute(parameters);
            }
        }
        #region GRANTS
        public List<int> GetExcelReportSharedList(int parameters)
        {
            return ExecuteRead<GetExcelReportSharedListQuery, List<int>, int>(Session.User.Id);
        }
        public List<DashboardDTO> GetExcelReportUsersSharedList(int parameters)
        {
            return ExecuteRead<GetExcelReportUsersSharedListQuery, List<DashboardDTO>, int>(Session.User.Id);
        }
        public List<DashboardDTO> GetExcelReportUserSharedList(int parameters)
        {
            return ExecuteRead<GetExcelReportUserSharedListQuery, List<DashboardDTO>, int>(Session.User.Id);
        }
        public List<DashboardGrantDTO> GetDashboardGrantList(int parameters)
        {
            return ExecuteRead<GetDashboardGrantListQuery, List<DashboardGrantDTO>, int>(parameters);
        }
        public void UpdateDashboardGrant(UpdateDashboardGrantParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                foreach (var dashboardGrantDto in parameters.NewGrants)
                {
                    new ShareDashboardCommand(context).Execute(new ShareDashboardParameterSet
                    {
                        DashboardId = dashboardGrantDto.DashboardId,
                        IsEditable = dashboardGrantDto.IsEdit,
                        IsGrantable = dashboardGrantDto.IsGrantable,
                        UserId = dashboardGrantDto.UserId
                    });
                }
                foreach (var dashboardGrantDto in parameters.UpdateGrants)
                {
                    new EditDashboardGrantCommand(context).Execute(new DashboardGrantParameterSet
                    {
                        DashboardId = dashboardGrantDto.DashboardId,
                        IsEditable = dashboardGrantDto.IsEdit,
                        IsGrantable = dashboardGrantDto.IsGrantable,
                        UserId = dashboardGrantDto.UserId
                    });
                }
            }
        }
        #endregion
        public void TrashDashboard(int parameters)
        {
            using (var context = OpenDbContext())
            {
                var dashboard =
                    new GetExcelReportListQuery(context).Execute(Session.User.Id)
                        .Single(d => d.Id == parameters);

                if (dashboard.FolderId.HasValue)
                    new DAL.Dashboards.DashboardFolder.DeleteDashboardFromFolderCommand(context).Execute(
                        new DashboardFolderParameterSet
                        {
                            DashboardId = dashboard.Id,
                            FolderId = dashboard.FolderId.Value
                        });
                new DeleteDashboardGrantCommand(context).Execute(
                    new DeleteDashboardGrantParameterSet
                    {
                        DashboardId = dashboard.Id,
                        UserId = Session.User.Id
                    });
            }
        }
        public void TrashDashboard2(DashboardFolderParameterSet parameters)
        {
            // 1. проверка наличия удаляемого дашборда
            var dash = GetAllExcelReportList().Single(e => e.Id == parameters.DashboardId);
            using (var context = OpenDbContext())
            {
                // 2. проверка наличия связи папки из которой удаляется
                if (parameters.FolderId.HasValue)
                {
//                    var dashsFolder = new GetAllExcelReportFolderListQuery(context).Execute()
//                                            .Single(e => e.DashboardId == parameters.DashboardId &&
//                                                         e.FolderId == parameters.FolderId);
                    new DeleteDashboardFromFolderCommand(context).Execute(
                        new DashboardFolderParameterSet
                        {
                            DashboardId = parameters.DashboardId,
                            FolderId = parameters.FolderId
                        });
                }
                new DeleteDashboardCommand(context).Execute(dash.Id);
            }
        }
    }
}
#region trash

//new AddDashboardPermissionCommand(context).Execute(new DashboardPermissionParameterSet
//                {
//                    Permission = 2,
//                    Id = dashId,
//                    SiteId = parameters.Site
//                });

//public int AddExcelReport2(DTO.ExcelReports.AddDashboardParameterSet parameters)
//{
//    int id;
//    using (var context = OpenDbContext())
//    {
//        id = new AddExcelReportCommand(context).Execute(parameters);
//        if (parameters.SortOrderParam != null)
//        {
//            parameters.SortOrderParam.Id = id;
//            new SetDashboardForderSortOrderCommand(context).Execute(parameters.SortOrderParam);
//        }
//    }
//    return id;
//
//}
#endregion