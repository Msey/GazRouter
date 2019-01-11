using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DAL.SeriesData;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PropertyTypes
{
	public class GetPropertyTypeListQuery : QueryReader<List<PropertyTypeDTO>>
	{
        public GetPropertyTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
            return @"   SELECT      prop.property_type_id,
                                    prop.phisical_type_id,
                                    prop.name,
                                    prop.description,
                                    prop.sys_name,
                                    prop.short_name,
                                    prop.dict_id,
                                    phy.phisical_type_name,
                                    phy.sys_name AS phy_sys_name,
                                    phy.value_type,
                                    phy.unit_name,
                                    phy.default_precision,
                                    phy.value_min,
                                    phy.value_max,                                    
                                    phy.trend_allowed                                                                                                         
                        FROM	    v_property_types prop 
                        INNER JOIN  v_phisical_types phy ON prop.phisical_type_id = phy.phisical_type_id
                        ORDER BY    name";
		}

        protected override List<PropertyTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<PropertyTypeDTO>();
            while (reader.Read())
			{
                var stateSetId = reader.GetValue<StateSet?>("dict_id");

			    var propertyType = stateSetId.HasValue
			        ? new PropertyTypeDictDTO {StateSetId = stateSetId.Value}
			        : new PropertyTypeDTO();

                propertyType.Id = reader.GetValue<int>("property_type_id");
                propertyType.PhysicalTypeId = reader.GetValue<PhysicalType>("phisical_type_id");
                propertyType.Name = reader.GetValue<string>("name");
                propertyType.Description = reader.GetValue<string>("description");
                propertyType.SysName = reader.GetValue<string>("sys_name");
                propertyType.ShortName = reader.GetValue<string>("short_name");


			    propertyType.PhysicalType = new PhysicalTypeDTO
			    {
			        Id = reader.GetValue<int>("phisical_type_id"),
                    SysName = reader.GetValue<string>("phy_sys_name"),
			        Name = reader.GetValue<string>("phisical_type_name"),
			        UnitName = reader.GetValue<string>("unit_name"),
			        ValueType = PropertyValueHelper.GetValueType(reader, "value_type"),
			        DefaultPrecision = reader.GetValue<int>("default_precision"),
                    ValueMin = reader.GetValue<double?>("value_min"),
                    ValueMax = reader.GetValue<double?>("value_max"),
                    TrendAllowed = reader.GetValue<bool>("trend_allowed")
                };


                result.Add(propertyType);
				
			}
			return result;
		}
	}
}