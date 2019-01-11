using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Bindings.EntityPropertyBindings;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.Attachments;
using GazRouter.DAL.DispatcherTasks.RecordNotes;
using GazRouter.DAL.DispatcherTasks.TaskRecords;
using GazRouter.DAL.DispatcherTasks.TaskStatuses;
using GazRouter.DataServices.ExchangeServices;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.DispatcherTasks.TaskRecords.AddTaskRecordCPDD;
using GazRouter.DAL.DispatcherTasks.Tasks;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;

namespace GazRouter.DataServices.DispatcherTask
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class DispatcherTaskService : ServiceBase, IDispatcherTaskService
    {

        #region TASKS
        public List<TaskDTO> GetTaskList(GetTaskListParameterSet parameters)
        {
            return ExecuteRead<GetTaskListQuery, List<TaskDTO>, GetTaskListParameterSet>(parameters);
        }
        
        public Guid AddTaskCpdd(AddTaskCpddParameterSet parameters)
        {
            return ExecuteRead<AddTaskCpddCommand, Guid, AddTaskCpddParameterSet>(parameters);
        }

		public void DeleteTask(Guid parameters)
		{
			ExecuteNonQuery<DeleteTaskCommand, Guid>(parameters);
		}

        public Guid AddTask(AddTaskParameterSet parameters)
        {
            return ExecuteRead<AddTaskCommand, Guid, AddTaskParameterSet>(parameters);
        }

        public void EditTask(EditTaskParameterSet parameters)
        {
            ExecuteNonQuery<EditTaskCommand, EditTaskParameterSet>(parameters);
        }


        public Guid AddMultiRecordTask(AddTaskParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var taskId = new AddTaskCommand(context).Execute(parameters);

                foreach (var siteId in parameters.SiteIdList)
                {
                    new AddTaskRecordPDSCommand(context).Execute(
                        new AddTaskRecordPdsParameterSet
                        {
                            TaskId = taskId,
                            EntityId = siteId,
                            PropertyTypeId = PropertyType.None,
                            PeriodTypeId = PeriodType.Twohours,
                            TargetValue = "",
                            Description = parameters.Description,
                            CompletionDate = parameters.CompletionDate,
                            SiteId = siteId
                        });
                }
                
                return taskId;
            }
        }

        public Guid CloneTask(AddTaskParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var taskId = new AddTaskCommand(context).Execute(parameters);
                
                var recordList = new GetTaskRecordsListQuery(context).Execute(
                    new GetTaskRecordsCpddParameterSet
                    {
                        IsCpdd = false,
                        TaskVersionId = parameters.SourceTaskId
                    });

                foreach (var rec in recordList)
                {
                    new AddTaskRecordPDSCommand(context).Execute(
                        new AddTaskRecordPdsParameterSet
                        {
                            TaskId = taskId,
                            EntityId = rec.Entity.Id,
                            PropertyTypeId = rec.PropertyTypeId,
                            PeriodTypeId = rec.PeriodTypeId,
                            TargetValue = rec.TargetValue,
                            Description = rec.Description,
                            CompletionDate = DateTime.Now.AddDays(1),
                            SiteId = rec.SiteId
                        });
                }

                return taskId;
            }
        }


        #endregion


        #region TASK ATTACHMENT
        public Guid AddTaskAttachment(AddTaskAttachmentParameterSet parameters)
		{
			return ExecuteRead<AddTaskAttachmentCommand, Guid, AddTaskAttachmentParameterSet>(parameters);
		}
        public void EditTaskAttachment(EditTaskAttachmentParameterSet parameters)
        {
            ExecuteNonQuery<EditTaskAttachmentCommand, EditTaskAttachmentParameterSet>(parameters);
        }

        public void DeleteTaskAttachment(Guid parameters)
        {
            ExecuteNonQuery<DeleteTaskAttachmentCommand, Guid>(parameters);
        }

        public List<TaskAttachmentDTO> GetTaskAttachementList(Guid parameters)
		{
			return ExecuteRead<GetTaskAttachmentListQuery, List<TaskAttachmentDTO>, Guid>(parameters);
		}
        #endregion



        #region TASK RECORDS
        public Guid AddTaskRecordCPDD(AddTaskRecordCpddParameterSet parameters)
        {
            return ExecuteRead<AddTaskRecordCPDDCommand, Guid, AddTaskRecordCpddParameterSet>(parameters);
        }

        public Guid AddTaskRecordPDS(AddTaskRecordPdsParameterSet parameters)
        {
            return ExecuteRead<AddTaskRecordPDSCommand, Guid, AddTaskRecordPdsParameterSet>(parameters);
        }

		public List<TaskRecordDTO> GetTaskRecordCPDDList(GetTaskRecordsCpddParameterSet parameters)
		{
			return ExecuteRead<GetTaskRecordsListQuery, List<TaskRecordDTO>, GetTaskRecordsCpddParameterSet>(parameters);
		}

		public List<TaskRecordPdsDTO> GetTaskRecordPDSList(GetTaskRecordsPdsParameterSet parameters)
		{
            return ExecuteRead<GetTaskRecordsLpuListQuery, List<TaskRecordPdsDTO>, GetTaskRecordsPdsParameterSet>(parameters);
		}
        #endregion




        #region TASK RECORD NOTE
        public Guid AddRecordNote(AddRecordNoteParameterSet parameters)
        {
            return ExecuteRead<AddRecordNoteCommand, Guid, AddRecordNoteParameterSet>(parameters);
        }

        public void EditTaskRecordNote(EditRecordNoteParameterSet parameters)
        {
            ExecuteNonQuery<EditRecordNoteCommand, EditRecordNoteParameterSet>(parameters);
        }

        public void RemoveTaskRecordNote(Guid parameters)
        {
            ExecuteNonQuery<RemoveRecordNoteCommand, Guid>(parameters);
        }

        public List<RecordNoteDTO> GetRecordNoteList(GetRecordNoteListParameterSet parameters)
        {
            return ExecuteRead<GetRecordNoteListQuery, List<RecordNoteDTO>, GetRecordNoteListParameterSet>(parameters);
        }
        #endregion



        public List<TaskStatusDTO> GetTaskStatusesList(Guid parameters)
        {
            return ExecuteRead<GetTaskStatusListQuery, List<TaskStatusDTO>, Guid>(parameters);
        }



        public void SetTaskStatus(SetTaskStatusParameterSet parameters)
        {
            switch (parameters.StatusType)
            {
                case StatusType.OnSubmit:
                    break;
                case StatusType.Submitted:
                    break;

                case StatusType.Corrected:
                    TaskCorrected(parameters);
                    break;

                case StatusType.ApprovedByCpdd:
                    TaskApprovedCPDD(parameters);
                    break;

                case StatusType.Created:
                    break;

                case StatusType.ApprovedForSite:
                    TaskApprovedPds(parameters.TaskId);
                    break;

                case StatusType.Performed:
                    TaskPerformed(parameters.TaskId);
                    break;

                case StatusType.Annuled:
                    TaskAnnuled(parameters);
                    break;

                case StatusType.Correcting:
                    TaskCorrecting(parameters);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Guid TaskApprovedPds(Guid parameters)
        {
            return ExecuteRead<TaskApprovedPdsCommand, Guid, Guid>(parameters);
        }

        public Guid TaskPerformed(Guid parameters)
        {
            Guid result;
            using (var context = OpenDbContext())
            {
                result = TaskPerformedInternal(parameters, context);
            }
            return result;
        }

        private Guid TaskPerformedInternal(Guid parameters, ExecutionContextReal context)
        {
            Guid result = new TaskPerformedCommand(context).Execute(parameters);
            var task = new GetTaskByVersionIdQuery(context).Execute(result);
            if (!string.IsNullOrEmpty(task.GlobalTaskId))
            {
                var fileName = string.Format("performed_task_{0}.xml", DateTime.Now.ToString("HH_mm_ss"));
                var records = GetRecords(task, context);
                var message = XmlMessage.Create(Session.User, task, records);
                XmlMessageFileHelper.Save(fileName, message);
            }
            return result;
        }

        public Guid TaskAnnuled(SetTaskStatusParameterSet parameters)
        {
            return ExecuteRead<TaskAnnuledCommand, Guid, SetTaskStatusParameterSet>(parameters);
        }

        public void TaskRecordExecuted(Guid parameters)
        {
            using (var context = OpenDbContext())
            {
                new TaskRecordsExecutedCommand(context).Execute(parameters);
                //var records = new GetTaskRecordsVersionListQuery(context).Execute(parameters);
                //if (!records.All(p => p.ExecutedDate.HasValue)) return;
                //var firstOrDefault = records.FirstOrDefault();
                //if (firstOrDefault != null)
                //    TaskPerformedInternal(firstOrDefault.TaskId, context);
            }
        }

        public void TaskRecordResetExecuted(Guid parameters)
        {
            ExecuteNonQuery<TaskRecordsResetExecutedCommand, Guid>(parameters);
        }

        public Guid TaskApprovedCPDD(SetTaskStatusParameterSet parameters)
        {
            return ExecuteRead<TaskApprovedCPDDCommand, Guid, SetTaskStatusParameterSet>(parameters);
        }

        public Guid TaskCorrected(SetTaskStatusParameterSet parameters)
        {
            var fileName = string.Format("corrected_task_{0}.xml", DateTime.Now.ToString("HH_mm_ss"));
            IEnumerable<TaskRecordDTO> records;
            Guid taskVersionId;
            TaskDTO task;
            using (var context = OpenDbContext())
            {
                taskVersionId = (new TaskCorrectedCommand(context)).Execute(parameters);
                task = new GetTaskByVersionIdQuery(context).Execute(taskVersionId);
                records = GetRecords(task, context);
            }
            var message = XmlMessage.Create(Session.User, task, records);
            XmlMessageFileHelper.Save(fileName, message);
            return taskVersionId;
        }

        private IEnumerable<TaskRecordDTO> GetRecords(TaskDTO task, ExecutionContextReal context)
        {
            var sourceId = new GetSourcesListQuery(context).Execute().Single(s => s.SysName == "asduesg").Id;
            var records = new GetTaskRecordsListQuery(context).Execute(new GetTaskRecordsCpddParameterSet {IsCpdd = true, TaskVersionId = task.LastVersionId});
            var command = new GetEntityPropertyBindingSourceQuery(context);
            foreach (var taskRecordDTO in records)
            {
                var tempData = command.Execute(new GetEntityPropertyBindingSourceParameterSet
                {
                    EntityId = taskRecordDTO.Entity.Id, PeriodTypeId = taskRecordDTO.PeriodTypeId, SourceId = sourceId, PropertyTypeId = taskRecordDTO.PropertyTypeId
                });
                if (tempData == null)
                    throw new Exception("");
                taskRecordDTO.ExtKey = tempData.ExtKey;
            }
            //var result =  from r in records let notes = GetRecordNoteList(new GetRecordNoteListParameterSet { TaskId = task.Id, CompUnitId = r.CompUnitId, PropertyTypeId = r.PropertyTypeId })
            //       select new Tuple<TaskRecordDTO, IEnumerable<RecordNoteDTO>>(r, notes);
            return records.ToList();
        }

        public Guid TaskCorrecting(SetTaskStatusParameterSet parameters)
        {
            return ExecuteRead<TaskCorrectingCommand, Guid, SetTaskStatusParameterSet>(parameters);
        }

        public Guid TaskSubmited(SetTaskStatusParameterSet parameters)
        {
            var fileName = string.Format("submited_task_{0}.xml", DateTime.Now.ToString("HH_mm_ss"));
            IEnumerable<TaskRecordDTO> records;
            Guid taskVersionId;
            TaskDTO task;
            using (var context = OpenDbContext())
            {
                taskVersionId = (new TaskSubmitedCommand(context)).Execute(parameters);
                task = new GetTaskByVersionIdQuery(context).Execute(taskVersionId);
                records = GetRecords(task, context);
            }
            var message = XmlMessage.Create(Session.User, task, records);
            XmlMessageFileHelper.Save(fileName, message);
            return taskVersionId;
        }

        public void RemoveTaskRecord(Guid parameters)
        {
            ExecuteNonQuery<RemoveTaskRecordCommand, Guid>(parameters);
        }

        public void ResetToControlTaskRecord(Guid parameters)
        {
            ExecuteNonQuery<ResetToControlTaskRecordCommand, Guid>(parameters);
        }

        public void SetToControlTaskRecord(Guid parameters)
        {
            ExecuteNonQuery<SetToControlTaskRecordCommand, Guid>(parameters);
        }

        public void EditTaskRecordPDS(EditTaskRecordPdsParameterSet parameters)
        {
            ExecuteNonQuery<EditTaskRecordPDSCommand, EditTaskRecordPdsParameterSet>(parameters);
        }

        public void EditTaskRecordCPDD(EditTaskRecordCpddParameterSet parameters)
        {
            ExecuteNonQuery<EditTaskRecordCPDDCommand, EditTaskRecordCpddParameterSet>(parameters);
        }


        public void SetACK(Guid parameters)
        {
            ExecuteNonQuery<SetTaskRecordAckCommand, Guid>(parameters);
        }

        public void ResetACK(Guid parameters)
        {
            ExecuteNonQuery<ResetTaskRecordAckCommand, Guid>(parameters);
        }

        public List<TaskRecordPdsDTO> GetNotAckedTaskList(GetTaskListParameterSet parameters)
        {
            return ExecuteRead<GetNonAckRecordsQuery, List<TaskRecordPdsDTO>, GetTaskListParameterSet>(parameters);
        }
    }
}
