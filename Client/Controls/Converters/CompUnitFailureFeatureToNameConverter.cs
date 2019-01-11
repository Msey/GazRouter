using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class CompUnitFailureFeatureToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var feature = (CompUnitFailureFeature)value;
            return ClientCache.DictionaryRepository.CompUnitFailureFeatures.Any(s => s.CompUnitFailureFeature == feature)
                ? ClientCache.DictionaryRepository.CompUnitFailureFeatures.Single(s => s.CompUnitFailureFeature == feature).Description
                : "";
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}