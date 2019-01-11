using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.MeasStations
{
    [DataContract]
    public class MeasStationDTO : EntityDTO
	{
        [DataMember]
        public Sign BalanceSignId { get; set; }

        [DataMember]
        public string BalanceSignName { get; set; }

        [DataMember]
        public bool IsIntermediate { get; set; }

        [DataMember]
        public Guid? NeighbourEnterpriseId { get; set; }

        public override EntityType EntityType
        { 
            get { return EntityType.MeasStation; }
        }

        [DataMember]
        public EntityStatus? Status { get; set; }

	}
}