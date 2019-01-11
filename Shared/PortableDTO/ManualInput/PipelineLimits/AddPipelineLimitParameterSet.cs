using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.ManualInput.PipelineLimits
{
    public class AddPipelineLimitParameterSet
    {
        /// <summary>
        /// Идентификатор газопровода
        /// </summary>
        [DataMember]
        public Guid PipelineId { get; set; }

        /// <summary>
        /// Километр начала
        /// </summary>
        [DataMember]
        public double? Begin { get; set; }

        /// <summary>
        /// Километр конца
        /// </summary>
        [DataMember]
        public double? End { get; set; }

        /// <summary>
        /// Максимальное давление на участке
        /// </summary>
        [DataMember]
        public double? MaxPressure { get; set; }

        /// <summary>
        /// Текст распоряжения об установке ограничения 
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
    }
}
