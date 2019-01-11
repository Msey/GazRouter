using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Authorization;
using GazRouter.DTO.Authorization.Action;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Authorization.TargetingList;

namespace GazRouter.DataServices.Authorization
{
    [ServiceContract]
    [Service("Сервис управления пользователями и ролями")]
    public interface IUserManagementService
    {

        [ServiceAction("Добавление роли пользователю")]
        [OperationContract]
        void AddUserRole(UserRoleParameterSet parameterSet);

        [ServiceAction("Удаление роли у пользователя")]
        [OperationContract]
        void RemoveUserRole(UserRoleParameterSet parameterSet);

        [ServiceAction(@"Добавление\Удаление действий у ролей")]
        [OperationContract]
        void BatchAddRemoveRoleAction(BatchAddRemoveRoleActionParameterSet parameterSet);

        [ServiceAction("Получение списка пользователей")]
        [OperationContract]
        List<UserDTO> GetUsers();

        [ServiceAction("Получение списка согласующих пользователей")]
        [OperationContract]
        List<AgreedUserDTO> GetAgreedUsers(GetAgreedUsersAllParameterSet parameterSet);


        [ServiceAction("Получение списка пользователей Active Directory")]
        [OperationContract]
        List<AdUserDTO> GetAdUsers();

        [ServiceAction("Получение списка пользователей Active Directory с учетом фильтра")]
        [OperationContract]
        List<AdUserDTO> GetAdUsersFiltered(AdUserFilterParameterSet parameterSet);


        [ServiceAction("")]
        [OperationContract]
        List<AdUserDTO> GetAdUsersTree(AdUserFilterParameterSet parameterSet);


        [ServiceAction("Получение списка ролей")]
        [OperationContract]
        List<RoleDTO> GetRoles();

        [ServiceAction("Получение списка действий")]
        [OperationContract]
        List<ActionGroupDTO> GetActionGroups();

        [ServiceAction("Добавление пользователя")]
        [OperationContract]
        int AddUser(AddUserParameterSet parameterSet);

        [ServiceAction("Редактирование пользователя")]
        [OperationContract]
        void EditUser(EditUserParameterSet parameterSet);

        [ServiceAction("Удаление пользователя")]
        [OperationContract]
        void RemoveUser(int userId);

        [ServiceAction("Добавление согласующего пользователя")]
        [OperationContract]
        int AddAgreedUser(AddAgreedUserParameterSet parameterSet);

        [ServiceAction("Редактирование согласующего пользователя")]
        [OperationContract]
        void EditAgreedUser(EditAgreedUserParameterSet parameterSet);

        [ServiceAction("Удаление согласующего пользователя")]
        [OperationContract]
        void RemoveAgreedUser(int userId);

        [ServiceAction("Добавление роли")]
        [OperationContract]
        int AddRole(AddRoleParameterSet parameterSet);

        [ServiceAction("Удаление роли")]
        [OperationContract]
        void RemoveRole(int roleId);

        [ServiceAction("Получение списка ролей пользователя")]
        [OperationContract]
        List<RoleDTO> GetUserRoles(int userId);
        /**/
        [ServiceAction("Добавление списка привилегий ролей")]
        [OperationContract]
        void AddPermissions(List<PermissionDTO> permissions);
        
        [ServiceAction("Добавление списка привилегий ролей")]
        [OperationContract]
        void UpdatePermissions(List<PermissionDTO> permissions);

        [ServiceAction("Получение списка привилегий ролей")]
        [OperationContract]
        List<PermissionDTO> GetPermissions();

        [ServiceAction("Получение списка привилегий для роли")]
        [OperationContract]
        List<PermissionDTO> GetPermissionsByRoleId(int roleId);

        [ServiceAction("Удаление всех привилегий ролей")]
        [OperationContract]
        void RemovePermissions();

        [ServiceAction("Получение списка поддомена")]
        [OperationContract]
        List<string> GetDomains();
        /**/
        [ServiceAction("Получение информации о пользователе")]
        [OperationContract]
        UserDTO GetProfileInfo();

        [ServiceAction("Редактирование настроек пользователя")]
        [OperationContract]
        void EditUserSettings(EditUserSettingsParameterSet parameterSet);

        [ServiceAction("Получить смещение часового пояса, относительное UTC, в часах")]
        [OperationContract]
        string TestDateTime(DateTime dt);
        
        [ServiceAction("Получить ссылку на SAP BO")]
        [OperationContract]
        string GetSapBoUri();




        [ServiceAction("Получение списков адресатов")]
        [OperationContract]
        List<TargetingListDTO> GetTargetingList(GetTargetingListParameterSet parameterSet);

        [ServiceAction("Добавление пользователя в список адресатов")]
        [OperationContract]
        int AddUserToTargetingList(AddEditTargetListUserParameterSet parameterSet);

        [ServiceAction("Редактирование пользователя в списке адресатов")]
        [OperationContract]
        void EditUserInTargetingList(AddEditTargetListUserParameterSet parameterSet);

        [ServiceAction("Удаление пользователя из списка адресатов")]
        [OperationContract]
        void RemoveUserFromTargetingList(DeleteTargetingListParametersSet parameterSet);

    }
}
