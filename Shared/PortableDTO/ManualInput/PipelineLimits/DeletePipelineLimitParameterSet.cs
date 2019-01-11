using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.ManualInput.PipelineLimits
{
    public class DeletePipelineLimitParameterSet
    {
        /// <summary>
        /// Идентификатор ограничения
        /// </summary>
        [DataMember]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
    }
}
