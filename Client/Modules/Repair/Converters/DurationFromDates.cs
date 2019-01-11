using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.Repair.Converters
{
    public class DurationFromDates
    {
        public static TimeSpan GetDuration(DateTime start, DateTime end)
        {
            return (TimeSpan)(end - start);
        }

        public static double GetDurationInHours(DateTime start, DateTime end)
        {
            return GetDuration(start, end).TotalHours;
        }

        public static double GetDurationFromMorningToEnd(DateTime start, DateTime end)
        {
            return GetDuration(new DateTime(start.Year, start.Month, start.Day, 9, 0, 0), new DateTime(end.Year, end.Month, end.Day, 18, 0, 0)).TotalHours;
        }

        public static int TotalHours(DateTime start, DateTime end)
        {
            return (int)((TimeSpan)(new DateTime(end.Year, end.Month, end.Day, 0, 0, 0) - new DateTime(start.Year, start.Month, start.Day, 0, 0, 0))).TotalHours;
        }

        public static int MaxHours(DateTime start, DateTime end)
        {
            return TotalHours(start, end) + 24;
        }
    }
}
