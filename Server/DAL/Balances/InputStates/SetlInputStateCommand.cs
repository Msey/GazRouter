using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.InputStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.InputStates
{
    public class SetInputStateCommand : CommandNonQuery<SetInputStateParameterSet>
    {
        public SetInputStateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetInputStateParameterSet parameters)
        {
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_contract_id", parameters.ContractId);
            command.AddInputParameter("p_state_id", (int)parameters.State);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetInputStateParameterSet parameters)
        {
            return "rd.P_BL_INPUT_STATE.Set_STATUS";
        }
    }
}