using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DTO.Exchange.ExchangeSettings;
using System.Collections.Specialized;

namespace GazRouter.Service.Exchange.Lib
{
    public class TimerSettings 
    {
        public TimerSettings()
        {
            _timers = InitTimerSettings().ToDictionary(t => t.TimerId);
        }

        private TimeSpan TransportTimerInterval => TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["transportTimerInterval"]));


        private TimeSpan ExportTimerInterval => TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["exportTimerInterval"]));


        private TimeSpan ExcelGeneratorTimerInterval => TimeSpan.FromMinutes(int.Parse((ConfigurationManager.GetSection("excelGeneratorService") as NameValueCollection)["TimerInterval"]));

        private TimeSpan EventExchangeTimerInterval
        {
            get
            {
                try
                {
                    return TimeSpan.FromMinutes(int.Parse((ConfigurationManager.GetSection("eventExchangeAgent") as NameValueCollection)["TimerInterval"]));
                }
                catch (Exception e)
                {
                    return TimeSpan.FromMinutes(-1);
                }
            }
        }
        private Dictionary<int, TimerSettingsDTO> _timers ;

        public Dictionary<int, TimerSettingsDTO> Timers => _timers;

        public Action<int, TimeSpan> IntervalChangedAction;

        private IEnumerable<TimerSettingsDTO> InitTimerSettings()
        {
            var exportTimerInterval = ExportTimerInterval;
            yield return new TimerSettingsDTO
            {
                TimerId = (int)TimerSettingId.Export,
                TimerName = "Таймер экспорта.",
                StartType = TimerStartType.Auto,
                Interval = exportTimerInterval,
                TimerStatus = (exportTimerInterval <= TimeSpan.Zero).ToTimerStatus()
            };
            var transportTimerInterval = TransportTimerInterval;
            yield return new TimerSettingsDTO
            {
                TimerId = (int)TimerSettingId.Transport,
                TimerName = "Таймер транспорта.",
                StartType = TimerStartType.Auto,
                Interval = transportTimerInterval,
                TimerStatus = (transportTimerInterval <= TimeSpan.Zero).ToTimerStatus()
            };
            var excelGeneratorTimerInterval = ExcelGeneratorTimerInterval;
            yield return new TimerSettingsDTO
            {
                TimerId = (int)TimerSettingId.ExcelGenerator,
                TimerName = "Таймер генерации xls файла.",
                StartType = TimerStartType.Auto,
                Interval = excelGeneratorTimerInterval,
                TimerStatus = (excelGeneratorTimerInterval <= TimeSpan.Zero).ToTimerStatus()
            };
            var eventExchangeTimerInterval = EventExchangeTimerInterval;
            yield return new TimerSettingsDTO
            {
                TimerId = (int)TimerSettingId.EventExchange,
                TimerName = "Таймер обмена нештатными ситуациями.",
                StartType = TimerStartType.Auto,
                Interval = eventExchangeTimerInterval,
                TimerStatus = (eventExchangeTimerInterval <= TimeSpan.Zero).ToTimerStatus()
            };
        }


        public  bool IsOff(TimerSettingId timerSettingId)
        {
            return Timers[(int) timerSettingId].TimerStatus == TimerStatus.Stopped;
        }
        public  TimeSpan Interval(TimerSettingId timerSettingId)
        {
            return Timers[(int)timerSettingId].Interval;
        }

        public  void Edit(TimerSettingsDTO dto)
        {
            var dto1 = Timers[dto.TimerId];
            dto1.StartType = dto.StartType;
            dto1.TimerStatus = dto.TimerStatus;
            dto1.TimerName = dto.TimerName;

            if (dto1.Interval != dto.Interval)
            {
                dto1.Interval = dto.Interval;
                IntervalChangedAction(dto1.TimerId, dto1.Interval);
            }
        }

    }

    public static class BoolExtensions
    {
        public static TimerStatus ToTimerStatus(this bool val)
        {
            return val ? TimerStatus.Stopped : TimerStatus.InProgress;
        }
    }

}