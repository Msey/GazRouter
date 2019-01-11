using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.InconsistencyTypes
{
	public class GetInconsistencyTypesQuery : QueryReader<List<InconsistencyTypeDTO>>
	{
		public GetInconsistencyTypesQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
			return @"   SELECT  inconsistency_type_id,
							    entity_type_id,
							    inconsistency_type_sys_name,
							    inconsistency_type_name,
							    description,
                                is_critical
                        FROM	v_inconsistency_types";
		}

		protected override List<InconsistencyTypeDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<InconsistencyTypeDTO>();
			while (reader.Read())
			{
                result.Add(
                   new InconsistencyTypeDTO
				   {
					   Id = reader.GetValue<int>("inconsistency_type_id"),
					   EntityTypeId = reader.GetValue<EntityType>("entity_type_id"),
					   SystemName = reader.GetValue<string>("inconsistency_type_sys_name"),
					   Name = reader.GetValue<string>("inconsistency_type_name"),
					   Description = reader.GetValue<string>("description"),
					   IsCritical = reader.GetValue<bool>("is_critical"),
				   });
			}
			return result;
		}
	}
}