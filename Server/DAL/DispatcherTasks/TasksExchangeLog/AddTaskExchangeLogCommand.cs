using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.TasksExchangeLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DispatcherTasks.TasksExchangeLog
{
    public class AddTaskExchangeLogCommand : CommandScalar<AddTaskExchangeLogParameterSet, Guid>
    {
        public AddTaskExchangeLogCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddTaskExchangeLogParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("p_task_exchange_id");
            command.AddInputParameter("p_task_id", parameters.TaskId);
            command.AddInputParameter("p_global_task_id", parameters.GlobalTaskID);
            command.AddInputParameter("p_exchange_date", parameters.ExchangeDate);
            command.AddInputParameter("p_exchange_status", parameters.ExchangeStatus);
            command.AddInputParameter("p_exchange_confirmed", parameters.ExchangeConfirmed);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddTaskExchangeLogParameterSet parameters)
        {
            return "tasks.P_TASK_EXCHANGE_LOG.ADDF";
        }
    }
}
