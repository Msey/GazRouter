using System;
using System.Globalization;
using System.Windows.Data;


namespace GazRouter.Repair.Agreement.Converters
{
    public class AgreedResultToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? val = value as bool?;
            if (val.HasValue)
            {
                if (val.Value)
                    return "Утверждено";
                else
                    return "Отклонено";
            }
            else
                return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
