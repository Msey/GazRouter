using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class PeriodTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var periodType = value as PeriodType?;
            return periodType != null
                ? ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == periodType).Name
                : "";
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}