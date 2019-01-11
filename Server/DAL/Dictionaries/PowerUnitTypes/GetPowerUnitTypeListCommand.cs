using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PowerUnitTypes
{
	public class GetPowerUnitTypeListQuery : QueryReader<List<PowerUnitTypeDTO>>
	{
        public GetPowerUnitTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
    	    return
                @"  SELECT  power_unit_type_id,
                            power_unit_type_name,
                            description,
                            engine_type_name,
                            engine_group,
                            engine_group_name,
                            rated_power,
                            fuel_consumption_rate
                    FROM    rd.V_POWER_UNIT_TYPES";
		}

        protected override List<PowerUnitTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<PowerUnitTypeDTO>();
			while (reader.Read())
			{
				var periodType =
                   new PowerUnitTypeDTO
				   {
                       Id = reader.GetValue<int>("power_unit_type_id"),
                       Name = reader.GetValue<string>("power_unit_type_name"),
                       Description = reader.GetValue<string>("description"),
                       FuelConsumptionRate = reader.GetValue<double>("fuel_consumption_rate"),
                       RatedPower = reader.GetValue<double>("rated_power"),
                       EngineGroup = (EngineGroup)reader.GetValue<int>("engine_group"),
                       EngineGroupName = reader.GetValue<string>("engine_group_name"),
                       EngineTypeName = reader.GetValue<string>("engine_type_name")
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}