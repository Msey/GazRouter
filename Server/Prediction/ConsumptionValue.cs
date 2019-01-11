using System;

namespace GazRouter.Prediction
{
    public class ConsumptionValue : IComparable
    {
        /// <summary>
        /// Метка времени
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Объем газопотребления
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Температура наружного воздуха
        /// </summary>
        public double TemperatureAir { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var other = obj as ConsumptionValue;
            if (other != null && Date >= other.Date) return 1;
            
            return -1;
        }
    }
}