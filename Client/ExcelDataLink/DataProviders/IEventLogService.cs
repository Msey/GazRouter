using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.DTO.EventLog.EventAnalytical;
using GazRouter.DTO.EventLog.EventRecipient;
using GazRouter.DTO.EventLog.EventTextTemplates;
using GazRouter.DTO.EventLog.TextTemplates;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.EventLog  
{
    [ServiceContract]
    public interface IEventLogService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEventList(GetEventListParameterSet parameter, AsyncCallback callback, object state);
        List<EventDTO> EndGetEventList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEventRecepientList(int parameters, AsyncCallback callback, object state);
        List<EventRecepientDTO> EndGetEventRecepientList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTakeToControlEvent(TakeToControlEventParameterSet parameters, AsyncCallback callback, object state);
        void EndTakeToControlEvent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginBackToNormalEvent(BackToNormalEventParameterSet parameters, AsyncCallback callback, object state);
        void EndBackToNormalEvent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEventAttachment(AddEventAttachmentParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddEventAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditEventAttachment(EditEventAttachmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditEventAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteEventAttachement(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteEventAttachement(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEventAndRecepients(AddEventAndRecepientsParameterSet parameters, AsyncCallback callback, object state);
        int EndAddEventAndRecepients(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditEventAndRecepients(EditEventAndRecepientsParameterSet parameters, AsyncCallback callback, object state);
        void EndEditEventAndRecepients(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRecepients(EditRecepientsParameterSet parameters, AsyncCallback callback, object state);
        void EndEditRecepients(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteEvent(int parameters, AsyncCallback callback, object state);
        void EndDeleteEvent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAckEvent(AckEventParameterSet parameters, AsyncCallback callback, object state);
        void EndAckEvent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEventTextTemplateList(Guid parameter, AsyncCallback callback, object state);
        List<EventTextTemplateDTO> EndGetEventTextTemplateList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEventTextTemplate(AddEventTextTemplateParameterSet parameters, AsyncCallback callback, object state);
        int EndAddEventTextTemplate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditEventTextTemplate(EditEventTextTemplateParameterSet parameters, AsyncCallback callback, object state);
        void EndEditEventTextTemplate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteEventTextTemplate(int parameters, AsyncCallback callback, object state);
        void EndDeleteEventTextTemplate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetNotAckEventCount(Guid parameters, AsyncCallback callback, object state);
        NonAckEventCountDTO EndGetNotAckEventCount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEventAnalyticalList(EventAnalyticalParameterSet parameters, AsyncCallback callback, object state);
        List<EventAnalyticalDTO> EndEventAnalyticalList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEventAnalyticalAckList(EventAnalyticalParameterSet parameters, AsyncCallback callback, object state);
        List<EventAnalyticalDTO> EndEventAnalyticalAckList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginMoveToTrash(MoveToTrashEventParameterSet parameters, AsyncCallback callback, object state);
        void EndMoveToTrash(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRestoreFromTrash(RestoreFromTrashEventParameterSet parameters, AsyncCallback callback, object state);
        void EndRestoreFromTrash(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEventAttachmentList(int parameters, AsyncCallback callback, object state);
        List<EventAttachmentDTO> EndGetEventAttachmentList(IAsyncResult result);
    }


    public class EventLogServiceProxy : DataProviderBase<IEventLogService>
	{
        protected override string ServiceUri
        {
            get { return "/EventLog/EventLogService.svc"; }
        }

        public Task<List<EventDTO>> GetEventListAsync(GetEventListParameterSet parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EventDTO>,GetEventListParameterSet>(channel, channel.BeginGetEventList, channel.EndGetEventList, parameter);
        }

        public Task<List<EventRecepientDTO>> GetEventRecepientListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EventRecepientDTO>,int>(channel, channel.BeginGetEventRecepientList, channel.EndGetEventRecepientList, parameters);
        }

        public Task TakeToControlEventAsync(TakeToControlEventParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginTakeToControlEvent, channel.EndTakeToControlEvent, parameters);
        }

        public Task BackToNormalEventAsync(BackToNormalEventParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginBackToNormalEvent, channel.EndBackToNormalEvent, parameters);
        }

        public Task<Guid> AddEventAttachmentAsync(AddEventAttachmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddEventAttachmentParameterSet>(channel, channel.BeginAddEventAttachment, channel.EndAddEventAttachment, parameters);
        }

        public Task EditEventAttachmentAsync(EditEventAttachmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditEventAttachment, channel.EndEditEventAttachment, parameters);
        }

        public Task DeleteEventAttachementAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteEventAttachement, channel.EndDeleteEventAttachement, parameters);
        }

        public Task<int> AddEventAndRecepientsAsync(AddEventAndRecepientsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEventAndRecepientsParameterSet>(channel, channel.BeginAddEventAndRecepients, channel.EndAddEventAndRecepients, parameters);
        }

        public Task EditEventAndRecepientsAsync(EditEventAndRecepientsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditEventAndRecepients, channel.EndEditEventAndRecepients, parameters);
        }

        public Task EditRecepientsAsync(EditRecepientsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRecepients, channel.EndEditRecepients, parameters);
        }

        public Task DeleteEventAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteEvent, channel.EndDeleteEvent, parameters);
        }

        public Task AckEventAsync(AckEventParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAckEvent, channel.EndAckEvent, parameters);
        }

        public Task<List<EventTextTemplateDTO>> GetEventTextTemplateListAsync(Guid parameter)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EventTextTemplateDTO>,Guid>(channel, channel.BeginGetEventTextTemplateList, channel.EndGetEventTextTemplateList, parameter);
        }

        public Task<int> AddEventTextTemplateAsync(AddEventTextTemplateParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEventTextTemplateParameterSet>(channel, channel.BeginAddEventTextTemplate, channel.EndAddEventTextTemplate, parameters);
        }

        public Task EditEventTextTemplateAsync(EditEventTextTemplateParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditEventTextTemplate, channel.EndEditEventTextTemplate, parameters);
        }

        public Task DeleteEventTextTemplateAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteEventTextTemplate, channel.EndDeleteEventTextTemplate, parameters);
        }

        public Task<NonAckEventCountDTO> GetNotAckEventCountAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<NonAckEventCountDTO,Guid>(channel, channel.BeginGetNotAckEventCount, channel.EndGetNotAckEventCount, parameters);
        }

        public Task<List<EventAnalyticalDTO>> EventAnalyticalListAsync(EventAnalyticalParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EventAnalyticalDTO>,EventAnalyticalParameterSet>(channel, channel.BeginEventAnalyticalList, channel.EndEventAnalyticalList, parameters);
        }

        public Task<List<EventAnalyticalDTO>> EventAnalyticalAckListAsync(EventAnalyticalParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EventAnalyticalDTO>,EventAnalyticalParameterSet>(channel, channel.BeginEventAnalyticalAckList, channel.EndEventAnalyticalAckList, parameters);
        }

        public Task MoveToTrashAsync(MoveToTrashEventParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginMoveToTrash, channel.EndMoveToTrash, parameters);
        }

        public Task RestoreFromTrashAsync(RestoreFromTrashEventParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRestoreFromTrash, channel.EndRestoreFromTrash, parameters);
        }

        public Task<List<EventAttachmentDTO>> GetEventAttachmentListAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EventAttachmentDTO>,int>(channel, channel.BeginGetEventAttachmentList, channel.EndGetEventAttachmentList, parameters);
        }

    }
}
