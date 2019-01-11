using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Appearance.Positions
{
    [DataContract]
    public class PositionDTO
	{
        [DataMember]
        public Guid EntityId { get; set; }


        [DataMember]
        public double Km { get; set; }

		[DataMember]
		public double X { get; set; }

        [DataMember]
        public double Y { get; set; }
        
	}
}