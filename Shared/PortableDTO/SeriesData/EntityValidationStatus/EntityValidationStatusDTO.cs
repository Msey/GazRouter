using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.SeriesData.EntityValidationStatus
{
    [DataContract]
    public class EntityValidationStatusDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public EntityType EntityType { get; set; }


        [DataMember]
        public EntityValidationStatus Status { get; set; }
    }
}