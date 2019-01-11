using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GazRouter.Modes.DispatcherTasks.Common.RecordUrgency
{
    public class RecordUrgencyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((RecordUrgency)value)
            {
                case RecordUrgency.Urgent:
                    return new SolidColorBrush(Colors.Orange);

                case RecordUrgency.Alarm:
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