using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Import;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public class ExchangeImportAgent
    {
        private static ExchangeImportAgent _instance;
        private readonly string _archiveDirectory = AppSettingsManager.ExchangeArchiveDirectory;
        private readonly List<ExchangeImporter> _exchangeImporters;
        private readonly FileWatcher _filesWatcher;
        private readonly string _importDirectory = AppSettingsManager.ExchangeImportDirectory;
        private MyLogger _myLogger = new MyLogger("exchangeLogger");

        private ExchangeImportAgent()
        {
            _exchangeImporters = new List<ExchangeImporter>
            {
                new AstraExchangeImporter(),
                new SpecificExchangeImporter(),
                new TypicalExchangeImporter(),
            };
            _filesWatcher = new FileWatcher(_importDirectory, RunOnFile);
            _filesWatcher.Init();
        }

        public static ExchangeImportAgent Instance => _instance ?? (_instance = new ExchangeImportAgent());

        private void RunOnFile(string fullPath)
        {
            try
            {
                _myLogger.Info($"Импорт: Поиск обработчиков для файла {fullPath}");
                if (!File.Exists(fullPath)) return;

                using (
                    var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                        _myLogger))
                {
                    bool notProccessed = false;
                    bool crypted = false;
                    try
                    {
                        var tasks = new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet
                        {
                            ExchangeTypeId = ExchangeType.Import, 
                        }).ToList();


                        foreach (var importer in _exchangeImporters)
                        {
                            var task = tasks.FirstOrDefault(t => importer.IsValid(t, fullPath));
                            if (task != null)
                            {
                                try
                                {
                                    importer.Run(context, task, fullPath);
                                    context.Logger.Info($"Импорт: Файл для импорта успешно обработан: {fullPath}");
                                    crypted = importer is TypicalExchangeImporter;
                                }
                                catch (Exception e)
                                {
                                    var seriesId = e.Data.Contains("SeriesId") ? null : (int?)e.Data["SeriesId"];
                                    ExchangeHelper.LogError(task, seriesId, null, null, string.Empty, e.Message, context);
                                    _myLogger.WriteFullException(e, $@"Импорт: ошибка при импорте файла = {fullPath}");
                                }
                                return;
                            }
                        }

                        if (_exchangeImporters.All(i => !tasks.Any(t => i.IsValid(t, fullPath))))
                        {
                            notProccessed = true;
                        }
                    }
                    catch (Exception e)
                    {
                        context.Logger.WriteException(e, $"Ошибка при импорте файла: {fullPath}");
                    }
                    finally
                    {
                        FileTools.TransferFileToArchiveDirectory(fullPath, _archiveDirectory, notProcessed: notProccessed, crypted: crypted);
                        if (notProccessed)
                        {
                            context.Logger.Info($"Импорт: Файл для импорта не обработан: {fullPath}");
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}