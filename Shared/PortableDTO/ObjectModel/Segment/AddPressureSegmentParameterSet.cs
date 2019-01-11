using System;

namespace GazRouter.DTO.ObjectModel.Segment
{
    public class AddPressureSegmentParameterSet
    {
        public Guid PipelineId { get; set; }
        public double KilometerOfStartPoint { get; set; }
        public double KilometerOfEndPoint { get; set; }
        public double Pressure { get; set; }
    }
}
