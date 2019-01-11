using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.PropertyValues
{
    public class GetEntityPropertyValueListQuery :
        QueryReader
            <GetEntityPropertyValueListParameterSet,
                Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>>
    {
        public GetEntityPropertyValueListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityPropertyValueListParameterSet parameters)
        {
            var q = @"  SELECT      pv.series_id,
                                    pv.entity_id,
                                    pv.property_type_id,
                                    pv.period_type_id,
                                    pv.key_date,
                                    pv.value_str, 
                                    pv.value_num, 
                                    pv.value_dat, 
                                    pv.value_type,
                                    pv.source_type                                                
                        FROM        rd.v_property_values_ext pv
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            // Если передано значение конкретной SeriesId,
            // то PeriodType, StartDate и EndDate игнорируются
            if (parameters.SeriesId.HasValue)
                sb.Append(" AND pv.series_id = :seriesid");
            else
            {
                sb.Append(" AND pv.period_type_id = :periodtypeid");

                if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                    sb.Append(" AND pv.key_date BETWEEN :startdate and :enddate");

                if (parameters.Year.HasValue)
                    sb.Append(" AND EXTRACT(year from pv.key_date) = :year");

                if (parameters.Month.HasValue)
                    sb.Append(" AND EXTRACT(month from pv.key_date) = :month");

                if (parameters.Day.HasValue)
                    sb.Append(" AND EXTRACT(day from pv.key_date) = :day");
            }


            if (parameters.EntityIdList.Count > 0)
                sb.Append($" AND pv.entity_id IN {CreateInClause(parameters.EntityIdList.Count, "e")}");

            if (parameters.PropertyList.Count > 0)
                sb.Append($" AND pv.property_type_id IN {CreateInClause(parameters.PropertyList.Count, "p")}");


            sb.Append(" ORDER BY pv.entity_id, pv.property_type_id, pv.key_date");

            return sb.ToString();
        }


        protected override void BindParameters(OracleCommand command, GetEntityPropertyValueListParameterSet parameters)
        {
            if (parameters.SeriesId.HasValue)
                command.AddInputParameter("seriesid", parameters.SeriesId);
            else
            {
                command.AddInputParameter("periodtypeid", parameters.PeriodType);

                if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                {
                    command.AddInputParameter("startdate", parameters.StartDate);
                    command.AddInputParameter("enddate", parameters.EndDate);
                }

                if (parameters.Year.HasValue)
                    command.AddInputParameter("year", parameters.Year);

                if (parameters.Month.HasValue)
                    command.AddInputParameter("month", parameters.Month);

                if (parameters.Day.HasValue)
                    command.AddInputParameter("day", parameters.Day);
            }

            for (var i = 0; i < parameters.EntityIdList.Count; i++)
                command.AddInputParameter($"e{i}", parameters.EntityIdList[i]);

            for (var i = 0; i < parameters.PropertyList.Count; i++)
                command.AddInputParameter($"p{i}", parameters.PropertyList[i]);

        }

        protected override Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> GetResult(
            OracleDataReader reader, GetEntityPropertyValueListParameterSet parameters)
        {
            var result = new Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>();
            Guid? entityId = null;
            PropertyType? propertyId = null;
            List<BasePropertyValueDTO> values = null;
            while (reader.Read())
            {
                var newEntityId = reader.GetValue<Guid>("entity_id");
                var newPropertyId = reader.GetValue<PropertyType>("property_type_id");

                if (entityId == null || newEntityId != entityId)
                {
                    entityId = newEntityId;
                    propertyId = newPropertyId;
                    values = new List<BasePropertyValueDTO>();
                    var dict = new Dictionary<PropertyType, List<BasePropertyValueDTO>>
                    {
                        {newPropertyId, values}
                    };
                    result.Add(newEntityId, dict);
                }
                else
                {
                    if (newPropertyId != propertyId.Value)
                    {
                        values = new List<BasePropertyValueDTO>();
                        propertyId = newPropertyId;
                        result[entityId.Value].Add(newPropertyId, values);
                    }
                }
                var val = PropertyValueHelper.GetValue(reader, parameters.CreateEmpty);
                if (val != null)
                    values.Add(val);

            }
            return result;
        }
    }
}
