using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DataExchange.ExchangeProperty
{
    [DataContract]
    public class ExchangePropertyDTO
    {
        [DataMember]
        public int ExchangeTaskId { get; set; }
        
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public PropertyType PropertyTypeId { get; set; }
        

        [DataMember]
        public string ExtId { get; set; }

        [DataMember]
        public double? Coeff { get; set; }

        [DataMember]
        public EntityType EntityTypeId { get; set; }
    }

    
}