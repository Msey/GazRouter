using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Values
{

	public class GetBalanceValueListQuery : QueryReader<GetBalanceValueListParameterSet, List<BalanceValueDTO>>
	{
		public GetBalanceValueListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override string GetCommandText(GetBalanceValueListParameterSet parameters)
		{

            var sql =  @"   SELECT      v.value_id,
                                        v.contract_id,
                                        v.gas_owner_id,
                                        v.entity_id,
                                        v.entity_type_id,
                                        c.distr_station_id,
                                        v.item_id,
                                        v.value_base,
                                        v.value_correct
                            FROM        v_bl_values v
                            LEFT JOIN   v_gas_consumers c ON c.gas_consumer_id = v.entity_id
                            WHERE       contract_id = :contractId";

            var sb = new StringBuilder(sql);
            if (parameters.SiteId.HasValue)
                sb.Append(" AND rd.P_ENTITY.GetSiteId(v.entity_id) = :siteId");

            return sb.ToString();
		}

        protected override void BindParameters(OracleCommand command, GetBalanceValueListParameterSet parameters)
        {
            command.AddInputParameter("contractId", parameters.ContractId);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId);
        }

	    protected override List<BalanceValueDTO> GetResult(OracleDataReader reader, GetBalanceValueListParameterSet parameters)
	    {
	        var result = new List<BalanceValueDTO>();

	        while (reader.Read())
	        {
	            result.Add(
	                new BalanceValueDTO
                    {
	                    Id = reader.GetValue<int>("value_id"),
	                    ContractId = reader.GetValue<int>("contract_id"),
	                    GasOwnerId = reader.GetValue<int>("gas_owner_id"),
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        EntityType = reader.GetValue<EntityType?>("entity_type_id"),
                        DistrStationId = reader.GetValue<Guid?>("distr_station_id"),
                        BalanceItem = reader.GetValue<BalanceItem>("item_id"),
                        BaseValue = Math.Round(reader.GetValue<double>("value_base"), 3),
                        Correction = reader.GetValue<double?>("value_correct"),
                    });
	        }

            return result;
        }

	    
	}
}
