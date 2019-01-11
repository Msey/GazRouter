using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Routes.Exceptions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes.Exceptions
{
    public class EditRouteExceptionCommand : CommandNonQuery<EditRouteExceptionParameterSet>
    {
        public EditRouteExceptionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditRouteExceptionParameterSet parameters)
        {
            command.AddInputParameter("p_route_exception_id", parameters.RouteExceptionId);
            command.AddInputParameter("p_gas_owner_id", parameters.OwnerId);
            command.AddInputParameter("p_length", parameters.Length);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditRouteExceptionParameterSet parameters)
        {
            return "P_ROUTE_EXCEPTION.Edit";
        }

    }
}