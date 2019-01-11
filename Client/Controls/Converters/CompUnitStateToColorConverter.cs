using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.Common.GoodStyles;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.Controls.Converters
{
    public class CompUnitStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = value as CompUnitState?;
            if (state.HasValue)
            {
                return Convert(state.Value);
            }
            return Brushes.Transparent;
        }

        public Brush Convert(CompUnitState state)
        {
            switch (state)
            {
                case CompUnitState.Work:
                    return Brushes.Green;

                case CompUnitState.Reserve:
                    return Brushes.Orange;

                case CompUnitState.Repair:
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