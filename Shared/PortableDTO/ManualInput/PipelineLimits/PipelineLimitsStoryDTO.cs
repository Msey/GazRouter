using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.ManualInput.PipelineLimits
{
    public class PipelineLimitsStoryDTO : BaseDto<int>
    {
        /// <summary>
        /// Идентификатор ограничения
        /// </summary>
        [DataMember]
        public int EntityId { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        [DataMember]
        public DateTime ChangeDate { get; set; }

        /// <summary>
        /// Статус ограничения
        /// </summary>
        [DataMember]
        public LimitStatus Status { get; set; }


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
