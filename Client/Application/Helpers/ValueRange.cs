using System;
using Utils.Units;

namespace GazRouter.Application.Helpers
{
    public abstract class ValueRangeBase<T> where T : IComparable, IComparable<T>
    {
        private readonly string _violationMessage;

        protected ValueRangeBase(T min, T max, string msg = null)
        {
            Min = min;
            Max = max;
            _violationMessage = msg;
        }

        public T Min { get; set; }
        public T Max { get; set; }
        public virtual string ViolationMessage => _violationMessage ?? $"Недопустимое значение (интервал допустимых значений от {Min} до {Max})";


        public bool IsOutsideRange(T value)
        {
            return ((value.CompareTo(Min) < 0) || (value.CompareTo(Max) > 0));
        }
    }

    public class ValueRange : ValueRangeBase<double>
    {
        public ValueRange(double min, double max, string msg = null) : base(min, max, msg)
        {
        }
    }

    public class TemperatureRange : ValueRangeBase<Temperature>
    {
        public TemperatureRange() : base(Temperature.FromCelsius(-100), Temperature.FromCelsius(100))
        {
        }

        public override string ViolationMessage => $"Недопустимое значение (интервал допустимых значений от {Min.ToString(UserProfile.Current.UserSettings.TemperatureUnit)} до {Max.ToString(UserProfile.Current.UserSettings.TemperatureUnit)})";
    }

    public class TemperatureAirRange : ValueRangeBase<Temperature>
    {
        public TemperatureAirRange() : base(Temperature.FromCelsius(-50), Temperature.FromCelsius(50))
        {
        }

        public override string ViolationMessage => $"Недопустимое значение (интервал допустимых значений от {Min.ToString(UserProfile.Current.UserSettings.TemperatureUnit)} до {Max.ToString(UserProfile.Current.UserSettings.TemperatureUnit)})";
    }



    public class PressureRange : ValueRangeBase<Pressure>
    {
        private PressureUnit? _pressureUnit;

        public PressureRange() : base(Pressure.FromKgh(0), Pressure.FromKgh(100))
        {
        }
        

        public PressureRange(PressureUnit pressureUnit, double min  , double max) : base(Pressure.From(min, pressureUnit),Pressure.From(max,pressureUnit) )
        {
            _pressureUnit = pressureUnit;
        }

        public override string ViolationMessage => $"Недопустимое значение (интервал допустимых значений от {Min.ToString(PressureUnit)} до {Max.ToString(PressureUnit)})";

        private PressureUnit PressureUnit
            => _pressureUnit.HasValue ? _pressureUnit.Value : UserProfile.Current.UserSettings.PressureUnit;
    }

    public class CombHeatRange : ValueRangeBase<CombustionHeat>
    {
        private CombustionHeatUnit? _units;

        public CombHeatRange() : base(CombustionHeat.FromKCal(7000), CombustionHeat.FromKCal(9500))
        {
        }


        public CombHeatRange(CombustionHeatUnit units, double min, double max) 
            : base(CombustionHeat.From(min, units), CombustionHeat.From(max, units))
        {
            _units = units;
        }

        public override string ViolationMessage => $"Недопустимое значение (интервал допустимых значений от {Min.ToString(Units)} до {Max.ToString(Units)})";

        private CombustionHeatUnit Units
            => _units.HasValue ? _units.Value : UserProfile.Current.UserSettings.CombHeatUnit;
    }
}