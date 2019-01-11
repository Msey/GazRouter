using System.Collections.Generic;
using GazRouter.DataServices.Dashboards;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Dashboards.Dashboards;
using GazRouter.DAL.ExcelReport;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.ExcelReports;

namespace GazRouter.DataServices.ExcelReports
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class ExcelReportService : ServiceBase, IExcelReportService
    {
        public int AddExcelReport(AddDashboardParameterSet parameters)
        {
            int id;
            using (var context = OpenDbContext())
            {
                id = new AddExcelReportCommand(context).Execute(parameters);
                if (parameters.SortOrderParam != null)
                {
                    parameters.SortOrderParam.Id = id;

                    {
                        new SetDashboardForderSortOrderCommand(context).Execute(parameters.SortOrderParam);
                    }
                }
            }
            return id;

        }

        public ExcelReportContentDTO GetExcelReportContent(int parameters)
        {
            return ExecuteRead<GetExcelReportContentQuery, ExcelReportContentDTO, int>(parameters);
        }

        public void UpdateExcelReportContent(ExcelReportContentDTO parameters)
        {
            ExecuteNonQuery<UpdateExcelReportContentCommand, ExcelReportContentDTO>(parameters);
        }

        public List<DashboardDTO> GetExcelReportList(int parameters)
        {
            return ExecuteRead<GetExcelReportListQuery, List<DashboardDTO>, int>(parameters);
        }
    }
}