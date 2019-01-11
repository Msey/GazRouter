using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Modes.Calculations.Converters
{
    public class ParameterTypeToNameConverter : IValueConverter
    {
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typeId = value as ParameterType?;
            return typeId.HasValue
                ? ClientCache.DictionaryRepository.ParameterTypes.Single(e => e.Id == (int)typeId).SysName
                : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}