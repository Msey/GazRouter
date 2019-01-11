using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityBindings
{
	public class DeleteEntityBindingCommand : CommandNonQuery<Guid>
    {
		public DeleteEntityBindingCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_entity_binding_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "P_ENTITY_BINDING.Remove";

        }

    }
}