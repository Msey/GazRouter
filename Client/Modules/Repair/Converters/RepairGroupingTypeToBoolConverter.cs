using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Repair.Enums;

namespace GazRouter.Repair.Converters
{
    public class RepairGroupingTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RepairGroupingType) value == (RepairGroupingType) int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RepairGroupingType)int.Parse(parameter.ToString());
        }
	}
    
}