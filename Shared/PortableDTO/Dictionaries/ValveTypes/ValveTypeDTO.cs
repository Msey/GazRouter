using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.ValveTypes
{
	[DataContract]
	public class ValveTypeDTO : BaseDictionaryDTO
	{
        [DataMember]
        public int DiameterId { get; set; }
        
        [DataMember]
		public double RatedConsumption { get; set; }

        [DataMember]
        public double DiameterConv { get; set; }

        [DataMember]
        public double DiameterReal { get; set; }

	}
}