using GazRouter.DataServices.Infrastructure;
using GazRouter.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.Service.Exchange.Lib.AsduEsg
{
    public static class DateTimeMoscowExtension
    {

        //https://msdn.microsoft.com/en-us/library/gg154758.aspxprivate 
        //1540
        //Russian Standard Time
        //(UTC+04:00) Moscow, St.Petersburg, Volgograd
        private const string MoscowZoneId = "Russian Standard Time";


        public static DateTime ToMoscow(this DateTime dateTime)
        {
            var logger = new MyLogger("exchangeLogger");

            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById(MoscowZoneId);
            DateTime moscowDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);
            var offset = TimeZoneInfo.FindSystemTimeZoneById(MoscowZoneId).BaseUtcOffset.Hours;
            logger.Info($@"Экспорт АСДУ ЕСГ: (конвертация даты) смещение для москвы - {offset} ");
            logger.Info($@"Экспорт АСДУ ЕСГ: (конвертация даты) время локальное - {DateTime.Now.ToString()} ");
            logger.Info($@"Экспорт АСДУ ЕСГ: (конвертация даты) время локальное UtcNow - {DateTime.UtcNow.ToString()} ");
            logger.Info($@"Экспорт АСДУ ЕСГ: (конвертация даты) moscowTimeZone - {moscowTimeZone.ToString()} ");
            logger.Info($@"Экспорт АСДУ ЕСГ: (конвертация даты) TimeZoneInfo.ConvertTimeFromUtc - {moscowDateTime.ToString()} ");
            var convDate = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(MoscowZoneId));
            logger.Info($@"Экспорт АСДУ ЕСГ: (конвертация даты) было - {dateTime.ToString()} время по москве - {convDate.ToString()} ");
            return !AppSettingsManager.СonvertDateTime ? dateTime : TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(MoscowZoneId));
        }

        public static string ToStringWithMoscowZone(this DateTime dateTime)
        {
            var offset = TimeZoneInfo.FindSystemTimeZoneById(MoscowZoneId).BaseUtcOffset.Hours;
            var timeZoneM =
                AppSettingsManager.CorrectMoscowZone == 0 ?
                   offset :
                   AppSettingsManager.CorrectMoscowZone;
            return $@"{dateTime:yyyy""-""MM""-""dd""T""HH"":""mm"":""ss}{timeZoneM.ToString("+00;-00;+00")}:00";
        }
    }
}
