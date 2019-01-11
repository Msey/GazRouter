using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.DTO.Dictionaries.PhisicalTypes;

namespace GazRouter.Controls.Converters
{
    public class TemperatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as double?;
            if (!val.HasValue) return null;
            return UserProfile.ToUserUnits(val.Value, PhysicalType.Temperature);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var val = double.Parse(value.ToString().Replace('.', ','));
            return UserProfile.ToServerUnits(val, PhysicalType.Temperature);
        }
    }
}