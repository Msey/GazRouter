using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Flobus.Misc
{
    public class Scale2PercentConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return ((double)value * 100).ToString("##") + "%";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}