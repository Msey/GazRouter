using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Application;
using Utils.Units;

namespace GazRouter.Controls.Converters
{
    public class TemperatureToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Temperature? temperature = (Temperature?) value;

            return temperature?.As(UserProfile.Current.UserSettings.TemperatureUnit);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            //
            var val = (double) value;
            return Temperature.From(val, UserProfile.Current.UserSettings.TemperatureUnit);
        }
    }
}