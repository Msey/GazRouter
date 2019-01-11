using GazRouter.Controls.Measurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompUnits;


namespace GazRouter.Flobus.UiEntities.FloModel.Measurings
{
    public class CompressorUnitMeasuring
    {

        public CompressorUnitMeasuring(CompUnitDTO unitDTO)
        {
            PressureSuperchargerInlet = new DoubleMeasuring(unitDTO.Id, PropertyType.PressureSuperchargerInlet, PeriodType.Twohours, false, true);
            PressureSuperchargerOutlet = new DoubleMeasuring(unitDTO.Id, PropertyType.PressureSuperchargerOutlet, PeriodType.Twohours, false, true);
            TemperatureSuperchargerInlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureSuperchargerInlet, PeriodType.Twohours, false, true);
            TemperatureSuperchargerOutlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureSuperchargerOutlet, PeriodType.Twohours, false, true);
            CompressorUnitState = new StateMeasuring(unitDTO.Id, PropertyType.CompressorUnitState, PeriodType.Twohours);
            StateChangingTimestamp = new DateMeasuring(unitDTO.Id, PropertyType.StateChangingTimestamp, PeriodType.Twohours);
            FuelGasConsumption = new DoubleMeasuring(unitDTO.Id, PropertyType.FuelGasConsumption, PeriodType.Twohours, false, true);
            
            RpmSupercharger = new DoubleMeasuring(unitDTO.Id, PropertyType.RpmSupercharger, PeriodType.Twohours, false, true);
            RpmHighHeadTurbine = new DoubleMeasuring(unitDTO.Id, PropertyType.RpmHighHeadTurbine, PeriodType.Twohours, false, true);
            RpmLowHeadTurbine = new DoubleMeasuring(unitDTO.Id, PropertyType.RpmLowHeadTurbine, PeriodType.Twohours, false, true);

            TemperatureHighHeadTurbineInlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureHighHeadTurbineInlet, PeriodType.Twohours, false, true);
            TemperatureHighHeadTurbineOutlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureHighHeadTurbineOutlet, PeriodType.Twohours, false, true);
            TemperatureLowHeadTurbineInlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureLowHeadTurbineInlet, PeriodType.Twohours, false, true);
            TemperatureLowHeadTurbineOutlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureLowHeadTurbineOutlet, PeriodType.Twohours, false, true);
            TemperatureFreeTurbineInlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureFreeTurbineInlet, PeriodType.Twohours, false, true);
            TemperatureFreeTurbineOutlet = new DoubleMeasuring(unitDTO.Id, PropertyType.TemperatureFreeTurbineOutlet, PeriodType.Twohours, false, true);
        }

        /// <summary>
        /// Давление газа перед нагнетателем
        /// </summary>
        public DoubleMeasuring PressureSuperchargerInlet { get; }

        /// <summary>
        /// Давление газа за нагнетателем
        /// </summary>
        public DoubleMeasuring PressureSuperchargerOutlet { get; }

        /// <summary>
        /// Температура газа перед нагнетателем
        /// </summary>
        public DoubleMeasuring TemperatureSuperchargerInlet { get; }

        /// <summary>
        /// Температура газа за нагнетателем
        /// </summary>
        public DoubleMeasuring TemperatureSuperchargerOutlet { get; }

        /// <summary>
        /// Температура газа перед турбиной высокого давления
        /// </summary>
        public DoubleMeasuring TemperatureHighHeadTurbineInlet { get; set; }

        /// <summary>
        /// Температура газа за турбиной высокого давления
        /// </summary>
        public DoubleMeasuring TemperatureHighHeadTurbineOutlet { get; set; }

        /// <summary>
        /// Температура газа перед турбиной низкого давления
        /// </summary>
        public DoubleMeasuring TemperatureLowHeadTurbineInlet { get; set; }

        /// <summary>
        /// Температура газа за турбиной низкого давления
        /// </summary>
        public DoubleMeasuring TemperatureLowHeadTurbineOutlet { get; set; }

        /// <summary>
        /// Температура газа перед свободной турбиной
        /// </summary>
        public DoubleMeasuring TemperatureFreeTurbineInlet { get; set; }

        /// <summary>
        /// Температура газа за свободной турбиной
        /// </summary>
        public DoubleMeasuring TemperatureFreeTurbineOutlet { get; set; }

        /// <summary>
        /// Состояние ГПА
        /// </summary>
        public StateMeasuring CompressorUnitState { get; }

        /// <summary>
        /// Время изменения состояния
        /// </summary>
        public DateMeasuring StateChangingTimestamp { get; }

        /// <summary>
        /// Расход топливного газа
        /// </summary>
        public DoubleMeasuring FuelGasConsumption { get; }

        /// <summary>
        /// Частота вращения вала центробежного нагнетателя
        /// </summary>
        public DoubleMeasuring RpmSupercharger { get; set; }

        /// <summary>
        /// Частота вращения ротора ТВД
        /// </summary>
        public DoubleMeasuring RpmHighHeadTurbine { get; set; }

        /// <summary>
        /// Частота вращения ротора ТНД
        /// </summary>
        public DoubleMeasuring RpmLowHeadTurbine { get; set; }
    }
}