using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ObjectModel.Segment
{
    public class AddRegionSegmentParameterSet
    {
        public int RegionId { get; set; }
        public Guid PipelineId { get; set; }
        public double KilometerOfStartPoint { get; set; }
        public double KilometerOfEndPoint { get; set; }
    }
}
