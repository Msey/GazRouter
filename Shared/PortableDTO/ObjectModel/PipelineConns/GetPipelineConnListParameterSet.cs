using System;

namespace GazRouter.DTO.ObjectModel.PipelineConns
{
    public class GetPipelineConnListParameterSet
    {
        public Guid? PipelineId { get; set; }
        public int? GasTrasportSystemId { get; set; } 
    }
}