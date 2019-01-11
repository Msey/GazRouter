using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.BalanceSigns
{
	public class GetBalanceSignsListQuery : QueryReader<List<BalanceSignDTO>>
	{
		public GetBalanceSignsListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
			return @"SELECT	    balance_sign_id
	                                    ,balance_sign_name
                            FROM	    V_BALANCE_SIGNS";
		}

		protected override List<BalanceSignDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<BalanceSignDTO>();
			while (reader.Read())
			{
				var periodType =
				   new BalanceSignDTO
				   {
					   Id = reader.GetValue<int>("balance_sign_id"),
					   Name = reader.GetValue<string>("balance_sign_name"),
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}