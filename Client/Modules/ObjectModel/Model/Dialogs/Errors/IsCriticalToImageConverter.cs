using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GazRouter.Controls.Converters
{
    public class IsCriticalToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string folder = "/Common;component/Images/10x10/";
            var b = value as bool?;
            
            return new BitmapImage(new Uri(folder + (b.HasValue && b.Value ? "warning.png" : "warning_orange.png"), UriKind.Relative));
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}