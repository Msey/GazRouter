using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.InputStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.InputStates
{
    public class GetManualInputStateListQuery : QueryReader<GetManualInputStateListParameterSet, List<ManualInputStateDTO>>
    {
        public GetManualInputStateListQuery(ExecutionContext context) : base(context)
        {
  
        }

        protected override string GetCommandText(GetManualInputStateListParameterSet parameters)
        {
            var q = @"  SELECT      s.site_id,
                                    s.state_id,
                                    s.user_name,
                                    s.state_date 
                        FROM        rd.v_input_states s 
                        WHERE       1 = 1
                        AND         s.series_id = :serieid";

            var sb = new StringBuilder(q);
            if (parameters.SiteId.HasValue)
                sb.Append(" AND s.site_id = :siteid");

            return sb.ToString();

        }

        protected override void BindParameters(OracleCommand command, GetManualInputStateListParameterSet parameters)
        {
            command.AddInputParameter("serieid", parameters.SerieId);
            command.AddInputParameter("siteid", parameters.SiteId);
        }

        protected override List<ManualInputStateDTO> GetResult(OracleDataReader reader, GetManualInputStateListParameterSet parameters)
        {
            var result = new List<ManualInputStateDTO>();
            while (reader.Read())
            {
                result.Add(new ManualInputStateDTO
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