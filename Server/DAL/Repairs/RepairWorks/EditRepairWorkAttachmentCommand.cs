using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairWorks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.RepairWorks
{
    public class EditRepairWorkAttachmentCommand :CommandNonQuery<EditRepairWorkAttachmentParameterSet>
    {
        public EditRepairWorkAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditRepairWorkAttachmentParameterSet parameters)
        {
            command.AddInputParameter("p_repair_attachment_id", parameters.AttachmentId);
            command.AddInputParameter("p_repair_id", parameters.ExternalId);
            command.AddInputParameter("p_description", parameters.Description);
            if (parameters.Data != null) command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_act_file_name", parameters.FileName);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditRepairWorkAttachmentParameterSet parameters)
        {
            return "P_REPAIR_ATTACHMENT.Edit";
        }
    }
}
