using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.ManualInput.ValveSwitches;

namespace GazRouter.Controls.Converters
{
    public class ValveSwitchTypeToNameConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ValveSwitchType) value)
            {
                case ValveSwitchType.Valve:
                    return "Основной кран";

                case ValveSwitchType.Bypass1:
                    return "Кран на байпасной линии №1";

                case ValveSwitchType.Bypass2:
                    return "Кран на байпасной линии №2";

                case ValveSwitchType.Bypass3:
                    return "Кран на байпасной линии №3";

                default:
                    return "";
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}