using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Repairs.Plan;

namespace GazRouter.Repair.Converters
{
    public class PlanningStageToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = PlanningStage.Filling;//сброс галки - повторный выбор
            return PlanningStage.TryParse(parameter.ToString(), out v) ? v : v;
            throw new NotSupportedException();
        }
	}
    
}