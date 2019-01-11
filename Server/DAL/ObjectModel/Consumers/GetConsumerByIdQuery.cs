using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Consumers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Consumers
{
    public class GetConsumerByIdQuery : QueryReader<Guid, ConsumerDTO>
	{
		public GetConsumerByIdQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
        {
            return @"       SELECT      c.gas_consumer_id, 
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
                            LEFT JOIN  v_distr_networks dn ON dn.distr_network_id = c.distr_network_id
                            WHERE       c.gas_consumer_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override ConsumerDTO GetResult(OracleDataReader reader, Guid parameters)
		{
			var result = new ConsumerDTO();
			while (reader.Read())
			{
				result = new ConsumerDTO
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
                };
			}
			return result;
		}
	}
}