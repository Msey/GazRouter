using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using System.Collections.Generic;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;

namespace GazRouter.DTO.ObjectModel.DistrStations
{
    [DataContract]
    public class DistrStationDTO : EntityDTO
    {
        public override EntityType EntityType => EntityType.DistrStation;
        
        [DataMember]
        public int RegionId { get; set; }

        [DataMember]
        public double? PressureRated { get; set; }

        [DataMember]
        public double? CapacityRated { get; set; }

        [DataMember]
        public bool UseInBalance { get; set; }

        [DataMember]
        public List<DistrStationOutletDTO> Outlets { get; set; }

        [DataMember]
        public bool IsForeign { get; set; }

        [DataMember]
        public EntityStatus? Status { get; set; }

    }
}