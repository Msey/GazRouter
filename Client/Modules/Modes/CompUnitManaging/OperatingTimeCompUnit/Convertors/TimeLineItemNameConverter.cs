using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Modes.CompressorUnitManaging.OperatingTimeCompUnit.Convertors
{
    public class TimeLineItemNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TimeLineSource;

            return item != null ? 
                string.Format("{0}, {1}, {2}", item.CompStationName, item.CompShopName, item.CompUnitName) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
