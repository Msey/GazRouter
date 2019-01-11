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
    public class GetSummariesParamContentListQuery : QueryReader<Guid, List<SummaryParamContentDTO>>
    {
        public GetSummariesParamContentListQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            string result = @"SELECT * FROM integro.V_SUMMARY_PARAMETER_CONTENT WHERE 1=1 ";
            var sb = new StringBuilder(result);
            if (parameters != null)
            {
                sb.Append($" AND summary_parameter_id =:summary_parameter_id");
            }
            return sb.ToString();
        }
        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            if (parameters != null)
                command.AddInputParameter("summary_parameter_id", parameters);
        }

        protected override List<SummaryParamContentDTO> GetResult(OracleDataReader reader, Guid parameters)
        {
            var result = new List<SummaryParamContentDTO>();
            while (reader.Read())
            {
                result.Add(new SummaryParamContentDTO
                {
                    SummaryParamId = reader.GetValue<Guid>("summary_parameter_id"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    PropertyTypeId = reader.GetValue<int>("property_type_id"),
                });
            }
            return result;
        }
    }
}
