using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.DataExchange.Integro;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.AsduEsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.Service.Exchange.Lib.Run
{

    public static class AsspootiExchangeAgent
    {
        private const string exchangeName = "АССПООТИ";
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
                string hour = dateNow.Hour.ToString();
                if (dateNow.Hour < 10)
                    hour = "0" + hour;

                var summaries = new GetSummariesListByParamsIdQuery(context).Execute(new GetSummaryParameterSet() { SystemId = (int)MappingSourceType.ASSPOOTI }).ToList();
                var taskList = new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet() { Ids = summaries.Where(w => w.ExchangeTaskId > 0).Select(s => s.ExchangeTaskId).ToList() }).ToList(); ;
                var summaryTasks = from s in summaries
                                   join t in taskList on s.ExchangeTaskId equals t.Id
                                   where (s.PeriodType == periodType && s.StatusId == 1 && !string.IsNullOrEmpty(t.ExcludeHours) && hour == t.ExcludeHours)
                                   select new SummaryExchTaskDTO() {Summary = s, ExchangeTask = t };
                if (!summaryTasks.Any())
                {
                    logger.Info($@"Экспорт {exchangeName}: не найдена сводка с типом {periodType} с серией {serie.Id} час {hour}");
                    return;
                }

                var exporter = new AsduEsgExchangeObjectExporter();

                //logger.Info($@"Экспорт АСДУ: передано {evList.Count()} событий");
                foreach (var exch in summaryTasks)
                {
                    logger.Info($@"Экспорт {exchangeName}: обрабатывается событие с типом {periodType} с серией {serie.Id} час {hour}");
                    var summary = exch.Summary;

                    var parameter = new ExportSummaryParams()
                    {
                        SystemId = (int)MappingSourceType.ASSPOOTI,
                        SummaryId = summary.Id,
                        PeriodType = periodType,
                        // уточнить время
                        //PeriodDate = exporter.GetCurrentSession(),
                        SeriesId = serie.Id
                    };
                    //
                    logger.Info($@"Экспорт {exchangeName}: запуск экспорта с параметрами: система = {exchangeName}; сводка = {parameter.SummaryId}; тип {parameter.PeriodType}; серия {parameter.SeriesId} час {hour}");
                    var res = exporter.ExportSummary(parameter);
                    if (res.ResultType != ExportResultType.Successful)
                    {
                        logger.Info($@"Экспорт {exchangeName}: ошибка при формировании файла: {res.Description}");
                    }
                    logger.Info($@"Экспорт {exchangeName}: завершен события с типом {parameter.PeriodType} с серией {parameter.SeriesId} час {hour}");
                }
            }

        }
    }
}
