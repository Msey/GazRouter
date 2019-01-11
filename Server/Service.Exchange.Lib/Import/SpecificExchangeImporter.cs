using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using GazRouter.DAL.Core;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Transformation;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;

namespace GazRouter.Service.Exchange.Lib.Import
{
    public class SpecificExchangeImporter : ExchangeImporter
    {
        private string _dateTimeFormat;
        private string _timeSpan;
        private MyLogger _logger = new MyLogger("exchangeLogger");

        public override bool IsValid(ExchangeTaskDTO task, string fullPath)
        {
            if (string.IsNullOrEmpty(task.FileNameMask)) return false;
            string start, end;
            if (!ExchangeHelper.TryParseFileNameMask(task.FileNameMask, out start, out end, out _dateTimeFormat, out _timeSpan))
                return task.FileNameMask == Path.GetFileName(fullPath);
            var fileName = Path.GetFileName(fullPath) ?? string.Empty;
            return task.EnterpriseId == null && fileName.Contains(start) && fileName.Contains(end);
        }


      

        public override void Run(ExecutionContext context, ExchangeTaskDTO task, string fullPath)
        {
            var seriesId = -1;
            string input = null;
            Exception error;
            DateTime? timeStamp = null;
            try
            {
                var fileName = Path.GetFileName(fullPath);

                timeStamp = GetTimeStamp(task, fileName, out error);
                if (error != null)
                {
                    _logger.Info($"Неудачное извлечение метки времени из имени файла {fileName}. Пытаемся извлечь из данных файла");
                }
                input = FileTools.ReadFile(fullPath);
                var eo = Import<ExtData>(new ImportParams { Text = input, Task = task, TimeStamp = timeStamp, FileName = fileName });

                if (eo?.Status == ExchangeStatus.NoData)
                {
                    throw new  NotSupportedException("Импорт данных со статусом нет данных. Импорт не состоялся.");
                }

                eo?.Sync(context, task, out seriesId);

                //добавление события завершения загрузки данных из файла
                new AddSysEventCommand(context).Execute(new AddSysEventParameters
                {
                    EventStatusId = SysEventStatus.Finished,
                    EventStatusIdMii = SysEventStatus.Finished,
                    EventTypeId = SysEventType.END_LOAD_ASTRA,
                    Description = $@"Нетиповой обмен. Загрузка данных завершена. Имя файла: {fileName}",
                    SeriesId = seriesId
                });
            }
            catch (Exception e)
            {
                e.Data.Add("SeriesId", seriesId == -1 ? (int?)null : seriesId);
                throw e;
            }
        }

        public ExchangeObject<T> Import<T>(ImportParams importParams) where T : new()
        {
            if(importParams == null) throw new ArgumentNullException("importParams");

            _logger.Info($"Import -> ImportParams: FileName = {importParams.FileName},  TimeStamp = {importParams.TimeStamp}, ");

            if (Path.GetExtension(importParams.FileName) == ".xml")
            {
                var buffer = new byte[20000];
                using (var ms = new MemoryStream(buffer))
                {
                    var transformation = importParams.Transformation ?? importParams.Task.Transformation;
                    new XsltProcessor(transformation).Run(importParams.Text, ms);
                    ms.Flush();

                    var eo = XmlHelper.Get<ExchangeObject<T>>(buffer);
                    if (importParams.TimeStamp.HasValue)
                    {
                        eo.HeaderSection.TimeStamp = (DateTime)importParams.TimeStamp;
                    }
                    eo.HeaderSection.GeneratedTime = DateTime.Now;
                    eo.HeaderSection.Comment = $@"Файл: {importParams.FileName}";
                    return eo;
                }
            }
            else
            {
                using (var ms = new MemoryStream())
                using (var streamReader = new StreamReader(ms))
                {

                    var transformation = importParams.Transformation ?? importParams.Task.Transformation;
                    var xsltProcessor = new XsltProcessor(xsl: transformation);
                    xsltProcessor.AddParameter("inputContent", importParams.Text);

                    if (importParams.TimeStamp.HasValue)
                    {
                        xsltProcessor.AddParameter("timestamp", $"{importParams.TimeStamp:s}");
                    }
                    if (!string.IsNullOrEmpty(importParams.FileName))
                    {
                        xsltProcessor.AddParameter("inputFileName", importParams.FileName);
                    }
                    xsltProcessor.Parse(ms);
                    ms.Flush();
                    ms.Position = 0;
                    var eo = XmlHelper.Get<ExchangeObject<T>>(streamReader);
                    if (importParams.TimeStamp.HasValue && eo.HeaderSection.TimeStamp == DateTime.MinValue)
                    {
                        eo.HeaderSection.TimeStamp = (DateTime)importParams.TimeStamp;
                    }
                    eo.HeaderSection.GeneratedTime = DateTime.Now;
                    eo.HeaderSection.Comment = $@"Файл: {importParams.FileName}";
                    return eo;
                }
            }
        }


        public DateTime? GetTimeStamp(ExchangeTaskDTO task, string fileName, out Exception e)
        {
            try
            {
                e = null;

                if (string.IsNullOrEmpty(_dateTimeFormat))
                {
                    string start, end;
                    ExchangeHelper.TryParseFileNameMask(task.FileNameMask, out start, out end, out _dateTimeFormat, out _timeSpan);
                   _logger.Info($@"SpecificExchangeImporter -> GetTimeStamp : datetime format = {_dateTimeFormat}, start = {start}, end = {end}, offset = {_timeSpan}");
                }
                if(string.IsNullOrEmpty(_dateTimeFormat)) return null;
                var dt = ExchangeHelper.GetRawDateTimeFromFileName(task.FileNameMask, fileName);
                _logger.Info($@"SpecificExchangeImporter -> GetTimeStamp -> ExchangeHelper.GetRawDateTimeFromFileName: rawDateTime = {dt} ");
                DateTime result;
                if (DateTime.TryParseExact(dt, _dateTimeFormat, null, DateTimeStyles.None, out result)) return result;
                if (DateTime.TryParseExact(dt, _dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) return result;
                if (DateTime.TryParseExact(dt, _dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out result)) return result;
                return null;
            }
            catch (Exception ex)
            {
                e = ex;
                return null;
            }
        }
    }
}
