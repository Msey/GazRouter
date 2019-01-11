using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Import;
using GazRouter.Service.Exchange.Lib.Transport;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public static class ExchangeExportAgent
    {
        public static void Run()
        {
            var logger = new MyLogger("exchangeLogger");

            Action<ExecutionContext, SeriesDTO, ExchangeTaskDTO> action = (ctx, seriesDto, task) =>
            {
                var seriesId = seriesDto.Id;
                try
                {
                    RunExchangeTask(task, seriesId);
                    ExchangeHelper.LogOk(task, seriesId, null, null, "ok");
                    logger.Info($@"Экспорт по таску {task.Name} c маской файла {task.FileNameMask} за серию {seriesId} завершен");


                    new AddSysEventCommand(ctx).Execute(new AddSysEventParameters
                    {
                        EventStatusId = SysEventStatus.Finished,
                        EventStatusIdMii = SysEventStatus.Finished,
                        EventTypeId = SysEventType.END_EXPORT_TASK,
                        Description = string.Empty,
                        SeriesId = seriesId
                    });
                }
                catch (Exception e)
                {
                    ExchangeHelper.LogError(task, seriesId, null, null, "error", e.Message);
                    logger.WriteFullException(e, e.Message);
                }

            };

            using (
                var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                try
                {
                    var tasks = new GetExchangeTaskListQuery(context)
                        .Execute(new GetExchangeTaskListParameterSet())
                        .Where(t => t.ExchangeTypeId == ExchangeType.Export)
                        .Where(t => t.DataSourceId != (int)DataSources.AsduEsg )
                        .Where(t => t.DataSourceId != (int)DataSources.Asspooti)
                        .ToList();

                    new ExchangeTaskScheduler(tasks, action, context).Tick();
                }
                catch (Exception e)
                {
                    logger.WriteFullException(e, $"Странная ошибка!!");
                }
            }
        }


        private static void RunTypicalExchangeTask(ExchangeTaskDTO task, SeriesDTO seriesDTO, out string fullPath)
        {
            fullPath = null;
            IEnumerable<Guid> entIds;
            var logger = new MyLogger("exchangeLogger");
            using (
                var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                entIds = new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet
                {
                    ExchangeTypeId = ExchangeType.Export
                }).Where(t => t.EnterpriseId.HasValue).Select(t => t.EnterpriseId).Cast<Guid>();
            }

            foreach (var entId in entIds)
                try
                {
                    TypicalExchangeObjectExporter exporter;
                    ExchangeObject<TypicalExchangeData> eo;
                    using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                        new MyLogger("exchangeLogger")))
                    {
                        exporter = new TypicalExchangeObjectExporter(task, context, entId);
                        if (seriesDTO == null) continue;
                        eo = exporter.Build(seriesDTO);
                    }
                    var enterprise = exporter.Enterprise;
                    var sourceCode = ExchangeHelper.CurrentEnterpriseDTO.Code;
                    var timeStamp = eo.HeaderSection.TimeStamp.ToString("yyyy-MM-dd-HH");
                    var directory = ExchangeHelper.GetFolder(task);
                    var periodType = (int) seriesDTO.PeriodTypeId;
                    fullPath = Path.Combine(directory, $"{sourceCode}_{timeStamp}.{periodType}.{enterprise.Code}");

                    var bytes = XmlHelper.GetBytes(eo);
                    using (var fs = FileTools.OpenOrCreate(fullPath))
                    {
                        var encrypt = Cryptoghraphy.Encrypt(bytes);
                        fs.Write(encrypt, 0, encrypt.Length);
                        fs.Flush();
                    }

                    logger.Info($@"Типовой экспорт: Формирование файла {fullPath} удачно завершен");
                }
                catch (Exception e)
                {
                    logger.WriteFullException(e, $@"Типовой экспорт: Ошибка при формировании файла {fullPath} ");
                }
        }

        public static void RunAndTransportConfig(ExchangeTaskDTO task, DateTime timeStamp, PeriodType? periodTypeId)
        {
            var logger = new MyLogger("exchangeLogger");

            // По таскам АСДУ ЕСГ и АССПООТИ отдельные потоки
            if (task.DataSourceId == (int) DataSources.AsduEsg || task.DataSourceId == (int) DataSources.Asspooti)
            {
                logger.Info($"По таскам АСДУ ЕСГ и АССПООТИ отдельные потоки");
                return;
            }
            try
            {
                logger.Info(
                    $"Начало обработки и транспортировки таска по кнопке: Id {task.Id}, TransportAddress {task.TransportAddress}, Login {task.TransportLogin}, Pwd {task.TransportPassword} ");
                RunExchangeTask(task, keyDate: timeStamp, periodTypeId: periodTypeId);
                TransportAgent.RunTask(task);

                ExchangeHelper.LogOk(task, null, timeStamp, periodTypeId, "ok");
                logger.Info(
                    $"Успешное завершение обработки и транспортировки таска по кнопке: Id {task.Id}, TransportAddress {task.TransportAddress}, Login {task.TransportLogin}, Pwd {task.TransportPassword} ");
            }
            catch (Exception e)
            {
                ExchangeHelper.LogError(task, null, timeStamp, periodTypeId, "error", e.Message);
                logger.WriteFullException(e,
                    $@"Oшибка обработки и транспортировки таска по кнопке: Id {task.Id}, TransportAddress {
                            task.TransportAddress
                        }, Login {task.TransportLogin}, Pwd {task.TransportPassword} ");
            }
        }

        private static void RunExchangeTask(ExchangeTaskDTO task, int? seriesId = null,
            DateTime? keyDate = null, PeriodType? periodTypeId = null)
        {
            string fullPath;
            var serie = ExchangeHelper.GetSerie(seriesId, keyDate, periodTypeId);
            if (serie == null)
            {
                throw new Exception($"Не найдена серия за дату {keyDate} с периодом {periodTypeId}");
            }

            if (task.EnterpriseId.HasValue)
                RunTypicalExchangeTask(task, serie, out fullPath);
            else
                RunSpecificExchangeTask(task, serie, out fullPath);

        }

        private static void RunSpecificExchangeTask(ExchangeTaskDTO task, SeriesDTO seriesDTO,
            out string fullPath)
        {
            ExchangeObject<SpecificExchangeData> eo;
            var logger = new MyLogger("exchangeLogger");
            fullPath = string.Empty;
            try
            {
                using (
                    var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                        new MyLogger("exchangeLogger")))
                {
                    var exporter = new SpecificExchangeObjectExporter(context, task);
                    eo = exporter.Export(seriesDTO);
                }
                var fileName = ExchangeHelper.GetFileName(task, seriesDTO.KeyDate);
                var folder = ExchangeHelper.GetFolder(task);
                fullPath = Path.Combine(folder, fileName);

                var xsl = task.Transformation;
                if (!task.IsTransform || string.IsNullOrEmpty(xsl))
                    XmlHelper.Save(eo, fullPath);
                else
                    RunXslt(fullPath, xsl, eo);

                logger.Info($@"Нетиповой экспорт: Формирование файла {fullPath} удачно завершен");
            }
            catch (Exception e)
            {
                logger.WriteFullException(e, $@"Нетиповой экспорт: Ошибка при формировании файла {fullPath} ");
            }
        }

        private static void RunXslt(string fullPath, string xsl, ExchangeObject<SpecificExchangeData> eo)
        {
            using (var outStream = FileTools.OpenOrCreate(fullPath))
            using (var inStream = XmlHelper.GetStream(eo))
            {
                new XsltProcessor(xsl).Run(inStream, outStream);
            }
        }
    }

    internal enum DataSources
    {
        AsduEsg = 1,
        Astra = 3,
        Asspooti = 4
    }
}