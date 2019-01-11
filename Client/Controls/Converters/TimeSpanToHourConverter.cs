using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Controls.Converters
{
    public class TimeSpanToHourConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var timeSpan = (TimeSpan)value;

                var str = "";
                if (timeSpan.TotalHours > 0)
                    str += timeSpan.TotalHours.ToString("d'д. '");

                return str;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
