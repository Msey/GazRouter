using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Dictionaries.GasCostItemGroups
{
	[DataContract]
	public class GasCostItemGroupDTO : BaseDictionaryDTO
	{
	    public GasCostItemGroup ItemGroup => (GasCostItemGroup) Id;
	}
}