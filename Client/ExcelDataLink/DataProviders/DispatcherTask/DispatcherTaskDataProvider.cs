using System;
using System.Collections.Generic;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;

namespace DataProviders.DispatcherTask
{
    public class DispatcherTaskDataProvider : DataProviderBase<IDispatcherTaskService>
    {
	    protected override string ServiceUri
	    {
            get { return "/DispatcherTask/DispatcherTaskService.svc"; }
	    }

		public void GetTaskList(Func<List<TaskDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
            Execute(channel, channel.BeginGetTaskList, channel.EndGetTaskList, callback, behavior);
		}

		public void DeleteTask(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginDeleteTask, channel.EndDeleteTask, callback, parameters, behavior);
		}

        public void AddTaskPds(AddTaskPdsParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddTaskPds, channel.EndAddTaskPds, callback, parameters, behavior);
        }

		public void GetTaskAttachementList(Guid parameters, Func<List<TaskAttachmentDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetTaskAttachementList, channel.EndGetTaskAttachementList, callback, parameters, behavior);
		}

        public void AddTaskRecordCPDD(AddTaskRecordCpddParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddTaskRecordCPDD, channel.EndAddTaskRecordCPDD, callback, parameters, behavior);
        }

        public void AddTaskRecordPDS(AddTaskRecordPdsParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddTaskRecordPDS, channel.EndAddTaskRecordPDS, callback, parameters, behavior);
        }

		public void GetTaskRecordCPDDList(GetTaskRecordsCpddParameterSet parameters, Func<List<TaskRecordDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetTaskRecordCPDDList, channel.EndGetTaskRecordCPDDList, callback, parameters, behavior);
		}

		public void GetTaskRecordPDSList(GetTaskRecordsPdsParameterSet parameters, Func<List<TaskRecordPdsDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetTaskRecordPDSList, channel.EndGetTaskRecordPDSList, callback, parameters, behavior);
		}

        public void AddRecordNote(AddRecordNoteParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginAddRecordNote, channel.EndAddRecordNote, callback, parameters, behavior);
        }

        public void GetRecordNoteList(GetRecordNoteListParameterSet parameters, Func<List<RecordNoteDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetRecordNoteList, channel.EndGetRecordNoteList, callback, parameters, behavior);
        }

        public void GetTaskStatusesList(Guid parameters, Func<List<TaskStatusDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetTaskStatusesList, channel.EndGetTaskStatusesList, callback, parameters, behavior);
        }

        public void TaskApprovedPds(Guid parameters, Func<Guid,Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginTaskApprovedPds, channel.EndTaskApprovedPds, callback, parameters, behavior);
        }

        public void TaskPerformed(Guid parameters, Func<Guid,Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginTaskPerformed, channel.EndTaskPerformed, callback, parameters, behavior);
        }

        public void TaskAnnuled(TaskAnnuledParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginTaskAnnuled, channel.EndTaskAnnuled, callback, parameters, behavior);
        }

		public void TaskRecordExecuted(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginTaskRecordExecuted, channel.EndTaskRecordExecuted, callback, parameters, behavior);
		}

		public void TaskRecordResetExecuted(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginTaskRecordResetExecuted, channel.EndTaskRecordResetExecuted, callback, parameters, behavior);
		}

		public void TaskApprovedCPDD(TaskParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginTaskApprovedCPDD, channel.EndTaskApprovedCPDD, callback, parameters, behavior);
		}

		public void TaskCorrected(TaskParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginTaskCorrected, channel.EndTaskCorrected, callback, parameters, behavior);
		}

		public void TaskCorrecting(TaskParameterSet parameters, Func<Guid, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginTaskCorrecting, channel.EndTaskCorrecting, callback, parameters, behavior);
		}

		public void TaskSubmited(TaskParameterSet parameters, Func<Guid,Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginTaskSubmited, channel.EndTaskSubmited, callback, parameters, behavior);
		}

        public void RemoveTaskRecordNote(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginRemoveTaskRecordNote, channel.EndRemoveTaskRecordNote, callback, parameters, behavior);
        }

        public void RemoveTaskRecord(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginRemoveTaskRecord, channel.EndRemoveTaskRecord, callback, parameters, behavior);
        }

        public void ResetToControlTaskRecord(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginResetToControlTaskRecord, channel.EndResetToControlTaskRecord, callback, parameters, behavior);
        }

        public void SetToControlTaskRecord(Guid parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginSetToControlTaskRecord, channel.EndSetToControlTaskRecord, callback, parameters, behavior);
        }

        public void EditTaskRecordPDS(EditTaskRecordPdsParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginEditTaskRecordPDS, channel.EndEditTaskRecordPDS, callback, parameters, behavior);
        }

        public void EditTaskRecordCPDD(EditTaskRecordCpddParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginEditTaskRecordCPDD, channel.EndEditTaskRecordPDS, callback, parameters, behavior);
        }
    }
}
