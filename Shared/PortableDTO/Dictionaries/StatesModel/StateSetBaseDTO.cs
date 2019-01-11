using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.StatesModel
{
	[DataContract]
    public abstract class StateSetBaseDTO : BaseDto<int>
	{
		[DataMember]
		public string SysName { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

	    public StateSet Set
	    {
	        get { return (StateSet) Id; }
	    }
	}
}