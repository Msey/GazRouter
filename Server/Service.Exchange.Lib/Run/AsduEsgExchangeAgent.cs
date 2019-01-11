using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Asdu;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Import;
using GazRouter.Service.Exchange.Lib.Transport;
using GazRouter.DAL.DataExchange.Integro;
using GazRouter.DTO.DataExchange.Integro;
//using Integro.Interfaces;
//using Integro.IUSExchange.Export;
using GazRouter.Service.Exchange.Lib.AsduEsg;
using GazRouter.DTO.Dictionaries.Integro;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public static class AsduEsgExchangeAgent
    {
        private const string exchangeName = "МАСДУ ЕСГ";
        public static void Run()
        {
            MyLogger logger = new MyLogger("exchangeLogger");

            var dateNow = DateTime.Now;
            var dateSerie = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, 0, 0);
            var periodType = DTO.Dictionaries.PeriodTypes.PeriodType.Twohours;
            var serie = ExchangeHelper.GetSerie(dt: dateSerie, periodTypeId: periodType); // передавать            
            if (serie == null)
            {
                logger.Info($@"Экспорт {exchangeName}:  Не найдена серия за  {dateSerie} с типом {periodType}");
                return;
            }
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
            {

                var summaries = new GetSummariesListByParamsIdQuery(context).Execute(new GetSummaryParameterSet() { SystemId = (int)MappingSourceType.ASDU_ESG }).ToList();
                var exporter = new AsduEsgExchangeObjectExporter();
                //logger.Info($@"Экспорт АСДУ: передано {evList.Count()} событий");
                //foreach (var ev in evList)
                {
                    logger.Info($@"Экспорт {exchangeName}: обрабатывается событие с типом {periodType} с серией {serie.Id}");
                    var summary = summaries.FirstOrDefault(f => f.PeriodType == periodType && f.StatusId == 1);
                    if (summary == null)
                    {
                        logger.Info($@"Экспорт {exchangeName}: не найдена сводка с типом {periodType}");
                        return;
                    }
                    var parameter = new ExportSummaryParams()
                    {
                        SystemId = (int)MappingSourceType.ASDU_ESG,
                        SummaryId = summary.Id,
                        PeriodType = periodType,
                        // уточнить время
                        //PeriodDate = exporter.GetCurrentSession(),
                        SeriesId = serie.Id
                    };
                    //
                    logger.Info($@"Экспорт {exchangeName}: запуск экспорта с параметрами: система = {exchangeName}; сводка = {parameter.SummaryId}; тип {parameter.PeriodType}; серия {parameter.SeriesId}");
                    var res = exporter.ExportSummary(parameter);
                    if (res.ResultType != ExportResultType.Successful)
                    {
                        logger.Info($@"Экспорт {exchangeName}: ошибка при формировании файла: {res.Description}");
                    }
                    logger.Info($@"Экспорт {exchangeName}: завершен события с типом {parameter.PeriodType} с серией {parameter.SeriesId}");
                }
            }

        }
    }
}
