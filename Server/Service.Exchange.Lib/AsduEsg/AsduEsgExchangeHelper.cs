using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Extensions;
using System.Threading.Tasks;
using GazRouter.DTO.DataExchange.ExchangeTask;
using System.Text.RegularExpressions;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.Log;
using GazRouter.DAL.DataExchange.Integro;
using GazRouter.DAL.DataExchange.ExchangeTask;
using System.IO;
using GazRouter.DAL.DataExchange.Integro.SessionData;
using System.Diagnostics;
using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.Service.Exchange.Lib.AsduEsg.ExchangeFormat;
using GazRouter.DTO.DataExchange.Integro.SessionData;
using GazRouter.DTO.SeriesData.PropertyValues;
using System.Globalization;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DAL.DataExchange.ExchangeLog;
using GazRouter.DAL.Core;
using GazRouter.DAL.Balances.Values;
using GazRouter.DAL.SeriesData;
using GazRouter.DTO.Balances.Values;

namespace GazRouter.Service.Exchange.Lib.AsduEsg
{
    public class AsduEsgExchangeHelper
    {
        private Dictionary<int, Dictionary<double, double>> coding = new Dictionary<int, Dictionary<double, double>>();
        public AsduEsgExchangeHelper()
        {
            coding.Add(43, new Dictionary<double, double> { { 42, 999999 }, { 1, 1 }, { 2, 2 }, { 3, 3 } });
            coding.Add(45, new Dictionary<double, double> { { 42, 999999 }, { 2, 1 }, { 1, 2 }, { 0, 3 }, { 3, 4 } });
            coding.Add(46, new Dictionary<double, double> { { 42, 999999 }, { 2, 1 }, { 1, 2 }, { 0, 3 }, { 3, 4 } });
            coding.Add(47, new Dictionary<double, double> { { 42, 999999 }, { 2, 1 }, { 1, 2 }, { 0, 3 }, { 3, 4 } });
            coding.Add(48, new Dictionary<double, double> { { 42, 999999 }, { 2, 1 }, { 1, 2 }, { 0, 3 }, { 3, 4 } });            
        }

        public void Export()
        {

        }

        private Stream GetStreamFromString(string s)
        {
            var result = new MemoryStream();
            var writeSt = new StreamWriter(result);
            writeSt.Write(s);
            writeSt.Flush();
            result.Position = 0;
            return result;
        }
        public Stream ExportStream(SummaryExchTaskDTO summaryExchTask, ExportSummaryParams param, string pathToXsd, ref ExportResult exportResult)
        {
            exportResult.ResultType = ExportResultType.Successful;
            exportResult.Description = string.Empty;
            var logger = new MyLogger("exchangeLogger");
            //var transformFileName = summaryExchTask.Summary.TransformFileName;
            //if (!string.IsNullOrEmpty(transformFileName))
            //{
            //    if (string.IsNullOrEmpty(pathToXslt))
            //    {
            //        exportResult.ResultType = ExportResultType.Error;
            //        exportResult.Description = "Путь к файлу xslt не указан";
            //        logger.Info($@"Экспорт АСДУ ЕСГ: Путь к файлу xslt не указан");
            //        return null;
            //    }
            //    transformFileName = Path.Combine(pathToXslt, transformFileName);
            //}
            var periodType = (PeriodType)summaryExchTask.Summary.PeriodType;

            var currentEnterprise = ExchangeHelper.CurrentEnterpriseDTO;
            Stream resultStream = null;
            var parametersSet = new SessionDataParameterSet
            {
                PeriodType = periodType,
                EndDate = param.PeriodDate,
                StartDate = periodType == PeriodType.Month ? param.PeriodDate.AddMonths(-1) : param.PeriodDate,
                //SystemId = summaryExchTask.ExchangeTask.DataSourceId,
                SummaryId = summaryExchTask.Summary.Id,
                SeriesId = param.SeriesId,
                ContractIds = param.ContractIds,
                SDType = summaryExchTask.Summary.SessionDataType
            };
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
            {
                //var data = new SessionDataQuery(context).Execute(parametersSet);
                var data = GetData(context, parametersSet);
                if (!data.Any())
                {
                    string contractIds = parametersSet.ContractIds != null ? string.Join(",", parametersSet.ContractIds) : "NULL";
                    string summaryId = parametersSet.SummaryId.HasValue ? parametersSet.SummaryId.Value.ToOracle() : "NULL";
                    exportResult.ResultType = ExportResultType.Error;
                    exportResult.Description = "Данные не найдены";
                    exportResult.LogData = 
                        $@"Не найдены данные по 
                       SummaryId=={summaryId};
                       SeriesId=={parametersSet.SeriesId}; 
                       ContractIds=={contractIds} 
                       PeriodType=={parametersSet.PeriodType.ToString()}";
                    logger.Info($@"Экспорт АСДУ ЕСГ: данные не найдены с {parametersSet.ToString()} ");
                    return null;
                }
                var keyDate = data.First().PropertyValue.Date;
                Debug.Assert(keyDate != null, "keyDate != null");
                var xmlData = new XmlExchangeObject()
                {
                    //HeaderSection = new ExchangeHeader(PeriodType, keyDate, PeriodType.Template(CurrentEnterprise.Code), $@"{CurrentEnterprise.Name}",                
                    HeaderSection = new XmlExchangeHeader(
                        periodType, 
                        keyDate, 
                        periodType.Template(currentEnterprise.Code, parametersSet.SDType), 
                        $@"{currentEnterprise.AsduCode}",
                        $@"{currentEnterprise.DisplayShortPath}"),
                    DataSections = GetDataSections(data).ToList()
                };
                //Получаем XML
                var xmlStream = new MemoryStream();
                XmlHelper.SaveToStream(xmlData, xmlStream);
                xmlStream.Position = 0;
                resultStream = xmlStream;

                // TODO:разобраться с MemoryStream !!!
                if (summaryExchTask.ExchangeTask.IsTransform)
                //if (!string.IsNullOrEmpty(transformFileName))
                {
                    // перед трансформацией заносим данные
                    if (param.GetResult)
                    {
                        exportResult.ExportData = StreamToString(xmlStream);
                        xmlStream.Position = 0;
                    }
                    if (!string.IsNullOrEmpty(summaryExchTask.ExchangeTask.Transformation))
                    {
                        using (var xsltStream = GetStreamFromString(summaryExchTask.ExchangeTask.Transformation))
                        {
                            resultStream = XmlHelper.Transform(xsltStream, xmlStream);
                            xsltStream.Close();
                            resultStream.Position = 0;
                        }

                    }
                    //Трансформация
                    //using (var xsltStream = new FileStream(transformFileName, FileMode.Open))
                    //{                       
                    //    resultStream = XmlHelper.Transform(xsltStream, xmlStream);
                    //    xsltStream.Close();
                    //    resultStream.Position = 0;
                    //}                    
                } else
                // TODO:разобраться с MemoryStream !!!
                //Валидация
                if (AppSettingsManager.SessionValidateAfterExport)// && !string.IsNullOrEmpty(summary.ValidateFileName))
                {
                    var validator = new XmlValidationHelper();
                    //var xsdDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Xsd");

                    var pt = periodType;
                    var schemas = GetSchemas(summaryExchTask.Summary.SessionDataType, pt);
                    foreach (var schemaName in schemas)
                    {
                        validator.AddSchema("", Path.Combine(pathToXsd, schemaName));
                    }

                    if (!validator.ValidateStream(resultStream))
                    {
                        string inf = $"Сформированный XML-результат не прошел валидацию: {string.Join(";", validator.Errors)}";
                        exportResult.ResultType = ExportResultType.ValidationError;
                        exportResult.Description = inf;
                        logger.Info($@"Экспорт АСДУ ЕСГ: {inf} ");
                    }
                }
            }
            return resultStream;
        }

        public SummaryExchTaskDTO GetSummaryExchTaskDTO(Guid summaryId)
        {
            var result = new SummaryExchTaskDTO();
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
            {
                result.Summary = new GetSummaryByIdCommand(context).Execute(summaryId);
                if (result.Summary != null && result.Summary.ExchangeTaskId > 0)
                {
                    var exchangeTask =  new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet() { Id = result.Summary.ExchangeTaskId });
                    result.ExchangeTask = exchangeTask.FirstOrDefault();
                }
            }
            return result;
        }
        public ExchangeLogDTO GetExchangeLog(int taskId, int serieId)
        {
            ExchangeLogDTO result;
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
            {
                var exchangeLog = new GetExchangeLogQuery(context).Execute(
                    new GetExchangeLogParameterSet { ExchangeTaskId = taskId, SerieId = serieId, GetDataContent = true });
                result = exchangeLog.FirstOrDefault();
            }
            return result;
        }
        public string GetSessionFileName(SummaryExchTaskDTO summary, DateTime date)
        {
            var currentEnterprise = ExchangeHelper.CurrentEnterpriseDTO;
            //var localKeyDate = AddTimeByPeriodTypeDetailWithDelta(date, summary.PeriodTypeId, summary.PeriodTypeDetail);
            var localKeyDate = date.ToLocal();
            var periodDate = summary.Summary.PeriodType != PeriodType.Twohours
                ? new DateTime(date.Year, date.Month, date.Day)
                : localKeyDate;
            if (summary.ExchangeTask != null && summary.ExchangeTask.DataSourceId == (int)MappingSourceType.ASDU_ESG)
            {
                var enterpriseCode = string.IsNullOrEmpty(summary.ExchangeTask.EnterpriseCode) ? currentEnterprise.Code : summary.ExchangeTask.EnterpriseCode;
                var pt = PeriodTypeManager.GetFullInfo(summary.Summary.PeriodType, summary.Summary.SessionDataType);
                return summary.Summary.PeriodType == PeriodType.Twohours
                    ? GetHourFileName(enterpriseCode, pt.Scale, pt.DataType, pt.Version, periodDate)
                    : GetFileName(enterpriseCode, pt.Scale, pt.DataType, pt.Version, periodDate);
            }
            else
            if (summary.ExchangeTask != null && summary.ExchangeTask.DataSourceId == (int)MappingSourceType.ASSPOOTI)
            {
                var enterpriseCode = string.IsNullOrEmpty(summary.ExchangeTask.EnterpriseCode) ? currentEnterprise.Code : summary.ExchangeTask.EnterpriseCode;
                var result = string.Empty;
                if (!string.IsNullOrEmpty(summary.ExchangeTask.FileNameMask))
                {
                    result = ExchangeHelper.GetFileName(summary.ExchangeTask, date);
                }
                else
                if (!string.IsNullOrEmpty(summary.Summary.Descriptor))
                {
                    var index = summary.Summary.Descriptor.IndexOf(".");
                    if (index > 0)
                        result = summary.Summary.Descriptor.Substring(0, index);
                    else
                        result = summary.Summary.Descriptor;
                    result = $"{result}_{date.ToString("dd.MM.yyyy")}.txt";
                }
                //
                //return $"{descName}_{date.ToString("ddMMyyyy")}.txt";
                return result;
            }
            else if(summary.ExchangeTask == null)
            {

                var pt = PeriodTypeManager.GetFullInfo(summary.Summary.PeriodType, summary.Summary.SessionDataType);
                return summary.Summary.PeriodType == PeriodType.Twohours
                    ? GetHourFileName(currentEnterprise.Code, pt.Scale, pt.DataType, pt.Version, periodDate)
                    : GetFileName(currentEnterprise.Code, pt.Scale, pt.DataType, pt.Version, periodDate);
            }
            return "UnDefined";
        }

        public string StreamToString(Stream stream)
        {
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
        private List<IntegroExchangePropertyDto> GetData(ExecutionContext context, SessionDataParameterSet param)
        {
            List<IntegroExchangePropertyDto> result = null;
            var logger = new MyLogger("exchangeLogger");
            if (param.PeriodType == PeriodType.Month)
            {
                if (param.ContractIds == null || !param.ContractIds.Any())
                    logger.Info($@"Экспорт АСДУ ЕСГ: получение месячных данных по контрактам, контракты не найдены");
                else
                    logger.Info($@"Экспорт АСДУ ЕСГ: получение месячных данных по контрактам  {string.Join(",", param.ContractIds)}");
                result = new List<IntegroExchangePropertyDto>();
                var valueList = new GetBalanceSessionValueListQuery(context).Execute(param);
                //var valueListGroup = (from v in valueList
                //        group v by new { v.EntityId, v.EntityName, v.EntityType, v.PropertyTypeId, v.ParameterGidString } into gc
                //        select new BalanceExchangeValueDTO()
                //        {
                //            Id = gc.Min(m => m.Id),
                //            ContractId = gc.Min(m => m.ContractId),
                //            GasOwnerId = gc.Min(m => m.GasOwnerId),
                //            EntityId = gc.Key.EntityId,
                //            EntityType = gc.Key.EntityType,
                //            EntityName = gc.Key.EntityName,
                //            PropertyTypeId = gc.Key.PropertyTypeId,
                //            PropertyUnitName = gc.Min(m => m.PropertyUnitName),
                //            PropertyDescription = gc.Min(m => m.PropertyDescription),
                //            PropertyValueType = gc.Min(m => m.PropertyValueType),
                //            DistrStationId = gc.Min(m => m.DistrStationId),
                //            BalanceItem = gc.Min(m => m.BalanceItem),
                //            BaseValue = gc.Sum(m => m.Correction ?? m.BaseValue),
                //            Correction = gc.Sum(m => m.Correction ?? 0),
                //            ParameterGidString = gc.Key.ParameterGidString,
                //            ContractDate = gc.Min(m => m.ContractDate),
                //        }).ToList();
                var valueListGroup = (from v in valueList
                                      group v by new { v.ParameterGidString } into gc
                                      select new BalanceExchangeValueDTO()
                                      {
                                          Id = gc.Min(m => m.Id),
                                          ContractId = gc.Min(m => m.ContractId),
                                          GasOwnerId = gc.Min(m => m.GasOwnerId),
                                          EntityId = gc.Min(m => m.EntityId),
                                          EntityType = gc.Min(m => m.EntityType),
                                          EntityName = gc.Min(m => m.EntityName),
                                          PropertyTypeId = gc.Min(m => m.PropertyTypeId),
                                          PropertyUnitName = gc.Min(m => m.PropertyUnitName),
                                          PropertyDescription = gc.Min(m => m.PropertyDescription),
                                          PropertyValueType = gc.Min(m => m.PropertyValueType),
                                          DistrStationId = gc.Min(m => m.DistrStationId),
                                          BalanceItem = gc.Min(m => m.BalanceItem),
                                          BaseValue = gc.Sum(m => m.Correction ?? m.BaseValue),
                                          Correction = gc.Sum(m => m.Correction ?? 0),
                                          ParameterGidString = gc.Key.ParameterGidString,
                                          ContractDate = gc.Min(m => m.ContractDate),
                                      }).ToList();
                foreach (var item in valueListGroup)
                {
                    var val = GetValue(item);
                    if (val == null) continue;
                    var dataItem = new IntegroExchangePropertyDto();
                    dataItem.EntityId = item.EntityId;
                    dataItem.EntityName = item.EntityName;
                    dataItem.PropertyType = new ExchangeSummaryProperty()
                    {
                        Id = item.PropertyTypeId,
                        UnitName = item.PropertyUnitName,
                        Description = item.PropertyDescription,
                    };
                    dataItem.PropertyValue = val;
                    dataItem.ContractId = item.ContractId;
                    dataItem.ParameterGidString = item.ParameterGidString;
                    result.Add(dataItem);
                }
            }
            else
            {
                result = new SessionDataQuery(context).Execute(param);
                result = ConvertDataToMASDUData(result);
            }
            return result;
        }

        private List<IntegroExchangePropertyDto> ConvertDataToMASDUData(List<IntegroExchangePropertyDto> list)
        {
            //var result = new List<IntegroExchangePropertyDto>();
            foreach(var item in list)
            {
                if (coding.ContainsKey(item.PropertyType.Id))
                {
                    var value = item.PropertyValue as PropertyValueDoubleDTO;
                    var propId = item.PropertyType.Id;
                    if (value != null && coding[propId].ContainsKey(value.Value))
                    {
                        value.Value = coding[propId][value.Value];
                        //value.Value == coding[item.PropertyType.Id][(double)value.Value];
                    }
                }
                
            }
            return list;
        }

        private BasePropertyValueDTO GetValue(BalanceExchangeValueDTO item)
        {
            BasePropertyValueDTO emptyDto =
                 new PropertyValueDoubleDTO
                 {
                     Value = item.BaseValue,
                     Date = item.ContractDate,
                     Year = item.ContractDate.Year,
                     Month = item.ContractDate.Month,
                     Day = item.ContractDate.Day,
                     PeriodTypeId = PeriodType.Month,
                 };
            return emptyDto;
        }
        private IEnumerable<XmlDataSection> GetDataSections(List<IntegroExchangePropertyDto> data)
        {
            foreach (var item in data)
            {
                var value = GetValue(item.PropertyValue);
                if (string.IsNullOrEmpty(value)) continue;
                if (item.AnalyticGid == null && string.IsNullOrEmpty(item.ParameterGidString)) continue;

                yield return new XmlDataSection
                {
                    Value = new XmlExchangeData
                    {
                        Value = value,
                    },
                    Identifier = new XmlExchangeIdentifier { Id = item.AnalyticGid != null ? item.AnalyticGid.Value.ToOracle() : item.ParameterGidString },
                    Dimension = AppSettingsManager.SessionExportDimension ? AppSettingsManager.UnitDictionary.Recode(item.PropertyType.UnitName ?? item.PropertyType.Id.ToString(), false) : null,
                    ParameterFullName = AppSettingsManager.ParameterFullName ?
                     string.Format("{0};{1}; {2}/{3}", item.EntityId.ToOracle(), item.PropertyType.Id, item.EntityName, item.PropertyType.Description) : null,
                };
            }
        }
        private string GetValue(BasePropertyValueDTO propertyValue)
        {
            var doubleDto = propertyValue as PropertyValueDoubleDTO;
            if (doubleDto != null)
            {
                return doubleDto.Value.ToString(CultureInfo.InvariantCulture);
            }
            var stringDro = propertyValue as PropertyValueStringDTO;
            if (stringDro != null)
            {
                return stringDro.Value;
            }
            var dateDto = propertyValue as PropertyValueDateDTO;
            if (dateDto != null)
            {
                return dateDto.Value.ToString();
            }
            return string.Empty; //Уточнить
        }

        private string GetFileName(ExchangeTaskDTO cfg, DateTime timestamp)
        {
            string defaultMask = String.Format(@"export_{0}_{1}", cfg.DataSourceId, timestamp.ToShortDateString());
            string mask = cfg.FileNameMask ?? defaultMask;
            string result = Regex.Replace(mask, @"\<(.*)\>", m => timestamp.ToString(m.Groups[1].Value));
            return result.Replace(":", "_");
        }

        private static string GetHourFileName(string code, string sessionTypeStr, string dataTypeStr, string version, DateTime keyDate, string template = null)
        {
            //согласно примерам - первое время в шаблоне - локальное, второе - московское
            // устарело - 16.10.2017
            //var localTime = keyDate;
            var localTime = DateTime.Now;
            var mscTime = localTime.ToMoscow();

            var result = string.IsNullOrEmpty(template) ?
                //$"{code}.{sessionTypeStr}.{dataTypeStr}.V{version}_{localTime:yyyy_MM_dd_HH_mm}_{mscTime:yyyyMMdd_HHmmss}.xml" :
                $"{code}.{sessionTypeStr}.{dataTypeStr}.V{version}_{mscTime:yyyy_MM_dd_HH_mm_ss}.xml" :
                string.Format(template, code, sessionTypeStr, dataTypeStr, version, localTime, mscTime);
            return result;
        }

        private static string GetFileName(string code, string sessionTypeStr, string dataTypeStr, string version, DateTime keyDate, string template = null)
        {
            var localTime = DateTime.Now;
            var mscTime = localTime.ToMoscow();
            //var result = $"SD_{sessionTypeStr}_{keyDate:yyyy_MM_dd_HH_mm}_{keyDate:yyyyMMdd_HHmmss}.xml";
            var result = string.IsNullOrEmpty(template) ?
                //$"{code}.{sessionTypeStr}.{dataTypeStr}.V{version}_{keyDate:yyyy_MM_dd_HH_mm}_{keyDate:yyyyMMdd_HHmmss}.xml" :
                $"{code}.{sessionTypeStr}.{dataTypeStr}.V{version}_{mscTime:yyyy_MM_dd_HH_mm_ss}.xml" :
                string.Format(template, code, sessionTypeStr, dataTypeStr, version, keyDate, keyDate);

            return result;
        }

        private string[] GetSchemas(SessionDataType sessionDataType, PeriodType pt)
        {
            var keyName = $"{sessionDataType}:{PeriodExportAsduHelper.ScaleName(pt)}";
            // TODO: название брать из сводки
            return (AppSettingsManager.ValidationSchemeDictionary.Recode(keyName) ?? "").Split(',');
        }

        //private DateTime AddTimeByPeriodTypeDetailWithDelta(DateTime keyDate, PeriodType ptype)
        //{
        //    // 
        //    //var moscowTime = keyDate.ToMoscow();
        //    //var mhour = moscowTime.Hour == 0 ? 24 : moscowTime.Hour;
        //    //var localHour = keyDate.Hour == 0 ? 24 : keyDate.Hour;
        //    //var deltaHours = localHour - mhour;
        //    //return ptype != PeriodType.Twohours ? keyDate : keyDate.AddHours((int)period + deltaHours);
        //}
    }
}
