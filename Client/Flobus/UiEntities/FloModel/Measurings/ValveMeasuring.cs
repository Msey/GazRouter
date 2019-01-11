using System;
using System.ComponentModel;
using GazRouter.Controls.Measurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.Valves;


namespace GazRouter.Flobus.UiEntities.FloModel.Measurings
{
    public class ValveMeasuring
    {
        public ValveMeasuring(ValveDTO dto)
        {
            PressureInlet = new DoubleMeasuring(dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, false, true);
            PressureOutlet = new DoubleMeasuring(dto.Id, PropertyType.PressureOutlet, PeriodType.Twohours, false, true);
            TemperatureInlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, false, true);
            TemperatureOutlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureOutlet, PeriodType.Twohours, false, true);
            State = new StateMeasuring(dto.Id, PropertyType.StateValve, PeriodType.Twohours);
            StateChangingTimestamp = new DateMeasuring(dto.Id, PropertyType.StateChangingTimestamp, PeriodType.Twohours);

            StateBypass1 = new StateMeasuring(dto.Id, PropertyType.StateBypass1, PeriodType.Twohours);
            StateBypass2 = new StateMeasuring(dto.Id, PropertyType.StateBypass2, PeriodType.Twohours);
            StateBypass3 = new StateMeasuring(dto.Id, PropertyType.StateBypass3, PeriodType.Twohours);

            State.PropertyChanged += StateChanged;
            StateBypass1.PropertyChanged += StateChanged;
            StateBypass2.PropertyChanged += StateChanged;
            StateBypass3.PropertyChanged += StateChanged;
        }

        private void StateChanged(object obj, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Value")
                NeedRefresh?.Invoke();
        }
        

        public Action NeedRefresh { get; set; }

        /// <summary>
        /// Состояние крана
        /// </summary>
        public StateMeasuring State { get; }

        /// <summary>
        /// Состояние байпаса 1
        /// </summary>
        public StateMeasuring StateBypass1 { get; }

        /// <summary>
        /// Состояние байпаса 2
        /// </summary>
        public StateMeasuring StateBypass2 { get; }

        /// <summary>
        /// Состояние байпаса 3
        /// </summary>
        public StateMeasuring StateBypass3 { get; }

        /// <summary>
        /// Время изменения состояния
        /// </summary>
        public DateMeasuring StateChangingTimestamp { get; set; }

        /// <summary>
        /// Давление газа до крана
        /// </summary>
        public DoubleMeasuring PressureInlet { get; }

        /// <summary>
        /// Давление газа после крана
        /// </summary>
        public DoubleMeasuring PressureOutlet { get; }

        /// <summary>
        /// Температура газа до крана
        /// </summary>
        public DoubleMeasuring TemperatureInlet { get; }

        /// <summary>
        /// Температура газа после крана
        /// </summary>
        public DoubleMeasuring TemperatureOutlet { get; }
    }
}