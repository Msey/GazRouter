using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.ExcelReports;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.ExcelReports  
{
    [ServiceContract]
    public interface IExcelReportService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddExcelReport(AddDashboardParameterSet parameters, AsyncCallback callback, object state);
        int EndAddExcelReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddExcelWithPermission(AddDashboardPermissionParameterSet parameters, AsyncCallback callback, object state);
        int EndAddExcelWithPermission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExcelReportContent(int parameters, AsyncCallback callback, object state);
        ExcelReportContentDTO EndGetExcelReportContent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdateExcelReportContent(ExcelReportContentDTO parameters, AsyncCallback callback, object state);
        void EndUpdateExcelReportContent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAllExcelReportList(object parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetAllExcelReportList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExcelReportList(int parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetExcelReportList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEvaluateString(string parameters, AsyncCallback callback, object state);
        GazRouter.DTO.ObjectModel.CommonEntityDTO EndEvaluateString(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEvaluateExcelReport(EvaluateExcelReportContentParameterSet parameters, AsyncCallback callback, object state);
        ExcelReportContentDTO EndEvaluateExcelReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAllExcelReportFolderList(object parameters, AsyncCallback callback, object state);
        List<FolderDTO> EndGetAllExcelReportFolderList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExcelReportFolderList(int parameters, AsyncCallback callback, object state);
        List<FolderDTO> EndGetExcelReportFolderList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddExcelReportFolder(AddFolderParameterSet parameters, AsyncCallback callback, object state);
        int EndAddExcelReportFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginMoveExcelReportFolder(MoveDashboardFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveExcelReportFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditFolder(EditFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndEditFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDashboard(EditDashboardParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteDashboard(int parameters, AsyncCallback callback, object state);
        void EndDeleteDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetSortOrder(SetSortOrderParameterSet parameters, AsyncCallback callback, object state);
        void EndSetSortOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteFolder(int parameters, AsyncCallback callback, object state);
        void EndDeleteFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardGrantList(int parameters, AsyncCallback callback, object state);
        List<DashboardGrantDTO> EndGetDashboardGrantList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdateDashboardGrant(UpdateDashboardGrantParameterSet parameters, AsyncCallback callback, object state);
        void EndUpdateDashboardGrant(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTrashDashboard(int parameters, AsyncCallback callback, object state);
        void EndTrashDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExcelReportSharedList(int parameters, AsyncCallback callback, object state);
        List<int> EndGetExcelReportSharedList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExcelReportUsersSharedList(int parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetExcelReportUsersSharedList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetExcelReportUserSharedList(int parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetExcelReportUserSharedList(IAsyncResult result);
    }

	public interface IExcelReportServiceProxy
	{

        Task<int> AddExcelReportAsync(AddDashboardParameterSet parameters);

        Task<int> AddExcelWithPermissionAsync(AddDashboardPermissionParameterSet parameters);

        Task<ExcelReportContentDTO> GetExcelReportContentAsync(int parameters);

        Task UpdateExcelReportContentAsync(ExcelReportContentDTO parameters);

        Task<List<DashboardDTO>> GetAllExcelReportListAsync();

        Task<List<DashboardDTO>> GetExcelReportListAsync(int parameters);

        Task<GazRouter.DTO.ObjectModel.CommonEntityDTO> EvaluateStringAsync(string parameters);

        Task<ExcelReportContentDTO> EvaluateExcelReportAsync(EvaluateExcelReportContentParameterSet parameters);

        Task<List<FolderDTO>> GetAllExcelReportFolderListAsync();

        Task<List<FolderDTO>> GetExcelReportFolderListAsync(int parameters);

        Task<int> AddExcelReportFolderAsync(AddFolderParameterSet parameters);

        Task MoveExcelReportFolderAsync(MoveDashboardFolderParameterSet parameters);

        Task EditFolderAsync(EditFolderParameterSet parameters);

        Task EditDashboardAsync(EditDashboardParameterSet parameters);

        Task DeleteDashboardAsync(int parameters);

        Task SetSortOrderAsync(SetSortOrderParameterSet parameters);

        Task DeleteFolderAsync(int parameters);

        Task<List<DashboardGrantDTO>> GetDashboardGrantListAsync(int parameters);

        Task UpdateDashboardGrantAsync(UpdateDashboardGrantParameterSet parameters);

        Task TrashDashboardAsync(int parameters);

        Task<List<int>> GetExcelReportSharedListAsync(int parameters);

        Task<List<DashboardDTO>> GetExcelReportUsersSharedListAsync(int parameters);

        Task<List<DashboardDTO>> GetExcelReportUserSharedListAsync(int parameters);

    }

    public sealed class ExcelReportServiceProxy : DataProviderBase<IExcelReportService>, IExcelReportServiceProxy
	{
        protected override string ServiceUri => "/ExcelReports/ExcelReportService.svc";
      


        public Task<int> AddExcelReportAsync(AddDashboardParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDashboardParameterSet>(channel, channel.BeginAddExcelReport, channel.EndAddExcelReport, parameters);
        }

        public Task<int> AddExcelWithPermissionAsync(AddDashboardPermissionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDashboardPermissionParameterSet>(channel, channel.BeginAddExcelWithPermission, channel.EndAddExcelWithPermission, parameters);
        }

        public Task<ExcelReportContentDTO> GetExcelReportContentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<ExcelReportContentDTO,int>(channel, channel.BeginGetExcelReportContent, channel.EndGetExcelReportContent, parameters);
        }

        public Task UpdateExcelReportContentAsync(ExcelReportContentDTO parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdateExcelReportContent, channel.EndUpdateExcelReportContent, parameters);
        }

        public Task<List<DashboardDTO>> GetAllExcelReportListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>>(channel, channel.BeginGetAllExcelReportList, channel.EndGetAllExcelReportList);
        }

        public Task<List<DashboardDTO>> GetExcelReportListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>,int>(channel, channel.BeginGetExcelReportList, channel.EndGetExcelReportList, parameters);
        }

        public Task<GazRouter.DTO.ObjectModel.CommonEntityDTO> EvaluateStringAsync(string parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<GazRouter.DTO.ObjectModel.CommonEntityDTO,string>(channel, channel.BeginEvaluateString, channel.EndEvaluateString, parameters);
        }

        public Task<ExcelReportContentDTO> EvaluateExcelReportAsync(EvaluateExcelReportContentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<ExcelReportContentDTO,EvaluateExcelReportContentParameterSet>(channel, channel.BeginEvaluateExcelReport, channel.EndEvaluateExcelReport, parameters);
        }

        public Task<List<FolderDTO>> GetAllExcelReportFolderListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<FolderDTO>>(channel, channel.BeginGetAllExcelReportFolderList, channel.EndGetAllExcelReportFolderList);
        }

        public Task<List<FolderDTO>> GetExcelReportFolderListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<FolderDTO>,int>(channel, channel.BeginGetExcelReportFolderList, channel.EndGetExcelReportFolderList, parameters);
        }

        public Task<int> AddExcelReportFolderAsync(AddFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddFolderParameterSet>(channel, channel.BeginAddExcelReportFolder, channel.EndAddExcelReportFolder, parameters);
        }

        public Task MoveExcelReportFolderAsync(MoveDashboardFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveExcelReportFolder, channel.EndMoveExcelReportFolder, parameters);
        }

        public Task EditFolderAsync(EditFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditFolder, channel.EndEditFolder, parameters);
        }

        public Task EditDashboardAsync(EditDashboardParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDashboard, channel.EndEditDashboard, parameters);
        }

        public Task DeleteDashboardAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteDashboard, channel.EndDeleteDashboard, parameters);
        }

        public Task SetSortOrderAsync(SetSortOrderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetSortOrder, channel.EndSetSortOrder, parameters);
        }

        public Task DeleteFolderAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteFolder, channel.EndDeleteFolder, parameters);
        }

        public Task<List<DashboardGrantDTO>> GetDashboardGrantListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardGrantDTO>,int>(channel, channel.BeginGetDashboardGrantList, channel.EndGetDashboardGrantList, parameters);
        }

        public Task UpdateDashboardGrantAsync(UpdateDashboardGrantParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdateDashboardGrant, channel.EndUpdateDashboardGrant, parameters);
        }

        public Task TrashDashboardAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginTrashDashboard, channel.EndTrashDashboard, parameters);
        }

        public Task<List<int>> GetExcelReportSharedListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<int>,int>(channel, channel.BeginGetExcelReportSharedList, channel.EndGetExcelReportSharedList, parameters);
        }

        public Task<List<DashboardDTO>> GetExcelReportUsersSharedListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>,int>(channel, channel.BeginGetExcelReportUsersSharedList, channel.EndGetExcelReportUsersSharedList, parameters);
        }

        public Task<List<DashboardDTO>> GetExcelReportUserSharedListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>,int>(channel, channel.BeginGetExcelReportUserSharedList, channel.EndGetExcelReportUserSharedList, parameters);
        }

    }
}
