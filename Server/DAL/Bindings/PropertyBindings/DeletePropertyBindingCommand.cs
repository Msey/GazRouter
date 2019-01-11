using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.PropertyBindings
{
    public class DeletePropertyBindingCommand : CommandNonQuery<Guid>
    {
		public DeletePropertyBindingCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_property_bindings_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "P_PROPERTY_BINDING.Remove";

        }

    }
}