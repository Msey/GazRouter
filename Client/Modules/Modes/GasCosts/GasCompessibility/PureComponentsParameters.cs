using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Modes.GasCosts.GasCompessibility
{
    public class PureComponentsParameters
    {
        public PropertyType Component { get; set; }

        /// <summary>
        /// Молярная масса,кг/кмоль
        /// </summary>
        public double Mi { get; set; }
         
        /// <summary>
        /// Коэффициент сжимаемости при стандартных условиях, -
        /// </summary>
        public double zci { get; set;}

        /// <summary>
        /// Энергетический параметр, -
        /// </summary>
        public double Ei { get; set; }

        /// <summary>
        /// Параметр размера, (м3/кмоль)^(1/3)
        /// </summary>
        public double Ki { get; set; }

        /// <summary>
        /// Ориентационный параметр, -
        /// </summary>
        public double Gi { get; set; }

        /// <summary>
        /// Квадрупольный параметр, -
        /// </summary>
        public double Qi { get; set; }

        /// <summary>
        /// Высокотемпературный параметр, -
        /// </summary>
        public double Fi { get; set; }

        /// <summary>
        /// Дипольный параметр, -
        /// </summary>
        public double Si { get; set; }

        /// <summary>
        /// Параметр ассоциации, -
        /// </summary>
        public double Wi { get; set; }
    }
}