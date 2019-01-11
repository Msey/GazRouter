using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Core;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public class ExchangeTaskScheduler
    {
        private readonly ExecutionContext _ctx;
        private readonly Action<ExecutionContext, SeriesDTO, ExchangeTaskDTO> _action;
        private MyLogger _logger = new MyLogger("exchangeLogger");

        public ExchangeTaskScheduler(List<ExchangeTaskDTO> tasks,
            Action<ExecutionContext, SeriesDTO, ExchangeTaskDTO> action, ExecutionContext ctx)
        {
            _action = action;
            _ctx = ctx;
            _tasks = PrepareTasks(tasks);
        }

        private static List<ExchangeTaskDTO> PrepareTasks(List<ExchangeTaskDTO> tasks)
        {
            var tasks1 =  tasks ?? new List<ExchangeTaskDTO>();
            var typical = tasks1.Where(t => t.EnterpriseId.HasValue).Where(t => t.ExchangeTypeId == ExchangeType.Export);
            tasks1 = tasks1.Except(typical).Union(typical.SelectMany(
                t => {
                    t.Lag = t.Lag == 0 ? 10 : t.Lag;
                    var t1 = t.Clone();
                    t.PeriodTypeId = PeriodType.Twohours;
                    t1.PeriodTypeId = PeriodType.Day;
                    return new List<ExchangeTaskDTO> {t, t1};
                })).ToList();
                
                
            return tasks1.Where(t => t.PeriodTypeId == PeriodType.Twohours || t.PeriodTypeId == PeriodType.Day).ToList();
        }

        private List<ExchangeTaskDTO> _tasks { get; }

        public void Tick(DateTime? time = null)
        {
            try
            {
                var exs = new List<Exception>();
                var gr = _tasks.Where(t => t.ExchangeStatus != DTO.DataExchange.ExchangeTask.ExchangeStatus.Off).GroupBy(t => t.ExchangeStatus);
                var eventTasks = gr.Where(g => g.Key == DTO.DataExchange.ExchangeTask.ExchangeStatus.Event).SelectMany(g => g).ToList();
                var now = time ?? DateTime.Now;

                IEnumerable<Action<ExecutionContext>> result1 = new List<Action<ExecutionContext>>();
                if (eventTasks.Any())
                {
                    var events = new GetSysEventListQuery(_ctx).Execute(new GetSysEventListParameters
                    {
                        EventTypeId = SysEventType.END_CALCULATION_AFTER_LOAD_DATA,
                        EventStatusId = SysEventStatus.Waiting,
                        CreateDate = DateTime.Today.AddDays(-1)
                    });
                    result1 =
                        from ev in events
                        from t in eventTasks
                        where t.PeriodTypeId == ev.PeriodTypeId 
                        let seriesDTO = ExchangeHelper.GetSerie(_ctx, ev.SeriesId)
                        select new Action<ExecutionContext>(ctx =>
                        {
                            try
                            {
                                new SetStatusSysEventCommand(ctx).Execute(new SetStatusSysEventParameters
                                {
                                    EventId = ev.Id,
                                    EventStatusId = SysEventStatus.Finished,
                                    ResultId = SysEventResult.Success
                                });
                                _logger.Error($"Экспорт по событию: Выполняется задание {t.Name} за метку времени {seriesDTO.KeyDate} с периодом {seriesDTO.PeriodTypeId}. Серия {seriesDTO.Id}");
                                _action(ctx, seriesDTO, t);
                            }
                            catch (Exception e)
                            {
                                _logger.WriteFullException(e, $"Ошибка экспорта по событию");
                            }
                        });
                }
                var result2 = gr.Where(g => g.Key == DTO.DataExchange.ExchangeTask.ExchangeStatus.Scheduled)
                    .SelectMany(g => g)
                    .GroupBy(t => new {t.PeriodTypeId, t.Lag})
                    .Where(g =>
                    {
                        var period = g.Key.PeriodTypeId.ToTimeSpan();
                        var lag = TimeSpan.FromMinutes(g.Key.Lag);
                        var delta = AppSettingsManager.ExportTimerInterval.Add(-TimeSpan.FromSeconds(1));
                        return (now - lag).Ticks % period.Ticks <= delta.Ticks;
                    })
                    .SelectMany(g =>
                    {
                        var endDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                        var periodTypeId = g.Key.PeriodTypeId;
                        var seriesDTO = ExchangeHelper.GetSerie(_ctx, dt: endDate, periodTypeId: periodTypeId);
                        if (seriesDTO != null)
                        {
                            foreach (var t in g)
                            {
                                _logger.Error($"Экспорт по расписанию: Выполняется задание {t.Name} за метку времени {seriesDTO.KeyDate} с периодом {seriesDTO.PeriodTypeId}. Серия {seriesDTO.Id}");
                            }

                            return g.Select(t => new Tuple<ExchangeTaskDTO, SeriesDTO>(t, seriesDTO));
                        }

                        _logger.Error($"Экспорт по расписанию: Не найдена серия за метку времени {endDate} с периодом {periodTypeId}");
                        return new List<Tuple<ExchangeTaskDTO, SeriesDTO>>();
                    })
                    .Select(t => new Action<ExecutionContext>(ctx =>
                    {
                        _action(ctx, t.Item2, t.Item1);
                    }));
                var actions = result1.Union(result2).ToList();
            

                new MultipleCommand(_ctx, actions) {OnException = e =>{exs.Add(e);}}.Execute();
                exs.ForEach(e => _logger.WriteFullException(e, "Экспорт: ошибка"));

            }
            catch (Exception e)
            {
                _logger.WriteFullException(e, $"Такая ошибка не должна произойти!!");
            }
        }

    }

    public static class ExchangeTaskDtoExtensions
    {
        public static ExchangeTaskDTO Clone(this ExchangeTaskDTO dto)
        {
            var result = new ExchangeTaskDTO
            {
                Id = dto.Id,
                Name = dto.Name,
                DataSourceId = dto.DataSourceId,
                PeriodTypeId = dto.PeriodTypeId,
                ExchangeTypeId = dto.ExchangeTypeId,
                FileNameMask = dto.FileNameMask,
                IsCritical = dto.IsCritical,
                IsTransform = dto.IsTransform,
                Transformation = dto.Transformation,
                IsSql = dto.IsSql,
                SqlProcedureName = dto.SqlProcedureName,
                ExchangeStatus = dto.ExchangeStatus,
                Lag = dto.Lag,
                EnterpriseId = dto.EnterpriseId,
                EnterpriseCode = dto.EnterpriseCode,
                TransportAddress = dto.TransportAddress,
                SendAsAttachment = dto.SendAsAttachment,
                TransportLogin = dto.TransportLogin,
                TransportTypeId = dto.TransportTypeId,
                TransportPassword = dto.TransportPassword,
                HostKey = dto.HostKey,
                DataSourceName = dto.DataSourceName
            };
            return result;
        }
    }
}