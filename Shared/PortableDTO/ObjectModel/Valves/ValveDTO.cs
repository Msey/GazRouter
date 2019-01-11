using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ValvePurposes;

namespace GazRouter.DTO.ObjectModel.Valves
{
    [DataContract]
    public class ValveDTO : EntityDTO
    {
        [DataMember]
        public int ValveTypeId { get; set; }

        [DataMember]
        public ValvePurpose ValvePurposeId { get; set; }

        [DataMember]
        public Guid? CompShopId { get; set; }


        [DataMember]
        public string SystemName { get; set; }


        [DataMember]
        public string CompStationName { get; set; }

        [DataMember]
        public string PipelineName { get; set; }

        [DataMember]
        public double Kilometer { get; set; }

        [DataMember]
        public int? Bypass1TypeId { get; set; }

        [DataMember]
        public int? Bypass2TypeId { get; set; }

        [DataMember]
        public int? Bypass3TypeId { get; set; }

        [DataMember]
        public bool IsControlPoint { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }

        public override EntityType EntityType
        {
            get { return EntityType.Valve; }
        }
    }
}
