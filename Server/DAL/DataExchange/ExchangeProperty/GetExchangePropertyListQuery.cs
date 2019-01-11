using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeProperty
{
    public class GetExchangePropertyListQuery : QueryReader<GetExchangeEntityListParameterSet, List<ExchangePropertyDTO>>
    {
        public GetExchangePropertyListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetExchangeEntityListParameterSet parameters)
        {
            var q = @"   SELECT     t.exchange_task_id,
                                    t.entity_id,
                                    t.property_type_id,
                                    t.ext_id,
                                    t.coeff,
                                    e.entity_type_id
                                                                        
                        FROM        v_exchange_properties t
                        INNER JOIN  v_exchange_entities ee ON ee.entity_id = t.entity_id AND ee.exchange_task_id = t.exchange_task_id
                        INNER JOIN  v_entities e ON t.entity_id = e.entity_id
                        INNER JOIN  v_entity_type_properties pt ON e.entity_type_id = pt.entity_type_id AND t.property_type_id = pt.property_type_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.ExchangeTaskIdList.Any())
                    sb.Append($" AND t.exchange_task_id IN {CreateInClause(parameters.ExchangeTaskIdList.Count)}");
                if (parameters.IsActive) sb.Append(" AND ee.is_active = 1");
            }

            sb.Append(" ORDER BY pt.sort_order");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetExchangeEntityListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.ExchangeTaskIdList.Any())
                    for (var i = 0; i < parameters.ExchangeTaskIdList.Count; i++)
                        command.AddInputParameter($"p{i}", parameters.ExchangeTaskIdList[i]);
            }
        }

        protected override List<ExchangePropertyDTO> GetResult(OracleDataReader reader, GetExchangeEntityListParameterSet parameters)
        {
            var result = new List<ExchangePropertyDTO>();
            while (reader.Read())
            {
                result.Add(new ExchangePropertyDTO
                {
                    ExchangeTaskId = reader.GetValue<int>("exchange_task_id"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    EntityTypeId = reader.GetValue<EntityType>("entity_type_id"),
                    PropertyTypeId = reader.GetValue<PropertyType>("property_type_id"),
                    ExtId = reader.GetValue<string>("ext_id"),
                    Coeff = reader.GetValue<double?>("coeff")
                });
            }
            return result;
        }
    }
}