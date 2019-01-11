using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GazRouter.Controls.InputStory
{
    public class DataStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as DataStatus?;
            switch (status)
            {
                case DataStatus.Waiting:
                    return new SolidColorBrush(Colors.Orange);

                case DataStatus.Ok:
                    return new SolidColorBrush(Colors.Green);

                case DataStatus.Error:
                    return new SolidColorBrush(Colors.Red);

                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}