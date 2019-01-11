using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes
{
	public class DeleteRouteCommand : CommandNonQuery<int>
	{
        public DeleteRouteCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, int parameters)
	    {
            command.AddInputParameter("p_route_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }

        protected override string GetCommandText(int parameters)
	    {
            return "P_ROUTE.Remove";
	    }
	}
}