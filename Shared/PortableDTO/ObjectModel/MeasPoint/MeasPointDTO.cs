using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.MeasPoint
{
    [DataContract]
    public class MeasPointDTO : EntityDTO   
    {
		[DataMember]
	    public Guid? MeasLineId { get; set; }
		
		[DataMember]
	    public Guid? CompShopId { get; set; }
		
		[DataMember]
	    public Guid? DistrStationId { get; set; }
		
		[DataMember]
	    public string MeasLineName{ get; set; }
		
		[DataMember]
	    public string CompShopName { get; set; }
		
		[DataMember]
	    public string DistrStationName { get; set; }


        [DataMember]
        public double ChromatographConsumptionRate  { get; set; }

        [DataMember]
        public int ChromatographTestTime { get; set; }


        [DataMember]
        public Guid SiteId { get; set; }


        public override EntityType EntityType
		{
			get { return EntityType.MeasPoint; }
		}


        [DataMember]
        public EntityStatus? Status { get; set; }
	}
}
