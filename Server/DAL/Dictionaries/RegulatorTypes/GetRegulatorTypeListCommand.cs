using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.RegulatorTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.RegulatorTypes
{
    public class GetRegulatorTypeListQuery : QueryReader<List<RegulatorTypeDTO>>
    {
		public GetRegulatorTypeListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return @"SELECT * FROM	V_REGULATOR_TYPES";
        }

		protected override List<RegulatorTypeDTO> GetResult(OracleDataReader reader)
        {
			var complexList = new List<RegulatorTypeDTO>();
            while (reader.Read())
            {
                complexList.Add(
					new RegulatorTypeDTO
                    {
						Id = reader.GetValue<int>("REGULATOR_TYPES_ID"),
                        Name = reader.GetValue<string>("REGULATOR_TYPE_NAME"),
						Description = reader.GetValue<string>("DESCRIPTION"),
						GasConsumptionRate = reader.GetValue<double>("GAS_CONSUMPTION_RATE"),
						SortOrder = reader.GetValue<int>("SORT_ORDER")
                    });
            }
            return complexList;
        }
    }
}
