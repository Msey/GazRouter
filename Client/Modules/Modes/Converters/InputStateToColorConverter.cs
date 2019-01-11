using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.DTO.ManualInput.InputStates;

namespace GazRouter.Modes.Converters
{
    public class InputStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ManualInputState) value)
            {
                case ManualInputState.Input:
                    return new SolidColorBrush(Colors.Orange);

                case ManualInputState.Approved:
                    return new SolidColorBrush(Colors.Green);
                    
                default:
                    return new SolidColorBrush(Colors.Gray);

            }

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}