using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.BalanceItems;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.GasOwners
{

	public class GetGasOwnerDisableListQuery : QueryReader<List<GasOwnerDisableDTO>>
	{
		public GetGasOwnerDisableListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override string GetCommandText()
        {

            return @"   SELECT  gas_owner_id,
                                entity_id,
                                item_id                            
                        FROM    v_owners_disables";
        }
        
        protected override List<GasOwnerDisableDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<GasOwnerDisableDTO>();
		    
            while (reader.Read())
            {
                result.Add(
                    new GasOwnerDisableDTO
                    {
                        OwnerId = reader.GetValue<int>("gas_owner_id"),
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        BalanceItem = reader.GetValue<BalanceItem>("item_id")
                    });
            }
			return result;
		}
	}
}
