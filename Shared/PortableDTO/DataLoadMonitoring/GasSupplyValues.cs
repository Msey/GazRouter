using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public class GasSupplyValue
    {
        //[DataMember]
        //public SeriesDTO Serie { get; set; }
        [DataMember]
        public  Guid  PipelineId { get; set; }
        [DataMember]
        public double KmStart { get; set; }
        [DataMember]
        public double KmEnd { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Double? GazVolume { get; set; }
        [DataMember]
        public Double? GazVolumeChange { get; set; }
    }
}
