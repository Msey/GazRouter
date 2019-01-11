using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Application;
using Utils.Units;


namespace GazRouter.Controls.Converters
{
    public class CombHeatToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CombustionHeat? combHeat = (CombustionHeat?) value;
            return combHeat?.As(Units ?? UserProfile.Current.UserSettings.CombHeatUnit);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var val = value as double?;
            if (!val.HasValue) return null;
            return CombustionHeat.From(val.Value, Units ?? UserProfile.Current.UserSettings.CombHeatUnit);
        }

        public CombustionHeatUnit? Units { get; set; }
    }
}