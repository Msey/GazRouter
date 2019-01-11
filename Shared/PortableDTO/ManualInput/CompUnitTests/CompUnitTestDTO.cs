using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;

namespace GazRouter.DTO.ManualInput.CompUnitTests
{
    [DataContract]
	public class CompUnitTestDTO : BaseDto<int>
	{
        /// <summary>
        /// Плотность газа, кг/м3
        /// </summary>
        [DataMember]
        public double? Density { get; set; }
        
        /// <summary>
        /// Температура на входе в нагнетатель, гр. С
        /// </summary>
        [DataMember]
        public double? TemperatureIn { get; set; }

        /// <summary>
        /// Давление на входе в нагнетатель, кг/см2
        /// </summary>
        [DataMember]
        public double? PressureIn { get; set; }

        /// <summary>
        /// Идентификатор ГПА
        /// </summary>
        [DataMember]
        public Guid CompUnitId { get; set; }

        /// <summary>
        /// Дата проведения испытания
        /// </summary>
        [DataMember]
        public DateTime CompUnitTestDate { get; set; }

        /// <summary>
        /// Описание испытания
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Минимально допустимая объемная подача, м3/мин
        /// </summary>
        [DataMember]
        public double? QMin { get; set; }

        /// <summary>
        /// Максимально допустимая объемная подача, м3/мин
        /// </summary>
        [DataMember]
        public double? QMax { get; set; }

        /// <summary>
        /// Вложения
        /// </summary>
        [DataMember]
        public List<AttachmentDTO<int, int>> AttachmentList { get; set; }

        /// <summary>
        /// Точки на ГДХ
        /// </summary>
        [DataMember]
        public List<SuperchargerChartPointDTO> ChartPoints { get; set; }
        
	}
}