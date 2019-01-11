using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.PropertyBindings;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.PropertyBindings
{
	public class AddPropertyBindingCommand : CommandScalar<AddPropertyBindingParameterSet, Guid>
    {
        public AddPropertyBindingCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AddPropertyBindingParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("entity_binding_id");
            command.AddInputParameter("p_source_id", parameters.SourceId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyId);
            command.AddInputParameter("p_ext_property_type_id", parameters.ExtEntityId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddPropertyBindingParameterSet parameters)
        {
            return "P_PROPERTY_BINDING.AddF";
        }

    }
}