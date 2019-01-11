using System;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.Application.Helpers
{
    public static class PeriodHelper
    {
        /// <summary>
        /// Текущий год
        /// </summary>
        public static Period ThisYear
        {
            get { return new Period(DateTime.Today.Year); }
        }

        /// <summary>
        /// Текущий квартал
        /// </summary>
        public static Period ThisQuarter
        {
            get
            {
                var qs = QuarterToBeginMonth(MonthToQuarter(DateTime.Today.Month));
                return new Period
                {
                    Begin = new DateTime(DateTime.Today.Year, qs, 1),
                    End = new DateTime(DateTime.Today.Year, qs, 1).AddMonths(3).AddSeconds(-1),
                    Type = PeriodType.Quarter
                };
            }
        }
        
        /// <summary>
        /// Текущий месяц
        /// </summary>
        public static Period ThisMonth
        {
            get
            {
                return new Period
                {
                    Begin = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                    End = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddSeconds(-1),
                    Type = PeriodType.Month
                };
            }
        }


        /// <summary>
        /// Предыдущий год
        /// </summary>
        public static Period PrevYear
        {
            get { return new Period(DateTime.Today.Year - 1); }
        }

        /// <summary>
        /// Предыдущий квартал
        /// </summary>
        public static Period PrevQuarter
        {
            get
            {
                var qs = QuarterToBeginMonth(MonthToQuarter(DateTime.Today.Month));
                return new Period
                {
                    Begin = new DateTime(DateTime.Today.Year, qs, 1).AddMonths(-3),
                    End = new DateTime(DateTime.Today.Year, qs, 1).AddSeconds(-1),
                    Type = PeriodType.Quarter
                };
            }
        }

        /// <summary>
        /// Предыдущий месяц
        /// </summary>
        public static Period PrevMonth
        {
            get
            {
                return new Period
                {
                    Begin = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1),
                    End = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddSeconds(-1),
                    Type = PeriodType.Month
                };
            }
        }

        
        /// <summary>
        /// Находит номер квартала по номеру месяца
        /// </summary>
        /// <param name="month">номер месяца</param>
        /// <returns>номер квартала</returns>
        public static int MonthToQuarter(int month)
        {
            return (month - 1) / 3 + 1;
        }


        /// <summary>
        /// Находит первый месяц квартала по номеру квартала
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public static int QuarterToBeginMonth(int quarter)
        {
            return (quarter - 1)*3 + 1;
        }

    }
    
    public class Period : PropertyChangedBase
    {
        private DateTime _begin;
        private DateTime _end;
        private PeriodType _type;

        public Period()
        {

        }

        public Period(DateTime begin, DateTime end)
        {
            _begin = begin;
            _end = end;
            _type = PeriodType.None;
        }

        public Period(int year)
        {
            _begin = new DateTime(year, 1, 1);
            _end = new DateTime(year, 1, 1).AddYears(1).AddSeconds(-1);
            _type = PeriodType.Year;
        }
        
        public Period(int year, int monthOrQuarter, bool isQuarter)
        {
            if (isQuarter)
            {
                var qs = PeriodHelper.QuarterToBeginMonth(monthOrQuarter);
                _begin = new DateTime(year, qs, 1);
                _end = new DateTime(year, qs, 1).AddMonths(3).AddSeconds(-1);
                _type = PeriodType.Quarter;
            }
            else
            {
                _begin = new DateTime(year, monthOrQuarter, 1);
                _end = new DateTime(year, monthOrQuarter, 1).AddMonths(1).AddSeconds(-1);
                _type = PeriodType.Month;
            }
            
        }

        /// <summary>
        /// Дата начала периода
        /// </summary>
        public DateTime Begin
        {
            get { return _begin; }
            set
            {
                if (SetProperty(ref _begin, value))
                {
                    Type = PeriodType.None;
                    OnPropertyChanged(() => DisplayString);
                    OnPropertyChanged(() => Span);
                }
            }
        }

        /// <summary>
        /// Дата окончания периода
        /// </summary>
        public DateTime End
        {
            get { return _end; }
            set
            {
                if (SetProperty(ref _end, value))
                {
                    Type = PeriodType.None;
                    OnPropertyChanged(() => DisplayString);
                    OnPropertyChanged(() => Span);
                }
            }
        }


        public PeriodType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }


        public TimeSpan Span => End - Begin;


        public string DisplayString
        {
            get
            {
                switch (Type)
                {
                    case PeriodType.Year:
                        return Begin.ToString("yyyyг.");

                    case PeriodType.Quarter:
                        return string.Format("{0}кв. {1}г.", PeriodHelper.MonthToQuarter(Begin.Month), Begin.Year);

                    case PeriodType.Month:
                        return Begin.ToString("MMMM yyyyг.");
                }

                return string.Format("{0} - {1}", Begin.ToString("dd.MM.yyyy"), End.ToString("dd.MM.yyyy"));
            }
        }

        
    }
}