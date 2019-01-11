using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.BoilerTypes
{
	public class GetBoilerTypeListQuery : QueryReader<List<BoilerTypeDTO>>
	{
        public GetBoilerTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
                @"  SELECT      boiler_type_id,
                                boiler_type_name,
                                description,
                                rated_heating_efficiency,
                                rated_efficiency_factor,
                                heating_area,
                                is_small,
                                group_name
                    FROM        V_BOILER_TYPES
                    ORDER BY    group_name, boiler_type_name";
		}

        protected override List<BoilerTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<BoilerTypeDTO>();
			while (reader.Read())
			{
				var periodType =
                   new BoilerTypeDTO
				   {
                       Id = reader.GetValue<int>("boiler_type_id"),
                       Name = reader.GetValue<string>("boiler_type_name"),
                       Description = reader.GetValue<string>("description"),
                       RatedHeatingEfficiency = reader.GetValue<double>("rated_heating_efficiency"),
                       RatedEfficiencyFactor = reader.GetValue<double>("rated_efficiency_factor"),
                       HeatingArea = reader.GetValue<double>("heating_area"),
                       IsSmall = reader.GetValue<bool>("is_small"),
                       GroupName = reader.GetValue<string>("group_name"),
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}