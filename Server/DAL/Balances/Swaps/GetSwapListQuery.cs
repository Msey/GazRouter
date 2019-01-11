using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Swaps;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;


namespace GazRouter.DAL.Balances.Swaps
{

	public class GetSwapListQuery : QueryReader<int, List<SwapDTO>>
	{
		public GetSwapListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override string GetCommandText(int parameters)
		{

            return @"   SELECT      s.entity_id, 
                                    e.entity_type_id,
                                    sn.entity_name, 
                                    s.item_id,
                                    s.src_gas_owner_id,
                                    so.gas_owner_name AS src_gas_owner_name,
                                    s.dest_gas_owner_id,
                                    do.gas_owner_name AS dest_gas_owner_name,
                                    s.volume
                        FROM        v_bl_value_swaps s
                        INNER JOIN  v_entities e ON e.entity_id = s.entity_id
                        INNER JOIN  v_nm_short_all sn ON sn.entity_id = s.entity_id
                        INNER JOIN  v_gas_owners so ON so.gas_owner_id = s.src_gas_owner_id
                        INNER JOIN  v_gas_owners do ON do.gas_owner_id = s.dest_gas_owner_id
                        WHERE       s.contract_id = :contractid";
		}

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("contractid", parameters);
        }

	    protected override List<SwapDTO> GetResult(OracleDataReader reader, int parameters)
	    {
	        var result = new List<SwapDTO>();

	        while (reader.Read())
	        {
	            result.Add(
	                new SwapDTO
                    {
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        EntityName = reader.GetValue<string>("entity_name"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id"),
                        BalItem = reader.GetValue<BalanceItem>("item_id"),
                        SrcOwnerId = reader.GetValue<int>("src_gas_owner_id"),
                        SrcOwnerName = reader.GetValue<string>("src_gas_owner_name"),
                        DestOwnerId = reader.GetValue<int>("dest_gas_owner_id"),
                        DestOwnerName = reader.GetValue<string>("dest_gas_owner_name"),
                        Volume = reader.GetValue<double>("volume")
                    });
	        }

            return result;
        }

	    
	}
}
