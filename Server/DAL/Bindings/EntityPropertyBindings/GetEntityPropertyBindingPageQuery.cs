using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
	public class GetEntityPropertyBindingPageQuery : QueryReader<GetEntityPropertyBindingsParameterSet, List<EntityPropertyBindingDTO>>
	{
		public GetEntityPropertyBindingPageQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(GetEntityPropertyBindingsParameterSet parameters)
		{
            const string queryTemplate = @"SELECT PT.NAME property_type_name, pt.Property_Type_id, B.BINDING_ID, b.ext_Key
         from V_ENTITIES e
join V_ENTITY_TYPE_PROPERTIES etp on etp.entity_type_id = e.entity_type_id
join V_PROPERTY_TYPES pt on pt.property_type_id = etp.property_type_id
Left join V_BINDINGS b on b.Entity_id= e.entity_id AND b.Source_id = :sourceId 
AND b.property_type_id =    pt.property_type_id  and b.period_type_id = :periodTypeId
           WHERE e.entity_id = :entityId      
            ";

            var commandText = String.Format(queryTemplate);
			return commandText;
		}

        protected override List<EntityPropertyBindingDTO> GetResult(OracleDataReader reader, GetEntityPropertyBindingsParameterSet parameters)
		{
			var entities = new List<EntityPropertyBindingDTO>();
			while (reader.Read())
			{
				entities.Add(new EntityPropertyBindingDTO
				{
					Id = reader.GetValue<Guid>("binding_id"),
					ExtKey = reader.GetValue<string>("ext_key"),
					PropertyId = reader.GetValue<PropertyType>("Property_Type_id"),
					PropertyName = reader.GetValue<string>("Property_Type_Name"),
				});
			}

	        return entities;
		}

		protected override void BindParameters(OracleCommand command, GetEntityPropertyBindingsParameterSet parameters)
		{
            command.AddInputParameter("sourceId", parameters.SourceId);
            command.AddInputParameter("periodTypeId", parameters.PeriodTypeId);
            command.AddInputParameter("entityId", parameters.EntityId);
		}
	}
}