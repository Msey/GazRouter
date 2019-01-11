using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Application;
using Utils.Units;


namespace GazRouter.Controls.Converters
{
    public class PressureToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Pressure? pressure = (Pressure?) value;
            return pressure?.As(Units ?? UserProfile.Current.UserSettings.PressureUnit);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var val = value as double?;
            if (!val.HasValue) return null;
            return Pressure.From(val.Value, Units ?? UserProfile.Current.UserSettings.PressureUnit);
        }

        public PressureUnit? Units { get; set; }
    }
}