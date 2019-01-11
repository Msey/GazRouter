using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.ManualInput.PipelineLimits
{
    public class GetPipelineLimitListParameterSet
    {
        /// <summary>
        /// Идентификатор газопровода
        /// </summary>
        [DataMember]
        public Guid? PipelineId { get; set; }

        /// <summary>
        /// Идентификатор ограничения
        /// </summary>
        [DataMember]
        public int? LimitId { get; set; }
    }
}
