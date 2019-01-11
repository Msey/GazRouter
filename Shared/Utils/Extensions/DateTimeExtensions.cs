using System;

namespace Utils.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToLocal(this DateTime given)
        {

            return DateTime.SpecifyKind(given, DateTimeKind.Local);
        }
        public static DateTime ToUnspecified(this DateTime given)
        {

            return DateTime.SpecifyKind(given, DateTimeKind.Unspecified);
        }

        public static string ToDailyDateTimeString(this DateTime given)
        {
            var current = DateTime.Now;

            if (DateTimeFormatHelper.IsFutureDateTime(current, given))
            {
                // Future dates and times are not supported.
                return given.ToString("dd.MM.yyyy HH:mm");
            }

            if (DateTimeFormatHelper.IsSameWeek(current, given))
            {
                int days = (current.Date - given.Date).Days;

                switch (days)
                {
                    case 0:
                        return given.ToString("HH:mm");
                    case 1:
                        return given.ToString("вчера HH:mm");
                    default:
                        return given.ToString("dddd HH:mm");

                }

            }

            return given.ToString("dd.MM.yyyy HH:mm");
        }


        public static DateTime? ToLocal(this DateTime? given)
        {
            if (!given.HasValue) return null;
            return DateTime.SpecifyKind(given.Value, DateTimeKind.Local);
        }

        public static int DaysInMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        public static int HoursInMonth(this DateTime date)
        {
            return date.DaysInMonth() * 24;
        }

        public static DateTime MonthStart(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime MonthEnd(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.DaysInMonth());
        }

        public static DateTime YearEnd(this DateTime date)
        {
            return new DateTime(date.Year, 12, date.DaysInMonth());
        } 
    }
}