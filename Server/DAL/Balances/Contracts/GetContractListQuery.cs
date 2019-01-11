using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Contracts
{

	public class GetContractListQuery : QueryReader<GetContractListParameterSet, List<ContractDTO>>
	{
		public GetContractListQuery(ExecutionContext context)
			: base(context)
		{ }

		protected override string GetCommandText(GetContractListParameterSet parameter)
		{
			var q = @"  SELECT      contract_id, 
						            period_type_id, 
						            target_id, 
						            contract_date,
                                    system_id,
                                    is_final
					    FROM        v_bl_contracts
					    WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameter.ContractId.HasValue)
		        sb.Append(" AND contract_id = :id");

            if (parameter.ContractDate.HasValue)
                sb.Append(" AND contract_date = :thedate");
            
            if (parameter.SystemId.HasValue)
                sb.Append(" AND system_id = :systemid");

            if (parameter.TargetId.HasValue)
                sb.Append(" AND target_id = :targetid");

            if (parameter.PeriodTypeId.HasValue)
                sb.Append(" AND period_type_id = :periodid");

            if (parameter.IsFinal.HasValue)
                sb.Append(" AND is_final = :isfinal");

            return sb.ToString();
		}

		protected override void BindParameters(OracleCommand command, GetContractListParameterSet parameters)
		{
            if (parameters.ContractId.HasValue)
                command.AddInputParameter("id", parameters.ContractId);

            if (parameters.ContractDate.HasValue)
                command.AddInputParameter("thedate", parameters.ContractDate);

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemid", parameters.SystemId);

            if (parameters.TargetId.HasValue)
                command.AddInputParameter("targetid", parameters.TargetId);

            if (parameters.PeriodTypeId.HasValue)
                command.AddInputParameter("periodid", parameters.PeriodTypeId);

            if (parameters.IsFinal.HasValue)
                command.AddInputParameter("isfinal", parameters.IsFinal);
        }

        protected override List<ContractDTO> GetResult(OracleDataReader reader, GetContractListParameterSet parameters)
        {
            var result = new List<ContractDTO>();
		    while (reader.Read())
			{
                result.Add(
                    new ContractDTO
				    {
					    Id = reader.GetValue<int>("contract_id"),
					    PeriodTypeId = (PeriodType)reader.GetValue<int>("period_type_id"),
					    TargetId = (Target)reader.GetValue<int>("target_id"),
					    ContractDate = reader.GetValue<DateTime>("contract_date"),
                        SystemId = reader.GetValue<int>("system_id"),
                        IsFinal = reader.GetValue<bool>("is_final")
                    });
			}
            return result;
		}
	}
}
