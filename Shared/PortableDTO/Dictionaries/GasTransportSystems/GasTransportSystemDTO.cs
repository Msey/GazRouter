using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.GasTransportSystems
{
	[DataContract]
	public class GasTransportSystemDTO : BaseDto<int>
	{
		[DataMember]
		public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

	}
}