using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.SeriesData.EntityValidationStatus;

namespace GazRouter.ManualInput.Hourly.Tree
{
    public class ValidationStatusToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var status = value as EntityValidationStatus?;
            switch (status)
            {
                case EntityValidationStatus.Good:
                    return
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(
                                @"/Common;component/Images/16x16/ok.png");

                case EntityValidationStatus.Alarm:
                    return
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(
                                @"/Common;component/Images/10x10/warning_orange.png");

                case EntityValidationStatus.Error:
                    return
                        (ImageSource)
                            new ImageSourceConverter().ConvertFromString(
                                @"/Common;component/Images/10x10/warning.png");

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}