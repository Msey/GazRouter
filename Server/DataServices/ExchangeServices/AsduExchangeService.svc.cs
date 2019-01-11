using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GazRouter.DAL.Bindings.EntityPropertyBindings;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.TaskRecords;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.DispatcherTasks.TaskRecords.AddTaskRecordCPDD;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.TaskRecords;

namespace GazRouter.DataServices.ExchangeServices
{
    [ErrorHandlerLogger("exchangeLogger")]
    public class AsduExchangeService : AnonymousServiceBase, IAsduExchangeService
    {

        public string GetTask(string data)
        {
            string statusMessage;
            try
            {
                ProcessAsduMessage(data);
                statusMessage = XmlMessageStatus.OkXml();
            }
            catch (IntegrationServiceException ex)
            {
                var id = Guid.NewGuid();
                _logger.WriteIntegrationServiceException(id, ex, data);
                statusMessage = XmlMessageStatus.ErrorXml(id, ex.Message, (int) ex.Code);
            }
            return statusMessage;
        }

        private void ProcessAsduMessage(string message)
        {
            var serializer = new XmlSerializer(typeof(XmlMessage));
            using (var reader = new StringReader(message))
            {
                XmlMessage xmlMessage;
                try
                {
                    xmlMessage = (XmlMessage)serializer.Deserialize(reader);
                }
                catch (Exception)
                {
                    throw new IntegrationServiceException("Формат xml не соответствует схеме.", IntegrationErrorCodes.Xml);
                }
                RegisterTask(xmlMessage);
            }
        }

        private static AddTaskRecordCpddParameterSet GetParameterSet(TaskRecord taskRecord, Guid taskId, ExecutionContext context)
        {
            var sourceId = new GetSourcesListQuery(context).Execute().Single(s => s.SysName == "asduesg").Id;
            
            var prms = new GetEntityPropertyBindingParameterSet
            {
                SourceId = sourceId,
                ExtKey = taskRecord.ExtKey
            };


            var binding = new GetEntityPropertyBindingQuery(context).Execute(prms);
            if (binding == null)
            {
                throw new IntegrationServiceException(string.Format("Запись в таблице перекодировки с id = '{0}' не найдена.", taskRecord.ExtKey), IntegrationErrorCodes.BindingDoesNotExists);
            }

            return new AddTaskRecordCpddParameterSet
            {
                TaskId = taskId,
                CompletionDate = taskRecord.CompletionDate,
                Description = taskRecord.Description,
                EntityId = binding.EntityId,
                OrderNo = taskRecord.OrderNo,
                PeriodTypeId = binding.PeriodTypeId,
                PropertyTypeId = binding.PropertyTypeId,
                TargetValue = taskRecord.TargetValue,
                UserNameCpdd = taskRecord.UserNameCPDD,
            };
        }

        private void RegisterTask(XmlMessage xml)
        {
            using (var context = OpenDbContext())
            {
	            Guid taskId;
	            var param = new AddTaskCpddParameterSet
		                        {
			                        GlobalTaskId = xml.GlobalTaskId,
			                        UserNameCpdd = xml.UserNameCpdd,
			                        Description = xml.Description,
			                        Subject = xml.Subject,
			                        IsAproved = false
		                        };
                switch (xml.Status)
                {
                    case MessageStatus.Consultative:
						taskId = new AddTaskCpddCommand(context).Execute(param);
						foreach (var tr in xml.GetTaskRecords())
                        {
                            var prm = GetParameterSet(tr, taskId, context);
                            var recordId = new AddTaskRecordCPDDCommand(context).Execute(prm);
							//foreach (var note in tr.Notes)
							//{
							//    new AddRecordNoteCommand(context).Execute(new AddRecordNoteParameterSet {TaskId = taskId, PropertyTypeId = prm.PropertyTypeId, CompUnitId = prm.CompUnitId, UserNameCpdd = prm.UserNameCPDD, Note = note});
							//}
                        }
                        break;
                    case MessageStatus.Confirmed:
						var task = new GetTaskByGlobalTaskId(context).Execute(xml.GlobalTaskId);
                        if (task == null)
                        {
                            throw new IntegrationServiceException(string.Format("Диспетчерское задание со статусом 'Consultative' and id = '{0}' не найдено.", xml.GlobalTaskId), IntegrationErrorCodes.TaskDoesNotExists);
                        }
		                param.IsAproved = true;
						taskId = new AddTaskCpddCommand(context).Execute(param);
                        foreach (var tr in xml.GetTaskRecords())
                        {
                            var prm = GetParameterSet(tr, taskId, context);
                            var recordId = new AddTaskRecordCPDDCommand(context).Execute(prm);
							//foreach (var note in tr.Notes)
							//{
							//    new AddRecordNoteCommand(context).Execute(new AddRecordNoteParameterSet { TaskId = taskId, PropertyTypeId = prm.PropertyTypeId, CompUnitId = prm.CompUnitId, UserNameCpdd = prm.UserNameCPDD, Note = note });
							//}
                        }
                        break;
                    default:
                        throw new NotSupportedException("Неизвестный статус задания");
                }
            }

        }
    }


    public class IntegrationServiceException : Exception
    {
        public IntegrationErrorCodes Code { get; private set; }

        public IntegrationServiceException(string message, IntegrationErrorCodes code)
            : base(message)
        {
            Code = code;
        }
        public IntegrationServiceException(string message, IntegrationErrorCodes code, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }
    }

    public enum IntegrationErrorCodes
    {
        Xml,
        TaskDoesNotExists,
        BindingDoesNotExists
    }
}
