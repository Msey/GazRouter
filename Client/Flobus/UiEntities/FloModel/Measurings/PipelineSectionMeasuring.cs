namespace GazRouter.Flobus.UiEntities.FloModel.Measurings
{
    public class PipelineSectionMeasuring
    {
        /// <summary>
        /// Давление газа в начале участка
        /// </summary>
        public double PressureInlet { get; set; }

        /// <summary>
        /// Давление газа в конце участка
        /// </summary>
        public double PressureOutlet { get; set; }

        /// <summary>
        /// Температура газа в начале участка
        /// </summary>
        public double TemperatureInlet { get; set; }

        /// <summary>
        /// Температура газа в конце участка
        /// </summary>
        public double TemperatureOutlet { get; set; }

        /// <summary>
        /// Плотность газа в участке
        /// </summary>
        public double Density { get; set; }

     
    }
}