using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PeriodTypes
{
	public class GetPeriodTypesListQuery : QueryReader<List<PeriodTypeDTO>>
	{
		public GetPeriodTypesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
		        @"SELECT	    PERIOD_TYPE_id
	                                    ,PERIOD_TYPE_name
										,Sys_name
                            FROM	    V_Period_Types";
		}

		protected override List<PeriodTypeDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<PeriodTypeDTO>();
			while (reader.Read())
			{
				var periodType =
				   new PeriodTypeDTO
				   {
					   Id = reader.GetValue<int>("PERIOD_TYPE_id"),
					   Name = reader.GetValue<string>("PERIOD_TYPE_name"),
					   SysName = reader.GetValue<string>("Sys_name"),
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}