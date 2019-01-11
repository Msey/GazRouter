using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CoolingUnitTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.CoolingUnitTypes
{
	public class GetCoolingUnitTypeListQuery : QueryReader<List<CoolingUnitTypeDTO>>
	{
        public GetCoolingUnitTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
    	    return
                @"  SELECT  cooling_unit_type_id,
                            cooling_unit_type_name,
                            description,
                            rated_power,
                            fuel_consumption_rate
                    FROM    rd.V_COOLING_UNIT_TYPES
                    ORDER BY cooling_unit_type_name";
		}

        protected override List<CoolingUnitTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<CoolingUnitTypeDTO>();
			while (reader.Read())
			{
				var periodType =
                   new CoolingUnitTypeDTO
				   {
                       Id = reader.GetValue<int>("cooling_unit_type_id"),
                       Name = reader.GetValue<string>("cooling_unit_type_name"),
                       Description = reader.GetValue<string>("description"),
                       FuelConsumptionRate = reader.GetValue<double>("fuel_consumption_rate"),
                       RatedPower = reader.GetValue<double>("rated_power"),
                   };
				result.Add(periodType);
			}
			return result;
		}
	}
}