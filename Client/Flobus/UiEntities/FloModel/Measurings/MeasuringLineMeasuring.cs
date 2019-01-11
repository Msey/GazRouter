using GazRouter.Controls.Measurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.MeasLine;

namespace GazRouter.Flobus.UiEntities.FloModel.Measurings
{
    public class MeasuringLineMeasuring
    {
        public MeasuringLineMeasuring(MeasLineDTO dto)
        {
            Pressure = new DoubleMeasuring(dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, false, true);
            Temperature = new DoubleMeasuring(dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, false, true);
            Flow = new DoubleMeasuring(dto.Id, PropertyType.Flow, PeriodType.Twohours, false, true);
        }

        /// <summary>
        /// Давление газа
        /// </summary>
        public DoubleMeasuring Pressure { get; set; }

        /// <summary>
        /// Температура газа
        /// </summary>
        public DoubleMeasuring Temperature { get; set; }

        /// <summary>
        /// Расход газа через замерную линию
        /// </summary>
        public DoubleMeasuring Flow { get; set; }
    }
}