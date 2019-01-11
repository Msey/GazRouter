using GazRouter.DAL.Core;
using GazRouter.DTO.ExcelReports;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ExcelReport
{
    public class UpdateExcelReportContentCommand : CommandNonQuery<ExcelReportContentDTO>
    {
        public UpdateExcelReportContentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, ExcelReportContentDTO parameters)
        {
            command.AddInputParameter("P_DASHBOARD_ID", parameters.ReportId);
            command.AddInputParameter("P_REPORT_DATA", parameters.Content);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(ExcelReportContentDTO parameters)
        {
            return "rd.P_DASHBOARD.Edit";
        }

    }
}