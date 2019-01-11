using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.Workflow;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Repair
{
    public class ChangeStateCommand : CommandNonQuery<ChangeRepairWfParametrSet>
    {
		public ChangeStateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, ChangeRepairWfParametrSet parameters)
        {
            command.AddInputParameter("p_repair_id", parameters.RepairId);            

            command.AddInputParameter("p_repair_state", parameters.WState);
            command.AddInputParameter("p_workflow_state", parameters.WFState);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(ChangeRepairWfParametrSet parameters)
        {
            return "P_REPAIR.Edit_State";
        }
    }
}
