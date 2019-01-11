using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Balances.Transport
{
    [DataContract]
	public class TransportDTO
    {
        [DataMember]
        public int OwnerId { get; set; }

        [DataMember]
        public string OwnerName { get; set; }

        [DataMember]
        public Guid InletId { get; set; }

        [DataMember]
        public Guid OutletId { get; set; }

        [DataMember]
        public string InletName { get; set; }

        [DataMember]
        public string OutletName { get; set; }

        [DataMember]
        public EntityType OutletType { get; set; }

        [DataMember]
        public BalanceItem BalanceItem { get; set; }

        [DataMember]
        public double Length { get; set; }

        [DataMember]
        public double Volume { get; set; }
        
        [DataMember]
        public int? RouteId { get; set; }

    }
}