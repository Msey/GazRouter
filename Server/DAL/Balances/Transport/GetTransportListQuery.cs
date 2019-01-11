using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Transport;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Transport
{

    public class GetTransportListQuery : QueryReader<HandleTransportListParameterSet, List<TransportDTO>>
	{
        public GetTransportListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override List<TransportDTO> GetResult(OracleDataReader reader, HandleTransportListParameterSet parameters)
		{
            var result = new List<TransportDTO>();
            while (reader.Read())
            {
                result.Add(
                    new TransportDTO
                    {
                        OwnerId = reader.GetValue<int>("gas_owner_id"),
                        OwnerName = reader.GetValue<string>("gas_owner_name"),
                        InletId = reader.GetValue<Guid>("inlet_id"),
                        OutletId = reader.GetValue<Guid>("outlet_id"),
                        RouteId = reader.GetValue<int?>("route_id"),
                        InletName = reader.GetValue<string>("inlet_name"),
                        OutletName = reader.GetValue<string>("outlet_name"),
                        OutletType = reader.GetValue<EntityType>("outlet_type_id"),
                        BalanceItem = reader.GetValue<BalanceItem>("item_id"),
                        Volume = reader.GetValue<double>("volume"),
                        Length = reader.GetValue<double>("leng")
                    });
            }
            return result;
		}

        protected override string GetCommandText(HandleTransportListParameterSet parameters)
        {
            return @"   SELECT      t.contract_id,
                                    t.gas_owner_id,
                                    go.gas_owner_name,
                                    t.inlet_id,
                                    t.outlet_id,
                                    t.route_id,
                                    i.entity_name AS inlet_name,
                                    osn.entity_name AS outlet_name,
                                    o.entity_type_id AS outlet_type_id,
                                    t.item_id,
                                    t.volume,  
                                    t.leng                                    

                        FROM        v_bl_transports t
                        INNER JOIN  v_gas_owners go ON go.gas_owner_id = t.gas_owner_id
                        INNER JOIN  v_entities i ON i.entity_id = t.inlet_id
                        INNER JOIN  v_entities o ON o.entity_id = t.outlet_id
                        INNER JOIN  v_nm_short_all osn ON osn.entity_id = t.outlet_id
                        
                        WHERE       1=1
                            AND     t.contract_id=:contractId
                        ORDER BY    go.sort_order,
                                    i.entity_name,
                                    osn.entity_name";
        }

        protected override void BindParameters(OracleCommand command, HandleTransportListParameterSet parameters)
        {
            command.AddInputParameter("contractId", parameters.ContractId);
        }
	}
}
