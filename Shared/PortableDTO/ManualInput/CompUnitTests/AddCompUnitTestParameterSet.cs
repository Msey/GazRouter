using System;

namespace GazRouter.DTO.ManualInput.CompUnitTests
{
    public class AddCompUnitTestParameterSet
    {
        /// <summary>
        /// Идентификатор ГПА
        /// </summary>
        public Guid CompUnitId { get; set; }

        /// <summary>
        /// Дата проведения испытания
        /// </summary>
        public DateTime CompUnitTestDate { get; set; }

        /// <summary>
        /// Описание испытания
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Минимально допустимая объемная подача, м3/мин
        /// </summary>
        public double? Qmin { get; set; }

        /// <summary>
        /// Максимально допустимая объемная подача, м3/мин
        /// </summary>
        public double? Qmax { get; set; }
        
        /// <summary>
        /// Плотность газа, кг/м3
        /// </summary>
        public double? Density { get; set; }

        /// <summary>
        /// Температура на входе в нагнетатель, гр. С
        /// </summary>
        public double? TemperatureIn { get; set; }

        /// <summary>
        /// Давление на входе в нагнетатель, кг/см2
        /// </summary>
        public double? PressureIn { get; set; }

    }
}
