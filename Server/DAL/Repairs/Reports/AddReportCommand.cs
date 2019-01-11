using GazRouter.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DTO.Repairs.RepairReport;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Reports
{
    public class AddReportCommand : CommandScalar<RepairReportParametersSet, int>
    {
        public AddReportCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, RepairReportParametersSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_repair_reports_id");
            command.AddInputParameter("p_repair_id", parameters.RepairID);
            command.AddInputParameter("p_creation_date", DateTime.Now);
            command.AddInputParameter("p_report_date", parameters.ReportDate);
            command.AddInputParameter("p_description", parameters.Comment);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(RepairReportParametersSet parameters)
        {
            return "P_REPAIR_REPORT.AddF";
        }
    }
}
