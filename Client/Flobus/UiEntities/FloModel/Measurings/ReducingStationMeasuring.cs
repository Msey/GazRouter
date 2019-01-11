using GazRouter.Controls.Measurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.ReducingStations;


namespace GazRouter.Flobus.UiEntities.FloModel.Measurings
{
    public class ReducingStationMeasuring
    {
        public ReducingStationMeasuring(ReducingStationDTO dto)
        {
            PressureInlet = new DoubleMeasuring(dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, false, true);
            PressureOutlet = new DoubleMeasuring(dto.Id, PropertyType.PressureOutlet, PeriodType.Twohours, false, true);

            TemperatureInlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, false, true);
            TemperatureOutlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureOutlet, PeriodType.Twohours, false, true);
        }

        /// <summary>
        /// Давление газа до редуцирования
        /// </summary>
        public DoubleMeasuring PressureInlet { get; set; }

        /// <summary>
        /// Давление газа после редуцирования
        /// </summary>
        public DoubleMeasuring PressureOutlet { get; set; }

        /// <summary>
        /// Температура газа до редуцирования
        /// </summary>
        public DoubleMeasuring TemperatureInlet { get; set; }

        /// <summary>
        /// Температура газа после редуцирования
        /// </summary>
        public DoubleMeasuring TemperatureOutlet { get; set; }
    }
}