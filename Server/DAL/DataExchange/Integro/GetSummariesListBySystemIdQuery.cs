using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class GetSummariesListByParamsIdQuery : QueryReader<GetSummaryParameterSet, List<SummaryDTO>>
    {
        Dictionary<string, SessionDataType> dictSessionDataType;
        public GetSummariesListByParamsIdQuery(ExecutionContext context) : base(context)
        {
            // TODO: ПЕРЕДЕЛАТЬ
            dictSessionDataType = new Dictionary<string, SessionDataType>()
            {
                {"RT", SessionDataType.RT},
                {"UB", SessionDataType.UB},
                {"PL", SessionDataType.PL},
                {"NSI", SessionDataType.NSI},
                {"PROD", SessionDataType.PROD},
                {"PRO", SessionDataType.PRO},
                {"F1P", SessionDataType.F1P},
                {"РТ2", SessionDataType.РТ2},
                {"РТ24", SessionDataType.РТ24},
            };
        }

        protected override string GetCommandText(GetSummaryParameterSet parameters)
        {
            string result = @"SELECT * FROM integro.v_summaries WHERE 1=1 ";
            var sb = new StringBuilder(result);
            if (parameters != null)
            {
                if (parameters.SystemId > 0)
                    sb.Append($" AND system_id =:system_id");
            }
            return sb.ToString();
        }
        protected override void BindParameters(OracleCommand command, GetSummaryParameterSet parameters)
        {
            if (parameters != null && parameters.SystemId > 0)
                command.AddInputParameter("system_id", (int)parameters.SystemId);
        }

        protected override List<SummaryDTO> GetResult(OracleDataReader reader, GetSummaryParameterSet parameters)
        {
            var result = new List<SummaryDTO>();
            while (reader.Read())
            {
                result.Add(new SummaryDTO
                {
                    Id = reader.GetValue<Guid>("summary_id"),
                    Name = reader.GetValue<string>("name"),
                    Descriptor = reader.GetValue<string>("descriptor"),
                    TransformFileName = reader.GetValue<string>("transform_file_name"),
                    PeriodType = reader.GetValue<PeriodType>("period_type_id"),
                    SystemId = reader.GetValue<int>("system_id"),
                    ExchangeTaskId = reader.GetValue<int>("EXCHANGE_TASK_ID"),
                    SessionDataCode = reader.GetValue<string>("SESSION_DATA_TYPE"),
                    SessionDataType = GetSessionDataType(reader.GetValue<string>("SESSION_DATA_TYPE")),
                    StatusId = reader.GetValue<int>("STATUS_ID"),
                });
            }
            return result;
        }
        // Странный метод.
        private SessionDataType GetSessionDataType(string sessionDataCode)
        {
            var defaultValue = SessionDataType.RT;
            if (sessionDataCode == null)
                return defaultValue;
            if (!dictSessionDataType.ContainsKey(sessionDataCode))
                return defaultValue;
            return dictSessionDataType[sessionDataCode];
        }
    }
}
