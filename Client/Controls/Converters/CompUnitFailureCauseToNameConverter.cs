using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class CompUnitFailureCauseToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var cause = (CompUnitFailureCause)value;
            return ClientCache.DictionaryRepository.CompUnitFailureCauses.Any(s => s.CompUnitFailureCause == cause)
                ? ClientCache.DictionaryRepository.CompUnitFailureCauses.Single(s => s.CompUnitFailureCause == cause).Name
                : "";
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}