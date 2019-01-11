using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.ExcelReports;
namespace GazRouter.DataServices.ExcelReports
{
    [Service("Отчеты")]
    [ServiceContract]
    public interface IExcelReportService
    {
#region Excel Reports
        [ServiceAction("Добавление отчета")]
        [OperationContract]
        int AddExcelReport(AddDashboardParameterSet parameters);

        [ServiceAction("Добавление отчета c привилегиями")]
        [OperationContract]
        int AddExcelWithPermission(AddDashboardPermissionParameterSet parameters);

        [ServiceAction("Получение содержимого отчета")]
        [OperationContract]
        ExcelReportContentDTO GetExcelReportContent(int parameters);

        [ServiceAction("Изменение содержимого отчета")]
        [OperationContract]
        void UpdateExcelReportContent(ExcelReportContentDTO parameters);
        
        [ServiceAction("Получение полного списка отчетов")]
        [OperationContract]
        List<DashboardDTO> GetAllExcelReportList();

        [ServiceAction("Получение списка отчетов")]
        [OperationContract]
        List<DashboardDTO> GetExcelReportList(int parameters);

        [ServiceAction("Получение объекта по регулярному выражению")]
        [OperationContract]
        GazRouter.DTO.ObjectModel.CommonEntityDTO EvaluateString(string parameters);

        [ServiceAction("Загрузка отчета по шаблону и метке времени")]
        [OperationContract]
        ExcelReportContentDTO EvaluateExcelReport(EvaluateExcelReportContentParameterSet parameters);
        #endregion

        [ServiceAction("Получение списка папок")]
        [OperationContract]
        List<FolderDTO> GetAllExcelReportFolderList();

        [ServiceAction("Получение списка папок")]
        [OperationContract]
        List<FolderDTO> GetExcelReportFolderList(int parameters);

        [ServiceAction("Создание папки")]
        [OperationContract]
        int AddExcelReportFolder(AddFolderParameterSet parameters);

        [ServiceAction("Перемещение папки")]
        [OperationContract]
        void MoveExcelReportFolder(MoveDashboardFolderParameterSet parameters);

        [ServiceAction("Редактирование папки")]
        [OperationContract]
        void EditFolder(EditFolderParameterSet parameters);

        [ServiceAction("Редактирование отчета")]
        [OperationContract]
        void EditDashboard(EditDashboardParameterSet parameters);

        [ServiceAction("Удаление отчета")]
        [OperationContract]
        void DeleteDashboard(int parameters);

        [ServiceAction("Сортировка")]
        [OperationContract]
        void SetSortOrder(SetSortOrderParameterSet parameters);

        [ServiceAction("Удаление папки")]
        [OperationContract]
        void DeleteFolder(int parameters);

        [ServiceAction("Получение прав")]
        [OperationContract]
        List<DashboardGrantDTO> GetDashboardGrantList(int parameters);

        [ServiceAction("Обновление прав")]
        [OperationContract]
        void UpdateDashboardGrant(UpdateDashboardGrantParameterSet parameters);
        
        [ServiceAction("Удаление информационной панели для ползователя")]
        [OperationContract]
        void TrashDashboard(int parameters);

        [ServiceAction("Получить список id дашбордов, которые были расшарены пользователем")]
        [OperationContract]
        List<int> GetExcelReportSharedList(int parameters);


        [ServiceAction("Получить список id дашбордов, которые были расшарены всеми пользователями")]
        [OperationContract]
        List<DashboardDTO> GetExcelReportUsersSharedList(int parameters);


        [ServiceAction("Получить список id дашбордов, которые были расшарены для пользователя")]
        [OperationContract]
        List<DashboardDTO> GetExcelReportUserSharedList(int parameters);
    }
}