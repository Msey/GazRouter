using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GazRouter.Balances.Commercial.OwnersSummary
{
    public class IsBalancedToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            return
                new BitmapImage(
                    new Uri(
                        val.HasValue && val.Value
                            ? "/Common;component/Images/16x16/ok2.png"
                            : "/Common;component/Images/16x16/fail.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}