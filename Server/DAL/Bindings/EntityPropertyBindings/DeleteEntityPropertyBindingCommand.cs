using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
    public class DeleteEntityPropertyBindingCommand : CommandNonQuery<Guid>
    {
		public DeleteEntityPropertyBindingCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_binding_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "P_BINDING.Remove";
        }
    }
}