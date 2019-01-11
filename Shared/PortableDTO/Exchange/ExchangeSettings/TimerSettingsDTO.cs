using System;

namespace GazRouter.DTO.Exchange.ExchangeSettings
{
    public class TimerSettingsDTO
    {
        public int TimerId { get; set; }
        public string TimerName { get; set; }
        public TimeSpan Interval { get; set; }  
        public TimerStatus TimerStatus { get; set; }
        public TimerStartType StartType { get; set; }
        
    }

    public enum TimerStatus
    {
        InProgress = 1, //таймер запущен
        Stopped = 0  //таймер остановлен
    }

    public enum TimerStartType
    {
        Auto = 1, //таймер запускается при старте приложения
        Manual = 0  //таймер запускаетя вручную из интерфейса
    }

    public enum TimerSettingId
    {
        Export,
        Transport,
        ExcelGenerator,
        EventExchange,
        Asdu,
        Asspooti        
    }

}
