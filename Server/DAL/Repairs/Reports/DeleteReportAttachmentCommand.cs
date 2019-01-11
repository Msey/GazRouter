using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Repairs.Reports
{
    public class DeleteReportAttachmentCommand : CommandNonQuery<int>
    {
        public DeleteReportAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("repair_report_attachment_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_REPAIR_REPORT_ATTACHMENT.Remove";
        }
    }
}
