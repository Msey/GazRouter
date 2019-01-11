using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.BalanceItems;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.GasOwners
{

	public class GetGasOwnerListQuery : QueryReader<int?, List<GasOwnerDTO>>
	{
		public GetGasOwnerListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override string GetCommandText(int? parameters)
		{
            
            var sql = @"SELECT      go.gas_owner_id, 
                                    go.gas_owner_name, 
                                    go.description,
                                    go.is_local_contract,
                                    go.sort_order,
                                    s2o.system_id,
                                    od.entity_id,
                                    od.item_id                                    
                            
                        FROM        v_gas_owners go
                        LEFT JOIN   v_systems_2_owners s2o 
                            ON      go.gas_owner_id = s2o.gas_owner_id
                        LEFT JOIN   v_owners_disables od
                            ON      go.gas_owner_id = od.gas_owner_id";

            var sb = new StringBuilder(sql);

            if (parameters.HasValue)
            {
                sb.Append(" WHERE s2o.system_id  = :systemid");
            }
            sb.Append(" ORDER BY go.sort_order, go.gas_owner_name");

            return sb.ToString();

		}

        protected override void BindParameters(OracleCommand command, int? parameters)
        {
            if(parameters.HasValue)
                command.AddInputParameter("systemid", parameters.Value);
        }

        protected override List<GasOwnerDTO> GetResult(OracleDataReader reader, int? parameters)
		{
			var gasOwners = new List<GasOwnerDTO>();
		    GasOwnerDTO owner = null;
            while (reader.Read())
            {
                var ownerId = reader.GetValue<int>("gas_owner_id");
                if (owner == null || owner.Id != ownerId)
                {
                    owner = new GasOwnerDTO
                    {
                        Id = ownerId,
                        Name = reader.GetValue<string>("gas_owner_name"),
                        SortOrder = reader.GetValue<int>("sort_order"),
                        Description = reader.GetValue<string>("description"),
                        IsLocalContract = reader.GetValue<bool>("is_local_contract")
                    };
                    gasOwners.Add(owner);
                }

                var systemId = reader.GetValue<int?>("system_id");
                if (systemId.HasValue)
                    owner.SystemList.Add(systemId.Value);
                
                var entityId = reader.GetValue<Guid?>("entity_id");
                if (entityId.HasValue)
                    owner.DisableList.Add(
                        new GasOwnerDisableDTO
                        {
                            EntityId = entityId.Value,
                            BalanceItem = reader.GetValue<BalanceItem>("item_id")
                        });
            }
			return gasOwners;
		}
	}
}
