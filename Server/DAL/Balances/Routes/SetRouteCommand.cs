using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Routes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes
{
    public class SetRouteCommand : CommandScalar<SetRouteParameterSet, int>
    {
        public SetRouteCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, SetRouteParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_route_id");
            command.AddInputParameter("p_inlet_id", parameters.InletId);
            command.AddInputParameter("p_outlet_id", parameters.OutletId);
            command.AddInputParameter("p_length", parameters.Length);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetRouteParameterSet parameters)
		{
            return "P_ROUTE.SetF";
		}

    }
}