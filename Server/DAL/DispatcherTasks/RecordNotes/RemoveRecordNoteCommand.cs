using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.RecordNotes
{
    public class RemoveRecordNoteCommand : CommandNonQuery<Guid>
    {
        public RemoveRecordNoteCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
		{
            command.AddInputParameter("p_record_notes_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

        protected override string GetCommandText(Guid parameters)
        {
            return "tasks.P_TASK_RECORD.Remove_NOTE";
        }
    }
}
