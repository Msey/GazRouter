using System.Text;

namespace GazRouter.Modes.ProcessMonitoring.Module
{
    public class GasModeChangeConditions
    {
        private const string textTemplate = @"Отображаются изменения по давлению #PressureChangeLimit кг/см² и #PressureChangeAlaram кг/см², по температуре #TemperatureChangeLimit °С, по замерам и расходам #FlowChange%";
        /// <summary>
        /// изменение давления не менее
        /// </summary>
        public double PressureChangeLimit { get; set; }
        /// <summary>
        /// изменение давления при котором цветом подкрашивать
        /// </summary>
        public double PressureChangeAlaram { get; set; }
        /// <summary>
        /// изменение температуры не менее
        /// </summary>
        public double TemperatureChangeLimit { get; set; }
        /// <summary>
        /// изменение по расходу газа (%)
        /// </summary>
        public double FlowChange { get; set; }

        public GasModeChangeConditions()
        {
            PressureChangeLimit = 0.2;
            PressureChangeAlaram = 0.5;
            TemperatureChangeLimit = 5;
            FlowChange = 5;
        }

        public string ConditionText
        {
            get
            {
                StringBuilder sb = new StringBuilder(textTemplate);
                sb.Replace("#PressureChangeLimit", PressureChangeLimit.ToString());
                sb.Replace("#PressureChangeAlaram", PressureChangeAlaram.ToString());
                sb.Replace("#TemperatureChangeLimit", TemperatureChangeLimit.ToString());
                sb.Replace("#FlowChange", FlowChange.ToString());
                return sb.ToString();
            }
        }
    }
}
