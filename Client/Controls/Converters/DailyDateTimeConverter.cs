using System;
using System.Globalization;
using System.Windows.Data;
using Utils.Extensions;

namespace GazRouter.Controls.Converters
{
    public class DailyDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return string.Empty;

            // Target value must be a System.DateTime object.
            if (!(value is DateTime))
            {
                throw new ArgumentException("Invalid type. Argument must be of the type System.DateTime.");
            }
            

            var given = (DateTime)value;

            return given.ToDailyDateTimeString();         
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}