using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Consumers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Consumers
{
    public class GetConsumerListQuery : QueryReader<GetConsumerListParameterSet, List<ConsumerDTO>>
	{
		public GetConsumerListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetConsumerListParameterSet parameters)
        {
            var sql = @"    SELECT      c.gas_consumer_id, 
                                        c.gas_consumer_name, 
                                        c.distr_station_id, 
                                        c.consumer_type_id, 
                                        c.region_id,
                                        c.system_id,
                                        c.sort_order,
                                        c.use_in_balance,
                                        c.description,
                                        c.distr_network_id,
                                        dn.distr_network_name                                        
                            FROM        rd.v_gas_consumers c
                            LEFT JOIN   v_distr_networks dn ON dn.distr_network_id = c.distr_network_id
                            WHERE       1=1";
                            

            var sb = new StringBuilder(sql);


            if (parameters?.DistrStationId != null)
                sb.Append(" AND c.distr_station_id  = :stationId");

            if (parameters?.SystemId != null)
                sb.Append(" AND c.system_id  = :systemId");

            sb.Append(" ORDER BY c.sort_order, c.gas_consumer_name");

            return sb.ToString();
		}

        protected override void BindParameters(OracleCommand command, GetConsumerListParameterSet parameters)
        {
            if (parameters?.DistrStationId != null)
                command.AddInputParameter("stationId", parameters.DistrStationId);
            if (parameters?.SystemId != null)
                command.AddInputParameter("systemId", parameters.SystemId);
        }

        protected override List<ConsumerDTO> GetResult(OracleDataReader reader, GetConsumerListParameterSet parameters)
		{
			var result = new List<ConsumerDTO>();
			while (reader.Read())
			{
				result.Add(new ConsumerDTO
				{
                    Id = reader.GetValue<Guid>("gas_consumer_id"),
                    ParentId = reader.GetValue<Guid>("distr_station_id"),
                    Name = reader.GetValue<string>("gas_consumer_name"),
                    ConsumerTypeId = reader.GetValue<int>("consumer_type_id"),
                    DistrStationId = reader.GetValue<Guid>("distr_station_id"),
                    RegionId = reader.GetValue<int>("region_id"),
                    SystemId = reader.GetValue<int>("system_id"),
                    DistrNetworkId = reader.GetValue<int?>("distr_network_id"),
                    DistrNetworkName = reader.GetValue<string>("distr_network_name"),
                    UseInBalance = reader.GetValue<bool>("use_in_balance"),
					SortOrder = reader.GetValue<int>("sort_order"),
                    Description = reader.GetValue<string>("description")
                });

			}
			return result;
		}
	}
}