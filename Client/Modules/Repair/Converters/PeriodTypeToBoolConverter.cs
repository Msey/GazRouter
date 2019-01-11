using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Repair.Enums;

namespace GazRouter.Repair.Converters
{
    public class PeriodTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PeriodType)value == (PeriodType)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PeriodType)int.Parse(parameter.ToString());
        }
	}
    
}