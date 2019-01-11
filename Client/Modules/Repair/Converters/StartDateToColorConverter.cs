using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GazRouter.Repair.Converters
{
    public class StartDateToColorConverter : IValueConverter
    {
        private readonly MonthToColorList _monthToColorList;

        public StartDateToColorConverter()
        {
            _monthToColorList = new MonthToColorList();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime) value;
            return new SolidColorBrush(MonthToColor(dateTime.Month));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public Color MonthToColor(int month)
        {
            return _monthToColorList.GetColor(month);
        }
    }
}