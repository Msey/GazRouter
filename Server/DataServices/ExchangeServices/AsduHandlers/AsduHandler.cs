using GazRouter.DAL.EventLog;
using GazRouter.DAL.EventLog.Attachments;
using GazRouter.DAL.EventLog.QueueExchangeEventCommands;
using GazRouter.DataServices.ExchangeServices.AsduDispTask;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GazRouter.DataServices.ExchangeServices.AsduHandlers
{
    public class AsduHandler : IHttpHandler
    {
         bool IHttpHandler.IsReusable => true;
        static MyLogger logger = new MyLogger("asduExchangeLogger");

        public void ProcessRequest(HttpContext context)
        {
           
            if (!IsEnabled())
            {
                context.Response.StatusCode = 500;
                context.Response.Write("ASDU handlers not enabled in config");
                return;
            }

            logger.Trace("1 AsduHandler is Enabled");

            if (!CheckAuth(context))
                return;

            logger.Trace("2 Auth is checked");

            if (context.Request.HttpMethod != "POST")
            {
                context.Response.Write("Wrong request, use POST!");
                return;
            }

            logger.Trace("3 request is correct");

            byte[] buffer = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(buffer, 0, buffer.Length);
          
            var logDir = AppSettingsManager.AsduSoapConfig.Dictionary.Get("logDirectory");
            if(!String.IsNullOrEmpty(logDir))
            {
                StoreRaw(logDir, context.Request, buffer);
            }


            if (CheckAndReadExchangeEvent(Encoding.UTF8.GetString(buffer, 0, buffer.Length)))
            {
                var u = context.User;

                MessageProcessing mp = new MessageProcessing(u.Identity.Name, "");
                DispatcherMessage ms = mp.ParseInput(buffer);

                mp.ParseMessage(ms, buffer);

                string reply = @"<env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope""><env:Header/><env:Body><Status>OK</Status></env:Body></env:Envelope>";

                context.Response.Headers.Add("Accept", "application/soap+xml");
                context.Response.ContentType = "application/soap+xml;charset=utf-8";
                context.Response.Write(reply);
            }
        }

        private bool CheckAuth(HttpContext context)
        {
            //return true;

            var auth = AppSettingsManager.AsduSoapConfig.Dictionary.Get("auth") ?? "";
            if(auth.Equals("basic", StringComparison.InvariantCultureIgnoreCase))
            {
                if(context.Request.IsAuthenticated)
                {
                    return true;
                }
                else
                {
                    context.Response.StatusCode = 401;
                    context.Response.Write("Authentication required");
                    return false;
                }
            }
            else if(auth.Equals("none", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                context.Response.StatusCode = 500;
                context.Response.Write("Authentication not configured");
                return false;
            }
        }

        private bool IsEnabled()
        {
            try
            {
                var enabledStr = AppSettingsManager.AsduSoapConfig.Dictionary.Get("enabled");
                return enabledStr.Equals("true", StringComparison.InvariantCultureIgnoreCase);
            }
            catch(Exception)
            {
                //Нет раздела в Web.config
                return false;
            }
        }

        private void StoreRaw(string logDir, HttpRequest request, byte[] buffer)
        {
            try
            {
                ExchangeHelper.EnsureDirectoryExist(logDir);
                var ticks = DateTime.Now.Ticks;
                string fileNameBase = Path.Combine(logDir, $"request_{ticks}");
                using (FileStream writer = System.IO.File.OpenWrite(fileNameBase + "_body.txt"))
                {
                    writer.Write(buffer, 0, buffer.Length);
                }
                using (var writer = System.IO.File.CreateText(fileNameBase + "_headers.txt"))
                {
                    writer.WriteLine(request.HttpMethod + " " + request.RawUrl);
                    foreach(var key in request.Headers.AllKeys)
                    {
                        writer.WriteLine(key + ": " + request.Headers[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }
        }


        private bool CheckAndReadExchangeEvent(string xml)
        {
            try
            {
                var Value = XDocument.Parse(xml);

                logger.Trace(xml);

                return false; // TODO : delete this after log observing

                SOAPEnvelope deserializedObject;
                using (var reader = Value.CreateReader(ReaderOptions.None))
                {
                    var ser = new XmlSerializer(typeof(SOAPEnvelope));
                    deserializedObject = (SOAPEnvelope)ser.Deserialize(reader);
                }

                var deliveryId = deserializedObject.Body.AcknowledgementMessage.HeaderSection.delivery.id;
                var resultStatus = deserializedObject.Body.AcknowledgementMessage.HeaderSection.result.status.ToLower();
                var templateId = deserializedObject.Body.AcknowledgementMessage.HeaderSection.template.id;


                if (templateId.Equals("application/psi-go-dpm-14-ims") && resultStatus.Equals("ok"))
                {
                    EventExchangeDTO appropriateEvent = GetSentExchangeEvents().Single(e => e.Id == deliveryId);

                    ChangeExchangeEventInDb(appropriateEvent);
                    AddEventAttachment(appropriateEvent);
                    return true;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }

            return false;
        }


        private static void AddEventAttachment(EventExchangeDTO template)
        {
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
            {
                new AddEventAttachmentCommand(context).Execute(new AddEventAttachmentParameterSet()
                {
                    Description = "ExchangeEventStatus.Read",
                    Data = new byte[] { },
                    FileName = "ExchangeEventTest",
                    EventId = template.Id
                });
            }
        }

        private static void ChangeExchangeEventInDb(EventExchangeDTO template)
        {
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
            {
                new AddQueueExchangeEventCommand(context).Execute(new QueueExchangeEventParameterSet()
                {
                    ChangingUserName = template.ChangingUserName,
                    EventDateTime = DateTime.Now,
                    EventId = template.Id,
                    EventStatus = ExchangeEventStatus.Read
                });
            }
            
        }

        public static List<EventExchangeDTO> GetSentExchangeEvents()
        {
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
            {
                return new GetQueueExchangeEventListQuery(context).Execute(new GetQueueExchangeEventListParameterSet() { Status = ExchangeEventStatus.Sent });
            }
        }



        #region parser_structs

        public struct Delivery
        {
            [XmlAttribute(AttributeName = "id", Namespace = "")]
            public int id { get; set; }
        }
        public struct Result
        {
            [XmlAttribute(AttributeName = "status", Namespace = "")]
            public string status { get; set; }
        }

        public struct Template
        {
            [XmlAttribute(AttributeName = "id", Namespace = "")]
            public string id { get; set; }
        }

        public struct AcknowledgementMessage
        {
            [XmlElement(ElementName = "HeaderSection", Namespace = "")]
            public HeaderSection HeaderSection { get; set; }
        }

        public struct HeaderSection
        {
            [XmlElement(ElementName = "Delivery", Namespace = "")]
            public Delivery delivery { get; set; }
            [XmlElement(ElementName = "Result", Namespace = "")]
            public Result result { get; set; }
            [XmlElement(ElementName = "Template", Namespace = "")]
            public Template template { get; set; }
        }

        public struct Body
        {
            [XmlElement(ElementName = "AcknowledgementMessage", Namespace = "")]
            public AcknowledgementMessage AcknowledgementMessage { get; set; }
        }

        [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap-envelope/")]
        public struct SOAPEnvelope
        {

            [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
            public Body Body { get; set; }
        }

        #endregion
    }
}