using System;
using GazRouter.DTO.Dictionaries.PipelineEndType;

namespace GazRouter.DTO.ObjectModel.PipelineConns
{
    public class AddPipelineConnParameterSet
    {
        public double? Kilometr { get; set; }
        public PipelineEndType EndTypeId { get; set; }
        public Guid PipelineId { get; set; }
        public Guid DestEntityId { get; set; }
    }
}
