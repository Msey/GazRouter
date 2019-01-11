using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ManualInput.PipelineLimits.Attachment
{
    public class EditPipelineLimitAttachmentCommand : CommandNonQuery<AddAttachmentParameterSet<int>>
    {
        public EditPipelineLimitAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddAttachmentParameterSet<int> parameters)
        {
            command.AddInputParameter("p_limit_attachment_id", parameters.ExternalId);
            //command.AddInputParameter("p_limit_id", parameters.);
            command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_act_file_name", parameters.FileName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddAttachmentParameterSet<int> parameters)
        {
            return "P_PIPLINE_LIMIT_ATTACHMENT.Edit";
        }
    }
}
