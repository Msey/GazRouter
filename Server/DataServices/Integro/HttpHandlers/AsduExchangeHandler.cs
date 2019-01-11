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
using GazRouter.Service.Exchange.Lib.AsduEsg;
using GazRouter.DTO.DataExchange.Integro;
using Integro.Interfaces;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace GazRouter.DataServices.Integro.HttpHandlers
{
    public class AsduExchangeHandler : IHttpHandler
    {
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var logger = new MyLogger("exchangeLogger");
            try
            {
                var request = context.Request;
                var response = context.Response;
                //
                long ticks;
                if (!Int64.TryParse(request.QueryString["dt"], out ticks))
                {
                    response.Write("Неверная метка времени");
                    return;
                }
                var periodDate = new DateTime(ticks);
                Guid summaryId;
                if (!Guid.TryParse(request.QueryString["summaryId"], out summaryId))
                {
                    response.Write("Неверно указана Id сводки");
                    return;
                }
                int seriesId;
                if (!int.TryParse(request.QueryString["seriesId"], out seriesId)) { }
                int periodTypeId;
                if (!int.TryParse(request.QueryString["periodTypeId"], out periodTypeId)) { }
                int systemTypeId;
                if (!int.TryParse(request.QueryString["systemTypeId"], out systemTypeId)) { }
                //
                var parameter = new ExportSummaryParams()
                {
                    SystemId = systemTypeId,
                    SummaryId = summaryId,
                    PeriodType = (DTO.Dictionaries.PeriodTypes.PeriodType)periodTypeId,
                    // уточнить время
                    PeriodDate = periodDate,
                    SeriesId = seriesId
                };
         

                string fileName;
                ExportResult exportResult;

                var exporter = new AsduEsgExchangeObjectExporter();
                var stream = exporter.ExportSreamSummary(parameter, out exportResult, out fileName);

                SendData(context, stream, exportResult, fileName);

            }
            catch (Exception e)
            {
                logger.WriteException(e, e.Message);
                var messageBytes = Encoding.Unicode.GetBytes(e.Message);
                context.Response.BufferOutput = true;
                context.Response.OutputStream.Write(messageBytes, 0, messageBytes.Length);
            }
 
        }

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        private static byte[] ReadFromStream(Stream s)
        {
            if (s == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                s.Position = 0;
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private void SendData(HttpContext context, Stream streamToSend, ExportResult exportResult, string fileName)
        {
            var logger = new MyLogger("exchangeLogger");
            var request = context.Request;
            var response = context.Response;
            string contentType;
            //var filePath = HttpContext.Current.Server.MapPath($@"~/{fileName}");
            var ext = Path.GetExtension(fileName);
            contentType = "application/" + ext;
            streamToSend?.Seek(0, SeekOrigin.Begin);
            byte[] bytes = ReadFromStream(streamToSend);
            //logger.Info($@"АСДУ ЕСГ. Выгрузка файла на клиента : Путь к файлу {filePath}");
            response.ClearContent();
            response.ContentType = contentType;
            //response.BufferOutput = false;
            response.AddHeader("Content-Disposition", $@"attachment; filename=""{fileName}""");
            if (exportResult.ResultType == ExportResultType.Error)
            {
                logger.Info($@"АСДУ ЕСГ: Выгрузка файла {fileName} на локальную машину не удалась. Ошибка: {exportResult.Description}");
                var messageBytes = Encoding.Unicode.GetBytes(exportResult.Description);
                response.BufferOutput = false;
                response.OutputStream.Write(messageBytes, 0, messageBytes.Length);
            }
            else
            {
                logger.Info($@"АСДУ ЕСГ: Выгрузка файла {fileName} на локальную машину.");
                response.BufferOutput = false;
                response.OutputStream.Write(bytes, 0, bytes.Length);
                logger.Info($@"АСДУ ЕСГ: Выгрузка файла {fileName} на локальную машину завершена.");
            }
            response.Flush();
            response.Close();
        }

        private void SendDataCopy(HttpContext context, Stream streamToSend, ExportResult exportResult, string fileName)
        {
            var logger = new MyLogger("exchangeLogger");
            var request = context.Request;
            var response = context.Response;
            string contentType;
            //var filePath = HttpContext.Current.Server.MapPath($@"~/{fileName}");
            var ext = Path.GetExtension(fileName);
            contentType = "application/" + ext;
            streamToSend?.Seek(0, SeekOrigin.Begin);
            byte[] bytes = ReadFromStream(streamToSend);
            //logger.Info($@"АСДУ ЕСГ. Выгрузка файла на клиента : Путь к файлу {filePath}");
            response.ClearContent();
            response.ContentType = contentType;
            response.AddHeader("Content-Disposition", $@"attachment; filename=""{fileName}""");
            if (exportResult.ResultType == ExportResultType.Error)
            {
                logger.Info($@"АСДУ ЕСГ: Выгрузка файла {fileName} на локальную машину не удалась. Ошибка: {exportResult.Description}");
                var messageBytes = Encoding.Unicode.GetBytes(exportResult.Description);
                response.BufferOutput = true;
                response.OutputStream.Write(messageBytes, 0, messageBytes.Length);
            }
            else
            {
                logger.Info($@"АСДУ ЕСГ: Выгрузка файла {fileName} на локальную машину.");
                response.BufferOutput = true;
                response.OutputStream.Write(bytes, 0, bytes.Length);
                logger.Info($@"АСДУ ЕСГ: Выгрузка файла {fileName} на локальную машину завершена.");
            }
            response.Flush();
            response.Close();
        }
    }
}