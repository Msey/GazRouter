using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.CompStations
{
    [DataContract]
    public class CompStationDTO : EntityDTO
	{
        [DataMember]
        public int RegionId { get; set; }

        [DataMember]
        public bool UseInBalance { get; set; }

        [DataMember]
        public EntityStatus? Status { get; set; }
        
        public override EntityType EntityType
        {
            get { return EntityType.CompStation; }
        }
    }
}