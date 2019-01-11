using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.GasCosts;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.GasCosts
{
    public class GetCostTypeListQuery : QueryReader<List<GasCostTypeDTO>>
    {
        public GetCostTypeListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"SELECT 
                                AUX_ITEM_ID, 
                                entity_type_id, 
                                AUX_ITEM_NAME,
                                AUX_ITEM_GROUP_ID,
                                TAB_NUM, 
                                IS_REGULAR
                     FROM v_aux_items i 
                     ORDER BY SORT_ORDER";
        }

        protected override List<GasCostTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<GasCostTypeDTO>();
            while (reader.Read())
            {
                try
                {
                    result.Add(new GasCostTypeDTO
                    {
                        CostType = reader.GetValue<CostType>("aux_item_Id"),
                        CostTypeName = reader.GetValue<string>("AUX_ITEM_NAME"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id"),
                        GroupId = reader.GetValue<int>("AUX_ITEM_GROUP_ID"),
                        TubNum = reader.GetValue<int>("TAB_NUM"),
                        IsRegular = reader.GetValue<int?>("IS_REGULAR"),
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return result;
        }
    }
}