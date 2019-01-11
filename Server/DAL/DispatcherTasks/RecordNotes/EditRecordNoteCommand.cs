using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using Oracle.ManagedDataAccess.Client;


namespace GazRouter.DAL.DispatcherTasks.RecordNotes
{
    public class EditRecordNoteCommand : CommandNonQuery<EditRecordNoteParameterSet>
    {
        public EditRecordNoteCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditRecordNoteParameterSet parameters)
        {
            command.AddInputParameter("p_record_notes_id", parameters.RecordNoteId);
            command.AddInputParameter("p_note", parameters.Note);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditRecordNoteParameterSet parameters)
        {
            return "tasks.P_TASK_RECORD.EDIT_NOTE";
        }
    }
}
