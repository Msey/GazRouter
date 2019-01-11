using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.Asdu
{
    public class GetAsduPropertyListQuery : QueryReader<GetAsduEntityListParameterSet, List<AsduPropertyDTO>>
    {
        public GetAsduPropertyListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetAsduEntityListParameterSet parameters)
        {
            var q = @"  SELECT      t.parameter_gid,
                                    t.entity_id,
                                    t.property_type_id,
                                    e.entity_type_id,
                                    ea.object_gid                                    
                        FROM        v_property_2_asdu t
                        INNER JOIN  v_entities e ON t.entity_id = e.entity_id
                        LEFT JOIN  v_entity_2_asdu ea ON t.entity_id = ea.entity_id
                        INNER JOIN  v_entity_type_properties pt ON e.entity_type_id = pt.entity_type_id AND t.property_type_id = pt.property_type_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            //if (parameters != null)
            //{
            //    if (parameters.AsduIdList.Any())
            //        sb.Append($" AND exchange_task_id IN {CreateInClause(parameters.AsduIdList.Count)}");
            //}

            sb.Append(" ORDER BY pt.sort_order");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetAsduEntityListParameterSet parameters)
        {
            //if (parameters != null && parameters.AsduIdList.Any())
            //    for (var i = 0; i < parameters.AsduIdList.Count; i++)
            //        command.AddInputParameter($"p{i}", parameters.AsduIdList[i]);
        }

        protected override List<AsduPropertyDTO> GetResult(OracleDataReader reader, GetAsduEntityListParameterSet parameters)
        {
            var result = new List<AsduPropertyDTO>();
            while (reader.Read())
            {
                result.Add(new AsduPropertyDTO
                {
                    ParameterGid = reader.GetValue<Guid>("parameter_gid"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    PropertyTypeId = reader.GetValue<PropertyType>("property_type_id"),
                    EntityGid = reader.GetValue<Guid?>("object_gid")
                });
            }
            return result;
        }
    }
}