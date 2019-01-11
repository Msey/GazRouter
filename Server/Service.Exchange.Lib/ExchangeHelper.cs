using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.DataExchange.ExchangeLog;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.Log;
using ExecutionContext = GazRouter.DAL.Core.ExecutionContext;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DTO.Balances.Contracts;
using Utils.Extensions;
using GazRouter.DTO.Dictionaries.Targets;
using System.Collections.Generic;

namespace GazRouter.Service.Exchange.Lib
{
    public enum ExchangeFolder
    {
        SpecificImport,
        SpecificExport,
        TypicalImport,
        TypicalExport,
        MailImport
    }

    public class ExchangeHelper
    {
        public static string FileNameEmailSubject = @"#{0}#";

        private static EnterpriseDTO _currentEnterpriseDTO;
        public static string DefaultEmailSubject => AppSettingsManager.EmailSubjectFlag;

        public static EnterpriseDTO CurrentEnterpriseDTO
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref _currentEnterpriseDTO, () =>

                {
                    using (
                        var context =
                            DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                                new MyLogger("exchangeLogger")))
                    {
                        return new GetEnterpriseListQuery(context).Execute(AppSettingsManager.CurrentEnterpriseId)
                            .Single(e => e.Id == AppSettingsManager.CurrentEnterpriseId);
                    }
                });
                return _currentEnterpriseDTO;
            }
        }

        public static string EnsureDirectoryExist(params string[] paths)
        {
            var directory = paths.Aggregate(Path.Combine);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            return directory;
        }

        public static SeriesDTO GetSerie(ExecutionContext context, int? seriesId = null, DateTime? dt = null,
            PeriodType? periodTypeId = null)
        {
            if (seriesId.HasValue)
                return new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
                {
                    Id = seriesId
                });
            if (!dt.HasValue) return null;
            return new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
            {
                PeriodType = periodTypeId ?? PeriodType.Twohours,
                TimeStamp = dt
            });
        }
        // Уточнить почему GetSerie не отдает
        public static SeriesDTO GetSerieL(int? seriesId = null, DateTime? dt = null, PeriodType? periodTypeId = null)
        {
            using (  var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                new MyLogger("exchangeLogger")))
            {
                if (dt != null)
                {
                    PeriodType? pTypeId = periodTypeId;
                    if (periodTypeId != null && periodTypeId == PeriodType.Month) // костыль!!!
                        pTypeId = PeriodType.Day;
                    var stDate = new DateTime(dt.Value.Year, dt.Value.Month, dt.Value.Day);
                    var endDate = new DateTime(dt.Value.Year, dt.Value.Month, dt.Value.Day, 23, 59, 59);
                    var list = new GetSeriesListQuery(context).Execute(new GetSeriesListParameterSet { PeriodType = pTypeId, PeriodStart = (DateTime)stDate, PeriodEnd = (DateTime)endDate });
                    return list.FirstOrDefault();

                }
                else
                    return null;
            }
        }

        // Уточнить почему GetSerie не отдает
        public static SeriesDTO GetSerieByPeriod2H(DateTime? dtStart = null, DateTime? dtEnd = null, PeriodType? periodTypeId = null)
        {
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                new MyLogger("exchangeLogger")))
            {
                if (dtStart != null)
                {
                    var list = new GetSeriesListQuery(context).Execute(new GetSeriesListParameterSet { PeriodType = periodTypeId, PeriodStart = (DateTime)dtStart, PeriodEnd = (DateTime)dtEnd });
                    return list.Where(w => (w.KeyDate.Hour % 2) == 0).OrderByDescending(o => o.KeyDate).FirstOrDefault();

                }
                else
                    return null;
            }
        }
        public static SeriesDTO GetSerie(int? seriesId = null, DateTime? dt = null, PeriodType? periodTypeId = null)
        {
            using (
                var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                if (seriesId.HasValue)
                    return new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
                    {
                        Id = seriesId
                    });
                if (!dt.HasValue) return null;
                return new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
                {
                    PeriodType = periodTypeId ?? PeriodType.Twohours,
                    TimeStamp = dt
                });
            }
        }

        public static List<ContractDTO> GetContracts(DateTime date, string sessionDataCode)
        {
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,new MyLogger("exchangeLogger")))
            {
                var contracts = (new GetContractListQuery(context).Execute(
                    new GetContractListParameterSet
                    {
                        ContractDate = date.Date.MonthStart(),
                        //SystemId = parameters.SystemId,
                        PeriodTypeId = PeriodType.Month,
                        TargetId = sessionDataCode == "PRO" ? Target.Fact : Target.Plan
                    }));
                return contracts;
            }
        }
        public static SeriesDTO GetOrCreateSerie(ExecutionContext context, int? seriesId = null, DateTime? dt = null,
            PeriodType? periodTypeId = null)
        {
            //получение имеющейся серии
            if (seriesId.HasValue)
                return new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
                {
                    Id = seriesId
                });

            if (!dt.HasValue) return null;

            var periodType = periodTypeId ?? PeriodType.Twohours;
            var serie = new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
            {
                PeriodType = periodType,
                TimeStamp = dt
            });
            if (serie == null)
            {
                //создание новой серии
                var keyDate = dt.Value;
                seriesId = new AddSeriesCommand(context).Execute(new AddSeriesParameterSet
                {
                    KeyDate = keyDate,
                    PeriodTypeId = periodType,
                    Description = string.Empty,
                    Day = keyDate.Day,
                    Month = keyDate.Month,
                    Year = keyDate.Year
                });
                serie = new GetSeriesQuery(context).Execute(new GetSeriesParameterSet
                {
                    Id = seriesId
                });
            }
            return serie;
        }

        public static byte[] ParseHex(string text)
        {
            var ret = new byte[text.Length / 2];
            for (var i = 0; i < ret.Length; i++)
                ret[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
            return ret;
        }

        private static void Log(ExchangeTaskDTO task, int? seriesId, DateTime? timeStamp, PeriodType? periodType,
            bool isOk, string content, string error, ExecutionContext context)
        {
            ExecutionContext context1 = null;
            try
            {
                context1 =
                    context ?? DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                        new MyLogger("exchangeLogger"));

                if (!seriesId.HasValue)
                {
                    var seriesDTO = new GetSeriesQuery(context1).Execute(new GetSeriesParameterSet
                    {
                        PeriodType = periodType ?? PeriodType.Twohours,
                        TimeStamp = timeStamp
                    });
                    if ((seriesId = seriesDTO?.Id) == null)
                        seriesId = new AddSeriesCommand(context1).Execute(new AddSeriesParameterSet
                        {
                            PeriodTypeId = periodType ?? PeriodType.Twohours,
                            KeyDate = timeStamp.GetValueOrDefault(),
                            Description = string.Empty
                        });
                }


                var parameterSet = new AddEditExchangeLogParameterSet
                {
                    ExchangeTaskId = task.Id,
                    IsOk = isOk,
                    SeriesId = seriesId,
                    Content = content,
                    Error = error
                };
                new AddExchangeLogCommand(context1).Execute(parameterSet);
            }
            catch(Exception e)
            {
                context1.Logger.WriteFullException(e, $"Ошибка при записи в таблицу логов: serieId = {seriesId}, taskId = {task?.Id} ");
            }
            finally
            {
                if (context == null) context1?.Dispose();
            }
        }

        public static void LogOk(ExchangeTaskDTO task, int? seriesId, DateTime? timeStamp, PeriodType? periodType,
            string content, ExecutionContext context = null)
        {
            Log(task, seriesId, timeStamp, periodType, true, content, null, context);
        }

        public static void LogError(ExchangeTaskDTO task, int? seriesId, DateTime? timeStamp, PeriodType? periodType,
            string content, string error, ExecutionContext context = null)
        {
            Log(task, seriesId, timeStamp, periodType, false, content, error, context);
        }

        public static dynamic GetValue(BasePropertyValueDTO propertyValue)
        {
            dynamic value;
            var doubleDTO = propertyValue as PropertyValueDoubleDTO;
            if (doubleDTO != null)
                return doubleDTO.Value;
            var stringDTO = propertyValue as PropertyValueStringDTO;
            if (stringDTO != null)
                return stringDTO.Value;
            var dateDTO = propertyValue as PropertyValueDateDTO;
            if (dateDTO != null)
                return dateDTO.Value;
            return null;
        }
        public static CellValue GetCellValue(BasePropertyValueDTO propertyValue)
        {
            dynamic value;
            var doubleDTO = propertyValue as PropertyValueDoubleDTO;
            if (doubleDTO != null)
                return new CellValue {Number = doubleDTO.Value, ValueType = CellValueType.Number};
            var stringDTO = propertyValue as PropertyValueStringDTO;
            if (stringDTO != null)
                return new CellValue { RawValue = stringDTO.Value, ValueType = CellValueType.Text};
            var dateDTO = propertyValue as PropertyValueDateDTO;
            if (dateDTO != null)
                return new CellValue { DateTime = dateDTO.Value, ValueType = CellValueType.DateTime};
            return null;
        }

        public static string GetFileName(ExchangeTaskDTO task, DateTime timestamp)
        {
            return GetFileName(task.FileNameMask, timestamp, task.DataSourceId);
        }

        public static string GetFileName(string fileNameMask, DateTime timestamp, int? dataSourceId = null)
        {
            var defaultMask = dataSourceId == null
                ? $@"export_{timestamp.ToShortDateString()}"
                : $@"export_{dataSourceId}_{timestamp.ToShortDateString()}";
            var mask = fileNameMask ?? defaultMask;
            string dateFormat;
            timestamp = AdjustTimeStamp(fileNameMask, timestamp, out dateFormat);
            var result = Regex.Replace(mask, @"\<(.*)\>",
                m => dateFormat == null
                    ? timestamp.ToString(CultureInfo.InvariantCulture)
                    : timestamp.ToString(dateFormat));
            return result.Replace(":", "_");
        }


        public static bool TryParseFileNameMask(string pattern, out string start, out string end, out string dateFormat,
            out string offset)
        {
            end = start = dateFormat = offset = null;
            pattern = pattern ?? "";
            var m = Regex.Match(pattern,
                @"(?<start>[^\<]*)\<(?<dateFormat>[^\#]*)(?<sharp>\#?)(?<offset>[^\>]*)\>(?<end>.*)");
            if (!m.Success) return false;
            end = m.Groups["end"]?.Value;
            start = m.Groups["start"]?.Value;
            dateFormat = m.Groups["dateFormat"]?.Value;
            offset = m.Groups["sharp"].Success ? m.Groups["offset"]?.Value : string.Empty;

            return true;
        }

        public static string GetRawDateTimeFromFileName(string pattern, string fileName)
        {
            pattern = (pattern ?? "").ToLower();
            var result = (fileName ?? "").ToLower();
            string start, end, dateFormat, offset;
            TryParseFileNameMask(pattern, out start, out end, out dateFormat, out offset);
            if (!string.IsNullOrEmpty(start)) result = result.Replace(start, string.Empty);
            if (!string.IsNullOrEmpty(end)) result = result.Replace(end, string.Empty);
            return result;
        }

        public static DateTime AdjustTimeStamp(string fileNameMask, DateTime timeStamp, out string dateFormat)
        {
            var result = timeStamp;
            dateFormat = null;
            if (string.IsNullOrEmpty(fileNameMask)) return result;
            string start, end;
            string offset;
            TryParseFileNameMask(fileNameMask, out start, out end, out dateFormat, out offset);

            TimeSpan ts;
            if (!TimeSpan.TryParse(offset, CultureInfo.InvariantCulture, out ts)) return result;
            return result.Add(ts);
        }


        public static string GetFolder(ExchangeTaskDTO task)
        {
            return GetFolder(task.ExchangeTypeId, task.TransportTypeId, task.EnterpriseId, task.Id);
        }

        public static string GetFolder(ExchangeType exchangeType, TransportType? transportType,
            Guid? enterpriseId = null, int? taskId = null)
        {
            string folder = null;
            if (enterpriseId.HasValue)
            {
                folder = exchangeType == ExchangeType.Export
                    ? EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Export", "Typical")
                    : EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Import", "Typical");
            }
            else
            {
                if (exchangeType == ExchangeType.Export)
                {
                    if (transportType == TransportType.Ftp)
                        folder = EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Export", "Specific",
                            "Sftp");
                    if (transportType == TransportType.Email)
                        folder = EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Export", "Specific",
                            "Email");
                    if (transportType == TransportType.Folder)
                        folder = EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Export", "Specific",
                            "Smb");
                    if (taskId.HasValue)
                        folder = EnsureDirectoryExist(folder, taskId.ToString());
                }
                else
                {
                    folder = EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Import");
                }
            }
            return folder;
        }


        public static byte[] GetXslTransformationResultRaw(Stream inStream, string xsl)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    inStream.Position = 0;
                    new XsltProcessor(xsl).Run(inStream, ms);
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException) throw;
                new MyLogger("exchangeLogger").WriteFullException(e, e.Message);

                return null;
            }
        }
    }
}