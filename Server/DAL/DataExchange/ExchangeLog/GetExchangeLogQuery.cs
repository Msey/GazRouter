using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeLog
{
    public class GetExchangeLogQuery : QueryReader<GetExchangeLogParameterSet, List<ExchangeLogDTO>>
    {
        public GetExchangeLogQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetExchangeLogParameterSet parameters)
        {
            var q = @"  SELECT      l.series_id,
                                    s.key_date,
                                    s.period_type_id,
                                    l.exchange_task_id,
                                    l.start_time,
                                    l.is_ok,
                                    l.data_content,
                                    l.processing_error,
                                    t.name AS exchange_task_name,
                                    t.exchange_type_id,
                                    src.source_id,
                                    src.source_name
                        FROM        v_exchange_log l
                        INNER JOIN  v_value_series s ON l.series_id = s.series_id
                        INNER JOIN  v_exchange_tasks t ON t.exchange_task_id = l.exchange_task_id
                        INNER JOIN  v_sources src on src.source_id = t.source_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                    sb.Append(" AND s.key_date BETWEEN :startdate AND :enddate");

                if (parameters.ExchangeTaskId.HasValue)
                    sb.Append(" AND l.exchange_task_id = :taskid");

                if (parameters.SerieId.HasValue)
                    sb.Append(" AND l.series_id = :series_id");

            }

            sb.Append(" ORDER BY l.start_time DESC");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetExchangeLogParameterSet parameters)
        {
            if (parameters != null )
            {
                if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                {
                    command.AddInputParameter("startdate", parameters.StartDate);
                    command.AddInputParameter("enddate", parameters.EndDate);
                }

                if (parameters.ExchangeTaskId.HasValue)
                    command.AddInputParameter("taskid", parameters.ExchangeTaskId);

                if (parameters.SerieId.HasValue)
                    command.AddInputParameter("series_id", parameters.SerieId);
            }
                
        }

        protected override List<ExchangeLogDTO> GetResult(OracleDataReader reader, GetExchangeLogParameterSet parameters)
        {
            var log = new List<ExchangeLogDTO>();
            while (reader.Read())
            {
                log.Add(new ExchangeLogDTO
                {
                    SerieId = reader.GetValue<int>("series_id"),
                    Timestamp = reader.GetValue<DateTime>("key_date"),
                    PeriodType = reader.GetValue<PeriodType>("period_type_id"),
                    SourceId = reader.GetValue<int>("source_id"),
                    SourceName = reader.GetValue<string>("source_name"),
                    ExchangeTaskId = reader.GetValue<int>("exchange_task_id"),
                    ExchangeTaskName = reader.GetValue<string>("exchange_task_name"),
                    ExchangeType = reader.GetValue<ExchangeType>("exchange_type_id"),
                    StartTime = reader.GetValue<DateTime>("start_time"),
                    IsOk = reader.GetValue<bool>("is_ok"),
                    //DataContent = GetString(reader.GetValue<byte[]>("data_content")),
                    ProcessingError = reader.GetValue<string>("processing_error")
                });
            }
            return log;
        }

        static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length/sizeof (char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}