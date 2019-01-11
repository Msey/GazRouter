using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.GasCosts
{
    public class GasCostAccessDTO
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }
        
        [DataMember]
        public bool Norm { get; set; }

        [DataMember]
        public bool Plan { get; set; }

        [DataMember]
        public bool Fact { get; set; }

        [DataMember]
        public string ChangeUser { get; set; }

        [DataMember]
        public PeriodType PeriodType { get; set; }
    }
}