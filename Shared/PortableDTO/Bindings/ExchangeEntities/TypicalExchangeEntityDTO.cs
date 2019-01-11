using System;
using System.Runtime.Serialization;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.Bindings.ExchangeEntities
{
    [DataContract]
    public class TypicalExchangeEntityDTO : CommonEntityDTO
    {
        [DataMember]
        public Guid EnterpriseId { get; set; }
    }
}