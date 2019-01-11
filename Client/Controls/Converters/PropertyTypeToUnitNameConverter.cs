using GazRouter.DTO.Dictionaries.PropertyTypes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Controls.Converters
{
    public class PropertyTypeToUnitNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = value as PropertyType?;
            return t.HasValue ? Convert(t.Value) : string.Empty;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public string Convert(PropertyType type)
        {
            return Application.UserProfile.UserUnitName(type);
        }
    }
}