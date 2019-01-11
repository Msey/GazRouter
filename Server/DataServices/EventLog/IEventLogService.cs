using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.DTO.EventLog.EventAnalytical;
using GazRouter.DTO.EventLog.EventRecipient;
using GazRouter.DTO.EventLog.EventTextTemplates;
using GazRouter.DTO.EventLog.TextTemplates;

namespace GazRouter.DataServices.EventLog
{
    [Service("Журнал событий")]
    [ServiceContract]
    public interface IEventLogService
    {
        [ServiceAction("Получение списка событий")]
        [OperationContract]
        List<EventDTO> GetEventList(GetEventListParameterSet parameter);

        [ServiceAction("Получение списка пользователей в журнале событий")]
        [OperationContract]
        List<EventRecepientDTO> GetEventRecepientList(int parameters);

        [ServiceAction("Взять на контроль")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        void TakeToControlEvent(TakeToControlEventParameterSet parameters);

		[ServiceAction("Снять с контроля")]
		[OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
		void BackToNormalEvent(BackToNormalEventParameterSet parameters);

        [ServiceAction("Вложение файла")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        Guid AddEventAttachment(AddEventAttachmentParameterSet parameters);

        [ServiceAction("Редактирование вложенного файла")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        void EditEventAttachment(EditEventAttachmentParameterSet parameters);

        [ServiceAction("Удаление вложения")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        void DeleteEventAttachement(Guid parameters);

		[ServiceAction("Добавить событие и получателей")]
		[OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        int AddEventAndRecepients(AddEventAndRecepientsParameterSet parameters);
        
        [ServiceAction("Редактировать событие и получателей")]
		[OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        void EditEventAndRecepients(EditEventAndRecepientsParameterSet parameters);
        
        [ServiceAction("Редактировать получателей")]
		[OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
		void EditRecepients(EditRecepientsParameterSet parameters);

		[ServiceAction("Удалить событие")]
		[OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
		void DeleteEvent(int parameters);

        
		[ServiceAction("Добавить Событие")]
		[OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
		void AckEvent(AckEventParameterSet parameters);


        [ServiceAction("Получение список нештатных ситуаций")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        List<EventExchangeDTO> GetQueueExchangeEventList(GetQueueExchangeEventListParameterSet parameters);

        [ServiceAction("Получение списка шаблонов текста сообщения")]
        [OperationContract]
        List<EventTextTemplateDTO> GetEventTextTemplateList(Guid parameter);

        [ServiceAction("Добавить шаблон текста сообщения")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        int AddEventTextTemplate(AddEventTextTemplateParameterSet parameters);

        [ServiceAction("Редактировать шаблон текста сообщения")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        void EditEventTextTemplate(EditEventTextTemplateParameterSet parameters);

        [ServiceAction("Удаление шаблона текста сообщения")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        void DeleteEventTextTemplate(int parameters);

        [ServiceAction("Получение списка неквитированных событий")]
        [OperationContract]
        NonAckEventCountDTO GetNotAckEventCount(Guid parameters);

        [ServiceAction("Аналитика журнала событий (диаграммы)")]
        [OperationContract]
        List<EventAnalyticalDTO> EventAnalyticalList(EventAnalyticalParameterSet parameters);

        [ServiceAction("Аналитика журнала событий (квитирование)")]
        [OperationContract]
        List<EventAnalyticalDTO> EventAnalyticalAckList(EventAnalyticalParameterSet parameters);

        [ServiceAction("Поместить получателя в корзину")]
        [OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
        void MoveToTrash(MoveToTrashEventParameterSet parameters);

		[ServiceAction("Достать получателя из корзины")]
		[OperationContract]
        [UpdateModuleStateBehavior(Module.EventLog)]
		void RestoreFromTrash(RestoreFromTrashEventParameterSet parameters);

        [ServiceAction("Получение списков примечаний и вложений")]
        [OperationContract]
        List<EventAttachmentDTO> GetEventAttachmentList(int parameters);
    }
}
