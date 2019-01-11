using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Authorization;
using GazRouter.DTO.Authorization.Action;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Authorization  
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
        IAsyncResult BeginGetAdUsers(object parameters, AsyncCallback callback, object state);
        List<AdUserDTO> EndGetAdUsers(IAsyncResult result);

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
    }


    public class UserManagementServiceProxy : DataProviderBase<IUserManagementService>
	{
        protected override string ServiceUri
        {
            get { return "/Authorization/UserManagementService.svc"; }
        }

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

        public Task<List<AdUserDTO>> GetAdUsersAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AdUserDTO>>(channel, channel.BeginGetAdUsers, channel.EndGetAdUsers);
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

    }
}
