using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dashboards.DashboardFolder;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Dashboards  
{
    [ServiceContract]
    public interface IDashboardService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDashboardToFolder(DashboardFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndAddDashboardToFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteDashboardFromFolder(DashboardFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndDeleteDashboardFromFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddFolder(AddFolderParameterSet parameters, AsyncCallback callback, object state);
        int EndAddFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteFolder(int parameters, AsyncCallback callback, object state);
        void EndDeleteFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditFolder(EditFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndEditFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetFolderList(int parameters, AsyncCallback callback, object state);
        List<FolderDTO> EndGetFolderList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteDashboardGrant(DeleteDashboardGrantParameterSet parameters, AsyncCallback callback, object state);
        void EndDeleteDashboardGrant(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDashboardGrant(DashboardGrantParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDashboardGrant(IAsyncResult result);

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
        IAsyncResult BeginAddDashboard(AddDashboardParameterSet parameters, AsyncCallback callback, object state);
        int EndAddDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteDashboard(int parameters, AsyncCallback callback, object state);
        void EndDeleteDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDashboard(EditDashboardParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardList(int parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetDashboardList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginShareDashboard(ShareDashboardParameterSet parameters, AsyncCallback callback, object state);
        void EndShareDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginMoveDashboardFolder(MoveDashboardFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveDashboardFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardContent(int parameters, AsyncCallback callback, object state);
        DashboardContentDTO EndGetDashboardContent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdateDashboardContent(DashboardContentDTO parameters, AsyncCallback callback, object state);
        void EndUpdateDashboardContent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetSortOrder(SetSortOrderParameterSet parameters, AsyncCallback callback, object state);
        void EndSetSortOrder(IAsyncResult result);
    }


    public class DashboardServiceProxy : DataProviderBase<IDashboardService>
	{
        protected override string ServiceUri
        {
            get { return "/Dashboards/DashboardService.svc"; }
        }

        public Task AddDashboardToFolderAsync(DashboardFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddDashboardToFolder, channel.EndAddDashboardToFolder, parameters);
        }

        public Task DeleteDashboardFromFolderAsync(DashboardFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteDashboardFromFolder, channel.EndDeleteDashboardFromFolder, parameters);
        }

        public Task<int> AddFolderAsync(AddFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddFolderParameterSet>(channel, channel.BeginAddFolder, channel.EndAddFolder, parameters);
        }

        public Task DeleteFolderAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteFolder, channel.EndDeleteFolder, parameters);
        }

        public Task EditFolderAsync(EditFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditFolder, channel.EndEditFolder, parameters);
        }

        public Task<List<FolderDTO>> GetFolderListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<FolderDTO>,int>(channel, channel.BeginGetFolderList, channel.EndGetFolderList, parameters);
        }

        public Task DeleteDashboardGrantAsync(DeleteDashboardGrantParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteDashboardGrant, channel.EndDeleteDashboardGrant, parameters);
        }

        public Task EditDashboardGrantAsync(DashboardGrantParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDashboardGrant, channel.EndEditDashboardGrant, parameters);
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

        public Task<int> AddDashboardAsync(AddDashboardParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDashboardParameterSet>(channel, channel.BeginAddDashboard, channel.EndAddDashboard, parameters);
        }

        public Task DeleteDashboardAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteDashboard, channel.EndDeleteDashboard, parameters);
        }

        public Task EditDashboardAsync(EditDashboardParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDashboard, channel.EndEditDashboard, parameters);
        }

        public Task<List<DashboardDTO>> GetDashboardListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>,int>(channel, channel.BeginGetDashboardList, channel.EndGetDashboardList, parameters);
        }

        public Task ShareDashboardAsync(ShareDashboardParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginShareDashboard, channel.EndShareDashboard, parameters);
        }

        public Task MoveDashboardFolderAsync(MoveDashboardFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveDashboardFolder, channel.EndMoveDashboardFolder, parameters);
        }

        public Task<DashboardContentDTO> GetDashboardContentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<DashboardContentDTO,int>(channel, channel.BeginGetDashboardContent, channel.EndGetDashboardContent, parameters);
        }

        public Task UpdateDashboardContentAsync(DashboardContentDTO parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdateDashboardContent, channel.EndUpdateDashboardContent, parameters);
        }

        public Task SetSortOrderAsync(SetSortOrderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetSortOrder, channel.EndSetSortOrder, parameters);
        }

    }
}
