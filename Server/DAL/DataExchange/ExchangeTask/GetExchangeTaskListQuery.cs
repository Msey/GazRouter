using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace GazRouter.DAL.DataExchange.ExchangeTask
{
    public class GetExchangeTaskListQuery : QueryReader<GetExchangeTaskListParameterSet, List<ExchangeTaskDTO>>
    {
        public GetExchangeTaskListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetExchangeTaskListParameterSet parameters)
        {
            var q = @"  SELECT      t.exchange_task_id,
                                    t.name,
                                    t.exchange_type_id,
                                    t.exchange_lag,
                                    t.source_id,
                                    s.source_name,
                                    t.period_type_id,
                                    t.is_critical,
                                    t.file_name_mask,
                                    t.is_plsql,
                                    t.plsql_procedure,
                                    t.is_transform,
                                    t.transformation,
                                    t.transport_type_id,
                                    t.transport_address,
                                    t.transport_login,
                                    t.transport_password,
                                    t.enterprise_id,
                                    t.secure_key,
                                    t.send_as_attachment,
                                    t.exchange_status,
                                    t.exclude_hours,
                                    e.code
                        FROM        v_exchange_tasks t
                        INNER JOIN  v_sources s ON s.source_id = t.source_id
                        LEFT JOIN   v_enterprises e ON t.enterprise_id = e.enterprise_id 
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.Id.HasValue)
                    sb.Append(" AND t.exchange_task_id = :id");

                if (parameters.Ids != null && parameters.Ids.Any())
                    sb.Append(string.Format(" AND t.exchange_task_id in {0}", CreateInClause(parameters.Ids.Count)));

                if (parameters.SourceId.HasValue)
                    sb.Append(" AND t.source_id = :sourceid");
                
                if (parameters.ExchangeTypeId.HasValue)
                    sb.Append(" AND t.exchange_type_id = :exchangeTypeId");
                
                if (parameters.EnterpriseId.HasValue)
                    sb.Append(" AND t.enterprise_id = :enterpriseId");
                
                if (parameters.PeriodTypeId.HasValue)
                    sb.Append(" AND t.period_type_id = :periodTypeId");
            }

            sb.Append(" ORDER BY t.source_id, t.name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetExchangeTaskListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.Id.HasValue)
                    command.AddInputParameter("id", parameters.Id);

                if (parameters.Ids != null)
                {
                    for (var i = 0; i < parameters.Ids.Count; i++)
                    {
                        command.AddInputParameter(string.Format("p{0}", i), parameters.Ids[i]);
                    }
                }

                if (parameters.SourceId.HasValue)
                    command.AddInputParameter("sourceid", parameters.SourceId);

                if (parameters.ExchangeTypeId.HasValue)
                    command.AddInputParameter("exchangeTypeId", parameters.ExchangeTypeId);

                if (parameters.EnterpriseId.HasValue)
                    command.AddInputParameter("enterpriseId", parameters.EnterpriseId.GetValueOrDefault());

                if (parameters.PeriodTypeId.HasValue)
                    command.AddInputParameter("periodTypeId", parameters.PeriodTypeId);
            }
            
        }

        protected override List<ExchangeTaskDTO> GetResult(OracleDataReader reader, GetExchangeTaskListParameterSet parameters)
        {
            var entities = new List<ExchangeTaskDTO>();
            while (reader.Read())
            {
                entities.Add(new ExchangeTaskDTO
                {
                    Id = reader.GetValue<int>("exchange_task_id"),
                    Name = reader.GetValue<string>("name"),
                    ExchangeTypeId = reader.GetValue<ExchangeType>("exchange_type_id"),
                    ExchangeStatus = reader.GetValue<ExchangeStatus?>("exchange_status") ?? ExchangeStatus.Off, // на случай если будут null
                    Lag = reader.GetValue<int>("exchange_lag"),
                    DataSourceId = reader.GetValue<int>("source_id"),
                    DataSourceName = reader.GetValue<string>("source_name"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
                    IsCritical = reader.GetValue<bool>("is_critical"),
                    FileNameMask = reader.GetValue<string>("file_name_mask"),
                    IsSql = reader.GetValue<bool>("is_plsql"),
                    SqlProcedureName = reader.GetValue<string>("plsql_procedure"),
                    IsTransform = reader.GetValue<bool>("is_transform"),
                    Transformation = reader.GetValue<string>("transformation"),
                    TransportTypeId = reader.GetValue<TransportType?>("transport_type_id"),
                    TransportAddress = reader.GetValue<string>("transport_address"),
                    TransportLogin = reader.GetValue<string>("transport_login"),
                    TransportPassword = reader.GetValue<string>("transport_password"),
                    SendAsAttachment = reader.GetValue<bool>("send_as_attachment"),
                    EnterpriseId = reader.GetValue<Guid?>("enterprise_id"),
                    EnterpriseCode = reader.GetValue<string>("code"),
                    HostKey = reader.GetValue<string>("secure_key"),
                    ExcludeHours = reader.GetValue<string>("exclude_hours"),                    
                });
            }
            return entities;
        }
    }
}