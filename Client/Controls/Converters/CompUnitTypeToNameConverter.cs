using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using Microsoft.Practices.ServiceLocation;


namespace GazRouter.Controls.Converters
{
    public class CompUnitTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var typeId = (int)value;
            return Convert(typeId);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public static string Convert(int typeId)
        {
            return ClientCache.DictionaryRepository.CompUnitTypes.Any(s => s.Id == typeId)
                ? ClientCache.DictionaryRepository.CompUnitTypes.Single(s => s.Id == typeId).Name
                : "";
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

    }
}