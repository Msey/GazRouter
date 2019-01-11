using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GazRouter.Balances.Commercial.Plan.Irregularity
{
    public class WrongIrregularityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            return
                new BitmapImage(
                    new Uri(
                        val.HasValue && val.Value
                            ? "/Common;component/Images/10x10/irregularity_red.png"
                            : "/Common;component/Images/10x10/irregularity_blue.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}