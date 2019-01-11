using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.DTO.SeriesData.Trends
{
	[DataContract]
	public class TrendDTO
	{
		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public List<BasePropertyValueDTO> Data { get; set; }

	}
}
