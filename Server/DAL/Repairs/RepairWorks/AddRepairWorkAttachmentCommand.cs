using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairWorks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.RepairWorks
{
    public class AddRepairWorkAttachmentCommand : CommandScalar<AddRepairWorkAttachmentParameterSet, int>
    {
        public AddRepairWorkAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddRepairWorkAttachmentParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_repair_attachment_id");
            command.AddInputParameter("p_repair_id", parameters.ExternalId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_act_file_name", parameters.FileName);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRepairWorkAttachmentParameterSet parameters)
        {
            return "P_REPAIR_ATTACHMENT.AddF";
        }
    }
}
