using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
    public class ResetTaskRecordAckCommand : CommandNonQuery<Guid>
    {
        public ResetTaskRecordAckCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_record_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "tasks.P_TASK_RECORD.ReSetACK";
        }
    }
}
