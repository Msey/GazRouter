using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Remoting.Contexts;
using System.Xml.Serialization;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Authorization.Action;
using GazRouter.DAL.Authorization.Role;
using GazRouter.DAL.Authorization.User;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Authorization;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Authorization.Permission;
using GazRouter.DTO.Authorization;
using GazRouter.DTO.Authorization.Action;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
using GazRouter.Log;
using NLog;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Authorization.TargetingList;

namespace GazRouter.DataServices.Authorization
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class UserManagementService : ServiceBase, IUserManagementService
    {
        public void AddUserRole(UserRoleParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                new AddUserRoleCommand(context).Execute(parameterSet);
                SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
            }
        }
        public void RemoveUserRole(UserRoleParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                new RemoveUserRoleCommand(context).Execute(parameterSet);
                SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
            }
        }
        public void BatchAddRemoveRoleAction(BatchAddRemoveRoleActionParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                foreach (var actionToAdd in parameterSet.ActionsToAdd)
                {
                    new AddRoleActionCommand(context).Execute(actionToAdd);
                }

                foreach (var actionToRemove in parameterSet.ActionsToRemove)
                {
                    new RemoveRoleActionCommand(context).Execute(actionToRemove);
                }
            }

            SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
        }
        public List<UserDTO> GetUsers()
        {
            using (var context = OpenDbContext())
            {
                return new GetUsersAllQuery(context).Execute();
            }
        }
        public List<AdUserDTO> GetAdUsers()
        {
            return AdManager.GetAdUsers();
        }
        public List<AdUserDTO> GetAdUsersFiltered(AdUserFilterParameterSet parameterSet)
        {
            return AdManager.GetAdUsers(parameterSet);
        }
        public List<AdUserDTO> GetAdUsersTree(AdUserFilterParameterSet parameterSet)
        {
            return AdManager.AdForestUsers(parameterSet);
        }
        public List<RoleDTO> GetRoles()
        {
            using (var context = OpenDbContext())
            {
                return new GetRolesAllQuery(context).Execute();
            }
        }
        public List<AgreedUserDTO> GetAgreedUsers(GetAgreedUsersAllParameterSet parameterSet)
        {
            if (parameterSet == null)
                parameterSet = new GetAgreedUsersAllParameterSet();
            using (var context = OpenDbContext())
            {
                return new GetAgreedUsersAllQuery(context).Execute(parameterSet);
            }
        }
        public List<ActionGroupDTO> GetActionGroups()
        {
            using (var context = OpenDbContext())
            {
                var actions = new GetActionsAllQuery(context).Execute();
                var roles = new GetRolesAllQuery(context).Execute();


                var rolesActions = new Dictionary<int, List<ActionDTO>>();
                var dict = new Dictionary<string, ActionGroupDTO>();

                foreach (var actionDto in actions)
                {
                    var existsInDll = ServiceActions.AllActions.Any(item => item.ActionPath == actionDto.Path);

                    if (!existsInDll)//такая ситуация возможна при работе нескольких IIS с одним инстансом БД
                        continue;

                    var rolePermits = new List<RolePermit>();

                    actionDto.IsAllowedByDefault = ServiceActions.AllActions.First(item => item.ActionPath == actionDto.Path).IsAllowedByDefault;

                    if (actionDto.IsAllowedByDefault)
                    {
                        rolePermits.AddRange(roles.Select(r => new RolePermit { RoleId = r.Id, RoleName = r.Name, IsAllowed = true }));
                    }
                    else
                    {
                        foreach (var role in roles)
                        {
                            //кэширование для минимизации количества обращений к БД
                            if (!rolesActions.ContainsKey(role.Id))
                            {
                                rolesActions.Add(role.Id, new GetActionsByRoleIdQuery(context).Execute(role.Id));
                            }
                            rolePermits.Add(new RolePermit { RoleId = role.Id, RoleName = role.Name, IsAllowed = rolesActions[role.Id].Any(a => a.Path == actionDto.Path) });
                        }
                    }

                    actionDto.RolePermits = rolePermits;

                    var key = actionDto.ServiceDescription;

                    if (!dict.ContainsKey(key))
                        dict.Add(key, new ActionGroupDTO { GroupName = key });

                    dict[key].Actions.Add(actionDto);
                }

                var actionGroups = dict.Select(item => item.Value).ToList();

                return actionGroups;
            }
        }
        public int AddUser(AddUserParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                return new AddUserCommand(context).Execute(parameterSet);
            }
        }
        public void EditUser(EditUserParameterSet parameterSet)
        {
            UserDTO user;
            using (var context = OpenDbContext())
            {
                new EditUserCommand(context).Execute(parameterSet);
                user = new GetUserByIdQuery(context).Execute(parameterSet.Id);
            }

            var session = SessionManager.GetSession(user.Login);
            if (session != null)
            {
                session.User = user;
            }
        }
        public void RemoveUser(int userId)
        {
            using (var context = OpenDbContext())
            {
                new DeleteUserCommand(context).Execute(userId);
            }

            SessionManager.RemoveUserSessionByUserId(userId);
        }

        public int AddAgreedUser(AddAgreedUserParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                return new AddAgreedUserCommand(context).Execute(parameterSet);
            }
        }
        public void EditAgreedUser(EditAgreedUserParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                new EditAgreedUserCommand(context).Execute(parameterSet);
            }
        }
        public void RemoveAgreedUser(int userId)
        {
            using (var context = OpenDbContext())
            {
                new DeleteAgreedUserCommand(context).Execute(userId);
            }
        }
        public int AddRole(AddRoleParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                var id = new AddRoleCommand(context).Execute(parameterSet);
                SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
                return id;
            }

        }
        public void RemoveRole(int roleId)
        {
            using (var context = OpenDbContext())
            {
                new DeleteRoleCommand(context).Execute(roleId);
            }

            SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
        }
        public List<RoleDTO> GetUserRoles(int userId)
        {
            using (var context = OpenDbContext())
            {
                return new GetRolesByUserIdQuery(context).Execute(userId);
            }
        }

        public UserDTO GetProfileInfo()
        {
            new MyLogger("loginLogger").Info($"{Session.User?.Login}, {Session.User?.UserName}    ({Session.User?.SiteName})");
            return Session.User;
        }
        private IEnumerable<ServiceAction> GetRealActionsByUserId(int userId)
        {
            using (var context = OpenDbContext())
            {
                var actionDtos = new GetActionsByUserIdQuery(context).Execute(userId);
                return ServiceActions.AllActions.Join(actionDtos, a1 => a1.ActionPath, a2 => a2.Path, (a1, a2) => a1);
            }
        }
        public void EditUserSettings(EditUserSettingsParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                new EditUserSettingsCommand(context).Execute(parameterSet);
                Session.User = new GetUserByIdQuery(context).Execute(Session.User.Id);
            }
        }
        public string TestDateTime(DateTime dt)
        {
            return dt.ToLongTimeString();
        }
#region authorization
        public string GetSapBoUri()
        {
            return AppSettingsManager.SapBoUrl;            
        }
        [Obsolete]
        public void RemovePermissions()
        {            
            using (var context = OpenDbContext())
            {
                new RemovePermissionsCommand(context).Execute();
            }
            SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
        }
        [Obsolete]
        public void AddPermissions(List<PermissionDTO> permissions)
        {
            using (var context = OpenDbContext())
            {
                permissions.ForEach(e => new AddPermissionsCommand(context).Execute(e));
            }
            SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
        }
        public void UpdatePermissions(List<PermissionDTO> permissions)
        {
            using (var context = OpenDbContext())
            {
                var permissionDtos = new GetPermissionsQuery(context).Execute();
                var lookup = permissionDtos.ToLookup(e => e.ItemId);
                permissions.ForEach(item =>
                {
                    if (lookup.Contains(item.ItemId))
                    {
                        var permission = lookup[item.ItemId].FirstOrDefault(e => e.RoleId == item.RoleId);
                        if (permission == null) { 
                            if (item.Permission > 0) new AddPermissionsCommand(context).Execute(item);
                        }
                        else
                        {
                            if (item.Permission == 0)
                            {
                                item.Id = permission.Id;
                                new RemovePermissionCommand(context).Execute(item.Id);
                            }
                            else
                            {
                                item.Id = permission.Id;
                                new EditPermissionCommand(context).Execute(item);
                            }
                        }
                    }
                    else
                    {
                        if (item.Permission > 0) new AddPermissionsCommand(context).Execute(item);
                    }
                });
            }
            SessionManager.UpdateRolesForActiveSessions(GetRealActionsByUserId);
        }
        [Obsolete]
        public List<PermissionDTO> GetPermissions()
        {
            using (var context = OpenDbContext())
            {
                return new GetPermissionsQuery(context).Execute();
            }
        }
        public List<PermissionDTO> GetPermissionsByRoleId(int roleId)
        {
            using (var context = OpenDbContext())
            {
                return new GetPermissionsByRoleIdQuery(context).Execute(roleId);
            }
        }
#endregion
        /// <summary> for test </summary>
        /// <returns></returns>
        public List<string> GetDomains()
        {
            return AdManager.SubDomainList(AppSettingsManager.ActiveDirectory);
        }

        public List<TargetingListDTO> GetTargetingList(GetTargetingListParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                return new GetTargetListUsersQuery(context).Execute(parameterSet);
            }
        }

        public int AddUserToTargetingList(AddEditTargetListUserParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                return new AddTargetListUserCommand(context).Execute(parameterSet);
            }
        }

        public void EditUserInTargetingList(AddEditTargetListUserParameterSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                new EditTargetListUserCommand(context).Execute(parameterSet);
            }
        }

        public void RemoveUserFromTargetingList(DeleteTargetingListParametersSet parameterSet)
        {
            using (var context = OpenDbContext())
            {
                new DeleteTargetListUserCommand(context).Execute(parameterSet);
            }
        }
    }
}
