using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.DispatcherTask  
{
    [ServiceContract]
    public interface IDispatcherTaskService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTaskList(GetTaskListParameterSet parameters, AsyncCallback callback, object state);
        List<TaskDTO> EndGetTaskList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddTaskCpdd(AddTaskCpddParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddTaskCpdd(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddTask(AddTaskParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddMultiRecordTask(AddTaskParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddMultiRecordTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginCloneTask(AddTaskParameterSet parameters, AsyncCallback callback, object state);
        Guid EndCloneTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditTask(EditTaskParameterSet parameters, AsyncCallback callback, object state);
        void EndEditTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteTask(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddTaskAttachment(AddTaskAttachmentParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddTaskAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditTaskAttachment(EditTaskAttachmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditTaskAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteTaskAttachment(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteTaskAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTaskAttachementList(Guid parameters, AsyncCallback callback, object state);
        List<TaskAttachmentDTO> EndGetTaskAttachementList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddTaskRecordCPDD(AddTaskRecordCpddParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddTaskRecordCPDD(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddTaskRecordPDS(AddTaskRecordPdsParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddTaskRecordPDS(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditTaskRecordPDS(EditTaskRecordPdsParameterSet parameters, AsyncCallback callback, object state);
        void EndEditTaskRecordPDS(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditTaskRecordCPDD(EditTaskRecordCpddParameterSet parameters, AsyncCallback callback, object state);
        void EndEditTaskRecordCPDD(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTaskRecordCPDDList(GetTaskRecordsCpddParameterSet parameters, AsyncCallback callback, object state);
        List<TaskRecordDTO> EndGetTaskRecordCPDDList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTaskRecordPDSList(GetTaskRecordsPdsParameterSet parameters, AsyncCallback callback, object state);
        List<TaskRecordPdsDTO> EndGetTaskRecordPDSList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveTaskRecord(Guid parameters, AsyncCallback callback, object state);
        void EndRemoveTaskRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRecordNote(AddRecordNoteParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddRecordNote(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditTaskRecordNote(EditRecordNoteParameterSet parameters, AsyncCallback callback, object state);
        void EndEditTaskRecordNote(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveTaskRecordNote(Guid parameters, AsyncCallback callback, object state);
        void EndRemoveTaskRecordNote(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRecordNoteList(GetRecordNoteListParameterSet parameters, AsyncCallback callback, object state);
        List<RecordNoteDTO> EndGetRecordNoteList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTaskStatusesList(Guid parameters, AsyncCallback callback, object state);
        List<TaskStatusDTO> EndGetTaskStatusesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetTaskStatus(SetTaskStatusParameterSet parameters, AsyncCallback callback, object state);
        void EndSetTaskStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskApprovedPds(Guid parameters, AsyncCallback callback, object state);
        Guid EndTaskApprovedPds(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskPerformed(Guid parameters, AsyncCallback callback, object state);
        Guid EndTaskPerformed(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskAnnuled(SetTaskStatusParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskAnnuled(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskApprovedCPDD(SetTaskStatusParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskApprovedCPDD(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskCorrected(SetTaskStatusParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskCorrected(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskCorrecting(SetTaskStatusParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskCorrecting(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskSubmited(SetTaskStatusParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskSubmited(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskRecordExecuted(Guid parameters, AsyncCallback callback, object state);
        void EndTaskRecordExecuted(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskRecordResetExecuted(Guid parameters, AsyncCallback callback, object state);
        void EndTaskRecordResetExecuted(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginResetToControlTaskRecord(Guid parameters, AsyncCallback callback, object state);
        void EndResetToControlTaskRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetToControlTaskRecord(Guid parameters, AsyncCallback callback, object state);
        void EndSetToControlTaskRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetACK(Guid parameters, AsyncCallback callback, object state);
        void EndSetACK(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginResetACK(Guid parameters, AsyncCallback callback, object state);
        void EndResetACK(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetNotAckedTaskList(GetTaskListParameterSet parameters, AsyncCallback callback, object state);
        List<TaskRecordPdsDTO> EndGetNotAckedTaskList(IAsyncResult result);
    }

	public interface IDispatcherTaskServiceProxy
	{

        Task<List<TaskDTO>> GetTaskListAsync(GetTaskListParameterSet parameters);

        Task<Guid> AddTaskCpddAsync(AddTaskCpddParameterSet parameters);

        Task<Guid> AddTaskAsync(AddTaskParameterSet parameters);

        Task<Guid> AddMultiRecordTaskAsync(AddTaskParameterSet parameters);

        Task<Guid> CloneTaskAsync(AddTaskParameterSet parameters);

        Task EditTaskAsync(EditTaskParameterSet parameters);

        Task DeleteTaskAsync(Guid parameters);

        Task<Guid> AddTaskAttachmentAsync(AddTaskAttachmentParameterSet parameters);

        Task EditTaskAttachmentAsync(EditTaskAttachmentParameterSet parameters);

        Task DeleteTaskAttachmentAsync(Guid parameters);

        Task<List<TaskAttachmentDTO>> GetTaskAttachementListAsync(Guid parameters);

        Task<Guid> AddTaskRecordCPDDAsync(AddTaskRecordCpddParameterSet parameters);

        Task<Guid> AddTaskRecordPDSAsync(AddTaskRecordPdsParameterSet parameters);

        Task EditTaskRecordPDSAsync(EditTaskRecordPdsParameterSet parameters);

        Task EditTaskRecordCPDDAsync(EditTaskRecordCpddParameterSet parameters);

        Task<List<TaskRecordDTO>> GetTaskRecordCPDDListAsync(GetTaskRecordsCpddParameterSet parameters);

        Task<List<TaskRecordPdsDTO>> GetTaskRecordPDSListAsync(GetTaskRecordsPdsParameterSet parameters);

        Task RemoveTaskRecordAsync(Guid parameters);

        Task<Guid> AddRecordNoteAsync(AddRecordNoteParameterSet parameters);

        Task EditTaskRecordNoteAsync(EditRecordNoteParameterSet parameters);

        Task RemoveTaskRecordNoteAsync(Guid parameters);

        Task<List<RecordNoteDTO>> GetRecordNoteListAsync(GetRecordNoteListParameterSet parameters);

        Task<List<TaskStatusDTO>> GetTaskStatusesListAsync(Guid parameters);

        Task SetTaskStatusAsync(SetTaskStatusParameterSet parameters);

        Task<Guid> TaskApprovedPdsAsync(Guid parameters);

        Task<Guid> TaskPerformedAsync(Guid parameters);

        Task<Guid> TaskAnnuledAsync(SetTaskStatusParameterSet parameters);

        Task<Guid> TaskApprovedCPDDAsync(SetTaskStatusParameterSet parameters);

        Task<Guid> TaskCorrectedAsync(SetTaskStatusParameterSet parameters);

        Task<Guid> TaskCorrectingAsync(SetTaskStatusParameterSet parameters);

        Task<Guid> TaskSubmitedAsync(SetTaskStatusParameterSet parameters);

        Task TaskRecordExecutedAsync(Guid parameters);

        Task TaskRecordResetExecutedAsync(Guid parameters);

        Task ResetToControlTaskRecordAsync(Guid parameters);

        Task SetToControlTaskRecordAsync(Guid parameters);

        Task SetACKAsync(Guid parameters);

        Task ResetACKAsync(Guid parameters);

        Task<List<TaskRecordPdsDTO>> GetNotAckedTaskListAsync(GetTaskListParameterSet parameters);

    }

    public sealed class DispatcherTaskServiceProxy : DataProviderBase<IDispatcherTaskService>, IDispatcherTaskServiceProxy
	{
        protected override string ServiceUri => "/DispatcherTask/DispatcherTaskService.svc";
      


        public Task<List<TaskDTO>> GetTaskListAsync(GetTaskListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TaskDTO>,GetTaskListParameterSet>(channel, channel.BeginGetTaskList, channel.EndGetTaskList, parameters);
        }

        public Task<Guid> AddTaskCpddAsync(AddTaskCpddParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskCpddParameterSet>(channel, channel.BeginAddTaskCpdd, channel.EndAddTaskCpdd, parameters);
        }

        public Task<Guid> AddTaskAsync(AddTaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskParameterSet>(channel, channel.BeginAddTask, channel.EndAddTask, parameters);
        }

        public Task<Guid> AddMultiRecordTaskAsync(AddTaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskParameterSet>(channel, channel.BeginAddMultiRecordTask, channel.EndAddMultiRecordTask, parameters);
        }

        public Task<Guid> CloneTaskAsync(AddTaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskParameterSet>(channel, channel.BeginCloneTask, channel.EndCloneTask, parameters);
        }

        public Task EditTaskAsync(EditTaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditTask, channel.EndEditTask, parameters);
        }

        public Task DeleteTaskAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteTask, channel.EndDeleteTask, parameters);
        }

        public Task<Guid> AddTaskAttachmentAsync(AddTaskAttachmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskAttachmentParameterSet>(channel, channel.BeginAddTaskAttachment, channel.EndAddTaskAttachment, parameters);
        }

        public Task EditTaskAttachmentAsync(EditTaskAttachmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditTaskAttachment, channel.EndEditTaskAttachment, parameters);
        }

        public Task DeleteTaskAttachmentAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteTaskAttachment, channel.EndDeleteTaskAttachment, parameters);
        }

        public Task<List<TaskAttachmentDTO>> GetTaskAttachementListAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TaskAttachmentDTO>,Guid>(channel, channel.BeginGetTaskAttachementList, channel.EndGetTaskAttachementList, parameters);
        }

        public Task<Guid> AddTaskRecordCPDDAsync(AddTaskRecordCpddParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskRecordCpddParameterSet>(channel, channel.BeginAddTaskRecordCPDD, channel.EndAddTaskRecordCPDD, parameters);
        }

        public Task<Guid> AddTaskRecordPDSAsync(AddTaskRecordPdsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskRecordPdsParameterSet>(channel, channel.BeginAddTaskRecordPDS, channel.EndAddTaskRecordPDS, parameters);
        }

        public Task EditTaskRecordPDSAsync(EditTaskRecordPdsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditTaskRecordPDS, channel.EndEditTaskRecordPDS, parameters);
        }

        public Task EditTaskRecordCPDDAsync(EditTaskRecordCpddParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditTaskRecordCPDD, channel.EndEditTaskRecordCPDD, parameters);
        }

        public Task<List<TaskRecordDTO>> GetTaskRecordCPDDListAsync(GetTaskRecordsCpddParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TaskRecordDTO>,GetTaskRecordsCpddParameterSet>(channel, channel.BeginGetTaskRecordCPDDList, channel.EndGetTaskRecordCPDDList, parameters);
        }

        public Task<List<TaskRecordPdsDTO>> GetTaskRecordPDSListAsync(GetTaskRecordsPdsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TaskRecordPdsDTO>,GetTaskRecordsPdsParameterSet>(channel, channel.BeginGetTaskRecordPDSList, channel.EndGetTaskRecordPDSList, parameters);
        }

        public Task RemoveTaskRecordAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveTaskRecord, channel.EndRemoveTaskRecord, parameters);
        }

        public Task<Guid> AddRecordNoteAsync(AddRecordNoteParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddRecordNoteParameterSet>(channel, channel.BeginAddRecordNote, channel.EndAddRecordNote, parameters);
        }

        public Task EditTaskRecordNoteAsync(EditRecordNoteParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditTaskRecordNote, channel.EndEditTaskRecordNote, parameters);
        }

        public Task RemoveTaskRecordNoteAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveTaskRecordNote, channel.EndRemoveTaskRecordNote, parameters);
        }

        public Task<List<RecordNoteDTO>> GetRecordNoteListAsync(GetRecordNoteListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RecordNoteDTO>,GetRecordNoteListParameterSet>(channel, channel.BeginGetRecordNoteList, channel.EndGetRecordNoteList, parameters);
        }

        public Task<List<TaskStatusDTO>> GetTaskStatusesListAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TaskStatusDTO>,Guid>(channel, channel.BeginGetTaskStatusesList, channel.EndGetTaskStatusesList, parameters);
        }

        public Task SetTaskStatusAsync(SetTaskStatusParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetTaskStatus, channel.EndSetTaskStatus, parameters);
        }

        public Task<Guid> TaskApprovedPdsAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,Guid>(channel, channel.BeginTaskApprovedPds, channel.EndTaskApprovedPds, parameters);
        }

        public Task<Guid> TaskPerformedAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,Guid>(channel, channel.BeginTaskPerformed, channel.EndTaskPerformed, parameters);
        }

        public Task<Guid> TaskAnnuledAsync(SetTaskStatusParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,SetTaskStatusParameterSet>(channel, channel.BeginTaskAnnuled, channel.EndTaskAnnuled, parameters);
        }

        public Task<Guid> TaskApprovedCPDDAsync(SetTaskStatusParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,SetTaskStatusParameterSet>(channel, channel.BeginTaskApprovedCPDD, channel.EndTaskApprovedCPDD, parameters);
        }

        public Task<Guid> TaskCorrectedAsync(SetTaskStatusParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,SetTaskStatusParameterSet>(channel, channel.BeginTaskCorrected, channel.EndTaskCorrected, parameters);
        }

        public Task<Guid> TaskCorrectingAsync(SetTaskStatusParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,SetTaskStatusParameterSet>(channel, channel.BeginTaskCorrecting, channel.EndTaskCorrecting, parameters);
        }

        public Task<Guid> TaskSubmitedAsync(SetTaskStatusParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,SetTaskStatusParameterSet>(channel, channel.BeginTaskSubmited, channel.EndTaskSubmited, parameters);
        }

        public Task TaskRecordExecutedAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginTaskRecordExecuted, channel.EndTaskRecordExecuted, parameters);
        }

        public Task TaskRecordResetExecutedAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginTaskRecordResetExecuted, channel.EndTaskRecordResetExecuted, parameters);
        }

        public Task ResetToControlTaskRecordAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginResetToControlTaskRecord, channel.EndResetToControlTaskRecord, parameters);
        }

        public Task SetToControlTaskRecordAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetToControlTaskRecord, channel.EndSetToControlTaskRecord, parameters);
        }

        public Task SetACKAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetACK, channel.EndSetACK, parameters);
        }

        public Task ResetACKAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginResetACK, channel.EndResetACK, parameters);
        }

        public Task<List<TaskRecordPdsDTO>> GetNotAckedTaskListAsync(GetTaskListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TaskRecordPdsDTO>,GetTaskListParameterSet>(channel, channel.BeginGetNotAckedTaskList, channel.EndGetNotAckedTaskList, parameters);
        }

    }
}
