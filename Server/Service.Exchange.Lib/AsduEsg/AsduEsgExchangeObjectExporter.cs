using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Import;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Utils.Extensions;
using System.Threading.Tasks;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DAL.Balances.Values;
using GazRouter.DTO.Dictionaries.Integro;

namespace GazRouter.Service.Exchange.Lib.AsduEsg
{
    public class AsduEsgExchangeObjectExporter
    {
        //private ExportSessionDataManager sessionExchangeManager;
        private AsduEsgExchangeHelper asduEsgExchangeHelper;
        public AsduEsgExchangeObjectExporter()
        {
            //sessionExchangeManager = new ExportSessionDataManager();
            asduEsgExchangeHelper = new AsduEsgExchangeHelper();
        }
        /// <summary>
        /// Получение потока для выгрузки на клиента
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="exportResult"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Stream ExportSreamSummary(ExportSummaryParams parameters, out ExportResult exportResult, out string fileName)
        {
            var logger = new MyLogger("exchangeLogger");
            var summary = asduEsgExchangeHelper.GetSummaryExchTaskDTO(parameters.SummaryId);
            MappingSourceType systemType = (MappingSourceType)parameters.SystemId;
            fileName = asduEsgExchangeHelper.GetSessionFileName(summary, parameters.PeriodDate);

            exportResult = new ExportResult();
            var exportStream = GetExportStream(parameters ,summary, logger, ref exportResult);
            //var exportStream = asduEsgExchangeHelper.ExportStream(summary, parameters, pathToXslt, pathToXsd, ref exportResult);

            if (exportResult.ResultType == ExportResultType.Error)
            {
                logger.Info($@"Экспорт АСДУ ЕСГ:  ошибка {exportResult.Description}");
                //fileName = "Error.txt";
            }
            return exportStream;
        }

        private Stream GetExportStream(ExportSummaryParams parameters, SummaryExchTaskDTO summary, MyLogger logger, ref ExportResult exportResult)
        {
            //var pathToXslt = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "XSLT_ASSPOOTI");
            var pathToXsd = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "XSD");
            if (parameters.PeriodType == DTO.Dictionaries.PeriodTypes.PeriodType.Month)
            {
                if ((parameters.ContractIds == null || !parameters.ContractIds.Any()))
                {
                    var contracts = ExchangeHelper.GetContracts(parameters.PeriodDate, summary.Summary.SessionDataCode);
                    parameters.ContractIds = contracts.Select(s => s.Id).ToList();
                    //var valueList = new GetBalanceValueListByContractsQuery(context).Execute(contract.Id);
                }
            }
            else
            {
                if (parameters.SeriesId == null || parameters.SeriesId == 0)
                {
                    var serie = ExchangeHelper.GetSerie(dt: parameters.PeriodDate, periodTypeId: parameters.PeriodType); // передавать
                    if (serie == null)
                        serie = ExchangeHelper.GetSerieL(dt: parameters.PeriodDate, periodTypeId: parameters.PeriodType); // передавать
                    if (serie != null)
                        parameters.SeriesId = serie.Id;
                    if (serie == null)
                        logger.Info($@"Экспорт АСДУ ЕСГ: серия не найдена с {parameters.PeriodType} и {parameters.PeriodDate}");
                }
            }
            //var exportParams = CollectParams(parameters.SummaryId, parameters.PeriodDate, parameters.SeriesId, pathToXslt, pathToXsd);
            var exportStream = asduEsgExchangeHelper.ExportStream(summary, parameters, pathToXsd, ref exportResult);
            return exportStream;
        }
        public ExportResult ExportSummary(ExportSummaryParams parameters, bool copyToLocal = false)
        {
            if (parameters.GetFromLog)
            {
                return GetFromLog(parameters);
            }
            string exchangeName = string.Empty;
            if (parameters.SystemId == (int)MappingSourceType.ASDU_ESG)
                exchangeName = "МАСДУ ЕСГ";
            else
                exchangeName ="АССПООТИ";

        string log = string.Empty;
            string errLog = string.Empty;
            try
            {
                var logger = new MyLogger("exchangeLogger");
                //if (sessionExchangeManager == null)
                //{
                //    return new ExportResult()
                //    {
                //        ResultType = ExportResultType.Error,
                //        Description = "Не подгружен модуль ExportSessionDataManager"
                //    };
                //}
                var summary = asduEsgExchangeHelper.GetSummaryExchTaskDTO(parameters.SummaryId);
                MappingSourceType systemType = (MappingSourceType)parameters.SystemId;
                //var filename = sessionExchangeManager.GetSessionFileName(systemType, parameters.SummaryId, parameters.PeriodDate);
                var filename = asduEsgExchangeHelper.GetSessionFileName(summary, parameters.PeriodDate);
                //log = log + "-filename => " + filename;
                string path = string.Empty;
                if (systemType == MappingSourceType.ASDU_ESG)
                    path = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "ASDU_ESG");
                else
                    path = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "ASSPOOTI");
                var pathSend = ExchangeHelper.EnsureDirectoryExist(path, "Send");
                var pathError = ExchangeHelper.EnsureDirectoryExist(path, "Error");
                var pathTemp = ExchangeHelper.EnsureDirectoryExist(path, "Temp");
                var fileFullNameSend = Path.Combine(pathSend, filename);
                var fileFullNameErr = Path.Combine(pathError, filename);
                var fileFullNameTemp = Path.Combine(pathTemp, filename);
                //
                //var pathToXslt = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "XSLT_ASSPOOTI");
                //var pathToXsd = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "XSD");
                //var exportResult = new ExportResult();
                //var exportStream = asduEsgExchangeHelper.ExportStream(summary, parameters.PeriodDate, parameters.SeriesId, pathToXslt, pathToXsd, ref exportResult);
                var exportResult = new ExportResult();
                var exportStream = GetExportStream(parameters, summary, logger, ref exportResult);
                if (exportResult.ResultType == ExportResultType.Error)
                {
                    logger.Info(exportResult.Description);
                    if (summary?.ExchangeTask != null)
                        ExchangeHelper.LogError(summary?.ExchangeTask, parameters.SeriesId, DateTime.Now, summary?.Summary?.PeriodType, "Error", exportResult.Description);
                    return exportResult;
                }
                exportStream?.Seek(0, SeekOrigin.Begin);
                if (!parameters.GetResult)
                {
                    //var fileFullName = exportResult.ResultType == ExportResultType.Successful ? fileFullNameSend : fileFullNameErr;
                    using (var fileStream = new FileStream(fileFullNameTemp, FileMode.Create, FileAccess.ReadWrite))
                    {
                        exportStream?.CopyTo(fileStream);
                        if (fileStream.Length == 0)
                        {
                            ////needDelete = true;
                            //throw new ExchangeException(ExchangeExceptionType.DataNotFound, "Данные не найдены");
                            logger.Info($@"Экспорт {exchangeName} : поток данных пустой");
                            exportResult = new ExportResult()
                            {
                                ResultType = ExportResultType.Error,
                                Description =$@"Экспорт {exchangeName} : поток данных пустой"
                            };
                            if (summary?.ExchangeTask != null)
                                ExchangeHelper.LogError(summary?.ExchangeTask, parameters.SeriesId, DateTime.Now, summary?.Summary?.PeriodType, "Error", exportResult.Description);
                        }
                        fileStream.Flush(true);
                        fileStream.Close();
                        logger.Info($@"Экспорт{exchangeName} : файл успешно выгружен в {fileFullNameTemp}");
                    }

                    if (summary?.ExchangeTask != null)
                    {
                        var archiveFile = FileTools.CopyFileToArchiveDirectory(fileFullNameTemp, AppSettingsManager.AsduExchangeArchiveDirectory);
                        logger.Info($@"Экспорт {exchangeName}: файл {filename} успешно успешно скопирован в архив: {archiveFile}");
                        if (exportResult.ResultType == ExportResultType.Successful)
                        {
                            FileTools.Copy(fileFullNameTemp, pathSend);
                            FileTools.EnsureDelete(fileFullNameTemp);
                            logger.Info($@"Экспорт {exchangeName}  : файл {filename} успешно перемещен в {pathSend}");
                            //string content = asduEsgExchangeHelper.StreamToString(exportStream);
                            ExchangeHelper.LogOk(summary?.ExchangeTask, parameters.SeriesId, DateTime.Now, summary?.Summary?.PeriodType, archiveFile);
                            exportResult.ExportFileName = Path.Combine(pathSend, filename);
                        }
                        else
                        {
                            FileTools.Move(fileFullNameTemp, pathError);
                            logger.Info($@"Экспорт {exchangeName} : файл {filename} перемещен в папку с ошибками {pathError}");
                            //string content = StreamToString(exportStream);
                            ExchangeHelper.LogError(summary?.ExchangeTask, parameters.SeriesId, DateTime.Now, summary?.Summary?.PeriodType, filename, exportResult.Description);
                            exportResult.ExportFileName = Path.Combine(pathError, filename);
                        }                        
                    }
                }
                else
                {                    
                    if (exportResult.ResultType == ExportResultType.ValidationError || exportResult.ResultType == ExportResultType.Error)
                    {
                        exportResult.LogData = exportResult.Description;
                    }
                    // Костыль надо переделать. Если данные заполнились раньше
                    if (string.IsNullOrEmpty(exportResult.ExportData))
                    {
                        exportResult.ExportData = asduEsgExchangeHelper.StreamToString(exportStream); ;
                    }
                }

                return exportResult;
            }
            catch (Exception e)
            {
                var result = new ExportResult()
                {
                    ResultType = ExportResultType.Error,
                    Description = $"{e.Message}: \n {e.InnerException}:"
                };
                result.Description = result.Description + " Уточнение ошибки: " + errLog + "Лог операций: " + log;
                return result;
            }
        }

        private ExportResult GetFromLog(ExportSummaryParams paramete)
        {
            var exportResult = new ExportResult();
            if (!paramete.ExchangeTaskId.HasValue || !paramete.SeriesId.HasValue)
            {
                if (!paramete.ExchangeTaskId.HasValue)
                    exportResult.Description = $"ID таски не может быть пустым";
                else
                    exportResult.Description = $"ID серии не может быть пустым";
                exportResult.ResultType = ExportResultType.Error;
                return exportResult;
            }

            var fileFullName = paramete.FileLogName;
            var dirToArchive = AppSettingsManager.AsduExchangeArchiveDirectory;
            if (string.IsNullOrEmpty(fileFullName))
            {
                var result = asduEsgExchangeHelper.GetExchangeLog(paramete.ExchangeTaskId.Value, paramete.SeriesId.Value);                
                fileFullName = Path.Combine(dirToArchive, result.DataContent); // в DataContent лежит путь к файлу
            }            
            
            if (File.Exists(fileFullName))
            {
                exportResult.ExportData = FileTools.ReadFile(fileFullName);
                exportResult.ResultType = ExportResultType.Successful;
            }
            else
            {
                exportResult.Description = $"В архиве не найден файл: {fileFullName}";
                exportResult.ResultType = ExportResultType.Error;
            }
            
            return exportResult;
        }

        //private List<ExchangeParameter> CollectParams(Guid summaryId, DateTime? date, int? seriesId, string pathToXslt, string pathToXsd)
        //{
        //    var resultParams = new List<ExchangeParameter>
        //    {
        //        new ExchangeParameter
        //        {
        //            ParamName = "summaryId",
        //            ParamValue = summaryId.ToString(),
        //        },
        //    };
        //    if (date != null)
        //        resultParams.Add(
        //            new ExchangeParameter
        //            {
        //                ParamName = "keyDate",
        //                ParamValue = date.Value.ToString(CultureInfo.CurrentCulture),
        //            });
        //    if (!string.IsNullOrEmpty(pathToXslt))
        //    {
        //        resultParams.Add(
        //            new ExchangeParameter
        //            {
        //                ParamName = "pathToXslt",
        //                ParamValue = pathToXslt,
        //            });
        //    }
        //    if (!string.IsNullOrEmpty(pathToXsd))
        //    {
        //        resultParams.Add(
        //            new ExchangeParameter
        //            {
        //                ParamName = "pathToXsd",
        //                ParamValue = pathToXsd,
        //            });
        //    }
        //    if (seriesId != null && seriesId > 0)
        //    {
        //        resultParams.Add(
        //            new ExchangeParameter
        //            {
        //                ParamName = "seriesId",
        //                ParamValue = seriesId.ToString(),
        //            });
        //    }
        //    //XsdDir = ExchangeParamHelper.ParamValueString(parameters, "pathToXsd");
        //    return resultParams;
        //}

        // Уточнить время
        private IEnumerable<TimeSpan> GetHours()
        {
            for (var i = 0; i < 24; i += 2)
            {
                yield return TimeSpan.FromHours(i);
            }
        }

        public  DateTime GetCurrentSession()
        {
            var now = DateTime.Now;
            var hmax = GetHours().Where(h => h <= TimeSpan.FromHours(now.Hour)).Max();
            return new DateTime(now.Year, now.Month, now.Day, hmax.Hours, 0, 0);
        }
    }
}
