using System;

namespace GazRouter.DTO.Dictionaries.PeriodTypes
{
	public enum PeriodType
	{
        None = 0,
        Month = 1,
		Day = 2,
		Twohours = 5,
		Year = 6,
		Quarter = 7,
		Decade = 8,
        FiveDays = 9

	}

    public static class PeriodTypeHelper
    {
        public static TimeSpan ToTimeSpan(this PeriodType pt)
        {
            switch (pt)
            {
                case PeriodType.Month:
                    return TimeSpan.FromDays(30);
                case PeriodType.Day:
                    return TimeSpan.FromDays(1);
                case PeriodType.FiveDays:
                    return TimeSpan.FromDays(5);
                case PeriodType.Twohours:
                    return TimeSpan.FromHours(2);
                case PeriodType.Year:
                    return TimeSpan.FromDays(345);
                case PeriodType.Decade:
                    return TimeSpan.FromDays(10);
                case PeriodType.Quarter:
                    return TimeSpan.FromDays(95);
                default:
                    throw new ArgumentOutOfRangeException("pt", "Не выбран PeriodType");
            }
        }
    }

}
