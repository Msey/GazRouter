using System;

namespace Utils.Extensions
{
    public static class DateTimeFormatHelper
    {

        /// <summary>
        /// An hour defined in minutes.
        /// </summary>
        private const double Hour = 60.0;

        /// <summary>
        /// A day defined in minutes.
        /// </summary>
        private const double Day = 24 * Hour;

        public static bool IsFutureDateTime(DateTime relative, DateTime given)
        {
            return relative < given;
        }

        public static bool IsAtLeastOneWeekOld(DateTime relative, DateTime given)
        {
            return ((int)(relative - given).TotalMinutes >= 7 * Day);
        }

        public static bool IsSameWeek(DateTime relative, DateTime given)
        {
            if (given <= DateTime.MinValue) return false;
            return (relative.Date.AddDays((int) relative.DayOfWeek*(-1)) ==
                    given.Date.AddDays((int) given.DayOfWeek*(-1)));

            /*if (IsAtLeastOneWeekOld(relative, given))
              return false;
            
            int relativeDayOfWeek = relative.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) relative.DayOfWeek;
            int givenDayOfWeek = given.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)given.DayOfWeek;
            return given <= relative;*/
        }
    }
}