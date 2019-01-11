using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.ExtendedPropertyValues;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.ExtendedPropertyValues
{
	public class GetExtendedPropertyValuesListQuery : QueryReader<GetExtendedPropertyValuesParameterSet, ExtendedPropertyValuesPageDTO>
	{
		public GetExtendedPropertyValuesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(GetExtendedPropertyValuesParameterSet param)
		{
			const string queryTemplate = @"SELECT
						key_date,
						SERIES_ID,
                        period_type_id,
						ENTITY_ID,
						PROPERTY_TYPE_ID,
						VALUE,
						VALUE_TYPE, 
                        value_str,
                        value_num,
                        value_dat,
						UNIT_NAME ,
						PropertyName,
						Source_Name,
						Source_id,
						Entity_Name,
						Path,
						cnt
				FROM (select 
						vs.key_date,
                        vs.period_type_id,
						pv.SERIES_ID,
						pv.ENTITY_ID,
						pv.PROPERTY_TYPE_ID,
						pv.VALUE,
                        pv.value_str,
                        pv.value_num,
                        pv.value_dat,
						pt.VALUE_TYPE,
						pt.UNIT_NAME ,
						pt.Name PropertyName,
						so.Source_Name,
						so.Source_id,
						et.Entity_Name,
						nm.Entity_name Path,
						count(*) over() cnt,
						row_number() over(order by {1}) rn
            
					FROM rd.v_property_values  pv
					JOIN V_VALUE_SERIES vs ON pv.series_id = vs.series_id
					 JOIN V_PROPERTY_TYPES pt on pt.Property_type_id = pv.PROPERTY_TYPE_ID
           Join V_PROPERTY_BINDINGS pb on pb.property_type_id = PT.PROPERTY_TYPE_ID
           join V_SOURCES so on so.source_id = pb.source_id
           join V_ENTITIES_ALL et on et.Entity_id=PV.Entity_id
           left join V_NM_ALL nm on nm.Entity_id=pv.entity_id
					where (1=1) AND vs.Series_id = :vSeries
          AND SO.SOURCE_ID = :vSource {0})
				WHERE rn BETWEEN :pageNumber * :pageSize + 1 AND ((:pageNumber + 1) * :pageSize) 
					";
			var commandText = String.Format(queryTemplate, GetEntityTypeConditionStrig(param.EntityType), string.Format(" {0} {1} ", GetSortColumnName(param.SortBy), param.SortOrder.ToString("g")));
			return commandText;
		}

		private static string GetSortColumnName(SortBy column)
		{
			switch (column)
			{
				case SortBy.Name:
					return "et.Entity_Name";
				case SortBy.Type:
					return "pt.Name";
				case SortBy.Path:
					return "nm.Entity_name";
				default:
					throw new InvalidEnumArgumentException();
			}
		}

		private static string GetEntityTypeConditionStrig(List<EntityType> entityTypes)
		{
			var types = entityTypes.Any() ? entityTypes : Enum.GetValues(typeof(EntityType)).OfType<EntityType>();
			return string.Format("AND et.entity_type_id in ({0})",
									   string.Join(",", types.Select(et => et.ToString("d"))));
		}

		protected override void BindParameters(OracleCommand command, GetExtendedPropertyValuesParameterSet parameters)
		{
			command.AddInputParameter("vSeries", parameters.SeriesId);
			command.AddInputParameter("vSource", parameters.SourceId);
			command.AddInputParameter("pageNumber", parameters.PageNumber);
			command.AddInputParameter("pageSize", parameters.PageSize);
		}

        protected override ExtendedPropertyValuesPageDTO GetResult(OracleDataReader reader, GetExtendedPropertyValuesParameterSet parameters)
		{
			int count = 0;
			var result = new List<ExtendedPropertyValueDTO>();
			while (reader.Read())
			{
				var entityId = reader.GetValue<Guid>("ENTITY_ID");
				var propertyTypeId = reader.GetValue<int>("PROPERTY_TYPE_ID");
				var seriesId = reader.GetValue<int>("SERIES_ID");
				var unitTypeName = reader.GetValue<string>("UNIT_NAME");
				var propertyName = reader.GetValue<string>("PropertyName");
				var sourceName = reader.GetValue<string>("Source_Name");
				var entityName = reader.GetValue<string>("Entity_Name");
				var path = reader.GetValue<string>("Path");
                var periodType = reader.GetValue<PeriodType>("period_type_id");

				count = reader.GetValue<int>("cnt");
                var propValue = PropertyValueHelper.GetValue(reader, periodType, "value_str", "value_num", "value_dat", "VALUE_TYPE", "key_date");
                if (propValue == null)
                    continue;

                result.Add(new ExtendedPropertyValueDTO
                {
                    EntityId = entityId,
                    PropertyTypeId = propertyTypeId,
                    SeriesId = seriesId,
                    UnitTypeName = unitTypeName,
                    EntityName = entityName,
                    Path = path,
                    PropertyName = propertyName,
                    SourceName = sourceName,
                    Value = propValue
                });
            }
            return new ExtendedPropertyValuesPageDTO
			{
				TotalCount = count,
				Entities = result
			};
		}
	}
}
