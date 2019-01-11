using System;
using System.Collections.Generic;
using GazRouter.DAL.EventLog;
using GazRouter.DAL.EventLog.Attachments;
using GazRouter.DAL.EventLog.EventAnalytical;
using GazRouter.DAL.EventLog.EventRecipient;
using GazRouter.DAL.EventLog.TextTemplates;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.DTO.EventLog.EventAnalytical;
using GazRouter.DTO.EventLog.EventRecipient;
using GazRouter.DTO.EventLog.EventTextTemplates;
using GazRouter.DTO.EventLog.TextTemplates;
using GazRouter.DAL.EventLog.QueueExchangeEventCommands;

namespace GazRouter.DataServices.EventLog
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class EventLogService : ServiceBase, IEventLogService
    {

        public List<EventDTO> GetEventList(GetEventListParameterSet parameter)
        {
  
            var result = ExecuteRead<GetEventListQuery, List<EventDTO>, GetEventListParameterSet>(parameter);
            return result;
        }

        public List<EventRecepientDTO> GetEventRecepientList(int parameters)
        {
            return ExecuteRead<GetEventRecepientListQuery, List<EventRecepientDTO>, int>(parameters);
        }

        public void TakeToControlEvent(TakeToControlEventParameterSet parameters)
        {
            ExecuteNonQuery<TakeToControlEventCommand, TakeToControlEventParameterSet>(parameters);
        }

		public void BackToNormalEvent(BackToNormalEventParameterSet parameters)
		{
			ExecuteNonQuery<BackToNormalEventCommand, BackToNormalEventParameterSet>(parameters);
		}

        public Guid AddEventAttachment(AddEventAttachmentParameterSet parameters)
        {
            return ExecuteRead<AddEventAttachmentCommand, Guid, AddEventAttachmentParameterSet>(parameters);
        }

        public void EditEventAttachment(EditEventAttachmentParameterSet parameters)
        {
            ExecuteNonQuery<EditEventAttachmentCommand, EditEventAttachmentParameterSet>(parameters);
        }

        public void EditRecepients(EditRecepientsParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var addRecep = new AddEventRecipientCommand(context);
                var deleteRecep = new DeleteEventRecipientCommand(context);
                foreach (var item in parameters.RecepientsToDelete)
                {
                    deleteRecep.Execute(item);
                }
                foreach (var item in parameters.RecepientsToAdd)
                {
                    addRecep.Execute(item);
                }
            }
        }

		public int AddEventAndRecepients(AddEventAndRecepientsParameterSet parameters)
		{
            using (var context = OpenDbContext())
            {
                int eventId = new RegisterEventCommand(context).Execute(parameters.Event);

                var addRecep = new AddEventRecipientCommand(context);

                foreach (var item in parameters.Recepients)
                {
                    item.EventId = eventId;
                    addRecep.Execute(item);
                }


                if (parameters.Event.TypeId == 3) // HC
                {
                    var exchangeEventParameterSet = new QueueExchangeEventParameterSet()
                    {
                        EventId = eventId,
                        EventDateTime = parameters.Event.EventDate,
                        EventStatus = ExchangeEventStatus.New
                    };

                    new AddQueueExchangeEventCommand(context).Execute(exchangeEventParameterSet);
                }

                return eventId;
            }
		}

        public void EditEventAndRecepients(EditEventAndRecepientsParameterSet parameters)
		{
            using (var context = OpenDbContext())
            {
                new EditEventCommand(context).Execute(parameters.Event);

                var addRecep = new AddEventRecipientCommand(context);
                var deleteRecep = new DeleteEventRecipientCommand(context);

                if (parameters.Event.TypeId == 3) // HC
                {
                    var exchangeEventParameterSet = new QueueExchangeEventParameterSet()
                    {
                        EventId = parameters.Event.Id,
                        EventDateTime = parameters.Event.EventDate,
                        EventStatus = ExchangeEventStatus.New
                    };

                    new AddQueueExchangeEventCommand(context).Execute(exchangeEventParameterSet);
                }

                foreach (var item in parameters.Recepients.RecepientsToDelete)
                {
                    deleteRecep.Execute(item);
                }
                foreach (var item in parameters.Recepients.RecepientsToAdd)
                {
                    addRecep.Execute(item);
                }
            }
		}


		public void DeleteEvent(int parameters)
		{
			ExecuteNonQuery<DeleteEventCommand, int>(parameters);
		}

        public List<EventAttachmentDTO> GetEventAttachmentList(int parameters)
        {
            return ExecuteRead<GetEventAttachmentListQuery, List<EventAttachmentDTO>, int>(parameters);
        }

		public void AckEvent(AckEventParameterSet parameters)
		{
			ExecuteNonQuery<AckEventCommand, AckEventParameterSet>(parameters); 
		}

        public List<EventExchangeDTO> GetQueueExchangeEventList(GetQueueExchangeEventListParameterSet parameters)
        {
            return ExecuteRead<GetQueueExchangeEventListQuery, List<EventExchangeDTO> ,GetQueueExchangeEventListParameterSet>(parameters);
        }

        public void DeleteEventAttachement(Guid parameters)
        {
            ExecuteNonQuery<DeleteEventAttachmentCommand, Guid>(parameters);
        }
        
        public NonAckEventCountDTO GetNotAckEventCount(Guid parameters)
        {
            return ExecuteRead<GetNotAckEventCountQuery, NonAckEventCountDTO, Guid>(parameters);
        }

        public List<EventAnalyticalDTO> EventAnalyticalList(EventAnalyticalParameterSet parameters)
        {
            return ExecuteRead<GetEventAnalyticalListQuery, List<EventAnalyticalDTO>, EventAnalyticalParameterSet>(parameters);
        }

        public List<EventAnalyticalDTO> EventAnalyticalAckList(EventAnalyticalParameterSet parameters)
        {
            return ExecuteRead<GetEventAnalyticalAckListQuery, List<EventAnalyticalDTO>, EventAnalyticalParameterSet>(parameters);
        }

        public void MoveToTrash(MoveToTrashEventParameterSet parameters)
        {
            ExecuteNonQuery<MoveToTrashEventCommand, MoveToTrashEventParameterSet>(parameters);
        }

		public void RestoreFromTrash(RestoreFromTrashEventParameterSet parameters)
		{
			ExecuteNonQuery<RestoreFromTrashEventCommand, RestoreFromTrashEventParameterSet>(parameters);
		}


        public List<EventTextTemplateDTO> GetEventTextTemplateList(Guid parameters)
        {
            return ExecuteRead<GetEventTextTemplateListQuery, List<EventTextTemplateDTO>, Guid>(parameters);
        }
        
        public int AddEventTextTemplate(AddEventTextTemplateParameterSet parameters)
        {
            return ExecuteRead<AddEventTextTemplateCommand, int, AddEventTextTemplateParameterSet>(parameters);
        }

        public void EditEventTextTemplate(EditEventTextTemplateParameterSet parameters)
        {
            ExecuteNonQuery<EditEventTextTemplateCommand, EditEventTextTemplateParameterSet>(parameters);
        }

        public void DeleteEventTextTemplate(int parameters)
        {
            ExecuteNonQuery<DeleteEventTextTemplateCommand, int>(parameters);
        }


    }
}
