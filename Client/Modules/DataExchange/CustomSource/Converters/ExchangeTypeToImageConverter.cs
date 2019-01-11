using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.DataExchange.CustomSource.Converters
{
    public class ExchangeTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as ExchangeType?;

            const string folder = "/Common;component/Images/16x16/";

            switch (type)
            {
                case ExchangeType.Import:
                    return new BitmapImage(new Uri(folder + "import.png", UriKind.Relative));

                case ExchangeType.Export:
                    return new BitmapImage(new Uri(folder + "export.png", UriKind.Relative));

                case null:
                    throw new ArgumentOutOfRangeException();
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}