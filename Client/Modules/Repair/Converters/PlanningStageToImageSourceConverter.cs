using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Enums;

namespace GazRouter.Repair.Converters
{
    public class PlanningStageToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string baseUrl = "/Common;component/Images/16x16";
            switch ((PlanningStage)value)
            {
                case PlanningStage.Filling:
                    return $"{baseUrl}/flag_green.png";
                case PlanningStage.Optimization:
                    return $"{baseUrl}/flag_orange.png";
                case PlanningStage.Approved:
                    return $"{baseUrl}/flag_red.png";
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