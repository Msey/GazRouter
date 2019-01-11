using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.CompStationCoolingRecomended
{
    [DataContract]
	public class CompStationCoolingRecomendedDTO
	{
		[DataMember]
		public Guid CompStationId { get; set; }

		[DataMember]
		public int Month { get; set; }

		[DataMember]
		public double Temperature { get; set; }
	}
}