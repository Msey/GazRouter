using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Controls.Converters
{
    public class ValueSourceTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as SourceType?;
            if (!type.HasValue) return "АСУ ТП";

            switch (type.Value)
            {
                case SourceType.StandardCalculation:
                    return "Типовой расчет";

                case SourceType.CustomCalculation:
                    return "Нетиповой расчет";

                case SourceType.OtherCalculation:
                    return "Прочий расчет";

                case SourceType.ManualInput:
                    return "Ручной ввод";

                case SourceType.CustomExchange:
                    return "Внешняя система";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}