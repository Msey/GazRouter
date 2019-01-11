using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.DistrStationOutlets
{
    public class DistrStationOutletDTO : EntityDTO
	{
        public override EntityType EntityType => EntityType.DistrStationOutlet;

        [DataMember]
        public double? CapacityRated { get; set; }

        [DataMember]
        public double? PressureRated { get; set; }

        [DataMember]
        public Guid? ConsumerId { get; set; }

        [DataMember]
        public string ConsumerName { get; set; }


        [DataMember]
        public EntityStatus? Status { get; set; }
	}
}