using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Action
{

	public class DeleteActionCommand : CommandNonQuery<int>
	{
        public DeleteActionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
	    {
            command.AddInputParameter("p_action_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }

        protected override string GetCommandText(int parameters)
	    {
            return "P_ACTION.Remove";
	    }
	}

}