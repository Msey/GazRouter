using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.Common.GoodStyles;


namespace GazRouter.Controls.Converters
{
    public class BoolToWarnConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (parameter as Brush) ?? Brushes.SoftOrange;
            
            return ((bool)value) ? color : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}