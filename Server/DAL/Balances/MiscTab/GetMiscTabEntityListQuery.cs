using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.MiscTab
{
    public class GetMiscTabEntityListQuery : QueryReader<int, List<CommonEntityDTO>>
    {
        public GetMiscTabEntityListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(int parameters)
        {
            var sql = @"    SELECT      mte.entity_id, 
                                        e.entity_name, 
                                        e.entity_type_id, 
                                        e.bal_name,
                                        p.entity_name AS short_path

                            FROM        v_bl_misc_tab_entities mte
                            INNER JOIN  v_entities_all e ON e.entity_id = mte.entity_id
                            INNER JOIN  v_nm_short_all p ON p.entity_id = e.entity_id
                            
                            WHERE       1=1
                                AND     mte.system_id = :sysId";
            
            return sql;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("sysId", parameters);
        }


        protected override List<CommonEntityDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<CommonEntityDTO>();
            while (reader.Read())
            {
                result.Add(
                    new CommonEntityDTO
                    {
                        Id = reader.GetValue<Guid>("entity_id"),
                        Name = reader.GetValue<string>("entity_name"),
                        ShortPath = reader.GetValue<string>("short_path"),
                        BalanceName = reader.GetValue<string>("bal_name"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id")
                    });
            }
            return result;
        }
    }
}