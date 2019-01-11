using System;

namespace GazRouter.DTO.ObjectModel.Segment
{
    public class AddDiameterSegmentParameterSet
    {
        public Guid PipelineId { get; set; }
        public double KilometerOfStartPoint { get; set; }
        public double KilometerOfEndPoint { get; set; }
        public int DiameterId { get; set; }
        public int ExternalDiameterId { get; set; }
    }
}