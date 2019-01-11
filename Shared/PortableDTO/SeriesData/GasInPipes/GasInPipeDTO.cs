using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.SeriesData.GasInPipes
{

	[DataContract]
	public class GasInPipeDTO
	{
		[DataMember]
		public DateTime Timestamp { get; set; }

		[DataMember]
		public Guid PipelineId { get; set; }

		[DataMember]
		public double KmBegin { get; set; }

		[DataMember]
        public double KmEnd { get; set; }

		[DataMember]
		public double? Volume { get; set; }

        [DataMember]
        public double? Delta { get; set; }
	}
}
