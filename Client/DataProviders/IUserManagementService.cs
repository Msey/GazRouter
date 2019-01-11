using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Authorization;
using GazRouter.DTO.Authorization.Action;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Authorization.TargetingList;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Authorization  
{
    [ServiceContract]
    public interface IUserManagementService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddUserRole(UserRoleParameterSet parameterSet, AsyncCallback callback, object state);
        void EndAddUserRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveUserRole(UserRoleParameterSet parameterSet, AsyncCallback callback, object state);
        void EndRemoveUserRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginBatchAddRemoveRoleAction(BatchAddRemoveRoleActionParameterSet parameterSet, AsyncCallback callback, object state);
        void EndBatchAddRemoveRoleAction(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetUsers(object parameters, AsyncCallback callback, object state);
        List<UserDTO> EndGetUsers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAgreedUsers(GetAgreedUsersAllParameterSet parameterSet, AsyncCallback callback, object state);
        List<AgreedUserDTO> EndGetAgreedUsers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAdUsers(object parameters, AsyncCallback callback, object state);
        List<AdUserDTO> EndGetAdUsers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAdUsersFiltered(AdUserFilterParameterSet parameterSet, AsyncCallback callback, object state);
        List<AdUserDTO> EndGetAdUsersFiltered(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAdUsersTree(AdUserFilterParameterSet parameterSet, AsyncCallback callback, object state);
        List<AdUserDTO> EndGetAdUsersTree(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRoles(object parameters, AsyncCallback callback, object state);
        List<RoleDTO> EndGetRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetActionGroups(object parameters, AsyncCallback callback, object state);
        List<ActionGroupDTO> EndGetActionGroups(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddUser(AddUserParameterSet parameterSet, AsyncCallback callback, object state);
        int EndAddUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditUser(EditUserParameterSet parameterSet, AsyncCallback callback, object state);
        void EndEditUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveUser(int userId, AsyncCallback callback, object state);
        void EndRemoveUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddAgreedUser(AddAgreedUserParameterSet parameterSet, AsyncCallback callback, object state);
        int EndAddAgreedUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditAgreedUser(EditAgreedUserParameterSet parameterSet, AsyncCallback callback, object state);
        void EndEditAgreedUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveAgreedUser(int userId, AsyncCallback callback, object state);
        void EndRemoveAgreedUser(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRole(AddRoleParameterSet parameterSet, AsyncCallback callback, object state);
        int EndAddRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveRole(int roleId, AsyncCallback callback, object state);
        void EndRemoveRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetUserRoles(int userId, AsyncCallback callback, object state);
        List<RoleDTO> EndGetUserRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddPermissions(List<PermissionDTO> permissions, AsyncCallback callback, object state);
        void EndAddPermissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginUpdatePermissions(List<PermissionDTO> permissions, AsyncCallback callback, object state);
        void EndUpdatePermissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPermissions(object parameters, AsyncCallback callback, object state);
        List<PermissionDTO> EndGetPermissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPermissionsByRoleId(int roleId, AsyncCallback callback, object state);
        List<PermissionDTO> EndGetPermissionsByRoleId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemovePermissions(object parameters, AsyncCallback callback, object state);
        void EndRemovePermissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDomains(object parameters, AsyncCallback callback, object state);
        List<string> EndGetDomains(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetProfileInfo(object parameters, AsyncCallback callback, object state);
        UserDTO EndGetProfileInfo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditUserSettings(EditUserSettingsParameterSet parameterSet, AsyncCallback callback, object state);
        void EndEditUserSettings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTestDateTime(DateTime dt, AsyncCallback callback, object state);
        string EndTestDateTime(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSapBoUri(object parameters, AsyncCallback callback, object state);
        string EndGetSapBoUri(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTargetingList(GetTargetingListParameterSet parameterSet, AsyncCallback callback, object state);
        List<TargetingListDTO> EndGetTargetingList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddUserToTargetingList(AddEditTargetListUserParameterSet parameterSet, AsyncCallback callback, object state);
        int EndAddUserToTargetingList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditUserInTargetingList(AddEditTargetListUserParameterSet parameterSet, AsyncCallback callback, object state);
        void EndEditUserInTargetingList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveUserFromTargetingList(DeleteTargetingListParametersSet parameterSet, AsyncCallback callback, object state);
        void EndRemoveUserFromTargetingList(IAsyncResult result);
    }

	public interface IUserManagementServiceProxy
	{

        Task AddUserRoleAsync(UserRoleParameterSet parameterSet);

        Task RemoveUserRoleAsync(UserRoleParameterSet parameterSet);

        Task BatchAddRemoveRoleActionAsync(BatchAddRemoveRoleActionParameterSet parameterSet);

        Task<List<UserDTO>> GetUsersAsync();

        Task<List<AgreedUserDTO>> GetAgreedUsersAsync(GetAgreedUsersAllParameterSet parameterSet);

        Task<List<AdUserDTO>> GetAdUsersAsync();

        Task<List<AdUserDTO>> GetAdUsersFilteredAsync(AdUserFilterParameterSet parameterSet);

        Task<List<AdUserDTO>> GetAdUsersTreeAsync(AdUserFilterParameterSet parameterSet);

        Task<List<RoleDTO>> GetRolesAsync();

        Task<List<ActionGroupDTO>> GetActionGroupsAsync();

        Task<int> AddUserAsync(AddUserParameterSet parameterSet);

        Task EditUserAsync(EditUserParameterSet parameterSet);

        Task RemoveUserAsync(int userId);

        Task<int> AddAgreedUserAsync(AddAgreedUserParameterSet parameterSet);

        Task EditAgreedUserAsync(EditAgreedUserParameterSet parameterSet);

        Task RemoveAgreedUserAsync(int userId);

        Task<int> AddRoleAsync(AddRoleParameterSet parameterSet);

        Task RemoveRoleAsync(int roleId);

        Task<List<RoleDTO>> GetUserRolesAsync(int userId);

        Task AddPermissionsAsync(List<PermissionDTO> permissions);

        Task UpdatePermissionsAsync(List<PermissionDTO> permissions);

        Task<List<PermissionDTO>> GetPermissionsAsync();

        Task<List<PermissionDTO>> GetPermissionsByRoleIdAsync(int roleId);

        Task RemovePermissionsAsync();

        Task<List<string>> GetDomainsAsync();

        Task<UserDTO> GetProfileInfoAsync();

        Task EditUserSettingsAsync(EditUserSettingsParameterSet parameterSet);

        Task<string> TestDateTimeAsync(DateTime dt);

        Task<string> GetSapBoUriAsync();

        Task<List<TargetingListDTO>> GetTargetingListAsync(GetTargetingListParameterSet parameterSet);

        Task<int> AddUserToTargetingListAsync(AddEditTargetListUserParameterSet parameterSet);

        Task EditUserInTargetingListAsync(AddEditTargetListUserParameterSet parameterSet);

        Task RemoveUserFromTargetingListAsync(DeleteTargetingListParametersSet parameterSet);

    }

    public sealed class UserManagementServiceProxy : DataProviderBase<IUserManagementService>, IUserManagementServiceProxy
	{
        protected override string ServiceUri => "/Authorization/UserManagementService.svc";
      


        public Task AddUserRoleAsync(UserRoleParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddUserRole, channel.EndAddUserRole, parameterSet);
        }

        public Task RemoveUserRoleAsync(UserRoleParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveUserRole, channel.EndRemoveUserRole, parameterSet);
        }

        public Task BatchAddRemoveRoleActionAsync(BatchAddRemoveRoleActionParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginBatchAddRemoveRoleAction, channel.EndBatchAddRemoveRoleAction, parameterSet);
        }

        public Task<List<UserDTO>> GetUsersAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<UserDTO>>(channel, channel.BeginGetUsers, channel.EndGetUsers);
        }

        public Task<List<AgreedUserDTO>> GetAgreedUsersAsync(GetAgreedUsersAllParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AgreedUserDTO>,GetAgreedUsersAllParameterSet>(channel, channel.BeginGetAgreedUsers, channel.EndGetAgreedUsers, parameterSet);
        }

        public Task<List<AdUserDTO>> GetAdUsersAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AdUserDTO>>(channel, channel.BeginGetAdUsers, channel.EndGetAdUsers);
        }

        public Task<List<AdUserDTO>> GetAdUsersFilteredAsync(AdUserFilterParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AdUserDTO>,AdUserFilterParameterSet>(channel, channel.BeginGetAdUsersFiltered, channel.EndGetAdUsersFiltered, parameterSet);
        }

        public Task<List<AdUserDTO>> GetAdUsersTreeAsync(AdUserFilterParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AdUserDTO>,AdUserFilterParameterSet>(channel, channel.BeginGetAdUsersTree, channel.EndGetAdUsersTree, parameterSet);
        }

        public Task<List<RoleDTO>> GetRolesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RoleDTO>>(channel, channel.BeginGetRoles, channel.EndGetRoles);
        }

        public Task<List<ActionGroupDTO>> GetActionGroupsAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ActionGroupDTO>>(channel, channel.BeginGetActionGroups, channel.EndGetActionGroups);
        }

        public Task<int> AddUserAsync(AddUserParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddUserParameterSet>(channel, channel.BeginAddUser, channel.EndAddUser, parameterSet);
        }

        public Task EditUserAsync(EditUserParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditUser, channel.EndEditUser, parameterSet);
        }

        public Task RemoveUserAsync(int userId)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveUser, channel.EndRemoveUser, userId);
        }

        public Task<int> AddAgreedUserAsync(AddAgreedUserParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddAgreedUserParameterSet>(channel, channel.BeginAddAgreedUser, channel.EndAddAgreedUser, parameterSet);
        }

        public Task EditAgreedUserAsync(EditAgreedUserParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditAgreedUser, channel.EndEditAgreedUser, parameterSet);
        }

        public Task RemoveAgreedUserAsync(int userId)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveAgreedUser, channel.EndRemoveAgreedUser, userId);
        }

        public Task<int> AddRoleAsync(AddRoleParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRoleParameterSet>(channel, channel.BeginAddRole, channel.EndAddRole, parameterSet);
        }

        public Task RemoveRoleAsync(int roleId)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveRole, channel.EndRemoveRole, roleId);
        }

        public Task<List<RoleDTO>> GetUserRolesAsync(int userId)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RoleDTO>,int>(channel, channel.BeginGetUserRoles, channel.EndGetUserRoles, userId);
        }

        public Task AddPermissionsAsync(List<PermissionDTO> permissions)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddPermissions, channel.EndAddPermissions, permissions);
        }

        public Task UpdatePermissionsAsync(List<PermissionDTO> permissions)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginUpdatePermissions, channel.EndUpdatePermissions, permissions);
        }

        public Task<List<PermissionDTO>> GetPermissionsAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<PermissionDTO>>(channel, channel.BeginGetPermissions, channel.EndGetPermissions);
        }

        public Task<List<PermissionDTO>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<PermissionDTO>,int>(channel, channel.BeginGetPermissionsByRoleId, channel.EndGetPermissionsByRoleId, roleId);
        }

        public Task RemovePermissionsAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemovePermissions, channel.EndRemovePermissions);
        }

        public Task<List<string>> GetDomainsAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<string>>(channel, channel.BeginGetDomains, channel.EndGetDomains);
        }

        public Task<UserDTO> GetProfileInfoAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<UserDTO>(channel, channel.BeginGetProfileInfo, channel.EndGetProfileInfo);
        }

        public Task EditUserSettingsAsync(EditUserSettingsParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditUserSettings, channel.EndEditUserSettings, parameterSet);
        }

        public Task<string> TestDateTimeAsync(DateTime dt)
        {
            var channel = GetChannel();
            return ExecuteAsync<string,DateTime>(channel, channel.BeginTestDateTime, channel.EndTestDateTime, dt);
        }

        public Task<string> GetSapBoUriAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<string>(channel, channel.BeginGetSapBoUri, channel.EndGetSapBoUri);
        }

        public Task<List<TargetingListDTO>> GetTargetingListAsync(GetTargetingListParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TargetingListDTO>,GetTargetingListParameterSet>(channel, channel.BeginGetTargetingList, channel.EndGetTargetingList, parameterSet);
        }

        public Task<int> AddUserToTargetingListAsync(AddEditTargetListUserParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEditTargetListUserParameterSet>(channel, channel.BeginAddUserToTargetingList, channel.EndAddUserToTargetingList, parameterSet);
        }

        public Task EditUserInTargetingListAsync(AddEditTargetListUserParameterSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditUserInTargetingList, channel.EndEditUserInTargetingList, parameterSet);
        }

        public Task RemoveUserFromTargetingListAsync(DeleteTargetingListParametersSet parameterSet)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveUserFromTargetingList, channel.EndRemoveUserFromTargetingList, parameterSet);
        }

    }
}
