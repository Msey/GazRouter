using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairReport;
using Oracle.ManagedDataAccess.Client;
using System;

namespace GazRouter.DAL.Repairs.Reports
{
    public class EditReportCommand : CommandNonQuery<RepairReportParametersSet>
    {
        public EditReportCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, 
RepairReportParametersSet parameters)
        {
            command.AddInputParameter("p_repair_reports_id", parameters.Id);
            command.AddInputParameter("p_repair_id", parameters.RepairID);
            command.AddInputParameter("p_creation_date", DateTime.Now);
            command.AddInputParameter("p_report_date", parameters.ReportDate);
            command.AddInputParameter("p_description", parameters.Comment);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(RepairReportParametersSet parameters)
        {
            return "P_REPAIR_REPORT.Edit";
        }
    }
}
