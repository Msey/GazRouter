using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DAL.SeriesData;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PhysicalTypes
{
	public class GetPhysicalTypeListQuery : QueryReader<List<PhysicalTypeDTO>>
	{
        public GetPhysicalTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
            return @"   SELECT      phisical_type_id,
                                    sys_name,
                                    phisical_type_name,
                                    value_type,
                                    unit_name,
                                    default_precision,
                                    trend_allowed,
                                    value_min,
                                    value_max
                        FROM	    v_phisical_types
                        ORDER BY    sort_order";
		}

        protected override List<PhysicalTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<PhysicalTypeDTO>();
			while (reader.Read())
			{
                var phisicalType = new PhysicalTypeDTO
                {
                    Id = reader.GetValue<int>("phisical_type_id"),
                    SysName = reader.GetValue<string>("sys_name"),
                    Name = reader.GetValue<string>("phisical_type_name"),
                    UnitName = reader.GetValue<string>("unit_name"),
                    ValueType = PropertyValueHelper.GetValueType(reader, "value_type"),
                    DefaultPrecision = reader.GetValue<int>("default_precision"),
                    ValueMin = reader.GetValue<double?>("value_min"),
                    ValueMax = reader.GetValue<double?>("value_max"),
                    TrendAllowed = reader.GetValue<bool>("trend_allowed")
                };
                result.Add(phisicalType);
				
			}
			return result;
		}
	}
}