using System;

namespace GazRouter.DTO.ObjectModel.Segment
{
    public class AddSiteSegmentParameterSet
    {
        public Guid SiteId { get; set; }
        public Guid PipelineId { get; set; }
        public double KilometerOfStartPoint { get; set; }
        public double KilometerOfEndPoint { get; set; }
    }
}
