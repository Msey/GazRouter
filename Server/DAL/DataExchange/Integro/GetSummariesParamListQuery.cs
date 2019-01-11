using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Integro;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class GetSummariesParamListQuery : QueryReader<Guid, List<SummaryParamDTO>>
    {
        public GetSummariesParamListQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            string result = @"SELECT * FROM integro.V_SUMMARY_PARAMETERS WHERE 1=1 ";
            var sb = new StringBuilder(result);
            if (parameters != null)
            {
                    sb.Append($" AND summary_id =:summary_id");
            }
            return sb.ToString();
        }
        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            if (parameters != null)
                command.AddInputParameter("summary_id", parameters);
        }

        protected override List<SummaryParamDTO> GetResult(OracleDataReader reader, Guid parameters)
        {
            var result = new List<SummaryParamDTO>();
            while (reader.Read())
            {
                result.Add(new SummaryParamDTO
                {
                    Id = reader.GetValue<Guid>("summary_parameter_id"),
                    SummaryId = reader.GetValue<Guid>("summary_id"),
                    Name = reader.GetValue<string>("name"),
                    ParameterGid = reader.GetValue<string>("parameter_gid"),
                    Aggregate = reader.GetValue<string>("aggregate")
                });
            }
            return result;
        }
    }
}
