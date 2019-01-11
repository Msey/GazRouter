using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.PropertyBindings;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.PropertyBindings
{
	public class GetPropertyEntitiesPageQuery : QueryReader<GetPropertyBindingsParameterSet, List<BindingDTO>>
	{
		public GetPropertyEntitiesPageQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(GetPropertyBindingsParameterSet parameters)
		{
            const string queryTemplate = @"         
            SELECT  e.Property_type_id,
                 e.Name,
				b.Property_bindings_id,
				
				b.ext_property_type_id

           FROM  V_PROPERTY_TYPES e
			{0} join V_PROPERTY_BINDINGS b on b.Property_type_id = e.Property_type_id AND b.Source_id = :sourceId
           WHERE (1=1)
 ";

			var commandText = String.Format(queryTemplate, GetShowAllConditionStrig(parameters.ShowAll));
			return commandText;
		}

		private static string GetShowAllConditionStrig(bool showAll)
		{
			return showAll ? "left" : string.Empty;
		}

        protected override List<BindingDTO> GetResult(OracleDataReader reader, GetPropertyBindingsParameterSet parameters)
		{
			var entities = new List<BindingDTO>();
			while (reader.Read())
			{
				entities.Add(new BindingDTO
				{
					Id = reader.GetValue<Guid>("Property_bindings_id"),
					Name = reader.GetValue<string>("Name"),
					Path = string.Empty,
					PropertyId = reader.GetValue<PropertyType>("Property_type_id"),
				
					ExtEntityId = reader.GetValue<string>("ext_property_type_id"),
				});
			}

		    return entities;
		}

		protected override void BindParameters(OracleCommand command, GetPropertyBindingsParameterSet parameters)
		{
            command.AddInputParameter("sourceId", parameters.SourceId);
		}
	}
}