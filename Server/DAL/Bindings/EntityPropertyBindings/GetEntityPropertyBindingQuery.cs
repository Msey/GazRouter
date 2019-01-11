using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
    public class GetEntityPropertyBindingQuery : QueryReader<GetEntityPropertyBindingParameterSet, SimpleEntityPropertyBindingDTO>
	{
        public GetEntityPropertyBindingQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(GetEntityPropertyBindingParameterSet parameters)
		{
			return @"
         SELECT  
                b.entity_id,
                b.binding_id,
				b.Source_id,
				b.Property_Type_id,
				b.period_type_id,
				b.ext_key
           FROM V_BINDINGS b 
           WHERE b.ext_key = :extKey and b.Source_id = :source_id
   ";
		}

        protected override SimpleEntityPropertyBindingDTO GetResult(OracleDataReader reader, GetEntityPropertyBindingParameterSet parameters)
        {
            SimpleEntityPropertyBindingDTO binding = null;
			if (reader.Read())
			{
                binding = new SimpleEntityPropertyBindingDTO
				{
					Id = reader.GetValue<Guid>("binding_id"),
					EntityId = reader.GetValue<Guid>("entity_id"),
					SourceId = reader.GetValue<int>("Source_id"),
					ExtKey = reader.GetValue<string>("ext_key"),
                    PropertyTypeId = reader.GetValue<PropertyType>("Property_Type_id"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
				};
			}
            return binding;
        }

        protected override void BindParameters(OracleCommand command, GetEntityPropertyBindingParameterSet parameters)
		{
            command.AddInputParameter("extKey", parameters.ExtKey);
            command.AddInputParameter("source_id", parameters.SourceId);
		}
	}
}