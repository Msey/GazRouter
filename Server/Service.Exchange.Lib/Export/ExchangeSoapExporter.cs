using GazRouter.DTO.EventLog;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Run;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GazRouter.Service.Exchange.Lib.Export
{
    /// <summary>
    /// 
    /// </summary>
    public class ExchangeSoapExporter
    {
        public ExchangeSoapExporter(EventExchangeParams param)
        {
            url = param.url;// AppSettingsManager.EventExchangeAgentUrl;
            user = param.user;// AppSettingsManager.EventExchangeAgentUser;
            password = param.password;// AppSettingsManager.EventExchangeAgentPassword;
            soap = param.soap;// AppSettingsManager.EventExchangeAgentSoap;
            enc = param.enc;// AppSettingsManager.EventExchangeAgentEnc;
            action = param.action;// AppSettingsManager.EventExchangeAgentAction;
        }

        public string url { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string soap { get; set; }
        public string enc { get; set; }
        public string action { get; set; }



        public string requestHeaderName = "SOAPAction";
        public string requestVersion = "1.0";
        public string requestCharset = ";charset=utf-8";
        public string requestMethod = "POST";

        public bool SendExchangeEvent(string request, MyLogger logger)
        {

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                delegate
                { return true; };


            logger.Trace("проверка урл и экшн");
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(action)) return false;

            try
            {
                logger.Trace("реквест разбиваем на байты");
                byte[] content = Encoding.UTF8.GetBytes(request);

                HttpWebRequest webRequest = WebRequest.CreateHttp(url);

                webRequest.Headers.Add(requestHeaderName, "\\"+action);
                webRequest.ProtocolVersion = Version.Parse(requestVersion);
                webRequest.ContentType = soap + requestCharset;
                webRequest.Accept = enc;
                webRequest.Method = requestMethod;
                webRequest.Credentials = new NetworkCredential(user, password);
                logger.Trace("реквест заполнили данными");
                using (Stream stm = webRequest.GetRequestStream())
                {
                    using (StreamWriter wtmw = new StreamWriter(stm))
                        stm.Write(content, 0, content.Length);                    
                }

                logger.Info("Запрос на веб-сервис отправлен");

                using (Stream stream = webRequest.GetResponse().GetResponseStream())
                {

                    StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);

                    logger.Info("Ответ от веб-сервиса получен. Декодируем...");
                    var xDoc = XDocument.Parse(streamReader.ReadToEnd());

                    StatusEnvelope deserializedObject;

                    using (var xmlReader = xDoc.CreateReader(ReaderOptions.None))
                    {
                        var serializer = new XmlSerializer(typeof(StatusEnvelope));
                        deserializedObject = (StatusEnvelope)serializer.Deserialize(xmlReader);
                    }
                    logger.Info("Декодирование прошло успешно. Проверяем статус");

                    if (deserializedObject.Body.Status.ToLower().Equals("ok"))
                    {
                        logger.Trace("Статус равен: ок");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }

            return false;
        }
    }
}
