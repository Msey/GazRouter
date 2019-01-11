using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PipelineTypes
{
	public class GetPipelineTypesListQuery : QueryReader<List<PipelineTypesDTO>>
	{
		public GetPipelineTypesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
			return
				string.Format(
					@"select t1.pipeline_type_id ID,t1.pipeline_type_name NAME,t1.description DESCRIPTION,t1.sort_order SortORDER from V_PIPELINE_TYPES t1
				where pipeline_type_id in ({0})",
					string.Join(",", Enum.GetValues(typeof (PipelineType)).Cast<int>()));
		}

		protected override List<PipelineTypesDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<PipelineTypesDTO>();
			while (reader.Read())
			{
				var periodType =
				   new PipelineTypesDTO
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