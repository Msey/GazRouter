using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.HeaterTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.HeaterTypes
{
	public class GetHeaterTypesListQuery : QueryReader<List<HeaterTypeDTO>>
	{
        public GetHeaterTypesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
                @"select heater_type_id, heater_type_name, description, gas_consumption_rate, efficiency_factor_rated from v_heater_types";
		}

        protected override List<HeaterTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<HeaterTypeDTO>();
			while (reader.Read())
			{
				var periodType =
                   new HeaterTypeDTO
				   {
                       Id = reader.GetValue<int>("heater_type_id"),
                       Name = reader.GetValue<string>("heater_type_name"),
                       Description = reader.GetValue<string>("description"),
                       GasConsumptionRate = reader.GetValue<double>("gas_consumption_rate"),
                       EffeciencyFactorRated = reader.GetValue<double?>("efficiency_factor_rated")
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}