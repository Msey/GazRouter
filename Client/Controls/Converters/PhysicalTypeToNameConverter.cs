using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class PhysicalTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = value as PhysicalType?;
            return t.HasValue ? Convert(t.Value) : string.Empty;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public string Convert(PhysicalType type)
        {
            return ClientCache.DictionaryRepository.PhisicalTypes.Single(t => t.PhysicalType == type).Name;
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

    }
}