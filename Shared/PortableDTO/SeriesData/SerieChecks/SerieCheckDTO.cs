using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.SeriesData.SerieChecks
{
    [DataContract]
    public class SerieCheckDTO : BaseDto<int>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }
    }
}
