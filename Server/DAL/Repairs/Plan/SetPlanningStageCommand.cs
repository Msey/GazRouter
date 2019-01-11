using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Plan
{
    public class SetPlanningStageCommand : CommandNonQuery<SetPlanningStageParameterSet>
    {
        public SetPlanningStageCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetPlanningStageParameterSet parameters)
        {
            command.AddInputParameter("p_system_id", parameters.SystemId);
            command.AddInputParameter("p_year", parameters.Year);
            command.AddInputParameter("p_lock_status", parameters.Stage);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetPlanningStageParameterSet parameters)
        {
            return "P_REPAIR.Set_Lock";
        }
    }
}
