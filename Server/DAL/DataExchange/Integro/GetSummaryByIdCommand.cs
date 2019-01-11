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
    public class GetSummaryByIdCommand : QueryReader<Guid, SummaryDTO>
    {
        Dictionary<string, SessionDataType> dictSessionDataType;
        public GetSummaryByIdCommand(ExecutionContext context) : base(context)
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

        protected override string GetCommandText(Guid summaryGuid)
        {
            string result = @"SELECT * FROM integro.v_summaries WHERE summary_id =:summary_id";
            return result;
        }
        protected override void BindParameters(OracleCommand command, Guid summaryGuid)
        {
            command.AddInputParameter("summary_id", summaryGuid);
        }

        protected override SummaryDTO GetResult(OracleDataReader reader, Guid summaryGuid)
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
            return result.FirstOrDefault();
        }
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
