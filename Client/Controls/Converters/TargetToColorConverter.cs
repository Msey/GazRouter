using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.Common.GoodStyles;
using GazRouter.DTO.Dictionaries.Targets;


namespace GazRouter.Controls.Converters
{
    public class TargetToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Target)value)
            {
                case Target.Fact:
                    return Brushes.Green;
                case Target.Plan:
                    return Brushes.Orange;
                case Target.Norm:
                    return Brushes.Red;
                default:
                    return Brushes.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
	}
    
}