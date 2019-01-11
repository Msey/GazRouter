using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class EntityTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = value as EntityType?;
            return t != null ? Convert(t.Value) : "";
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public string Convert(EntityType type)
        {
            return ClientCache.DictionaryRepository.EntityTypes.Single(t => t.EntityType == type).ShortName;
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

    }
}