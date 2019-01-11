using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.DTO.Dictionaries.PhisicalTypes;

namespace GazRouter.Controls.Converters
{
    public class PressureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as double?;
            if (val.HasValue)
                return Math.Round(UserProfile.ToUserUnits(val.Value, PhysicalType.Pressure), 2);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as double?;
            if (val.HasValue)
                return Math.Round(UserProfile.ToServerUnits(val.Value, PhysicalType.Pressure), 2);
            return null;
        }
    }
}