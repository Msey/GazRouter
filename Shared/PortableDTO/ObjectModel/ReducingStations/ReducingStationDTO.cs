using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.ReducingStations
{
    [DataContract]
    public class ReducingStationDTO : EntityDTO
    {
        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public Guid PipelineId { get; set; }

        [DataMember]
        public string PipelineName { get; set; }

        [DataMember]
        public double Kilometer { get; set; }

        [DataMember]
        public EntityStatus? Status { get; set; }

        public override EntityType EntityType => EntityType.ReducingStation;
    }
}
