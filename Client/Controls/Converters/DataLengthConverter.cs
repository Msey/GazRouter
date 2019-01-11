using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Controls.Converters
{
    public class DataLengthConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var len = double.Parse(value.ToString());
            return Convert(len);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }


        public static string Convert(double len)
        {
            const double kb = 1024;
            const double mb = kb * kb;
            
            if (len / mb >= 1)
                return $"{len/mb:n1}Мб";

            if (len / kb >= 1)
                return $"{len/kb:n0}кб";

            return $"{len:n0}байт";
        }
    }
}