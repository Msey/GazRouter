using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.DistrNetworks
{
	[DataContract]
	public class DistrNetworkDTO : NamedDto<int>
	{
        [DataMember]
        public int RegionId { get; set; }

        [DataMember]
        public string RegionName { get; set; }


        [DataMember]
		public int SortOrder { get; set; }

		
	}
}