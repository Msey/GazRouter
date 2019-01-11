using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
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
using GazRouter.Service.Exchange.Lib.Run;

namespace GazRouter.DataServices.HttpHandlers
{
    public class AstraHandler : IHttpHandler
    {
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            long ticks;

            if (!Int64.TryParse(request.QueryString["dt"], out ticks))
            {
                response.Write("Неверная метка времени");
                return;
            }
            int periodType;
            if (!int.TryParse(request.QueryString["periodtype"], out periodType))
            {
                response.Write("Неверный тип периода");
                return;
            }

            try
            {
                var dt = new DateTime(ticks);
                var archive = HttpContext.Current.Server.MapPath("~/archive.zip");
                var temp = HttpContext.Current.Server.MapPath("~/temp");

                // clear any existing archive
                if (File.Exists(archive))
                {
                    File.Delete(archive);
                }

                ExchangeHelper.EnsureDirectoryExist(temp);
                // empty the temp folder
                Directory.EnumerateFiles(temp).ToList().ForEach(File.Delete);

                //ExchangeExportAgent.RunAstra((PeriodType)periodType, dt, temp);

                // create a new archive
                ZipFile.CreateFromDirectory(temp, archive);


                var fileName = Path.ChangeExtension(ticks.ToString(), "zip");
                response.ContentType = "application/zip";
                response.AddHeader("Content-Disposition", $@"attachment; filename=""{fileName}""");
                response.TransmitFile(archive);
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