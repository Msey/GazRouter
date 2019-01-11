using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.EventLog.QueueExchangeEventCommands;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.Log;
using GazRouter.DTO.EventLog;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.Net;
using System.IO;
using GazRouter.DAL.EventLog;
using System.Xml;
using System.Xml.Linq;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.DAL.EventLog.Attachments;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.DAL.Dictionaries.Enterprises;
using System.Linq;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public class EventExchangeParams
    {
        public string url = AppSettingsManager.EventExchangeAgentUrl;
        public string user = AppSettingsManager.EventExchangeAgentUser;
        public string password = AppSettingsManager.EventExchangeAgentPassword;
        public string soap = AppSettingsManager.EventExchangeAgentSoap;
        public string enc = AppSettingsManager.EventExchangeAgentEnc;
        public string action = AppSettingsManager.EventExchangeAgentAction;
        public Guid EnterpriseId = AppSettingsManager.CurrentEnterpriseId;
        public string Enterprise = "";
    }


    public static class IncidentSituationsDispatcherTasksAgent
    {


        public static string evExchDatetimePattern = "{DateTime.Now}";
        public static string evExchChangingUserNamePattern = "{template.ChangingUserName}";
        public static string evExchIdPattern = "{template.Id}";
        public static string evExchEventDatetimePattern = "{template.EventDateTime}";
        public static string evExchDescriptionPattern = "{template.EventDescription}";
        public static string evExchangeDateTimeFormat = "yyyy-MM-ddTHH:mm:ss\"\"zzz";
        public static string evExchangeEnterpriseIdPattern = "{template.EnterpriseAsduId}";

        static MyLogger logger = new MyLogger("eventExchangeLogger");

        static EventExchangeParams instance = new EventExchangeParams();

        static ExchangeSoapExporter exporter = new ExchangeSoapExporter(instance);

        public static void Run()
        {
            logger.Trace($"[{DateTime.Now.ToShortDateString()}]: xml генерация началась");
            var newExchangeEvents = GetNewExchangeEvents();
            logger.Info("Найдены новые нештатные ситуации: "+ newExchangeEvents.Count);
            for (int i = 0; i < newExchangeEvents.Count; i++)
            {
                try {
                    logger.Trace("генерируем СОАП сообщение для веб сервиса для нештатной ситуации");

                    if (instance.Enterprise == "")
                    {
                        logger.Trace("получаем значение поля для идентификатора отправителя");
                        using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
                        {                           
                            var enterprise = new GetEnterpriseListQuery(context).Execute(instance.EnterpriseId).FirstOrDefault(o => !string.IsNullOrEmpty(o.AsduCode));
                            if (enterprise != null)
                            {
                                instance.Enterprise = enterprise.AsduCode;                                
                            }

                            logger.Info("Название предприятия: " + (string.IsNullOrEmpty(instance.Enterprise)? "отсутствует": instance.Enterprise));
                        }
                    }

                    string request = CreateSoapXmlRequestFromItem(newExchangeEvents[i]);
                    if (exporter.SendExchangeEvent(request, logger))
                    {
                        logger.Trace("меняем      статус на *sent*");
                        ChangeExchangeEventInDb(newExchangeEvents[i], ExchangeEventStatus.Sent);
                      //  AddEventAttachment(newExchangeEvents[i]);
                        logger.Info("изменен статус нештатной ситуации на *sent*");
                    }
                } catch (Exception ex)
                {
                    logger.Error(ex.Message + "\n" + ex.StackTrace);
                }                
            }
        }


        public static string CreateSoapXmlRequestFromItem(EventExchangeDTO template)
        {
            string soapString = Resource.RequestString;
            soapString = soapString.Replace(evExchDatetimePattern, DateTime.Now.ToString(evExchangeDateTimeFormat));
            soapString = soapString.Replace(evExchChangingUserNamePattern, template.ChangingUserName);
            soapString = soapString.Replace(evExchIdPattern, template.Id.ToString());
            soapString = soapString.Replace(evExchEventDatetimePattern, template.EventDateTime.ToString(evExchangeDateTimeFormat));
            soapString = soapString.Replace(evExchDescriptionPattern, template.EventDescription);
            soapString = soapString.Replace(evExchangeEnterpriseIdPattern, instance.Enterprise);
            logger.Trace("сгенерировали СОАП сообщение в виде: "+ soapString);
            return soapString;
        }

        private static void ChangeExchangeEventInDb(EventExchangeDTO template, ExchangeEventStatus status)
        {
            try
            {
                using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
                {
                    new AddQueueExchangeEventCommand(context).Execute(new QueueExchangeEventParameterSet()
                    {
                        ChangingUserName = template.ChangingUserName,
                        EventDateTime = template.EventDateTime,
                        EventId = template.Id,
                        EventStatus = status
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private static void AddEventAttachment(EventExchangeDTO template)
        {
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
            {
                new AddEventAttachmentCommand(context).Execute(new AddEventAttachmentParameterSet()
                {
                    Description = "ExchangeEventStatus.Sent",
                    Data = new byte[] { },
                    FileName = "ExchangeEventTest",
                    EventId = template.Id
                });
            }            
        }



        public static List<EventExchangeDTO> GetNewExchangeEvents()
        {
            logger.Trace("получаем новые нештатные ситуации");
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
            {
                return new GetQueueExchangeEventListQuery(context).Execute(new GetQueueExchangeEventListParameterSet() { Status = ExchangeEventStatus.New });
            }
        }
    }


    public class Body
    {
        [XmlElement(ElementName = "Status", Namespace = "")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public class StatusEnvelope
    {
        [XmlElement(ElementName = "Body")]
        public Body Body { get; set; }
    }
}
