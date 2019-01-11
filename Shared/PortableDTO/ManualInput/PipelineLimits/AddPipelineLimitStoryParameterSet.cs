using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.ManualInput.PipelineLimits
{
    public class AddPipelineLimitStoryParameterSet
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
        /// Имя пользователя
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
    }

    /// <summary>
    /// Статус ограничения
    /// </summary>
    public enum LimitStatus
    {
        /// <summary>
        /// Ограничение активно
        /// </summary>
        Active = 1,

        /// <summary>
        /// Ограничение снято
        /// </summary>
        Inactive = 2
    }
}
