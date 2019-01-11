using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityBindings;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityBindings
{
	public class EditEntityBindingCommand : CommandNonQuery<EditEntityBindingParameterSet>
    {
        public EditEntityBindingCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditEntityBindingParameterSet parameters)
        {

            command.AddInputParameter("p_entity_binding_id", parameters.Id);
            command.AddInputParameter("p_source_id", parameters.SourceId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_ext_entity_id", parameters.ExtEntityId);
            command.AddInputParameter("p_is_active", parameters.IsActive);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(EditEntityBindingParameterSet parameters)
        {
            return "P_ENTITY_BINDING.Edit";
        }

    }

}
