using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.Segment
{
    [DataContract]
	public class PressureSegmentDTO : BaseSegmentDTO
	{
		[DataMember]
		public double Pressure { get; set; }
    }
}
