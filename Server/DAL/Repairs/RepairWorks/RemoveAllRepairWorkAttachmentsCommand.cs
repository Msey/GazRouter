using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairWorks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.RepairWorks
{
    public class RemoveAllRepairWorkAttachmentsCommand :CommandNonQuery<int>
    {
        public RemoveAllRepairWorkAttachmentsCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_repair_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_REPAIR_ATTACHMENT.Remove";
        }
    }
}
