using System;

namespace GazRouter.DTO.ObjectModel.Segment
{
    public class AddGroupSegmentParameterSet
    {
        public Guid PipelineGroupId { get; set; }
        public Guid PipelineId { get; set; }
        public double KilometerOfStartPoint { get; set; }
        public double KilometerOfEndPoint { get; set; }
    }
}
