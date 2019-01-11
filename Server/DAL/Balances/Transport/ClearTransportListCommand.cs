using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Transport;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Transport
{
    public class ClearTransportListCommand : CommandNonQuery<HandleTransportListParameterSet>
    {
        public ClearTransportListCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, HandleTransportListParameterSet parameters)
        {
            command.AddInputParameter("p_contract_id", parameters.ContractId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(HandleTransportListParameterSet parameters)
		{
            return "P_BL_TRANSPORT.Remove";
		}

    }
}