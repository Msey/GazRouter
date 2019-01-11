using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.ValvePurposes
{
	public class GetValvePurposeListQuery : QueryReader<List<ValvePurposeDTO>>
	{
		public GetValvePurposeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return @"select VALVE_PURPOSE_ID,VALVE_PURPOSE_NAME, DESCRIPTION,SORT_ORDER from V_VALVE_PURPOSES";
		}

		protected override List<ValvePurposeDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<ValvePurposeDTO>();
			while (reader.Read())
			{
				var periodType =
					new ValvePurposeDTO
						{
							Id = reader.GetValue<int>("VALVE_PURPOSE_ID"),
							Name = reader.GetValue<string>("VALVE_PURPOSE_NAME"),
							Description = reader.GetValue<string>("DESCRIPTION"),
							SortOrder = reader.GetValue<int>("SORT_ORDER")
						};
				result.Add(periodType);
			}
			return result;
		}
	}
}