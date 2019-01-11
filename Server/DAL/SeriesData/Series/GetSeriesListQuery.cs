using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.Series;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.Series
{
    public class GetSeriesListQuery : QueryReader<GetSeriesListParameterSet, List<SeriesDTO>>
    {
        public GetSeriesListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, GetSeriesListParameterSet parameters)
        {
            if (parameters == null) return;

            command.AddInputParameter("periodstart", parameters.PeriodStart);
            command.AddInputParameter("periodend", parameters.PeriodEnd);

            if (parameters.PeriodType.HasValue)
                command.AddInputParameter("periodtype", parameters.PeriodType);
            
        }

        protected override string GetCommandText(GetSeriesListParameterSet parameter)
        {
            var q =  @" SELECT      series_id,
                                    key_date,
                                    period_type_id,                                    
                                    description
                        FROM        v_value_series
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameter != null)
            {
                sb.Append(" AND key_date BETWEEN :periodstart AND :periodend");

                if (parameter.PeriodType.HasValue)
                    sb.Append(" AND period_type_id = :periodtype");
            }

            sb.Append(" ORDER BY key_date DESC");

            return sb.ToString();
        }

        protected override List<SeriesDTO> GetResult(OracleDataReader reader, GetSeriesListParameterSet parameters)
        {
            var result = new List<SeriesDTO>();
            while (reader.Read())
            {
                result.Add(new SeriesDTO
                {
                    Id = reader.GetValue<int>("series_id"),
                    KeyDate = reader.GetValue<DateTime>("key_date"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
                    Description = reader.GetValue<string>("description")
                });
            }

            return result;
        }
    }


}