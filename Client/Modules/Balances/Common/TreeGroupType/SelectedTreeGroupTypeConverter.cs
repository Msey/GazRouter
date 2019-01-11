using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Balances.Common.TreeGroupType
{
    public class SelectedTreeGroupTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as TreeGroupType?;
            var param = (TreeGroupType)int.Parse(parameter.ToString());

            if (val == param) return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TreeGroupType)int.Parse(parameter.ToString());
        }
    }
}