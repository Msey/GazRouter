using System;
using System.Globalization;
using System.Windows.Data;

namespace DataExchange.ASDU
{
    public class GuidToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid? && (!(value as Guid?).HasValue || (Guid)value == Guid.Empty))
            {
                return string.Empty;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && string.IsNullOrEmpty((string)value))
            {
                return null;
            }
            return value;
        }
    }
}