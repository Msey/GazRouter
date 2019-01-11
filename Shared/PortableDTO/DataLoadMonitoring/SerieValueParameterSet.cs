using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public class SerieValueParameterSet
    {
        [DataMember]
        public DateTime KeyDate { get; set; }
        [DataMember]
        public int PeriodTypeId { get; set; }
    }
}
