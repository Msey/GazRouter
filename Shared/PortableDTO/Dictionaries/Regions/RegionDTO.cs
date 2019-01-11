using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.Regions
{
	[DataContract]
    public class RegionDTO : BaseDto<int>
	{
        [DataMember]
		public string Name { get; set; }

        [DataMember]
	    public bool IsFrigid { get; set; }
	}
}