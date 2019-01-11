using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.ConsumerContracts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.ConsumerContracts
{
    public class GetConsumerContractListQuery : QueryReader<GetConsumerContractListParameterSet, List<ConsumerContractDTO>>
	{
		public GetConsumerContractListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetConsumerContractListParameterSet parameters)
		{
            var sql = @"    SELECT      c.consumer_contract_id,
                                        c.gas_consumer_id,
                                        c.gas_owner_id,
                                        o.gas_owner_name,
                                        c.start_date,
                                        c.end_date,
                                        c.is_active
                            FROM        v_consumers_contracts c
                            INNER JOIN  v_gas_owners o ON o.gas_owner_id = c.gas_owner_id  
                            WHERE   1=1";

            var sb = new StringBuilder(sql);

            if (parameters != null)
            {
                if (parameters.ConsumerId.HasValue)
                    sb.Append(" AND c.gas_consumer_id = :consumerId");

                if (parameters.IsActive.HasValue)
                    sb.Append(" AND c.is_active = :isActive");

                if (parameters.TheDate.HasValue)
                    sb.Append(@" AND ((:thedate BETWEEN c.start_date AND c.end_date) 
                                    OR (c.start_date <= :thedate AND c.end_date IS NULL)) ");
            }

            sb.Append(" ORDER BY o.sort_order, c.start_date");


            return sb.ToString();
		}

        protected override void BindParameters(OracleCommand command, GetConsumerContractListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.ConsumerId.HasValue)
                    command.AddInputParameter("consumerId", parameters.ConsumerId);

                if (parameters.IsActive.HasValue)
                    command.AddInputParameter("isActive", parameters.IsActive);

                if (parameters.TheDate.HasValue)
                    command.AddInputParameter("thedate", parameters.TheDate);
            }
            
            
        }

        protected override List<ConsumerContractDTO> GetResult(OracleDataReader reader, GetConsumerContractListParameterSet parameters)
		{
			var result = new List<ConsumerContractDTO>();
			while (reader.Read())
			{
				result.Add(new ConsumerContractDTO
                {
                    Id = reader.GetValue<int>("consumer_contract_id"),
                    ConsumerId = reader.GetValue<Guid>("gas_consumer_id"),
                    GasOwnerId = reader.GetValue<int>("gas_owner_id"),
                    GasOwnerName = reader.GetValue<string>("gas_owner_name"),
                    StartDate = reader.GetValue<DateTime>("start_date"),
                    EndDate = reader.GetValue<DateTime?>("end_date"),
                    IsActive = reader.GetValue<bool>("is_active")
                });

			}
			return result;
		}
	}
}