using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DAL.SeriesData;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.DataExchange.Integro.SessionData;

namespace GazRouter.DAL.DataExchange.Integro.SessionData
{
    public class SessionDataQuery :
        QueryReader<SessionDataParameterSet, List<IntegroExchangePropertyDto>>
    {
        public SessionDataQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(SessionDataParameterSet parameters)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append($@"  SELECT      
            pv.entity_id,
            pa.entity_name,
            pv.property_type_id,
            pv.period_type_id,
            pv.source_type,
            pv.key_date,
            pv.value_str, 
            pv.value_num, 
            pv.value_dat, 
            pv.value_type, 
            sp.PARAMETER_GID,
            sp.AGGREGATE,
            pt.unit_name,
            pt.description,            
            pv.series_id
            FROM        
            rd.v_property_values_ext pv 
            inner join rd.v_nm_all pa on pa.entity_id=pv.entity_id
            inner join rd.v_property_types pt on pt.property_type_id=pv.property_type_id
            inner join integro.V_SUMMARY_PARAMETER_CONTENT spc on pv.entity_id = spc.entity_id and pv.property_type_id = spc.property_type_id
            inner join integro.V_SUMMARY_PARAMETERS sp on spc.summary_parameter_id=sp.summary_parameter_id
            WHERE 
            1=1 ");

            // Если передано значение конкретной SeriesId,
            // то PeriodType, StartDate и EndDate игнорируются
            if (parameters.SeriesId.HasValue)
                queryBuilder.Append(" AND pv.series_id = :seriesid");
            else
            {
                queryBuilder.Append(" AND pv.period_type_id = :periodtypeid");
                queryBuilder.Append(" AND (pv.key_date BETWEEN :startdate and :enddate)");
            }
            if (parameters.EntityIdList.Any())
                queryBuilder.Append($" AND pv.entity_id IN {CreateInClause(parameters.EntityIdList.Count)}");
            if (parameters.SummaryId.HasValue)
                queryBuilder.Append($" AND sp.SUMMARY_ID =:summaryid");
            //sb.Append(" ORDER BY pv.entity_id, pv.property_type_id, pv.key_date");
            return queryBuilder.ToString();
        }


        protected override void BindParameters(OracleCommand command, SessionDataParameterSet parameters)
        {
            if (parameters.SeriesId.HasValue)
                command.AddInputParameter("seriesid", parameters.SeriesId);
            else
            {
                command.AddInputParameter("periodtypeid", (int) parameters.PeriodType);
                command.AddInputParameter("startdate", parameters.StartDate);
                command.AddInputParameter("enddate", parameters.EndDate);
            }
            if (parameters.SummaryId.HasValue)
                command.AddInputParameter("summaryid", parameters.SummaryId);
            for (var i = 0; i < parameters.EntityIdList.Count; i++)
                command.AddInputParameter($"p{i}", parameters.EntityIdList[i]);
        }

        protected override List<IntegroExchangePropertyDto> GetResult(
            OracleDataReader reader, SessionDataParameterSet parameters)
        {
            var result = new List<IntegroExchangePropertyDto>();
            while (reader.Read())
            {
                var val = PropertyValueHelper.GetValue(reader, parameters.CreateEmpty);
                if (val == null) continue;
                var dataItem = new IntegroExchangePropertyDto
                {
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    EntityName = reader.GetValue<string>("entity_name"),
                    PropertyType = new ExchangeSummaryProperty()
                    {
                        Id = reader.GetValue<int>("property_type_id"),
                        UnitName = reader.GetValue<string>("unit_name"),
                        Description = reader.GetValue<string>("description"),
                    },
                    PropertyValue = val,
                    SeriesId = reader.GetValue<int>("series_id"),
                };
                dataItem.ParameterGidString = reader.GetValue<string>("parameter_gid");
                result.Add(dataItem);
            }
            return result;
        }
    }
}