using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ManualInput.PipelineLimits.Attachment
{
    public class DeletePipelineLimitAttachmentCommand : CommandNonQuery<int>
    {
        public DeletePipelineLimitAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_limit_attachment_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(int parameters)
        {
            return "rd.P_PIPLINE_LIMIT_ATTACHMENT.Remove";
        }
    }
}
