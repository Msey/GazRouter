using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.Diameters
{
	[DataContract]
	public class DiameterDTO : BaseDictionaryDTO
	{
        [DataMember]
        public double DiameterConv { get; set; }

        [DataMember]
        public double DiameterReal { get; set; }

	}
}