﻿using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
	public class AddEntityPropertyBindingCommand : CommandScalar<AddEntityPropertyBindingParameterSet, Guid>
    {
		public AddEntityPropertyBindingCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

		protected override void BindParameters(OracleCommand command, AddEntityPropertyBindingParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("binding_id");
            command.AddInputParameter("p_source_id", parameters.SourceId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyId);
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_ext_key", parameters.ExtKey);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddEntityPropertyBindingParameterSet parameters)
        {
            return "P_BINDING.AddF";
        }

    }
}