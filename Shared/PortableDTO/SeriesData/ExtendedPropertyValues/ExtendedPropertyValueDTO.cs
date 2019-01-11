using System;
using System.Runtime.Serialization;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.DTO.SeriesData.ExtendedPropertyValues
{

	[DataContract]
	public class ExtendedPropertyValueDTO
	{
		[DataMember]
		public int SeriesId { get; set; }

		[DataMember]
		public Guid EntityId { get; set; }

		[DataMember]
		public int PropertyTypeId { get; set; }

		[DataMember]
		public string UnitTypeName { get; set; }

		[DataMember]
		public string Path { get; set; }

		[DataMember]
		public string EntityName { get; set; }

		[DataMember]
		public string PropertyName { get; set; }

		[DataMember]
		public string SourceName { get; set; }

		[DataMember]
		public BasePropertyValueDTO Value { get; set; }
	}
}
