using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Dictionaries.BalanceItems;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.SortOrder
{
    public class GetSortOrderListQuery : QueryReader<List<BalSortOrderDTO>>
	{
		public GetSortOrderListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText()
        {
            
            var sql = @"    SELECT      entity_id,
                                        bal_item_id,
                                        sort_order                                        
                            FROM        v_bl_sort_orders";

            return sql;
        }
        

        protected override List<BalSortOrderDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<BalSortOrderDTO>();
			while (reader.Read())
			{
				result.Add(
                    new BalSortOrderDTO
                    {
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        BalItem = reader.GetValue<BalanceItem>("bal_item_id"),
                        SortOrder = reader.GetValue<int>("sort_order")
                    });
			}
			return result;
		}
	}
}