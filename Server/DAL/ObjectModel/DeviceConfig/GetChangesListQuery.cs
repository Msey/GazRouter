using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DTO.ObjectModel.ChangeLogs;
using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.DeviceConfig
{
    public class GetChangesListQuery : QueryReader<GetChangeLogParameterSet, List<ChangeDTO>>
    {
        public GetChangesListQuery(ExecutionContext context) : base(context)
        {
        }
        protected override string GetCommandText(GetChangeLogParameterSet parameters)
        {
            var builder = new StringBuilder();
            builder.Append(
            @"  Select  e.log_id,
                        e.action_date,
                        e.user_name,
                        t1.name,
                        en.entity_name,
                        e.action,
                        e.action_name,
                        e.table_name
                         From  rd.V_IDS_LOG e left join (v_users t1 left join v_entities en on t1.SITE_ID = en.entity_id)  on t1.login = e.user_name
                    WHERE 1=1 ");

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
            {
                builder.Append(@"AND (e.action_date >= :startDate AND e.action_date <= :endDate)");
            }

            if (parameters.IsExec)
            {
                builder.Append(@"AND e.action != 4 ");
            }

            if (parameters.TableName != null)
            {
                builder.Append(@"AND e.table_name = :table_name ");
            }

            builder.Append(" ORDER BY  e.action_date DESC");
            return builder.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetChangeLogParameterSet parameters)
        {
            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
            {
                command.AddInputParameter("startDate", parameters.StartDate);
                command.AddInputParameter("endDate", parameters.EndDate);
            }
            if (parameters.TableNameToTable() != null)
                command.AddInputParameter("table_name", parameters.TableNameToTable());
        }

        protected override List<ChangeDTO> GetResult(OracleDataReader reader, GetChangeLogParameterSet parameters)
        {
            var result = new List<ChangeDTO>();
            while (reader.Read())
            {
                result.Add(new ChangeDTO
                {
                    LogId = reader.GetValue<int>("log_id"),
                    ActionDate = reader.GetValue<DateTime?>("action_date"),
                    Login = reader.GetValue<string>("user_name"),
                    UserName = reader.GetValue<string>("name"),
                    Site = reader.GetValue<string>("entity_name"),
                    Action = reader.GetValue<int>("action"),
                    ActionName = reader.GetValue<string>("action_name"),
                    TableName = reader.GetValue<string>("table_name")
                });
            }
            return result;
        }
    }
}
