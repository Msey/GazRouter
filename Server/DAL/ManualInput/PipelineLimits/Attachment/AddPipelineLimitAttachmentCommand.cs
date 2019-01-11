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
    public class AddPipelineLimitAttachmentCommand : CommandScalar<AddAttachmentParameterSet<int>, int>
    {
        public AddPipelineLimitAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddAttachmentParameterSet<int> parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_limit_attachment_id");
            command.AddInputParameter("p_limit_id", parameters.ExternalId);
            command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_act_file_name", parameters.FileName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);

        }

        protected override string GetCommandText(AddAttachmentParameterSet<int> parameters)
        {
            return "rd.p_pipline_limit_attachment.addf";
        }
    }
}
