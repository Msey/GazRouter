using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class PipelineTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tmp = value as PipelineType?;
            return tmp != null ?
                ClientCache.DictionaryRepository.PipelineTypes[tmp.Value].Name : null;
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
