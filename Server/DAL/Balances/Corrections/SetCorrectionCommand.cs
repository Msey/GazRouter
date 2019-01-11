using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Corrections;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Corrections
{
    public class SetCorrectionCommand : CommandNonQuery<SetCorrectionParameterSet>
    {
		public SetCorrectionCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, SetCorrectionParameterSet parameters)
        {
            command.AddInputParameter("p_value_id", parameters.BalanceValueId);
            command.AddInputParameter("p_doc_id", parameters.DocId);
            command.AddInputParameter("p_value_corrections", parameters.Value);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetCorrectionParameterSet parameters)
		{
			return "P_BL_VALUE.set_it_corrections";
		}
    }
}