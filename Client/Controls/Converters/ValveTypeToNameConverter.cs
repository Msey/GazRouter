using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Common.Cache;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class ValveTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typeId = value as int?;
            if (!typeId.HasValue) return "";

            return Convert(typeId.Value);

        }

        public string Convert(int typeId)
        {
            return ClientCache.DictionaryRepository.ValveTypes.SingleOrDefault(t => t.Id == typeId)?.Name;
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
