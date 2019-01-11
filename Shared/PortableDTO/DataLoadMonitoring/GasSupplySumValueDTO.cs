using System;
using System.Runtime.Serialization;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public class GasSupplySumValueDTO
    {
        [DataMember]
        public SeriesDTO Serie { get; set; }
        [DataMember]
        public Double? GazVolume { get; set; }
        [DataMember]
        public Double? GazVolumeChange { get; set; }

        public string Timestamp
        {
            get
            {
                return Serie.KeyDate.ToString();
            }

        }
        public DateTime Timestamp1
        {
            get
            {
                return Serie.KeyDate;
            }

        }
    }
}
