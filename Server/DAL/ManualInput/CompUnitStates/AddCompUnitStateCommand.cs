using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates
{
    public class AddCompUnitStateCommand: CommandScalar<AddCompUnitStateParameterSet, int>
    {
        public AddCompUnitStateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddCompUnitStateParameterSet parameters)
		{
            OutputParameter = command.AddReturnParameter<int>("p_comp_unit_state_change_id");

            command.AddInputParameter("p_comp_unit_id", parameters.CompUnitId);
            command.AddInputParameter("p_state_change_date", parameters.StateChangeDate);
            command.AddInputParameter("p_state", parameters.State);
            command.AddInputParameter("p_comp_unit_stop_type_id", parameters.StopType);

            command.AddInputParameter("p_is_repair_next", parameters.IsRepairNext);

            command.AddInputParameter("p_comp_unit_repair_type_id", parameters.RepairType);
            command.AddInputParameter("p_completion_date_plan", parameters.RepairCompletionDate);

            command.AddInputParameter("p_is_critical", parameters.IsCritical);
            command.AddInputParameter("p_failure_cause_id", parameters.FailureCause);
            command.AddInputParameter("p_failure_feature_id", parameters.FailureFeature);
            command.AddInputParameter("p_external_view", parameters.FailureExternalView);
            command.AddInputParameter("p_cause_description", parameters.FailureCauseDescription);
            command.AddInputParameter("p_work_performed", parameters.FailureWorkPerformed);
            
            command.AddInputParameter("p_user_name ", Context.UserIdentifier);


		}

        protected override string GetCommandText(AddCompUnitStateParameterSet parameters)
        {
            return "rd.P_COMP_UNIT_STATE_CHANGE.AddF";
        }
        
    }
}