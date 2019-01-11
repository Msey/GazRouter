using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.Segment
{
    [DataContract]
    public abstract class BaseSegmentDTO : BaseDto<int>
    {
        [DataMember]
        public Guid PipelineId { get; set; }

        [DataMember]
        public string PipelineName { get; set; }

        [DataMember]
        public double KilometerOfStartPoint { get; set; }

        [DataMember]
        public double KilometerOfEndPoint { get; set; }

        public string Lenght
        {
            get
            {
                try { return ((double)( KilometerOfEndPoint - KilometerOfStartPoint)).ToString("N3"); } catch { }
                return "0";
            }
        }
    }
}