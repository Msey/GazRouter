using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PipelineEndType
{
	public class GetPipelineEndTypeListQuery : QueryReader<List<PipelineEndTypeDTO>>
	{
		public GetPipelineEndTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
		        @"select t1.end_type_id ID,t1.end_type_name NAME,t1.description DESCRIPTION,t1.sort_order SortORDER from V_PIPELINE_END_TYPES t1";
		}

		protected override List<PipelineEndTypeDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<PipelineEndTypeDTO>();
			while (reader.Read())
			{
				var periodType =
				   new PipelineEndTypeDTO
				   {
					   Id = reader.GetValue<int>("ID"),
					   Name = reader.GetValue<string>("NAME"),
					   Description = reader.GetValue<string>("DESCRIPTION"),
					   SortOrder = reader.GetValue<int>("SortORDER")
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}