using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DTO.Repairs.RepairReport;

namespace GazRouter.DAL.Repairs.Reports
{
    public class AddReportAttachmentCommand : CommandScalar<RepairReportAttachmentParamentersSet, int>
    {
        public AddReportAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, RepairReportAttachmentParamentersSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("repair_report_attachment_id");
            command.AddInputParameter("p_repair_reports_id", parameters.RepairReportId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_act_file_name", parameters.Filename);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(RepairReportAttachmentParamentersSet parameters)
        {
            return "P_REPAIR_REPORT_ATTACHMENT.AddF";
        }
    }
}
