using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.InputStates;
using GazRouter.DTO.ManualInput.InputStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.InputStates
{
    public class GetInputStateListQuery : QueryReader<GetInputStateListParameterSet, List<InputStateDTO>>
    {
        public GetInputStateListQuery(ExecutionContext context) : base(context)
        {
  
        }

        protected override string GetCommandText(GetInputStateListParameterSet parameters)
        {
            var q = @"  SELECT      s.site_id,
                                    s.state_id,
                                    s.user_name,
                                    s.state_date 
                        FROM        rd.v_bl_input_states s                         
                        WHERE       1=1                        
                        AND         s.contract_id = :contractId";

            var sb = new StringBuilder(q);
            if (parameters.SiteId.HasValue)
                sb.Append(" AND s.site_id = :siteid");

            return sb.ToString();

        }

        protected override void BindParameters(OracleCommand command, GetInputStateListParameterSet parameters)
        {
            command.AddInputParameter("contractId", parameters.ContractId);
            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteid", parameters.SiteId);
        }

        protected override List<InputStateDTO> GetResult(OracleDataReader reader, GetInputStateListParameterSet parameters)
        {
            var result = new List<InputStateDTO>();
            while (reader.Read())
            {
                result.Add(new InputStateDTO
                {
                    SiteId = reader.GetValue<Guid>("site_id"),
                    State = reader.GetValue<ManualInputState>("state_id"),
                    UserName = reader.GetValue<string>("user_name"),
                    ChangeDate = reader.GetValue<DateTime?>("state_date"),
                });
            }

            return result;
        }
    }
}