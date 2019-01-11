using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Dashboards;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dashboards.DashboardFolder;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.ObjectModel.Sites;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Dashboards  
{
    [ServiceContract]
    public interface IDashboardService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginIsFolderContainPanels(int folderId, AsyncCallback callback, object state);
        bool EndIsFolderContainPanels(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetFolderList(object parameters, AsyncCallback callback, object state);
        List<FolderDTO> EndGetFolderList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddFolder(AddFolderParameterSet parameters, AsyncCallback callback, object state);
        int EndAddFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditFolder(EditFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndEditFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteFolder(int parameters, AsyncCallback callback, object state);
        void EndDeleteFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardList(object parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetDashboardList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAllDashboardList(object parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetAllDashboardList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDashboard(AddDashboardParameterSet parameters, AsyncCallback callback, object state);
        int EndAddDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDashboardWithPermission(AddDashboardPermissionParameterSet parameters, AsyncCallback callback, object state);
        int EndAddDashboardWithPermission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDashboardPermission(DashboardPermissionParameterSet parameters, AsyncCallback callback, object state);
        void EndAddDashboardPermission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSoftDeleteDashboard(EditDashboardParameterSet parameters, AsyncCallback callback, object state);
        void EndSoftDeleteDashboard(IAsyncResult result);

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
        IAsyncResult BeginGetDashboardPermissionList(object parameters, AsyncCallback callback, object state);
        List<DashboardGrantDTO2> EndGetDashboardPermissionList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdateDashboardPermissions(UpdateDashboardPermissionsParameterSet parameters, AsyncCallback callback, object state);
        void EndUpdateDashboardPermissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetFolderPermissionList(object parameters, AsyncCallback callback, object state);
        List<DashboardGrantDTO2> EndGetFolderPermissionList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddFolderPermission(DashboardPermissionParameterSet parameters, AsyncCallback callback, object state);
        void EndAddFolderPermission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdateFolderPermissions(UpdateDashboardPermissionsParameterSet parameters, AsyncCallback callback, object state);
        void EndUpdateFolderPermissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardUsersSharedList(int parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetDashboardUsersSharedList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardUserSharedList(int parameters, AsyncCallback callback, object state);
        List<DashboardDTO> EndGetDashboardUserSharedList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardSharedList(int parameters, AsyncCallback callback, object state);
        List<int> EndGetDashboardSharedList(IAsyncResult result);

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
        IAsyncResult BeginMoveDashboard(DashboardFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTrashDashboard(int parameters, AsyncCallback callback, object state);
        void EndTrashDashboard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTrashDashboard2(DashboardFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndTrashDashboard2(IAsyncResult result);

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
        IAsyncResult BeginGetEnterpriseSites(object parameters, AsyncCallback callback, object state);
        List<SiteDTO> EndGetEnterpriseSites(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardData(DashboardDataParameterSets parameters, AsyncCallback callback, object state);
        DashboardDataDTO EndGetDashboardData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetSortOrder(List<DashSortOrderDTO> orders, AsyncCallback callback, object state);
        void EndSetSortOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDashboardToFolder(DashboardFolderParameterSet parameters, AsyncCallback callback, object state);
        void EndAddDashboardToFolder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAllFolderList(object parameters, AsyncCallback callback, object state);
        List<FolderDTO> EndGetAllFolderList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDashboardFoldersList(object parameters, AsyncCallback callback, object state);
        List<DashboardFoldersDTO> EndGetDashboardFoldersList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginMoveSharedDashboard(DashboardFolderGenericParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveSharedDashboard(IAsyncResult result);
    }

	public interface IDashboardServiceProxy
	{

        Task<bool> IsFolderContainPanelsAsync(int folderId);

        Task<List<FolderDTO>> GetFolderListAsync();

        Task<int> AddFolderAsync(AddFolderParameterSet parameters);

        Task EditFolderAsync(EditFolderParameterSet parameters);

        Task DeleteFolderAsync(int parameters);

        Task<List<DashboardDTO>> GetDashboardListAsync();

        Task<List<DashboardDTO>> GetAllDashboardListAsync();

        Task<int> AddDashboardAsync(AddDashboardParameterSet parameters);

        Task<int> AddDashboardWithPermissionAsync(AddDashboardPermissionParameterSet parameters);

        Task AddDashboardPermissionAsync(DashboardPermissionParameterSet parameters);

        Task SoftDeleteDashboardAsync(EditDashboardParameterSet parameters);

        Task DeleteDashboardAsync(int parameters);

        Task EditDashboardAsync(EditDashboardParameterSet parameters);

        Task<List<DashboardGrantDTO2>> GetDashboardPermissionListAsync();

        Task UpdateDashboardPermissionsAsync(UpdateDashboardPermissionsParameterSet parameters);

        Task<List<DashboardGrantDTO2>> GetFolderPermissionListAsync();

        Task AddFolderPermissionAsync(DashboardPermissionParameterSet parameters);

        Task UpdateFolderPermissionsAsync(UpdateDashboardPermissionsParameterSet parameters);

        Task<List<DashboardDTO>> GetDashboardUsersSharedListAsync(int parameters);

        Task<List<DashboardDTO>> GetDashboardUserSharedListAsync(int parameters);

        Task<List<int>> GetDashboardSharedListAsync(int parameters);

        Task<List<DashboardGrantDTO>> GetDashboardGrantListAsync(int parameters);

        Task UpdateDashboardGrantAsync(UpdateDashboardGrantParameterSet parameters);

        Task MoveDashboardAsync(DashboardFolderParameterSet parameters);

        Task TrashDashboardAsync(int parameters);

        Task TrashDashboard2Async(DashboardFolderParameterSet parameters);

        Task<DashboardContentDTO> GetDashboardContentAsync(int parameters);

        Task UpdateDashboardContentAsync(DashboardContentDTO parameters);

        Task<List<SiteDTO>> GetEnterpriseSitesAsync();

        Task<DashboardDataDTO> GetDashboardDataAsync(DashboardDataParameterSets parameters);

        Task SetSortOrderAsync(List<DashSortOrderDTO> orders);

        Task AddDashboardToFolderAsync(DashboardFolderParameterSet parameters);

        Task<List<FolderDTO>> GetAllFolderListAsync();

        Task<List<DashboardFoldersDTO>> GetDashboardFoldersListAsync();

        Task MoveSharedDashboardAsync(DashboardFolderGenericParameterSet parameters);

    }

    public sealed class DashboardServiceProxy : DataProviderBase<IDashboardService>, IDashboardServiceProxy
	{
        protected override string ServiceUri => "/Dashboards/DashboardService.svc";
      


        public Task<bool> IsFolderContainPanelsAsync(int folderId)
        {
            var channel = GetChannel();
            return ExecuteAsync<bool,int>(channel, channel.BeginIsFolderContainPanels, channel.EndIsFolderContainPanels, folderId);
        }

        public Task<List<FolderDTO>> GetFolderListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<FolderDTO>>(channel, channel.BeginGetFolderList, channel.EndGetFolderList);
        }

        public Task<int> AddFolderAsync(AddFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddFolderParameterSet>(channel, channel.BeginAddFolder, channel.EndAddFolder, parameters);
        }

        public Task EditFolderAsync(EditFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditFolder, channel.EndEditFolder, parameters);
        }

        public Task DeleteFolderAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteFolder, channel.EndDeleteFolder, parameters);
        }

        public Task<List<DashboardDTO>> GetDashboardListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>>(channel, channel.BeginGetDashboardList, channel.EndGetDashboardList);
        }

        public Task<List<DashboardDTO>> GetAllDashboardListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>>(channel, channel.BeginGetAllDashboardList, channel.EndGetAllDashboardList);
        }

        public Task<int> AddDashboardAsync(AddDashboardParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDashboardParameterSet>(channel, channel.BeginAddDashboard, channel.EndAddDashboard, parameters);
        }

        public Task<int> AddDashboardWithPermissionAsync(AddDashboardPermissionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDashboardPermissionParameterSet>(channel, channel.BeginAddDashboardWithPermission, channel.EndAddDashboardWithPermission, parameters);
        }

        public Task AddDashboardPermissionAsync(DashboardPermissionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddDashboardPermission, channel.EndAddDashboardPermission, parameters);
        }

        public Task SoftDeleteDashboardAsync(EditDashboardParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSoftDeleteDashboard, channel.EndSoftDeleteDashboard, parameters);
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

        public Task<List<DashboardGrantDTO2>> GetDashboardPermissionListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardGrantDTO2>>(channel, channel.BeginGetDashboardPermissionList, channel.EndGetDashboardPermissionList);
        }

        public Task UpdateDashboardPermissionsAsync(UpdateDashboardPermissionsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdateDashboardPermissions, channel.EndUpdateDashboardPermissions, parameters);
        }

        public Task<List<DashboardGrantDTO2>> GetFolderPermissionListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardGrantDTO2>>(channel, channel.BeginGetFolderPermissionList, channel.EndGetFolderPermissionList);
        }

        public Task AddFolderPermissionAsync(DashboardPermissionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddFolderPermission, channel.EndAddFolderPermission, parameters);
        }

        public Task UpdateFolderPermissionsAsync(UpdateDashboardPermissionsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdateFolderPermissions, channel.EndUpdateFolderPermissions, parameters);
        }

        public Task<List<DashboardDTO>> GetDashboardUsersSharedListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>,int>(channel, channel.BeginGetDashboardUsersSharedList, channel.EndGetDashboardUsersSharedList, parameters);
        }

        public Task<List<DashboardDTO>> GetDashboardUserSharedListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardDTO>,int>(channel, channel.BeginGetDashboardUserSharedList, channel.EndGetDashboardUserSharedList, parameters);
        }

        public Task<List<int>> GetDashboardSharedListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<int>,int>(channel, channel.BeginGetDashboardSharedList, channel.EndGetDashboardSharedList, parameters);
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

        public Task MoveDashboardAsync(DashboardFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveDashboard, channel.EndMoveDashboard, parameters);
        }

        public Task TrashDashboardAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginTrashDashboard, channel.EndTrashDashboard, parameters);
        }

        public Task TrashDashboard2Async(DashboardFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginTrashDashboard2, channel.EndTrashDashboard2, parameters);
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

        public Task<List<SiteDTO>> GetEnterpriseSitesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SiteDTO>>(channel, channel.BeginGetEnterpriseSites, channel.EndGetEnterpriseSites);
        }

        public Task<DashboardDataDTO> GetDashboardDataAsync(DashboardDataParameterSets parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<DashboardDataDTO,DashboardDataParameterSets>(channel, channel.BeginGetDashboardData, channel.EndGetDashboardData, parameters);
        }

        public Task SetSortOrderAsync(List<DashSortOrderDTO> orders)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetSortOrder, channel.EndSetSortOrder, orders);
        }

        public Task AddDashboardToFolderAsync(DashboardFolderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddDashboardToFolder, channel.EndAddDashboardToFolder, parameters);
        }

        public Task<List<FolderDTO>> GetAllFolderListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<FolderDTO>>(channel, channel.BeginGetAllFolderList, channel.EndGetAllFolderList);
        }

        public Task<List<DashboardFoldersDTO>> GetDashboardFoldersListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DashboardFoldersDTO>>(channel, channel.BeginGetDashboardFoldersList, channel.EndGetDashboardFoldersList);
        }

        public Task MoveSharedDashboardAsync(DashboardFolderGenericParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveSharedDashboard, channel.EndMoveSharedDashboard, parameters);
        }

    }
}
