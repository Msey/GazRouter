using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DAL.SeriesData;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.Asdu
{
    public class GetEntityPropertyValueListForAsduQuery :
        QueryReader<GetEntityPropertyValueListParameterSet, List<AsduExchangePropertyValueDTO>>
    {
        public GetEntityPropertyValueListForAsduQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityPropertyValueListParameterSet parameters)
        {
            var q = @"  SELECT      pv.series_id,
                                    pv.entity_id,
                                    pv.property_type_id,
                                    pv.period_type_id,
                                    pv.source_type,
                                    pv.key_date,
                                    pv.value_str, 
                                    pv.value_num, 
                                    pv.value_dat, 
                                    pv.value_type, 
                                    p2a.parameter_gid, 
                                    pt.unit_name
                        FROM        rd.v_property_values_ext pv
                        INNER JOIN  rd.v_property_2_asdu p2a on ((pv.entity_id = p2a.entity_id) and (pv.property_type_id = p2a.property_type_id))
                        INNER JOIN  rd.v_property_types pt on pv.property_type_id = pt.property_type_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            // Если передано значение конкретной SeriesId,
            // то PeriodType, StartDate и EndDate игнорируются
            if (parameters.SeriesId.HasValue)
                sb.Append(" AND pv.series_id = :seriesid");
            else
            {
                sb.Append(" AND pv.period_type_id = :periodtypeid");
                sb.Append(" AND (pv.key_date BETWEEN :startdate and :enddate)");
            }


            if (parameters.EntityIdList.Any())
                sb.Append($" AND pv.entity_id IN {CreateInClause(parameters.EntityIdList.Count)}");


            //sb.Append(" ORDER BY pv.entity_id, pv.property_type_id, pv.key_date");

            return sb.ToString();
        }


        protected override void BindParameters(OracleCommand command, GetEntityPropertyValueListParameterSet parameters)
        {
            if (parameters.SeriesId.HasValue)
                command.AddInputParameter("seriesid", parameters.SeriesId);
            else
            {
                command.AddInputParameter("periodtypeid", parameters.PeriodType);
                command.AddInputParameter("startdate", parameters.StartDate);
                command.AddInputParameter("enddate", parameters.EndDate);
            }

            for (var i = 0; i < parameters.EntityIdList.Count; i++)
                command.AddInputParameter($"p{i}", parameters.EntityIdList[i]);

        }

        protected override List<AsduExchangePropertyValueDTO> GetResult(
            OracleDataReader reader, GetEntityPropertyValueListParameterSet parameters)
        {
            var result = new List<AsduExchangePropertyValueDTO>();
            while (reader.Read())
            {
                var val = PropertyValueHelper.GetValue(reader, parameters.CreateEmpty);
                if (val != null)
                    result.Add(new AsduExchangePropertyValueDTO
                    {
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        PropertyTypeId = reader.GetValue<PropertyType>("property_type_id"),
                        ParameterGid = reader.GetValue<Guid>("parameter_gid"),
                        PropertyValue = val, 
                        UnitName = reader.GetValue<string>("unit_name") 
                    });

            }
            return result;
        }
    }
}