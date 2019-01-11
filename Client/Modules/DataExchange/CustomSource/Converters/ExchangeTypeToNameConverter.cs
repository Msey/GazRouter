using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.DataExchange.CustomSource.Converters
{
    public class ExchangeTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as ExchangeType?;
            return type.HasValue ? ClientCache.DictionaryRepository.ExchangeTypes.Single(s => s.ExchangeType == type).Name : "";
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}