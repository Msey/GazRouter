using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.PropertyBindings;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.PropertyBindings
{
	public class EditPropertyBindingCommand : CommandNonQuery<EditPropertyBindingParameterSet>
    {
        public EditPropertyBindingCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditPropertyBindingParameterSet parameters)
        {

            command.AddInputParameter("p_property_bindings_id", parameters.Id);
            command.AddInputParameter("p_source_id", parameters.SourceId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyId);
            command.AddInputParameter("p_ext_property_type_id", parameters.ExtEntityId);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(EditPropertyBindingParameterSet parameters)
        {
            return "P_PROPERTY_BINDING.Edit";
        }

    }

}
