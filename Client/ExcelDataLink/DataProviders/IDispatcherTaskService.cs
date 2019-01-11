using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.DispatcherTask  
{
    [ServiceContract]
    public interface IDispatcherTaskService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetTaskList(object parameters, AsyncCallback callback, object state);
        List<TaskDTO> EndGetTaskList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddTaskCpdd(AddTaskCpddParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddTaskCpdd(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteTask(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteTask(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddTaskPds(AddTaskPdsParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddTaskPds(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddFileAttachment(AddTaskCpddAttachmentParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddFileAttachment(IAsyncResult result);

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
        IAsyncResult BeginAddRecordNote(AddRecordNoteParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddRecordNote(IAsyncResult result);

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
        IAsyncResult BeginTaskApprovedPds(Guid parameters, AsyncCallback callback, object state);
        Guid EndTaskApprovedPds(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskPerformed(Guid parameters, AsyncCallback callback, object state);
        Guid EndTaskPerformed(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskAnnuled(TaskAnnuledParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskAnnuled(IAsyncResult result);

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
        IAsyncResult BeginTaskApprovedCPDD(TaskParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskApprovedCPDD(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskCorrected(TaskParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskCorrected(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskCorrecting(TaskParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskCorrecting(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTaskSubmited(TaskParameterSet parameters, AsyncCallback callback, object state);
        Guid EndTaskSubmited(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveTaskRecordNote(Guid parameters, AsyncCallback callback, object state);
        void EndRemoveTaskRecordNote(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveTaskRecord(Guid parameters, AsyncCallback callback, object state);
        void EndRemoveTaskRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginResetToControlTaskRecord(Guid parameters, AsyncCallback callback, object state);
        void EndResetToControlTaskRecord(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetToControlTaskRecord(Guid parameters, AsyncCallback callback, object state);
        void EndSetToControlTaskRecord(IAsyncResult result);
    }


    public class DispatcherTaskServiceProxy : DataProviderBase<IDispatcherTaskService>
	{
        protected override string ServiceUri
        {
            get { return "/DispatcherTask/DispatcherTaskService.svc"; }
        }

        public Task<List<TaskDTO>> GetTaskListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<TaskDTO>>(channel, channel.BeginGetTaskList, channel.EndGetTaskList);
        }

        public Task<Guid> AddTaskCpddAsync(AddTaskCpddParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskCpddParameterSet>(channel, channel.BeginAddTaskCpdd, channel.EndAddTaskCpdd, parameters);
        }

        public Task DeleteTaskAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteTask, channel.EndDeleteTask, parameters);
        }

        public Task<Guid> AddTaskPdsAsync(AddTaskPdsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskPdsParameterSet>(channel, channel.BeginAddTaskPds, channel.EndAddTaskPds, parameters);
        }

        public Task<Guid> AddFileAttachmentAsync(AddTaskCpddAttachmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddTaskCpddAttachmentParameterSet>(channel, channel.BeginAddFileAttachment, channel.EndAddFileAttachment, parameters);
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

        public Task<Guid> AddRecordNoteAsync(AddRecordNoteParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddRecordNoteParameterSet>(channel, channel.BeginAddRecordNote, channel.EndAddRecordNote, parameters);
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

        public Task<Guid> TaskAnnuledAsync(TaskAnnuledParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,TaskAnnuledParameterSet>(channel, channel.BeginTaskAnnuled, channel.EndTaskAnnuled, parameters);
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

        public Task<Guid> TaskApprovedCPDDAsync(TaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,TaskParameterSet>(channel, channel.BeginTaskApprovedCPDD, channel.EndTaskApprovedCPDD, parameters);
        }

        public Task<Guid> TaskCorrectedAsync(TaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,TaskParameterSet>(channel, channel.BeginTaskCorrected, channel.EndTaskCorrected, parameters);
        }

        public Task<Guid> TaskCorrectingAsync(TaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,TaskParameterSet>(channel, channel.BeginTaskCorrecting, channel.EndTaskCorrecting, parameters);
        }

        public Task<Guid> TaskSubmitedAsync(TaskParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,TaskParameterSet>(channel, channel.BeginTaskSubmited, channel.EndTaskSubmited, parameters);
        }

        public Task RemoveTaskRecordNoteAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveTaskRecordNote, channel.EndRemoveTaskRecordNote, parameters);
        }

        public Task RemoveTaskRecordAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveTaskRecord, channel.EndRemoveTaskRecord, parameters);
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

    }
}
