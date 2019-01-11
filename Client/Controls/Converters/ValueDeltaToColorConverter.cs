using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common.GoodStyles;

namespace GazRouter.Controls.Converters
{
    public class ValueDeltaToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dblValue = value as double?;
            if (!dblValue.HasValue) return null;

            return dblValue.Value >= 0 ? Brushes.NiceGreen : Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}