using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;


namespace GazRouter.Application.Helpers
{
    public static class SeriesHelper
    {
        // Возвращает расхождение во времени между сервером и клиентом
        // Используется для того, чтобы настроить календари, исходя из этого будет определятся,
        // использовать четную или нечетную метку времени для сеансов
        private static int GetTimeOffset()
        {
            return (int)TimeZoneInfo.Local.BaseUtcOffset.TotalHours - Settings.ServerTimeUtcOffset;
        }

        private static bool IsOddHour => Math.Abs(GetTimeOffset()) % 2 == 0; 
        

        public static IEnumerable<TimeSpan> GetHours()
        {
            var offset = GetTimeOffset();
            for (var i = 0; i < 24; i += 2)
            {
                yield return TimeSpan.FromHours(i + Math.Abs(offset) % 2);
            }
        }

        /// <summary>
        /// Получить текущую метку времени
        /// </summary>
        /// <returns>Текущая метка времени</returns>
        public static DateTime GetCurrentSession()
        {
            var now = DateTime.Now;
            var hmax = GetHours().Where(h => h <= TimeSpan.FromHours(now.Hour)).Max();
            return new DateTime(now.Year, now.Month, now.Day, hmax.Hours, 0, 0).ToLocal();
        }
        

        public static DateTime GetLastCompletedSession()
        {
            return GetCurrentSession();
        }

        /// <summary>
        /// Получить начало и конец текущего сеанса
        /// </summary>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime> GetCurrentSessionPeriod()
        {
            var start = GetCurrentSession().ToLocal();
            var end = start.AddHours(2).ToLocal();
            return new Tuple<DateTime, DateTime>(start, end);
        }

        /// <summary>
        /// Получить текущие диспетчерские сутки
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDispDay()
        {
            return DateTime.Now.Hour >= Settings.DispatherDayStartHour ? DateTime.Today.ToLocal() : DateTime.Today.AddDays(-1).ToLocal();
        }

        /// <summary>
        /// Получить прошедшие диспетчерские сутки
        /// </summary>
        /// <returns></returns>
        public static DateTime GetPastDispDay()
        {
            return GetCurrentDispDay().AddDays(-1).ToLocal();
        }

        public static DateTime GetCurrentDispDayStart()
        {
            var now = DateTime.Now;
            return now.Hour >= Settings.DispatherDayStartHour
                ? new DateTime(now.Year, now.Month, now.Day, Settings.DispatherDayStartHour, 0, 0).ToLocal()
                : new DateTime(now.Year, now.Month, now.Day, Settings.DispatherDayStartHour, 0, 0).AddDays(-1).ToLocal();
        }

        public static DateTime GetDispDayStart(DateTime day)
        {
            return day.AddHours(Settings.DispatherDayStartHour);
        }

        public static DateTime GetDispDayEnd(DateTime day)
        {
            return day.AddHours(Settings.DispatherDayStartHour).AddDays(1).AddSeconds(-1);
        }
    }
}