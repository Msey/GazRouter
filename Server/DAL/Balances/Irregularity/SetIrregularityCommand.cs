using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Irregularity;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Irregularity
{
    public class SetIrregularityCommand : CommandNonQuery<SetIrregularityParameterSet>
    {
		public SetIrregularityCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, SetIrregularityParameterSet parameters)
        {
            command.AddInputParameter("p_value_id", parameters.BalanceValueId);
            command.AddInputParameter("p_start_day", parameters.StartDayNum);
            command.AddInputParameter("p_end_day", parameters.EndDayNum);
            command.AddInputParameter("p_value", parameters.Value);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetIrregularityParameterSet parameters)
		{
			return "P_BL_IRREGULARITY.Add";
		}
    }
}