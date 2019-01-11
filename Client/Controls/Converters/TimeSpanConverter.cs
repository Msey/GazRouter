using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Controls.Converters
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var timeSpan = (TimeSpan) value;

                var str = "";
                if (timeSpan.Days > 0)
                    str += timeSpan.ToString("d'д. '");
                if (timeSpan.Hours > 0)
                    str += timeSpan.ToString("h'ч. '");
                if (timeSpan.Minutes > 0)
                    str += timeSpan.ToString("mm'м.'");
                
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
