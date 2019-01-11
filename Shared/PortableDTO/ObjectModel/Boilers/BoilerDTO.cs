using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.Boilers
{
    [DataContract]
	public class BoilerDTO : EntityDTO
    {
        [DataMember]
        public double Kilometr { get; set; }

        [DataMember]
        public EntityType ParentEntityType { get; set; }

        public override EntityType EntityType
        {
            get { return EntityType.Boiler;}
        }

		[DataMember]
		public int BoilerTypeId { get; set; }


        [DataMember]
        public double HeatLossFactor { get; set; }
        
        [DataMember]
        public double HeatSupplySystemLoad { get; set; }

        [DataMember]
        public Guid? SiteId { get; set; }
    }
}
