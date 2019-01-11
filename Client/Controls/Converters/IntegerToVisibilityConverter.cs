using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace GazRouter.Controls.Converters
{
    public class IntegerToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)(value ?? 0) > 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (int)value;
            else
                return 0;
        }
    }
}
