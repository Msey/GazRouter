using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using GazRouter.DataServices.Dictionaries;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Asdu;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Run;

namespace GazRouter.DataServices.HttpHandlers
{
    public class AsduHandler : IHttpHandler
    {
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            long ticks;

            if (!Int64.TryParse(request.QueryString["timeStamp"], out ticks))
            {
                response.Write("Неверная метка времени");
                return;
            }
            int periodType;
            if (!int.TryParse(request.QueryString["periodType"], out periodType))
            {
                response.Write("Неверный тип периода");
                return;
            }

            try
            {
                var dt = new DateTime(ticks);

                string fullPath;
                AsduExchangeAgent.Run(dt, (PeriodType) periodType, out fullPath);
                response.ContentType = "application/xml";
                response.AddHeader("Content-Disposition", $@"attachment; filename=""{Path.GetFileName(fullPath)}""");
                response.TransmitFile(fullPath);
                response.Flush();
            }
            catch(Exception e)
            {
            }

        }

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }


        private static string GetFileName(ExchangeTaskDTO cfg, DateTime timestamp)
        {
            string defaultMask = String.Format(@"export_{0}_{1}", cfg.DataSourceId, timestamp.ToShortDateString());
            string mask = cfg.FileNameMask ?? defaultMask;
            string result = Regex.Replace(mask, @"\<(.*)\>", m => timestamp.ToString(m.Groups[1].Value));
            return result.Replace(":", "_");
        }

        private static byte[] GetTypicalExchangeRaw(DateTime date, Guid enterpiseId, bool isCryptable)
        {
            ExchangeObject<TypicalExchangeData> eo;
            var enterprise = DictionaryRepository.Dictionaries.Enterprises.Single(e => e.Id == enterpiseId);
            using (
                var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                eo = new TypicalExchangeObjectExporter(context, enterprise).Build(date);
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