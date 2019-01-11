using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.DTO.GasCosts
{
    public class DefaultParamValuesDTO
    {

        [DataMember]
        public Target Target { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public DateTime Period { get; set; }

        [DataMember]
        public double PressureAir { get; set; }

        [DataMember]
        public double Density { get; set; }

        [DataMember]
        public double CombHeat { get; set; }

        [DataMember]
        public double NitrogenContent { get; set; }

        [DataMember]
        public double CarbonDioxideContent { get; set; }
        
    }

}