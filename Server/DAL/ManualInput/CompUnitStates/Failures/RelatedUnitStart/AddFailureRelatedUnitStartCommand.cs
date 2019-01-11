using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates.Failures.RelatedUnitStart
{
    public class AddFailureRelatedUnitStartCommand: CommandNonQuery<AddFailureRelatedUnitStartParameterSet>
    {
        public AddFailureRelatedUnitStartCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddFailureRelatedUnitStartParameterSet parameters)
		{
            command.AddInputParameter("p_comp_unit_state_change_id", parameters.StateChangeId);
            command.AddInputParameter("p_comp_unit_failure_detail_id", parameters.FailureDetailId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

        protected override string GetCommandText(AddFailureRelatedUnitStartParameterSet parameters)
        {
            return "rd.P_COMP_UNIT_STATE_CHANGE.Add_RELATED";
        }
        
    }
}