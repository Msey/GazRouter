using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DataExchange.Asdu
{
    [DataContract]
    public class AsduEntityDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public Guid? ParameterGid { get; set; }

        [DataMember]
        public EntityType EntityTypeId { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string EntityPath { get; set; }


        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public Guid? EntityGid { get; set; }
    }
}