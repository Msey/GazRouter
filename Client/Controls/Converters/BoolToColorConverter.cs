using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Common.GoodStyles;


namespace GazRouter.Controls.Converters
{
    public class BoolToColorConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? Brushes.Black : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}