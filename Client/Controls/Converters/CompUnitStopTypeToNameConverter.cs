using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class CompUnitStopTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var type = (CompUnitStopType)value;
            return ClientCache.DictionaryRepository.CompUnitStopTypes.Any(s => s.CompUnitStopType == type)
                ? ClientCache.DictionaryRepository.CompUnitStopTypes.Single(s => s.CompUnitStopType == type).Name
                : "";
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}