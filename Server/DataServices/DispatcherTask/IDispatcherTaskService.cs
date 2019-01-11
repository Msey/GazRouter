using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;

namespace GazRouter.DataServices.DispatcherTask
{
    [Service("Диспетческие задания")]
    [ServiceContract]
    public interface IDispatcherTaskService
    {
#region TASKS

        [ServiceAction("Получение списка диспетчерских заданий")]
        [OperationContract]
        List<TaskDTO> GetTaskList(GetTaskListParameterSet parameters);

        
        [ServiceAction("Добавление нового задания ЦПДД")]
        [OperationContract]
        Guid AddTaskCpdd(AddTaskCpddParameterSet parameters);

		
        [ServiceAction("Добавление нового задания")]
        [OperationContract]
        Guid AddTask(AddTaskParameterSet parameters);


        [ServiceAction("Добавление нового задания для нескольких ЛПУ")]
        [OperationContract]
        Guid AddMultiRecordTask(AddTaskParameterSet parameters);


        [ServiceAction("Копирование ДЗ")]
        [OperationContract]
        Guid CloneTask(AddTaskParameterSet parameters);


        [ServiceAction("Редактирование задания")]
        [OperationContract]
        void EditTask(EditTaskParameterSet parameters);


        [ServiceAction("Удаление задания")]
        [OperationContract]
        void DeleteTask(Guid parameters);

        #endregion


#region TASK ATTACHMENT
        [ServiceAction("Вложение файла")]
		[OperationContract]
        Guid AddTaskAttachment(AddTaskAttachmentParameterSet parameters);

        [ServiceAction("Изменение файла")]
        [OperationContract]
        void EditTaskAttachment(EditTaskAttachmentParameterSet parameters);

        [ServiceAction("Удаление файла")]
        [OperationContract]
        void DeleteTaskAttachment(Guid parameters);

        [ServiceAction("Получение списка вложений")]
		[OperationContract]
		List<TaskAttachmentDTO> GetTaskAttachementList(Guid parameters);
        #endregion



#region TASK RECORDS
        [ServiceAction("Добавление новой записи задания ЦПДД")]
        [OperationContract]
        Guid AddTaskRecordCPDD(AddTaskRecordCpddParameterSet parameters);

        [ServiceAction("Добавление новой записи задания ПДС")]
        [OperationContract]
        Guid AddTaskRecordPDS(AddTaskRecordPdsParameterSet parameters);

        [ServiceAction("Редактирование записи задания ЦПДД")]
        [OperationContract]
        void EditTaskRecordPDS(EditTaskRecordPdsParameterSet parameters);

        [ServiceAction("Редактирование записи задания ПДС")]
        [OperationContract]
        void EditTaskRecordCPDD(EditTaskRecordCpddParameterSet parameters);

        [ServiceAction("Получение списка записей для задания ЦПДД")]
		[OperationContract]
		List<TaskRecordDTO> GetTaskRecordCPDDList(GetTaskRecordsCpddParameterSet parameters);

		[ServiceAction("Получение списка записей для задания ПДС")]
		[OperationContract]
		List<TaskRecordPdsDTO> GetTaskRecordPDSList(GetTaskRecordsPdsParameterSet parameters);

        [ServiceAction("Удаление строки задания")]
        [OperationContract]
        void RemoveTaskRecord(Guid parameters);

        #endregion



        #region TASK RECORD NOTES
        [ServiceAction("Добавление нового комментария")]
        [OperationContract]
        Guid AddRecordNote(AddRecordNoteParameterSet parameters);

        [ServiceAction("Редактирование комментария")]
        [OperationContract]
        void EditTaskRecordNote(EditRecordNoteParameterSet parameters);

        [ServiceAction("Удаление комментария")]
        [OperationContract]
        void RemoveTaskRecordNote(Guid parameters);

        [ServiceAction("Получение списка комментариев")]
        [OperationContract]
        List<RecordNoteDTO> GetRecordNoteList(GetRecordNoteListParameterSet parameters);
#endregion




        [ServiceAction("Получение списка статусов")]
        [OperationContract]
        List<TaskStatusDTO> GetTaskStatusesList(Guid parameters);


        [ServiceAction("Установка нового статуса")]
        [OperationContract]
        void SetTaskStatus(SetTaskStatusParameterSet parameters);



        [ServiceAction("Утверждено для ЛПУ")]
        [OperationContract]
        Guid TaskApprovedPds(Guid parameters);

        [ServiceAction("Выполнено")]
        [OperationContract]
        Guid TaskPerformed(Guid parameters);

        [ServiceAction("Аннулировано")]
        [OperationContract]
        Guid TaskAnnuled(SetTaskStatusParameterSet parameters);

		[ServiceAction("Утвержденно ЦПДД")]
		[OperationContract]
		Guid TaskApprovedCPDD(SetTaskStatusParameterSet parameters);

		[ServiceAction("Скоректированно")]
		[OperationContract]
		Guid TaskCorrected(SetTaskStatusParameterSet parameters);
		
        [ServiceAction("Корректируется")]
		[OperationContract]
		Guid TaskCorrecting(SetTaskStatusParameterSet parameters);

		[ServiceAction("Отправленно")]
		[OperationContract]
		Guid TaskSubmited(SetTaskStatusParameterSet parameters);



        [ServiceAction("Выполнение задачи")]
        [OperationContract]
        void TaskRecordExecuted(Guid parameters);

        [ServiceAction("Сброс Выполнение задачи")]
        [OperationContract]
        void TaskRecordResetExecuted(Guid parameters);



        [ServiceAction("Сбросить контроль со строки задания")]
        [OperationContract]
        void ResetToControlTaskRecord(Guid parameters);

        [ServiceAction("Взять на контроль строку задание")]
        [OperationContract]
        void SetToControlTaskRecord(Guid parameters);



        [ServiceAction("Квитировать строку")]
        [OperationContract]
        void SetACK(Guid parameters);

        [ServiceAction("Отменить квитирование строки")]
        [OperationContract]
        void ResetACK(Guid parameters);




        [ServiceAction("Получение списка неквитированных диспетчерских заданий с фильтрацией по ЛПУ")]
        [OperationContract]
        List<TaskRecordPdsDTO> GetNotAckedTaskList(GetTaskListParameterSet parameters);
    }
}
