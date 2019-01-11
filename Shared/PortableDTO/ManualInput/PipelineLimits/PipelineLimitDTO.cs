using GazRouter.DTO;
using GazRouter.DTO.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Utils.Units;

namespace GazRouter.DTO.ManualInput.PipelineLimits
{
    [DataContract]
    public class PipelineLimitDTO : BaseDto<int>
    {
        /// <summary>
        /// Идентификатор ограничения
        /// </summary>
        [DataMember]
        public int EntityId { get; set; }

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
        /// Максимально допустимый километр
        /// </summary>
        [DataMember]
        public double? MaxAllowableKm { get; set; }

        /// <summary>
        /// Минимально допустимый километр
        /// </summary>
        [DataMember]
        public double? MinAllowableKm { get; set; }

        /// <summary>
        /// Максимальное давление на участке
        /// </summary>
        [DataMember]
        public Pressure MaxPressure { get; set; }

        /// <summary>
        /// Текст распоряжения об установке ограничения 
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Вложения
        /// </summary>
        [DataMember]
        public List<AttachmentDTO<int, int>> AttachmentList { get; set; }


        /// <summary>
        /// Дата изменения
        /// </summary>
        [DataMember]
        public DateTime ChangeDate { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [DataMember]
        public string User { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Должность пользователя
        /// </summary>
        [DataMember]
        public string UserDescription { get; set; }

        /// <summary>
        /// Отделение пользователя
        /// </summary>
        [DataMember]
        public string UserSite { get; set; }
    }
}
