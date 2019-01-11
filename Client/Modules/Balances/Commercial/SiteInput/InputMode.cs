using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Balances.Commercial.SiteInput
{
    public enum InputMode
    {
        DayValue = 1,
        MonthValue = 2
    }

    public class InputModeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = value as InputMode?;
            switch (mode)
            {
                case InputMode.DayValue:
                    return "За сутки";

                case InputMode.MonthValue:
                    return "За месяц";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}