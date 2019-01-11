using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GazRouter.Controls.Converters
{
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            return (FontWeight) value == FontWeights.Bold;
        }
    }
}