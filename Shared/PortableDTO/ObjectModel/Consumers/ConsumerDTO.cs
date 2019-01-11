using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.Consumers
{
    [DataContract]
    public class ConsumerDTO : EntityDTO
    {
        [DataMember]
        public Guid DistrStationId { get; set; }

        [DataMember]
        public int ConsumerTypeId { get; set; }
        
        [DataMember]
        public int RegionId { get; set; }

        [DataMember]
        public int? DistrNetworkId { get; set; }

        [DataMember]
        public string DistrNetworkName { get; set; }

        [DataMember]
        public bool UseInBalance { get; set; }

        public override EntityType EntityType => EntityType.Consumer;
    }
}