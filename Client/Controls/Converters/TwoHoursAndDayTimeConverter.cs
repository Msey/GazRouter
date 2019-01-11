using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Controls.Converters
{
    public class TwoHoursAndDayTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dto = value as PropertyValueDoubleDTO;
            if (dto != null)
                switch (dto.PeriodTypeId)
                {
                    case PeriodType.Day:
                        return string.Format("{0:dd.MM.yyyy}", dto.Date);

                    case PeriodType.Twohours:
                        return string.Format("{0:dd.MM.yyyy HH:mm}", dto.Date);
                }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}