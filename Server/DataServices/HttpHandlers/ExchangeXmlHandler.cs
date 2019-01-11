using System;
using System.IO;
using System.Linq;
using System.Web;
using GazRouter.DataServices.Dictionaries;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Export;

namespace GazRouter.DataServices.HttpHandlers
{
    public class ExchangeXmlHandler : IHttpHandler
    {
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;
                bool isSpecific;
                if (!Boolean.TryParse(request.QueryString["isSpecific"], out isSpecific))
                {
                    response.Write("Неверный exchange type");
                    return;
                }

                long ticks;
                if (!Int64.TryParse(request.QueryString["dt"], out ticks))
                {
                    response.Write("Неверная метка времени");
                    return;
                }

                var dt = new DateTime(ticks);

                byte[] bytes;
                string contentType;
                string fileName;
                if (isSpecific)
                {
                    int cfgId;
                    if (!Int32.TryParse(request.QueryString["id"], out cfgId))
                    {
                        response.Write("Неверный id");
                        return;
                    }
                    int periodTypeId ;
                    if (!int.TryParse(request.QueryString["periodTypeId"], out periodTypeId)) { }

                    var xmlOnly = !String.IsNullOrEmpty(request.QueryString["xmlOnly"]);
                    bytes = xmlOnly
                        ? GetSpecificExchangeXmlRaw(cfgId, dt, periodTypeId,  out fileName)
                        : GetSpecificExchangeXslRaw(cfgId, dt, periodTypeId, out fileName);
                    var ext = Path.GetExtension(fileName);
                    contentType = "application/" + ext;
                }
                else
                {
                    Guid id;
                    if (!Guid.TryParse(request.QueryString["id"], out id))
                    {
                        response.Write("Неверный id");
                        return;
                    }
                    bool isCryptable;
                    if (!Boolean.TryParse(request.QueryString["isCryptable"], out isCryptable)){}
                    int periodTypeId;
                    if (!int.TryParse(request.QueryString["periodTypeId"], out periodTypeId)){}

                    bytes = GetTypicalExchangeRaw(dt, id, (PeriodType) periodTypeId, isCryptable);
                    contentType = "application/xml";
                    fileName = Path.ChangeExtension(ticks.ToString(), "xml");
                }
                response.ContentType = contentType;
                response.AddHeader("Content-Disposition", $@"attachment; filename=""{fileName}""");
                response.BinaryWrite(bytes ?? new byte[] {});
            }
            catch (Exception e)
            {
                new MyLogger("exchangeLogger").WriteException(e, e.Message);
            }
        }

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }


        private static byte[] GetSpecificExchangeXslRaw(int cfgId, DateTime dt, int periodTypeId, out string fileName)
        {
            using (
                ExecutionContextReal context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                var cfg = new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet { Id = cfgId }).FirstOrDefault();
                var serie = ExchangeHelper.GetSerie(dt: dt, periodTypeId: (PeriodType?)periodTypeId);
                var eo = new SpecificExchangeObjectExporter(context, cfg).Export(serie);
                fileName = ExchangeHelper.GetFileName(cfg, dt);
                var inStream = XmlHelper.GetStream(eo);
                using (inStream)
                {
                    return ExchangeHelper.GetXslTransformationResultRaw(inStream, xsl:cfg.Transformation);
                }
            }
        }

        private static byte[] GetSpecificExchangeXmlRaw(int cfgId, DateTime dt, int periodTypeId, out string fileName)
        {
            using (
                ExecutionContextReal context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                var cfg = new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet { Id = cfgId }).FirstOrDefault();
                var serie = ExchangeHelper.GetSerie(dt:dt, periodTypeId: (PeriodType?) periodTypeId);
                var eo = new SpecificExchangeObjectExporter(context, cfg).Export(serie);
                fileName = ExchangeHelper.GetFileName(cfg, dt);
                return eo.ToArray();
            }
        }



        private static byte[] GetTypicalExchangeRaw(DateTime date, Guid enterpiseId, PeriodType periodTypeId, bool isCryptable)
        {
            ExchangeObject<TypicalExchangeData> eo;
            var enterprise = DictionaryRepository.Dictionaries.Enterprises.Single(e => e.Id == enterpiseId);
            using (
                var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                var serie = ExchangeHelper.GetSerie(dt: date, periodTypeId: periodTypeId);
                eo = new TypicalExchangeObjectExporter(context, enterprise).Build(serie);
            }

            var result = XmlHelper.GetBytes(eo);
            if (isCryptable)
            {
                result = Cryptoghraphy.Encrypt(result);
            }
            return result;
        }
    }
}