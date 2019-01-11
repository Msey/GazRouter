using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Inconsistency
{
	public class DeleteAllInconsistencyCommand : CommandNonQuery
    {
		public DeleteAllInconsistencyCommand(ExecutionContext context)
            : base(context)
		{
			IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command)
		{
			command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
		}

		protected override string GetCommandText()
		{
			return "rd.P_INCONSISTENCY.REMOVE_ALL";
		}
	}
}