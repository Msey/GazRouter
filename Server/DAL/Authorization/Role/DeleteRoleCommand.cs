using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Role
{

	public class DeleteRoleCommand : CommandNonQuery<int>
	{
		public DeleteRoleCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, int parameters)
	    {
            command.AddInputParameter("p_role_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }

        protected override string GetCommandText(int parameters)
	    {
            return "P_ROLE.Remove";
	    }
	}

}