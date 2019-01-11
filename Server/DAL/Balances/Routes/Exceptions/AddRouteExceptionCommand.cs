using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Routes.Exceptions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes.Exceptions
{
    public class AddRouteExceptionCommand : CommandScalar<AddRouteExceptionParameterSet, int>
    {
        public AddRouteExceptionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddRouteExceptionParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_route_exception_id ");
            command.AddInputParameter("p_route_id", parameters.RouteId);
            command.AddInputParameter("p_gas_owner_id", parameters.OwnerId);
            command.AddInputParameter("p_length", parameters.Length);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRouteExceptionParameterSet parameters)
        {
            return "P_ROUTE_EXCEPTION.AddF";
        }

    }
}