using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class PropertyTypeToNameConverter : IValueConverter
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
            return ClientCache.DictionaryRepository.PropertyTypes.Single(t => t.PropertyType == type).Name;
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

    }
}