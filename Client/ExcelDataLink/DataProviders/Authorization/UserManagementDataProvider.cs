using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DTO.Authorization;
using GazRouter.DTO.Authorization.Action;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;

namespace DataProviders.Authorization
{
    [ServiceContract]
    public class UserManagementDataProvider : DataProviderBase<IUserManagementService>
    {
        public void RemoveUserRole(UserRoleParameterSet parameterSet, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginRemoveUserRole,
                  channel.EndRemoveUserRole, callback, parameterSet, behavior);
        }

        public void BatchAddRemoveRoleAction(BatchAddRemoveRoleActionParameterSet parameterSet, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginBatchAddRemoveRoleAction,
                  channel.EndBatchAddRemoveRoleAction, callback, parameterSet, behavior);
        }

		public void GetUsers(Func<List<UserDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetUsers,
				  channel.EndGetUsers, callback, behavior);
        }

        public void GetAdUsers(Func<List<AdUserDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetAdUsers,
                  channel.EndGetAdUsers, callback, behavior);
        }

		public void GetRoles(Func<List<RoleDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetRoles,
				  channel.EndGetRoles, callback, behavior);
        }

		public void GetActionGroups(Func<List<ActionGroupDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetActionGroups,
				  channel.EndGetActionGroups, callback, behavior);
        }

        public void RemoveRole(int roleId, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginRemoveRole,
                  channel.EndRemoveRole, callback, roleId, behavior);
        }

		public void GetUserRoles(int userId, Func<List<RoleDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetUserRoles,
				  channel.EndGetUserRoles, callback, userId, behavior);
        }

        public void GetProfileInfo(Func<UserDTO, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetProfileInfo,
				  channel.EndGetProfileInfo, callback, behavior);
        }

        public void EditUserSettings(EditUserSettingsParameterSet parameterSet, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginEditUserSettings,
                  channel.EndEditUserSettings, callback, parameterSet, behavior);
        }

        protected override string ServiceUri
        {
            get { return "/Authorization/UserManagementService.svc"; }
        }

    
    }
}
