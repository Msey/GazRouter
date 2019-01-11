using System;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.Series;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.Series
{
    public class GetSeriesQuery : QueryReader<GetSeriesParameterSet, SeriesDTO>
    {
        public GetSeriesQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, GetSeriesParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.Id.HasValue)
                command.AddInputParameter("id", parameters.Id);
            else
            {
                if (parameters.PeriodType.HasValue)
                    command.AddInputParameter("period", parameters.PeriodType);

                if (parameters.TimeStamp.HasValue)
                    command.AddInputParameter("timestamp", parameters.TimeStamp);    
            }

            

        }

        protected override string GetCommandText(GetSeriesParameterSet parameters)
        {
            var q = @"  SELECT      s.series_id, 
                                    s.key_date, 
                                    s.period_type_id,
                                    s.description
                        FROM        v_value_series s                                        
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.Id.HasValue)
                    sb.Append(" AND s.series_id = :id");
                else
                {
                    sb.Append(@" AND s.series_id = 
                                    (   SELECT  MAX(series_id) 
                                        KEEP    (dense_rank first ORDER BY key_date DESC)
                                        FROM    v_value_series
                                        WHERE   1=1");
                    
                    if (parameters.PeriodType.HasValue)
                        sb.Append(" AND period_type_id = :period");
                    
                    if (parameters.TimeStamp.HasValue)
                        sb.Append(" AND key_date = :timestamp");

                    sb.Append(")");

                }
            }

            

            return sb.ToString();
        }

        protected override SeriesDTO GetResult(OracleDataReader reader, GetSeriesParameterSet parameters)
        {
            if (reader.Read())
            {
                return new SeriesDTO
                {
                    Id = reader.GetValue<int>("series_id"),
                    KeyDate = reader.GetValue<DateTime>("key_date"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
                    Description = reader.GetValue<string>("description"),
                };
            }
            return null;
        }
    }
}