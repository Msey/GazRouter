﻿using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes.Exceptions
{
    public class DeleteRouteExceptionCommand : CommandNonQuery<int>
    {
        public DeleteRouteExceptionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_route_exception_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_ROUTE_EXCEPTION.Remove";
        }

    }
}