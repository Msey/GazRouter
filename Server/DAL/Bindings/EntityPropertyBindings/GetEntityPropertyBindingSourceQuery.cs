using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
	public class GetEntityPropertyBindingSourceQuery : QueryReader<GetEntityPropertyBindingSourceParameterSet, SimpleEntityPropertyBindingDTO>
	{
		public GetEntityPropertyBindingSourceQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(GetEntityPropertyBindingSourceParameterSet parameters)
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
           WHERE b.entity_id = :entity 
				and b.Source_id = :source_id
				and b.Property_Type_id = :propertyType
				and b.period_type_id = :period
   ";
		}

        protected override SimpleEntityPropertyBindingDTO GetResult(OracleDataReader reader, GetEntityPropertyBindingSourceParameterSet parameters)
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

		protected override void BindParameters(OracleCommand command, GetEntityPropertyBindingSourceParameterSet parameters)
		{
			command.AddInputParameter("entity", parameters.EntityId);
			command.AddInputParameter("source_id", parameters.SourceId);
			command.AddInputParameter("propertyType", parameters.PropertyTypeId);
			command.AddInputParameter("period", parameters.PeriodTypeId);
		}
	}
}