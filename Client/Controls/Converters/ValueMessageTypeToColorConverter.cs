using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Common.GoodStyles;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.Controls.Converters
{
    public class ValueMessageTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as PropertyValueMessageType?;
            if (!type.HasValue) return Brushes.Transparent;

            switch (type.Value)
            {
                case PropertyValueMessageType.Error:
                    return Brushes.Red;

                case PropertyValueMessageType.Alarm:
                    return Brushes.Orange;

                default:
                    return Brushes.Transparent;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}