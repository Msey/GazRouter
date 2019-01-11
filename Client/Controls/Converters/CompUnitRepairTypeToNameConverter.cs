using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class CompUnitRepairTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var type = (CompUnitRepairType)value;
            return ClientCache.DictionaryRepository.CompUnitRepairTypes.Any(s => s.UnitRepairType == type)
                ? ClientCache.DictionaryRepository.CompUnitRepairTypes.Single(s => s.UnitRepairType == type).Name
                : "Не определено";
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}