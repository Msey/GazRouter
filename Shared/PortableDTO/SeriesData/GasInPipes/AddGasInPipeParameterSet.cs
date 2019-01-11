using System;

namespace GazRouter.DTO.SeriesData.GasInPipes
{
    public class AddGasInPipeParameterSet
    {
        public int SeriesId { get; set; }
        public Guid PipelineId { get; set; }
        public double  StartKm { get; set; }
        public double EndKm { get; set; }
        public double Value { get; set; }
        public string  Description { get; set; }
    }
}

