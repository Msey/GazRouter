using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.TasksExchangeLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TasksExchangeLog
{
    public class GetTaskExchangeLogQuery : QueryReader<Guid, List<TaskExchangeDTO>>
    {
        public GetTaskExchangeLogQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            var sql = @"    SELECT      a.task_exchange_id,
                                        a.task_id,
                                        a.global_task_id,
                                        a.exchange_date,
                                        a.exchange_status,
                                        a.exchange_confirmed

                            FROM        V_TASKS_EXCHANGE_LOG a 
                            WHERE       task_id = :taskid
                            order by  a.task_exchange_id";
            return sql;
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("taskid", parameters);
        }

        protected override List<TaskExchangeDTO> GetResult(OracleDataReader reader, Guid parameters)
        {
            var result = new List<TaskExchangeDTO>();
            while (reader.Read())
            {
                result.Add(
                    new TaskExchangeDTO
                    {
                        Id = reader.GetValue<Guid>("task_exchange_id"),
                        TaskId = reader.GetValue<Guid>("task_id"),
                        GlobalTaskID = reader.GetValue<string>("global_task_id"),
                        ExchangeDate = reader.GetValue<DateTime>("exchange_date"),
                        ExchangeStatus = reader.GetValue<string>("exchange_status"),
                        ExchangeConfirmed = reader.GetValue<bool?>("exchange_confirmed")
                    });
            }
            return result;
        }
    }
}
