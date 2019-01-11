using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Dashboards;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dashboards.DashboardFolder;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.ObjectModel.Sites;
namespace GazRouter.DataServices.Dashboards
{
    [Service("Информационные панели")]
    [ServiceContract]
    public interface IDashboardService
    {
        #region FOLDERS
        [ServiceAction("Проверка возможности удаления папки")]
        [OperationContract]
        bool IsFolderContainPanels(int folderId);


        [ServiceAction("Получение списка папок")]
        [OperationContract]
        List<FolderDTO> GetFolderList();

        [ServiceAction("Добавление папки")]
        [OperationContract]
        int AddFolder(AddFolderParameterSet parameters);

        [ServiceAction("Редактирование папки")]
        [OperationContract]
        void EditFolder(EditFolderParameterSet parameters);

        [ServiceAction("Удаление папки")]
        [OperationContract]
        void DeleteFolder(int parameters);
#endregion
#region DASHBOARDS
        [ServiceAction("Получение списка информационных панелей")]
        [OperationContract]
        List<DashboardDTO> GetDashboardList();


        [ServiceAction("Получение полного списка информационных панелей")]
        [OperationContract]
        List<DashboardDTO> GetAllDashboardList();


        [ServiceAction("Добавление информационной панели")]
        [OperationContract]
        int AddDashboard(AddDashboardParameterSet parameters);

        [ServiceAction("Добавление информационной панели c привилегиями")]
        [OperationContract]
        int AddDashboardWithPermission(AddDashboardPermissionParameterSet parameters);

        [ServiceAction("Добавление привилегии панели")]
        [OperationContract]
        void AddDashboardPermission(DashboardPermissionParameterSet parameters);

        [ServiceAction("Скрытие информационной панели от пользователей")]
        [OperationContract]
        void SoftDeleteDashboard(EditDashboardParameterSet parameters);


        [ServiceAction("Удаление информационной панели")]
        [OperationContract]
        void DeleteDashboard(int parameters);


        [ServiceAction("Редактирование информационной панели")]
        [OperationContract]
        void EditDashboard(EditDashboardParameterSet parameters);
#endregion
#region GRANTS
#region permissions
        [ServiceAction("Получить права доступа к информационным панелям")]
        [OperationContract]
        List<DashboardGrantDTO2> GetDashboardPermissionList();

        [ServiceAction("Получить права доступа к информационным панелям")]
        [OperationContract]
        void UpdateDashboardPermissions(UpdateDashboardPermissionsParameterSet parameters);

        [ServiceAction("Получить права доступа к папкам")]
        [OperationContract]
        List<DashboardGrantDTO2> GetFolderPermissionList();

        [ServiceAction("Добавить права доступа к папкам")]
        [OperationContract]
        void AddFolderPermission(DashboardPermissionParameterSet parameters);

        [ServiceAction("Изменить права доступа к папкам")]
        [OperationContract]
        void UpdateFolderPermissions(UpdateDashboardPermissionsParameterSet parameters);
#endregion

        [ServiceAction("Получить список id дашбордов, которые были расшарены всеми пользователями")]
        [OperationContract]
        List<DashboardDTO> GetDashboardUsersSharedList(int parameters);


        [ServiceAction("Получить список id дашбордов, которые были расшарены для пользователя")]
        [OperationContract]
        List<DashboardDTO> GetDashboardUserSharedList(int parameters);


        [ServiceAction("Получить список id дашбордов, которые были расшарены пользователем")]
        [OperationContract]
        List<int> GetDashboardSharedList(int parameters);

        [ServiceAction("Получить права доступа к информационной панели")]
        [OperationContract]
        List<DashboardGrantDTO> GetDashboardGrantList(int parameters);

        [ServiceAction("Изменить права доступа к информационной панели")]
        [OperationContract]
        void UpdateDashboardGrant(UpdateDashboardGrantParameterSet parameters);
        #endregion

        [ServiceAction("Перемещение информационной панели в другую папку")]
        [OperationContract]
        void MoveDashboard(DashboardFolderParameterSet parameters);

        [ServiceAction("Удаление информационной панели для ползователя")]
        [OperationContract]
        void TrashDashboard(int parameters);

        [ServiceAction("Удаление информационной панели")]
        [OperationContract]
        void TrashDashboard2(DashboardFolderParameterSet parameters);

        #region CONTENT
        [ServiceAction("Получение содержимого информационной панели")]
        [OperationContract]
        DashboardContentDTO GetDashboardContent(int parameters);


        [ServiceAction("Изменение содержимого информационной панели")]
        [OperationContract]
        void UpdateDashboardContent(DashboardContentDTO parameters);
        #endregion

        [ServiceAction("Выборка данных для построения панелей")]
        [OperationContract]
        List<SiteDTO> GetEnterpriseSites();


        [ServiceAction("Выборка данных для построения панелей")]
        [OperationContract]
        DashboardDataDTO GetDashboardData(DashboardDataParameterSets parameters);

        [ServiceAction("Обновление порядка сортировки для папок и панелей")]
        [OperationContract]
        void SetSortOrder(List<DashSortOrderDTO> orders);


        [ServiceAction("Добавление информационной панели в папку")]
        [OperationContract]
        void AddDashboardToFolder(DashboardFolderParameterSet parameters);


        [ServiceAction("Получение списка всех папок")]
        [OperationContract]
        List<FolderDTO> GetAllFolderList();


        [ServiceAction("Получение полного списка связей панелей и папок")]
        [OperationContract]
        List<DashboardFoldersDTO> GetDashboardFoldersList();


        [ServiceAction("Перемещение расшаренной панели в папку")]
        [OperationContract]
        void MoveSharedDashboard(DashboardFolderGenericParameterSet parameters);
    }
}
