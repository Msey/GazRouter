using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class BoolToOkBadImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string folder = "/Common;component/Images/16x16/";
            var b = value as bool?;
            
            return new BitmapImage(new Uri(folder + (b.HasValue && b.Value ? "ok2.png" : "fail.png"), UriKind.Relative));
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}