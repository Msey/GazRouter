using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Common.GoodStyles;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Controls.Converters
{
    public class ValueSourceTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as SourceType?;
            if (!type.HasValue) return Brushes.Dark;

            switch (type.Value)
            {
                case SourceType.StandardCalculation:
                    return Brushes.NiceGreen;

                case SourceType.CustomCalculation:
                    return Brushes.NiceGreen;

                case SourceType.OtherCalculation:
                    return Brushes.NiceGreen;
                    
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}