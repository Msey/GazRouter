using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.DTO.SeriesData.EntityValidationStatus;

namespace GazRouter.ManualInput.Converters
{
    public class ValidationStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as EntityValidationStatus?;
            switch (status)
            {
                case EntityValidationStatus.NotChecked:
                    return new SolidColorBrush(Colors.Transparent);

                case EntityValidationStatus.Alarm:
                    return new SolidColorBrush(Colors.Orange);

                case EntityValidationStatus.Error:
                    return new SolidColorBrush(Colors.Red);

                case EntityValidationStatus.Good:
                    return new SolidColorBrush(Colors.Green);


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