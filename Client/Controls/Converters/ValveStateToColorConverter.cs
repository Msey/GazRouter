using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.Common.GoodStyles;
using GazRouter.DTO.Dictionaries.StatesModel;
using Colors = System.Windows.Media.Colors;

namespace GazRouter.Controls.Converters
{
    public class ValveStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValveState)
                return Convert((ValveState)value);
            
            return new SolidColorBrush(Colors.Transparent);
        }

        public Brush Convert(ValveState state)
        {
            switch (state)
            {
                case ValveState.Opened:
                    return Brushes.Lime;

                case ValveState.Closed:
                    return Brushes.Red;

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