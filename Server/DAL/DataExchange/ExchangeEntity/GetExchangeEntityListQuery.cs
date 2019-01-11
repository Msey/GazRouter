using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeEntity
{
    public class GetExchangeEntityListQuery : QueryReader<GetExchangeEntityListParameterSet, List<ExchangeEntityDTO>>
    {
        public GetExchangeEntityListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetExchangeEntityListParameterSet parameters)
        {
            var q = @"  SELECT      t.exchange_task_id,
                                    t.entity_id,
                                    e.entity_name,
                                    p.entity_name AS entity_path,
                                    e.entity_type_id,
                                    t.is_active,
                                    t.ext_id                                    
                        FROM        v_exchange_entities t
                        INNER JOIN  v_entities e ON t.entity_id = e.entity_id
                        INNER JOIN  v_nm_short_all p ON t.entity_id = p.entity_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.ExchangeTaskIdList.Any())
                    sb.Append($" AND exchange_task_id IN {CreateInClause(parameters.ExchangeTaskIdList.Count)}");

                if (parameters.IsActive)
                    sb.Append(" AND t.is_active = :is_active");

                if (parameters.EntityType.HasValue)
                    sb.Append(" AND e.entity_type_id = :eType");

                if (parameters.EntityId.HasValue)
                    sb.Append(" AND t.entity_id = :entityId");
            }

            sb.Append(" ORDER BY p.entity_name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetExchangeEntityListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.ExchangeTaskIdList.Any())
                    for (var i = 0; i < parameters.ExchangeTaskIdList.Count; i++)
                        command.AddInputParameter($"p{i}", parameters.ExchangeTaskIdList[i]);

                if (parameters.IsActive)
                    command.AddInputParameter("is_active", parameters.IsActive);

                if (parameters.EntityType.HasValue)
                    command.AddInputParameter("eType", parameters.EntityType);

                if (parameters.EntityId.HasValue)
                    command.AddInputParameter("entityId", parameters.EntityId);
            }

        }

        protected override List<ExchangeEntityDTO> GetResult(OracleDataReader reader, GetExchangeEntityListParameterSet parameters)
        {
            var result = new List<ExchangeEntityDTO>();
            while (reader.Read())
            {
                result.Add(new ExchangeEntityDTO
                {
                    ExchangeTaskId = reader.GetValue<int>("exchange_task_id"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    EntityName = reader.GetValue<string>("entity_name"),
                    EntityPath = reader.GetValue<string>("entity_path"),
                    EntityTypeId = reader.GetValue<EntityType>("entity_type_id"),
                    IsActive = reader.GetValue<bool>("is_active"),
                    ExtId = reader.GetValue<string>("ext_id")
                });
            }
            return result;
        }
    }
}