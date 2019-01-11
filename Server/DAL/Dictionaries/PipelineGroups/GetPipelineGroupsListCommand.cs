using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineGroups;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PipelineGroups
{
	public class GetPipelineGroupListQuery : QueryReader<List<PipelineGroupDTO>>
	{
		public GetPipelineGroupListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
			return @"   SELECT      pipeline_group_id, 
                                    entity_type_id, 
                                    pipeline_group_name,
                                    is_virtual 
                        FROM        rd.v_pipeline_groups 
                        ORDER BY    pipeline_group_name";
		}

		protected override List<PipelineGroupDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<PipelineGroupDTO>();
			while (reader.Read())
			{
				result.Add(new PipelineGroupDTO
				{
					Id = reader.GetValue<Guid>("pipeline_group_id"),
					Name = reader.GetValue<string>("pipeline_group_name"),
					EntityType = reader.GetValue<EntityType>("entity_type_id"),
					Path = reader.GetValue<string>("pipeline_group_name"),
					ShortPath = reader.GetValue<string>("pipeline_group_name"),
					IsVirtual = reader.GetValue<bool>("is_virtual"),
				});

			}
			return result;
		}
	}
}