using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public class LastSeriesDTO
    {
        [DataMember]
        public DateTime? LastSerieKeyDate { get; set; }
        [DataMember]
        public DateTime? PreviousSerieKeyDate { get; set; }
    }
}
