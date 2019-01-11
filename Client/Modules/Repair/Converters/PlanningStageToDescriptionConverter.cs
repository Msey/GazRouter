using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Enums;

namespace GazRouter.Repair.Converters
{
    public class PlanningStageToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PlanningStage)value)
            {
                case PlanningStage.Filling:
                    return "Ввод потребности";
                case PlanningStage.Optimization:
                    return "Оптимизация сроков";
                case PlanningStage.Approved:
                    return "План утвержден";
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