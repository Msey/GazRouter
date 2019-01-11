using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.Controls.Converters
{
    public class TargetToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Target)value)
            {
                case Target.Norm:
                    return "Норма";
                case Target.Plan:
                    return "План";
                case Target.Fact:
                    return "Факт";
                default:
                    return "";
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Target)int.Parse(parameter.ToString());
        }
	}
    
}