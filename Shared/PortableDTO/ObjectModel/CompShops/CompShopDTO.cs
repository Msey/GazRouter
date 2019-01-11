using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.CompShops
{
    public class CompShopDTO : EntityDTO
	{
        [DataMember]
        public Guid PipelineId { get; set; }

        [DataMember]
        public string PipelineName { get; set; }

        public string StationName { get; set; }

        public override EntityType EntityType
        {
            get { return EntityType.CompShop; }
        }

        [DataMember]
        public double? KmOfConn { get; set; }
        
        [DataMember]
        public double? PipingVolume { get; set; }
        [DataMember]
        public double? PipingVolumeIn { get; set; }
        [DataMember]
        public double? PipingVolumeOut { get; set; }


        [DataMember]
        public EntityStatus? Status { get; set; }
        

		[DataMember]
        public EngineClass EngineClass { get; set; }

    }
}