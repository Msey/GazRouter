using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.DataExchange.ExchangeEntity
{
    [DataContract]
    public class ExchangeEntityDTO 
    {
        [DataMember]
        public int ExchangeTaskId { get; set; }
        
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public EntityType EntityTypeId { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string EntityPath { get; set; }

        
        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public string ExtId { get; set; }
        
    }

    
}