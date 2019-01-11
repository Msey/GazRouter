using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.StatesModel
{
	[DataContract]
    public class StateBaseDTO : BaseDto<int>
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

	    public CompUnitState State => (CompUnitState) Id;
	}
}