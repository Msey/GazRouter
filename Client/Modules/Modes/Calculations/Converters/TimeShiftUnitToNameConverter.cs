using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Parameter;

namespace GazRouter.Modes.Calculations.Converters
{
	public class TimeShiftUnitToNameConverter : IValueConverter
	{

        private string GetName(TimeShiftUnit unit)
	    {
	        switch (unit)
	        {
	            case TimeShiftUnit.Y:
	                return "Год";

                case TimeShiftUnit.M:
	                return "Месяц";

                case TimeShiftUnit.D:
	                return "День";

                case TimeShiftUnit.H:
	                return "Час";

                case TimeShiftUnit.Mi:
	                return "Минута";

                case TimeShiftUnit.Ss:
	                return "Секунда";

                case TimeShiftUnit.Sd:
	                return "sd";

                case TimeShiftUnit.Ddl:
	                return "ddl";

                default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }


	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
	        var unit = value as TimeShiftUnit?;
	        return unit.HasValue ? GetName(unit.Value) : string.Empty;
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
	        throw new NotImplementedException();
	    }
	}
}