using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.ManualInput.InputStates;

namespace GazRouter.Controls.Converters
{
    public class InputStateToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ManualInputState) value)
            {
                case ManualInputState.Input:
                    return "Ввод";

                case ManualInputState.Approved:
                    return "Подтверждено";
                    
                default:
                    return "?";

            }

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}