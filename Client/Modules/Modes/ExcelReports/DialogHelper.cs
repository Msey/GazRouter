using System;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.Folders;
using AddEditFolderView = 
    GazRouter.Modes.ProcessMonitoring.Dashboards.AddEditFolder.AddEditFolderView;
namespace GazRouter.Modes.ExcelReports
{
    public static class DialogHelper
    {
        public static void AddReport(int? folderId, Guid site, Action<int> callback)
        {
            var viewModel = new AddEditExcelReportsViewModel(folderId, site, callback);
            var view = new AddEditExcelReportsView { DataContext = viewModel };
            view.ShowDialog();
        }
        public static void EditReport(DashboardDTO model, Action<int> callback)
        {
            var viewModel = new AddEditExcelReportsViewModel(callback,model);
            var view = new AddEditExcelReportsView { DataContext = viewModel };
            view.ShowDialog();
        }
    }
}