using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.PowerUnits
{
    [DataContract]
	public class PowerUnitDTO : EntityDTO
    {
        [DataMember]
        public double Kilometr { get; set; }

        [DataMember]
        public EntityType ParentEntityType { get; set; }

        [DataMember]
        public double OperatingTimeFactor { get; set; }

        [DataMember]
        public double TurbineConsumption { get; set; }

        [DataMember]
        public int TurbineRuntime { get; set; }

        public override EntityType EntityType
        {
            get { return EntityType.PowerUnit;}
        }

		[DataMember]
		public int PowerUnitTypeId { get; set; }

        [DataMember]
        public Guid? SiteId { get; set; }

        public string Type { get; set; }

    }
}
