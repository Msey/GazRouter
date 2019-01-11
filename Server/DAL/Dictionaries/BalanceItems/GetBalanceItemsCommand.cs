using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.BalanceItems
{
	public class GetBalanceItemsListQuery : QueryReader<List<BalanceItemDTO>>
	{
		public GetBalanceItemsListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
			return @"SELECT	    item_id,
                                item_name,
                                balance_sign_id
                    FROM	    v_bl_items";
		}

		protected override List<BalanceItemDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<BalanceItemDTO>();
			while (reader.Read())
			{
				var item =
				   new BalanceItemDTO
                   {
					   Id = reader.GetValue<int>("item_id"),
					   Name = reader.GetValue<string>("item_name"),
                       BalanceSign = reader.GetValue<Sign>("balance_sign_id")
                   };
				result.Add(item);
			}
			return result;
		}
	}
}