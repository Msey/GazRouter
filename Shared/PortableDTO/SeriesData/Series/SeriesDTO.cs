using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.SeriesData.Series
{
    [DataContract]
    public class SeriesDTO : BaseDto<int>
    {
        [DataMember]
        public DateTime KeyDate { get; set; }

        [DataMember]
        public PeriodType PeriodTypeId { get; set; }
        
        [DataMember]
        public string Description { get; set; }
    }
}
