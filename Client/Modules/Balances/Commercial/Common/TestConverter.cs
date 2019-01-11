using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GazRouter.Balances.Commercial.Common
{
    public class TestConverter : IValueConverter
    {
        public string Test { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Test;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}