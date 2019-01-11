using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.RecordNotes
{
    public class AddRecordNoteCommand : CommandScalar<AddRecordNoteParameterSet, Guid>
    {
        public AddRecordNoteCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddRecordNoteParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("record_id");
            command.AddInputParameter("p_task_id", parameters.TaskId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            command.AddInputParameter("p_note", parameters.Note);
            command.AddInputParameter("p_user_name_cpdd", parameters.UserNameCpdd);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRecordNoteParameterSet parameters)
        {
            return "tasks.P_TASK_RECORD.AddF_NOTE";
        }
    }
}
